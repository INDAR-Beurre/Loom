using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Web.WebView2.Core;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Diagnostics;
using LoomBrowser.ViewModels;
using LoomBrowser.Views;
using LoomBrowser.Models;
using LoomBrowser.Services;
using System.Collections.Generic;

namespace LoomBrowser
{
    public partial class MainWindow : Window
    {
        private MainViewModel Vm => DataContext as MainViewModel;
        private HomeView _homeView;
        private SearchView _searchView;
        private AppSettings _settings;
        private List<AIMessage> _aiHistory;
        public MainWindow()
        {
            this.InitializeComponent();
            this.Loaded += Window_Loaded;
            DataContext = new MainViewModel();
            _homeView = new HomeView();
            _homeView.SearchRequested += HomeView_SearchRequested;
            _homeView.QuickLinkClicked += HomeView_QuickLinkClicked;
            _searchView = new SearchView();
            // load settings and AI history
            _settings = SettingsService.Load();
            _aiHistory = AIService.LoadHistory();
            foreach (var m in _aiHistory)
            {
                var prefix = m.Role == "user" ? "You: " : "AI: ";
                var color = m.Role == "user" ? System.Windows.Media.Brushes.LightGray : System.Windows.Media.Brushes.White;
                AIMessageStack.Children.Add(new TextBlock { Text = prefix + m.Content, Foreground = color, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 4, 0, 4) });
            }
        }

        private void InitializeErrorPage(string htmlPath, Exception ex = null)
        {
            try
            {
                string errorHtml = $@"
                        <html>
                        <head>
                            <style>
                                body {{ font-family: Arial, sans-serif; margin: 40px; background: #f0f0f0; }}
                                .error {{ background: #fff; padding: 20px; border-radius: 8px; border-left: 4px solid #ff6b6b; }}
                                h1 {{ color: #ff6b6b; margin: 0 0 10px 0; }}
                                p {{ color: #666; margin: 5px 0; }}
                            </style>
                        </head>
                        <body>
                            <div class='error'>
                                <h1>❌ Error: HTML File Not Found</h1>
                                <p><strong>File:</strong> index-fixed.html</p>
                                <p><strong>Expected Location:</strong> {htmlPath}</p>
                                <p><strong>Exception:</strong></p>
                                <pre>{ex?.Message}</pre>
                                <p><strong>Solution:</strong></p>
                                <ol>
                                    <li>Place 'index-fixed.html' in your project root</li>
                                    <li>Set 'Copy to Output Directory' to 'Copy if newer' in the file properties</li>
                                    <li>Rebuild the project</li>
                                </ol>
                            </div>
                        </body>
                        </html>";

                // Display HTML as a plain error message if WebView2 isn't available
                MainContentHost.Content = new System.Windows.Controls.TextBlock { Text = System.Text.RegularExpressions.Regex.Replace(errorHtml, "<.*?>", string.Empty), TextWrapping = TextWrapping.Wrap };
            }
            catch (Exception) { /* best-effort only */ }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Initialize content host
                MainContentHost.Content = _homeView;
            }
            catch (Exception ex)
            {
                InitializeErrorPage(Path.Combine(AppContext.BaseDirectory, "index-fixed.html"), ex);
                Debug.WriteLine($"Browser initialization error: {ex}");
            }
        }

        private void NewTab_Click(object sender, RoutedEventArgs e)
        {
            Vm.CreateTab();
            RefreshTabs();
        }

        private void HomeView_SearchRequested(string term)
        {
            if (string.IsNullOrWhiteSpace(term)) return;
            AddressBox.Text = term;
            GoButton_Click(null, null);
        }

        private void HomeView_QuickLinkClicked(string url)
        {
            AddressBox.Text = url;
            GoButton_Click(null, null);
        }

        private void SelectTab_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string id)
            {
                var t = Vm.Tabs.FirstOrDefault(x => x.Id == id);
                if (t != null) Vm.ActiveTab = t;
                ShowTabContent(t);
            }
        }

        private void CloseTab_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string id)
            {
                var t = Vm.Tabs.FirstOrDefault(x => x.Id == id);
                if (t != null)
                {
                    Vm.CloseTab(t);
                    RefreshTabs();
                    ShowTabContent(Vm.ActiveTab);
                }
            }
        }

        private void RefreshTabs()
        {
            TabsPresenter.ItemsSource = null;
            TabsPresenter.ItemsSource = Vm.Tabs;
        }

        private async void GoButton_Click(object sender, RoutedEventArgs e)
        {
            var text = AddressBox.Text?.Trim();
            if (string.IsNullOrEmpty(text)) return;

            bool isUrl = text.Contains('.') && !text.Contains(' ');
            if (isUrl)
            {
                if (Vm.ActiveTab == null) Vm.CreateTab();
                Vm.ActiveTab.Type = "site";
                Vm.ActiveTab.Url = text.StartsWith("http") ? text : "https://" + text;
                Vm.ActiveTab.Title = new Uri(Vm.ActiveTab.Url).Host;
                ShowTabContent(Vm.ActiveTab);
                EnsureBrowserAndNavigate(Vm.ActiveTab.Url);
            }
            else
            {
                // use Google for search queries
                if (Vm.ActiveTab == null) Vm.CreateTab();
                var queryUrl = "https://www.google.com/search?q=" + System.Net.WebUtility.UrlEncode(text);
                Vm.ActiveTab.Type = "site";
                Vm.ActiveTab.Url = queryUrl;
                Vm.ActiveTab.Title = text + " - Google";
                ShowTabContent(Vm.ActiveTab);
                EnsureBrowserAndNavigate(queryUrl);
            }
        }

        private void AddressBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                GoButton_Click(null, null);
            }
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            if (Vm.ActiveTab == null) Vm.CreateTab();
            Vm.ActiveTab.Type = "home";
            ShowTabContent(Vm.ActiveTab);
        }

        private void ShowTabContent(Tab t)
        {
            if (t == null) return;
            if (t.Type == "home") { MainContentHost.Content = _homeView; }
            else if (t.Type == "search") { MainContentHost.Content = _searchView; _searchView.LoadQuery(t.Query); }
            else if (t.Type == "site") { EnsureBrowserAndNavigate(t.Url); }
        }

        private async void EnsureBrowserAndNavigate(string url)
        {
            // Try to get a WebView2 instance from the host; create new one if needed
            var web = MainContentHost.Content as Microsoft.Web.WebView2.Wpf.WebView2;
            if (web == null)
            {
                web = new Microsoft.Web.WebView2.Wpf.WebView2();
                MainContentHost.Content = web;
            }

            try
            {
                if (web.CoreWebView2 == null)
                {
                    await web.EnsureCoreWebView2Async(null);
                }

                // inject basic dark CSS into every page so pages appear dark automatically
                try
                {
                    var css = @"(function(){ const css = `html,body,div,section,article,span,p,a,header,footer,nav,main { background-color: #071f14 !important; color: #d4ffd8 !important; } img,video,svg { filter: brightness(0.95) contrast(0.9) !important; } a { color: #8fe2b8 !important; }`; const style=document.createElement('style'); style.innerText = css; document.head.appendChild(style); })();";
                    await web.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(css);
                }
                catch { /* best effort */ }

                if (!string.IsNullOrEmpty(url)) web.CoreWebView2.Navigate(url);
            }
            catch (Exception ex)
            {
                // Show a simple error in the host
                MainContentHost.Content = new TextBlock { Text = "Unable to open page: " + ex.Message, TextWrapping = TextWrapping.Wrap };
            }
        }

        private void ToggleAI_Click(object sender, RoutedEventArgs e)
        {
            // animate the AI panel into view or hide it
            if (AiPanel.Visibility == Visibility.Collapsed)
            {
                AiPanel.Visibility = Visibility.Visible;
                var sb = new Storyboard();
                var fade = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(220)));
                Storyboard.SetTarget(fade, AiPanel);
                Storyboard.SetTargetProperty(fade, new PropertyPath("Opacity"));
                sb.Children.Add(fade);
                AiPanel.Opacity = 0;
                sb.Begin();
            }
            else
            {
                var sb = new Storyboard();
                var fade = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(180)));
                fade.Completed += (s, a) => AiPanel.Visibility = Visibility.Collapsed;
                Storyboard.SetTarget(fade, AiPanel);
                Storyboard.SetTargetProperty(fade, new PropertyPath("Opacity"));
                sb.Children.Add(fade);
                sb.Begin();
            }
        }

        private async void AI_Send_Click(object sender, RoutedEventArgs e)
        {
            var text = AIInput.Text?.Trim();
            if (string.IsNullOrEmpty(text)) return;

            // add user message
            var userMsg = new AIMessage { Role = "user", Content = text };
            _aiHistory.Add(userMsg);
            AIMessageStack.Children.Add(new TextBlock { Text = "You: " + text, Foreground = System.Windows.Media.Brushes.LightGray, TextWrapping = TextWrapping.Wrap });
            AIService.SaveHistory(_aiHistory);

            // call remote or simulate when no key
            // include current open tabs as a short system context so the assistant knows about the user's session
            var historyWithContext = new List<AIMessage>(_aiHistory);
            try
            {
                var openTabs = string.Join("\n", Vm.Tabs.Select(t => $"{t.Title ?? t.Url ?? t.Query} - {t.Url ?? t.Query}"));
                historyWithContext.Add(new AIMessage { Role = "system", Content = "Current open tabs:\n" + openTabs });
            }
            catch { /* best-effort */ }

            AIInput.IsEnabled = false;
            var reply = await AIService.SendMessageAsync(_settings, historyWithContext, text);
            AIInput.IsEnabled = true;

            var aiMsg = new AIMessage { Role = "assistant", Content = reply };
            _aiHistory.Add(aiMsg);
            AIMessageStack.Children.Add(new TextBlock { Text = "AI: " + reply, Foreground = System.Windows.Media.Brushes.White, TextWrapping = TextWrapping.Wrap });
            AIService.SaveHistory(_aiHistory);

            AIInput.Text = string.Empty;
        }

        private void OpenMenu_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Views.SettingsDialog();
            dlg.Owner = this;
            dlg.ShowDialog();
            // reload settings after dialog closed
            _settings = SettingsService.Load();
        }

        private void OpenAccount_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Views.SignInDialog();
            dlg.Owner = this;
            dlg.ShowDialog();
            // when dialog closes, user tokens may be saved by SignInDialog via SettingsService
            _settings = SettingsService.Load();
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // allow dragging the window by the title area
            if (e.ChangedButton == MouseButton.Left)
            {
                try { this.DragMove(); } catch { /* ignore */ }
            }
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void BrowserWebView_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine($"Navigation starting to: {args.Uri}");
        }

        private void BrowserWebView_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine($"Navigation completed. Success: {args.IsSuccess}");
        }
    }
}
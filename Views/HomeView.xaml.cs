using System.Windows.Controls;

namespace LoomBrowser.Views
{
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();
            SearchButton.Click += SearchButton_Click;
        }

        public delegate void SearchRequestedHandler(string term);
        public event SearchRequestedHandler SearchRequested;

        public delegate void QuickLinkHandler(string url);
        public event QuickLinkHandler QuickLinkClicked;

        private void SearchButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SearchRequested?.Invoke(SearchTextBox.Text?.Trim());
        }

        private void QuickLink_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is Button b && b.Tag is string url)
            {
                QuickLinkClicked?.Invoke(url);
            }
        }
    }
}

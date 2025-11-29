using System.Windows;
using LoomBrowser.Models;
using LoomBrowser.Services;

namespace LoomBrowser.Views
{
    public partial class SettingsDialog : Window
    {
        private AppSettings _settings;
        public SettingsDialog()
        {
            InitializeComponent();
            _settings = SettingsService.Load();
            ApiKeyBox.Password = _settings.GroqApiKey ?? string.Empty;
            SupabaseUrlBox.Text = _settings.SupabaseUrl ?? string.Empty;
            SupabaseAnonBox.Password = _settings.SupabaseAnonKey ?? string.Empty;
            if (!string.IsNullOrEmpty(_settings.GroqModel))
            {
                foreach (var item in ModelCombo.Items)
                {
                    if (item is System.Windows.Controls.ComboBoxItem cb && cb.Content?.ToString() == _settings.GroqModel)
                    {
                        ModelCombo.SelectedItem = item; break;
                    }
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _settings.GroqApiKey = ApiKeyBox.Password?.Trim();
            if (ModelCombo.SelectedItem is System.Windows.Controls.ComboBoxItem cb) _settings.GroqModel = cb.Content?.ToString();
            _settings.SupabaseUrl = SupabaseUrlBox.Text?.Trim() ?? "";
            _settings.SupabaseAnonKey = SupabaseAnonBox.Password?.Trim() ?? "";
            SettingsService.Save(_settings);
            MessageBox.Show("Settings saved.", "Loom", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();
    }
}

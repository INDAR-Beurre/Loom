using System.Windows;
using LoomBrowser.Services;
using LoomBrowser.Services;
using LoomBrowser.Services;
using LoomBrowser.Models;
using LoomBrowser.Services;

namespace LoomBrowser.Views
{
    public partial class SignInDialog : Window
    {
        public SignInDialog()
        {
            InitializeComponent();
        }

        private async void SignInBtn_Click(object sender, RoutedEventArgs e)
        {
            var settings = Services.SettingsService.Load();
            if (string.IsNullOrEmpty(settings.SupabaseUrl) || string.IsNullOrEmpty(settings.SupabaseAnonKey))
            {
                StatusText.Text = "Configure Supabase URL and anon key in Settings first.";
                return;
            }

            var (ok, msg, data) = await SupabaseService.SignIn(settings.SupabaseUrl, settings.SupabaseAnonKey, EmailBox.Text.Trim(), PasswordBox.Password);
            if (!ok)
            {
                StatusText.Text = "Sign-in failed: " + msg;
                return;
            }

            // Save token & user id if returned
            try
            {
                if (data.HasValue && data.Value.TryGetProperty("access_token", out var at))
                {
                    settings.SupabaseAccessToken = at.GetString() ?? string.Empty;
                }
                if (data.HasValue && data.Value.TryGetProperty("user", out var user) && user.TryGetProperty("id", out var uid))
                {
                    settings.SupabaseUserId = uid.GetString() ?? string.Empty;
                }

                SettingsService.Save(settings);
                StatusText.Text = "Signed in successfully.";
            }
            catch { StatusText.Text = "Signed in (no metadata)."; }
        }

        private async void SignUpBtn_Click(object sender, RoutedEventArgs e)
        {
            var settings = Services.SettingsService.Load();
            if (string.IsNullOrEmpty(settings.SupabaseUrl) || string.IsNullOrEmpty(settings.SupabaseAnonKey))
            {
                StatusText.Text = "Configure Supabase URL and anon key in Settings first.";
                return;
            }

            var (ok, msg, data) = await SupabaseService.SignUp(settings.SupabaseUrl, settings.SupabaseAnonKey, EmailBox.Text.Trim(), PasswordBox.Password);
            if (!ok)
            {
                StatusText.Text = "Sign-up failed: " + msg;
                return;
            }

            StatusText.Text = "Sign-up success. Sign in to continue.";
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();
    }
}

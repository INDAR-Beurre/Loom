using System;
using System.IO;
using System.Text.Json;
using LoomBrowser.Models;

namespace LoomBrowser.Services
{
    public static class SettingsService
    {
        private static readonly string _baseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Loom");
        private static readonly string _settingsFile = Path.Combine(_baseDir, "settings.json");

        public static AppSettings Load()
        {
            try
            {
                if (!Directory.Exists(_baseDir)) Directory.CreateDirectory(_baseDir);
                if (!File.Exists(_settingsFile))
                {
                    var s = new AppSettings();
                    Save(s);
                    return s;
                }

                var json = File.ReadAllText(_settingsFile);
                var settings = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();

                // allow environment variables to override local settings (safe, not committed to repo)
                bool changed = false;
                string envSupUrl = Environment.GetEnvironmentVariable("NEXT_PUBLIC_SUPABASE_URL") ?? Environment.GetEnvironmentVariable("SUPABASE_URL");
                string envSupAnon = Environment.GetEnvironmentVariable("NEXT_PUBLIC_SUPABASE_ANON_KEY") ?? Environment.GetEnvironmentVariable("SUPABASE_ANON_KEY");
                string envGroq = Environment.GetEnvironmentVariable("GROQ_API_KEY") ?? Environment.GetEnvironmentVariable("NEXT_PUBLIC_GROQ_KEY") ?? Environment.GetEnvironmentVariable("NEXT_PUBLIC_GROQ_API_KEY");

                if (!string.IsNullOrWhiteSpace(envSupUrl) && envSupUrl != settings.SupabaseUrl)
                {
                    settings.SupabaseUrl = envSupUrl;
                    changed = true;
                }
                if (!string.IsNullOrWhiteSpace(envSupAnon) && envSupAnon != settings.SupabaseAnonKey)
                {
                    settings.SupabaseAnonKey = envSupAnon;
                    changed = true;
                }
                if (!string.IsNullOrWhiteSpace(envGroq) && envGroq != settings.GroqApiKey)
                {
                    settings.GroqApiKey = envGroq;
                    changed = true;
                }

                if (changed)
                {
                    // persist the overrides so they are available on next app start
                    try { Save(settings); } catch { /* ignore save errors */ }
                }

                return settings;
            }
            catch
            {
                return new AppSettings();
            }
        }

        public static void Save(AppSettings settings)
        {
            if (!Directory.Exists(_baseDir)) Directory.CreateDirectory(_baseDir);
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_settingsFile, json);
        }
    }
}

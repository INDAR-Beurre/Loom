using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LoomBrowser.Models;

namespace LoomBrowser.Services
{
    public static class AIService
    {
        private static readonly string _baseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Loom");
        private static readonly string _historyFile = Path.Combine(_baseDir, "ai_history.json");

        public static List<AIMessage> LoadHistory()
        {
            try
            {
                if (!Directory.Exists(_baseDir)) Directory.CreateDirectory(_baseDir);
                if (!File.Exists(_historyFile)) return new List<AIMessage>();
                var json = File.ReadAllText(_historyFile);
                return JsonSerializer.Deserialize<List<AIMessage>>(json) ?? new List<AIMessage>();
            }
            catch
            {
                return new List<AIMessage>();
            }
        }

        public static void SaveHistory(List<AIMessage> history)
        {
            try
            {
                if (!Directory.Exists(_baseDir)) Directory.CreateDirectory(_baseDir);
                var json = JsonSerializer.Serialize(history, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_historyFile, json);
            }
            catch { }
        }

        public static async Task<string> SendMessageAsync(AppSettings settings, IEnumerable<AIMessage> previous, string userMessage)
        {
            // Simulation when no API key
            if (string.IsNullOrWhiteSpace(settings?.GroqApiKey))
            {
                await Task.Delay(800);
                return "(simulation) I see — to use full AI features add your Groq API key in Settings.";
            }

            try
            {
                using var http = new HttpClient();
                http.DefaultRequestHeaders.Add("Authorization", "Bearer " + settings.GroqApiKey);

                var messages = new List<object>();
                messages.Add(new { role = "system", content = "You are Loom assistant. Keep answers short and useful." });
                foreach (var m in previous)
                {
                    messages.Add(new { role = m.Role, content = m.Content });
                }
                messages.Add(new { role = "user", content = userMessage });

                var payload = new
                {
                    messages,
                    model = settings.GroqModel ?? "llama3-8b-8192"
                };

                var json = JsonSerializer.Serialize(payload);
                var resp = await http.PostAsync("https://api.groq.com/openai/v1/chat/completions", new StringContent(json, Encoding.UTF8, "application/json"));
                if (!resp.IsSuccessStatusCode)
                {
                    var t = await resp.Content.ReadAsStringAsync();
                    return "Remote error: " + resp.StatusCode + " - " + t;
                }

                using var stream = await resp.Content.ReadAsStreamAsync();
                var doc = await JsonSerializer.DeserializeAsync<JsonElement>(stream);
                if (doc.TryGetProperty("choices", out var choices) && choices.GetArrayLength() > 0)
                {
                    var first = choices[0];
                    if (first.TryGetProperty("message", out var message) && message.TryGetProperty("content", out var content))
                    {
                        return content.GetString() ?? "(no content)";
                    }
                }

                return "No assistant response.";
            }
            catch (Exception ex)
            {
                return "AI call failed: " + ex.Message;
            }
        }
    }
}

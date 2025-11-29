using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using LoomBrowser.Models;

namespace LoomBrowser.Services
{
    public static class SupabaseService
    {
        public static async Task<(bool ok, string message, JsonElement? data)> SignUp(string baseUrl, string anonKey, string email, string password)
        {
            try
            {
                var url = baseUrl.TrimEnd('/') + "/auth/v1/signup";
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("apikey", anonKey);
                var payload = new { email, password };
                var resp = await client.PostAsJsonAsync(url, payload);
                var text = await resp.Content.ReadAsStringAsync();
                if (!resp.IsSuccessStatusCode) return (false, text, null);
                var doc = JsonSerializer.Deserialize<JsonElement>(text);
                return (true, "Signed up", doc);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public static async Task<(bool ok, string message, JsonElement? data)> SignIn(string baseUrl, string anonKey, string email, string password)
        {
            try
            {
                var url = baseUrl.TrimEnd('/') + "/auth/v1/token?grant_type=password";
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("apikey", anonKey);
                var payload = new { email, password };
                var resp = await client.PostAsJsonAsync(url, payload);
                var text = await resp.Content.ReadAsStringAsync();
                if (!resp.IsSuccessStatusCode) return (false, text, null);
                var doc = JsonSerializer.Deserialize<JsonElement>(text);
                return (true, "Signed in", doc);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        // baseUrl - your supabase URL
        // anonKey - public anon key (required by REST), accessToken - user JWT (optional)
        public static async Task<(bool ok, string message, JsonElement? data)> GetBookmarks(string baseUrl, string anonKey, string accessToken, string userId)
        {
            try
            {
                var url = baseUrl.TrimEnd('/') + $"/rest/v1/bookmarks?user_id=eq.{userId}";
                using var client = new HttpClient();
            if (!string.IsNullOrEmpty(accessToken)) client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            // REST endpoints require the anon key in the apikey header
            if (!string.IsNullOrEmpty(anonKey)) client.DefaultRequestHeaders.Add("apikey", anonKey);
                var resp = await client.GetAsync(url);
                var text = await resp.Content.ReadAsStringAsync();
                if (!resp.IsSuccessStatusCode) return (false, text, null);
                var doc = JsonSerializer.Deserialize<JsonElement>(text);
                return (true, "ok", doc);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public static async Task<(bool ok, string message)> AddBookmark(string baseUrl, string anonKey, string accessToken, object bookmark)
        {
            try
            {
                var url = baseUrl.TrimEnd('/') + "/rest/v1/bookmarks";
                using var client = new HttpClient();
            if (!string.IsNullOrEmpty(accessToken)) client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            if (!string.IsNullOrEmpty(anonKey)) client.DefaultRequestHeaders.Add("apikey", anonKey);
                var resp = await client.PostAsJsonAsync(url, bookmark);
                var text = await resp.Content.ReadAsStringAsync();
                if (!resp.IsSuccessStatusCode) return (false, text);
                return (true, "ok");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}

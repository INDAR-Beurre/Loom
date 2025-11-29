namespace LoomBrowser.Models
{
    public class AppSettings
    {
        public string GroqApiKey { get; set; } = "";
        public string GroqModel { get; set; } = "llama3-8b-8192";
        // Supabase settings
        public string SupabaseUrl { get; set; } = "";
        public string SupabaseAnonKey { get; set; } = "";
        // When user signs in we store the access token (optional) and user id for server calls
        public string SupabaseAccessToken { get; set; } = "";
        public string SupabaseUserId { get; set; } = "";
    }
}

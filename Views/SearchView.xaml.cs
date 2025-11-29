using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LoomBrowser.Views
{
    public partial class SearchView : UserControl
    {
        public SearchView()
        {
            InitializeComponent();
        }

        public async Task LoadQuery(string query)
        {
            QueryHeading.Text = query;
            MetaText.Text = "Results from DuckDuckGo";

            try
            {
                using var http = new HttpClient();
                string url = $"https://api.duckduckgo.com/?q={System.Net.WebUtility.UrlEncode(query)}&format=json&pretty=1";
                var resp = await http.GetStringAsync(url);
                using var doc = JsonDocument.Parse(resp);
                if (doc.RootElement.TryGetProperty("AbstractText", out var abs) && abs.GetString()?.Length > 0)
                {
                    AbstractText.Text = abs.GetString();
                }
                else
                {
                    AbstractText.Text = "No instant answer — try opening on a web search.";
                }
            }
            catch (System.Exception ex)
            {
                AbstractText.Text = "Search failed: " + ex.Message;
            }
        }
    }
}

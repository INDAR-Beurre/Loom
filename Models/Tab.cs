using System.Collections.ObjectModel;

namespace LoomBrowser.Models
{
    public class Tab
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Type { get; set; } // home | search | site
        public string Url { get; set; }
        public string Query { get; set; }
    }
}

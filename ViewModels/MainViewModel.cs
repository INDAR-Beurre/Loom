using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LoomBrowser.Models;

namespace LoomBrowser.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Tab> Tabs { get; } = new ObservableCollection<Tab>();

        private Tab _activeTab;
        public Tab ActiveTab
        {
            get => _activeTab;
            set { _activeTab = value; OnPropertyChanged(); }
        }

        public MainViewModel()
        {
            var home = new Tab { Id = "home-1", Title = "Loom - Home", Type = "home" };
            Tabs.Add(home);
            ActiveTab = home;
        }

        public void CreateTab()
        {
            var t = new Tab { Id = System.Guid.NewGuid().ToString(), Title = "New Tab", Type = "home" };
            Tabs.Add(t);
            ActiveTab = t;
        }

        public void CloseTab(Tab tab)
        {
            if (!Tabs.Contains(tab)) return;
            if (Tabs.Count == 1) return;
            int idx = Tabs.IndexOf(tab);
            Tabs.Remove(tab);
            if (ActiveTab == tab)
            {
                ActiveTab = Tabs[Math.Min(idx, Tabs.Count - 1)];
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

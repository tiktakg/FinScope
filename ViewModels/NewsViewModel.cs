using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FinScope.ViewModels
{
    public partial class NewsViewModel : ObservableObject
    {
        private readonly HttpClient _httpClient = new();

        public ObservableCollection<NewsItem> News { get; } = new();

        public IRelayCommand<string> OpenNewsCommand { get; }

        public NewsViewModel()
        {
            OpenNewsCommand = new RelayCommand<string>(OpenInBrowser);
            _ = LoadNewsAsync();
        }

        private void OpenInBrowser(string? url)
        {
            if (!string.IsNullOrWhiteSpace(url))
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    });
                }
                catch { }
            }
        }

        private async Task LoadNewsAsync()
        {
            const string apiKey = "d128571r01qmhi3h1go0d128571r01qmhi3h1gog"; // ← замените на ключ от finnhub.io
            string url = $"https://finnhub.io/api/v1/news?category=general&token={apiKey}";

            try
            {
                var response = await _httpClient.GetStringAsync(url);
                var newsItems = JsonSerializer.Deserialize<List<NewsItem>>(response, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                News.Clear();
                foreach (var item in newsItems?.Take(20) ?? [])
                    News.Add(item);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка загрузки новостей: {ex.Message}");
            }
        }
        public class NewsItem
        {
            public string Headline { get; set; } = "";
            public string Source { get; set; } = "";
            public string Summary { get; set; } = "";
            public string Url { get; set; } = "";
            public DateTime Datetime => DateTimeOffset.FromUnixTimeSeconds(Time).DateTime;

            public long Time { get; set; }
        }
    }
}

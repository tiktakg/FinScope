using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;

namespace FinScope.ViewModels
{
    public partial class NewsViewModel : ObservableObject
    {
        private const string ApiKey = "80TWffhBOPoosAg9pywGOU0pC8yNJft9W7mYIWf9"; // Marketaux API ключ

        [ObservableProperty]
        private ObservableCollection<NewsArticle> _news = new();

        public IAsyncRelayCommand LoadNewsCommand { get; }

        public NewsViewModel()
        {
            LoadNewsCommand = new AsyncRelayCommand(LoadNewsAsync);
            LoadNewsCommand.Execute(null);
        }

        private async Task LoadNewsAsync()
        {
            News.Clear();

            await LoadMarketauxNewsAsync();
            await LoadRbcNewsAsync();
            await LoadVedomostiNewsAsync();
            await LoadYahooFinanceNewsAsync();
        }

        private async Task LoadMarketauxNewsAsync()
        {
            using var http = new HttpClient();
            var url = $"https://api.marketaux.com/v1/news/all?language=ru&limit=10&api_token={ApiKey}";

            try
            {
                var response = await http.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    return;

                using var stream = await response.Content.ReadAsStreamAsync();
                using var json = await JsonDocument.ParseAsync(stream);

                var root = json.RootElement;

                if (root.TryGetProperty("data", out var articlesJson))
                {
                    foreach (var item in articlesJson.EnumerateArray())
                    {
                        News.Add(new NewsArticle
                        {
                            Title = item.GetProperty("title").GetString(),
                            Description = item.GetProperty("description").GetString(),
                            Url = item.GetProperty("url").GetString(),
                            Source = item.GetProperty("source").GetString(),
                            PublishedAt = item.GetProperty("published_at").GetDateTime()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке с Marketaux: {ex.Message}");
            }
        }

        private async Task LoadRbcNewsAsync()
        {
            const string rbcRssUrl = "https://static.feed.rbc.ru/rbc/logical/footer/news.rss";

            await LoadRssNewsAsync(rbcRssUrl);
        }

        private async Task LoadVedomostiNewsAsync()
        {
            const string vedomostiRssUrl = "https://www.vedomosti.ru/rss/news";

            await LoadRssNewsAsync(vedomostiRssUrl);
        }

        private async Task LoadYahooFinanceNewsAsync()
        {
            const string yahooFinanceRssUrl = "https://finance.yahoo.com/rss/topstories";

            await LoadRssNewsAsync(yahooFinanceRssUrl);
        }

        private async Task LoadRssNewsAsync(string rssUrl)
        {
            using var http = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            http.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; FinScopeApp/1.0)");

            try
            {
                var stream = await http.GetStreamAsync(rssUrl);
                using var reader = XmlReader.Create(stream);
                var feed = SyndicationFeed.Load(reader);

                if (feed == null)
                    return;

                foreach (var item in feed.Items.Take(10))
                {
                    News.Add(new NewsArticle
                    {
                        Title = item.Title.Text,
                        Description = item.Summary?.Text ?? "",
                        Url = item.Links.FirstOrDefault()?.Uri.ToString() ?? "",
                        Source = feed.Title.Text,
                        PublishedAt = item.PublishDate.DateTime
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке RSS с {rssUrl}: {ex.Message}");
            }
        }

        public class NewsArticle
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public string Url { get; set; }
            public string Source { get; set; }
            public DateTime PublishedAt { get; set; }
        }
    }
}

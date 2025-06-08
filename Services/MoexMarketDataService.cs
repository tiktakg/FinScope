using FinScope.Interfaces;
using FinScope.ViewModels;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Net.Http.Json;
using System.Text.Json;
using System.Diagnostics;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

public class MoexMarketDataService : IMarketDataService
{
    private readonly HttpClient _client;
    private const string Base = "https://iss.moex.com/iss";

    private readonly string[] _top = { "SBER", "GAZP", "LKOH", "VTBR", "YNDX" };

    private readonly Dictionary<string, string> _indices = new()
    {
        { "MOEX", "IMOEX" },
        { "RTS", "RTSI" }
    };

    public MoexMarketDataService(HttpClient client) => _client = client;

    public async Task<IEnumerable<Stock>> GetTopStocksAsync()
    {
        var tasks = _top.Select(GetStockBySymbolAsync);
        var stocks = await Task.WhenAll(tasks);
        return stocks.Where(s => s != null)!;
    }
    public async Task<Stock?> GetStockBySymbolAsync(string symbol)
    {
        try
        {
            var url = $"https://iss.moex.com/iss/engines/stock/markets/shares/securities/{symbol}.json?iss.meta=off&iss.json=extended&securities.columns=SECID,SECNAME,SECTORID&marketdata.columns=LAST,LASTCHANGE,LASTCHANGEPRCNT,VOLTODAY";
            Debug.WriteLine($"[Request] {url}");

            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var responseData = JsonConvert.DeserializeObject<List<Root>>(json);

            if (responseData == null || responseData.Count < 2 ||
                responseData[1]?.securities == null || responseData[1].securities.Count == 0 ||
                responseData[1]?.marketdata == null || responseData[1].marketdata.Count == 0)
                return null;

            // Получаем данные из securities
            var security = responseData[1].securities.FirstOrDefault();
            var companyName = security?.SECNAME ?? "N/A";
            var sector = security?.SECTORID?.ToString() ?? "N/A";

            // Получаем данные из marketdata (первое не-null значение LAST)
            var marketData = responseData[1].marketdata
                .FirstOrDefault(m => m.LAST.HasValue) ?? responseData[1].marketdata.First();

            return new Stock
            {
                Symbol = symbol,
                CompanyName = companyName,
                Sector = sector,
                Price = marketData.LAST.HasValue ? (decimal)marketData.LAST.Value : 0,
                Change = (decimal)marketData.LASTCHANGE,
                ChangePercent = (decimal)marketData.LASTCHANGEPRCNT,
                Volume = marketData.VOLTODAY,
                Quantity = 0 // Это поле не доступно в стандартном ответе MOEX
            };
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[ERROR] {symbol}: {ex.GetType().Name} - {ex.Message}");
            return null;
        }
    }


    public async Task<Dictionary<string, Stock>> GetMarketIndicesAsync()
    {
        var result = new Dictionary<string, Stock>();
        foreach (var kv in _indices)
        {
            var stock = await GetStockBySymbolAsync(kv.Value);
            if (stock != null)
                result[kv.Key] = stock;
        }
        return result;
    }

    public async Task<IEnumerable<Stock>> GetStocksBySectorAsync(string sector)
    {
        // Поскольку MOEX API не возвращает секторы напрямую, используем фильтрацию по заранее загруженным акциям
        var all = await GetTopStocksAsync();
        return all.Where(s => s.Sector?.Equals(sector, StringComparison.OrdinalIgnoreCase) == true);
    }

    public Task<IEnumerable<string>> GetAvailableSymbolsAsync() =>
        Task.FromResult(_top.AsEnumerable());

    public async Task<IEnumerable<CandleStickData>> GetCandlestickDataAsync(string symbol)
    {
        try
        {
            var from = DateTime.UtcNow.AddDays(-30).ToString("yyyy-MM-dd");
            var to = DateTime.UtcNow.ToString("yyyy-MM-dd");
            var url = $"{Base}/engines/stock/markets/shares/securities/{symbol}/candles.json?from={from}&till={to}&interval=24";

            var json = await _client.GetStringAsync(url);
            using var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("candles", out var block))
                return Enumerable.Empty<CandleStickData>();

            var columns = block.GetProperty("columns").EnumerateArray().Select(c => c.GetString()).ToArray();
            var data = block.GetProperty("data").EnumerateArray();

            var result = new List<CandleStickData>();

            foreach (var row in data)
            {
                var values = row.EnumerateArray().ToArray();
                result.Add(new CandleStickData
                {
                    Date = DateTime.Parse(values[Array.IndexOf(columns, "TRADEDATE")].GetString()!),
                    Open = values[Array.IndexOf(columns, "OPEN")].GetDouble(),
                    High = values[Array.IndexOf(columns, "HIGH")].GetDouble(),
                    Low = values[Array.IndexOf(columns, "LOW")].GetDouble(),
                    Close = values[Array.IndexOf(columns, "CLOSE")].GetDouble(),
                    Volume = values[Array.IndexOf(columns, "VOLUME")].GetDouble()
                });
            }

            return result.OrderBy(c => c.Date);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[ERROR] Candles {symbol}: {ex.Message}");
            return Enumerable.Empty<CandleStickData>();
        }
    }

    public async Task<CompanyInfo> GetCompanyInfoAsync(string symbol)
    {
        try
        {
            var url = $"{Base}/securities/{symbol}.json";
            var json = await _client.GetStringAsync(url);
            using var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("securities", out var block)) return new CompanyInfo { Description = "" };

            var cols = block.GetProperty("columns").EnumerateArray().Select(c => c.GetString()).ToArray();
            var row = block.GetProperty("data")[0].EnumerateArray().ToArray();

            var map = cols.Zip(row, (c, v) => new { c, v }).ToDictionary(x => x.c!, x => x.v);
            return new CompanyInfo
            {
                Description = $"{map["SHORTNAME"].GetString()} | {map["SECID"].GetString()}"
            };
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[ERROR] CompanyInfo {symbol}: {ex.Message}");
            return new CompanyInfo { Description = "" };
        }
    }

    public async Task<IEnumerable<NewsItem>> GetNewsAsync(string symbol)
    {
        try
        {
            var url = $"{Base}/sitenews.json";
            var json = await _client.GetStringAsync(url);
            using var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("sitenews", out var newsBlock))
                return Enumerable.Empty<NewsItem>();

            var cols = newsBlock.GetProperty("columns").EnumerateArray().Select(x => x.GetString()).ToArray();
            var data = newsBlock.GetProperty("data").EnumerateArray();

            var news = new List<NewsItem>();

            foreach (var row in data)
            {
                var values = row.EnumerateArray().ToArray();
                var published = values[Array.IndexOf(cols, "published")].GetString();
                var title = values[Array.IndexOf(cols, "title")].GetString();
                var id = values[Array.IndexOf(cols, "id")].GetInt32();

                news.Add(new NewsItem
                {
                    Title = title ?? "Без названия",
                    Url = $"https://www.moex.com/n{id}",
                    PublishedAt = DateTime.TryParse(published, out var dt) ? dt : DateTime.MinValue,
                    Source = "MOEX"
                });
            }

            return news.OrderByDescending(n => n.PublishedAt);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[ERROR] News: {ex.Message}");
            return Enumerable.Empty<NewsItem>();
        }
    }

    // DTO классы
    public class CandleStickData
    {
        public DateTime Date { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public double Volume { get; set; }
    }

    public class NewsItem
    {
        public string Title { get; set; }
        public string Summary { get; set; } = string.Empty;
        public DateTime PublishedAt { get; set; }
        public string Url { get; set; }
        public string Source { get; set; }
    }

    public class CompanyInfo
    {
        public string Description { get; set; }
    }

private class QuoteResponse
    {
        public decimal c { get; set; }
        public decimal d { get; set; }
        public decimal dp { get; set; }
        public long v { get; set; }
    }

    private class ProfileResponse
    {
        public string name { get; set; }
        public string finnhubIndustry { get; set; }
    }

    private class SymbolResponse
    {
        public string symbol { get; set; }
    }

    private class CacheItem
    {
        public Stock Data { get; set; }
        public DateTime Timestamp { get; set; }
    }
}

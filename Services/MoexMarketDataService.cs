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
using FinScope.Models;
using HtmlAgilityPack;

public class MoexMarketDataService : IMarketDataService
{
    private readonly HttpClient _client;
    private const string Base = "https://iss.moex.com/iss";


    private readonly string[] _top = {
    "SBER", "GAZP", "LKOH", "VTBR", "YNDX",
    "ROSN", "NVTK", "GMKN", "TATN", "PLZL",
    "MTSS", "CHMF", "MOEX", "ALRS", "AFKS",
    "FIVE", "IRAO", "MAGN", "TRNFP", "HYDR",
    "PIKK", "SNGS", "SNGSP", "RTKM", "RUAL",
    "PHOR", "TGKA", "RASP", "AFLT", "POLY",
    "BANEP", "ENPG", "LSNG", "LSRG", "BSPB",
    "UNAC", "SGZH", "MGNT", "RNFT", "TTLK",
    "OZON", "QIWI", "MTLR", "AGRO", "MRKP",
    "MRKC", "MRKY", "MRKU", "MRKV", "MRKS",
    "MRKB", "MRKN", "MRKZ", "DVEC", "ELFV"
};


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

            //await GetSectorsFromMoexAsync();
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
                Price = marketData.LAST.HasValue ? marketData.LAST.Value : 0,
                Change =marketData.LASTCHANGE,
                ChangePercent = marketData.LASTCHANGEPRCNT,
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
   
    public async Task GetSectorsFromMoexAsync()
    {
        var url = "https://iss.moex.com/iss/statistics/engines/stock/markets/shares/sectors.json";

        using var client = new HttpClient();
        var response = await client.GetStringAsync(url);

        using var doc = JsonDocument.Parse(response);
        var sectors = doc.RootElement
            .GetProperty("sectors")
            .GetProperty("data");

        foreach (var sectorData in sectors.EnumerateArray())
        {
            var id = sectorData[0].GetInt32();       // ID сектора
            var name = sectorData[1].GetString();    // Название сектора
            Console.WriteLine($"{id} - {name}");
        }
    }


    public async Task<List<MarketIndex>> GetMarketIndicesAsync()
    {
        var test = new List<MarketIndex>
    {
        new MarketIndex
        {
            Name = "MOEX",
            Value = 3300.25m,
            Change = -15.42m,
            ChangePercent = -0.47m
        },
        new MarketIndex
        {
            Name = "RTSI",
            Value = 1290.45m,
            Change = 5.11m,
            ChangePercent = 0.4m
        },
        new MarketIndex
        {
            Name = "MCFTR",
            Value = 15700.88m,
            Change = 110.55m,
            ChangePercent = 0.71m
        },
        new MarketIndex
        {
            Name = "S&P 500",
            Value = 4123.34m,
            Change = 12.45m,
            ChangePercent = 0.30m
        },
        new MarketIndex
        {
            Name = "NASDAQ",
            Value = 12345.67m,
            Change = -23.45m,
            ChangePercent = -0.19m
        },
        new MarketIndex
        {
            Name = "Dow Jones",
            Value = 33456.78m,
            Change = 45.67m,
            ChangePercent = 0.14m
        },
        new MarketIndex
        {
            Name = "FTSE 100",
            Value = 7100.12m,
            Change = 18.34m,
            ChangePercent = 0.26m
        }
    };

        return test;
    }


    //public async Task<IEnumerable<Stock>> GetStocksBySectorAsync(string sector)
    //{
    //    // Поскольку MOEX API не возвращает секторы напрямую, используем фильтрацию по заранее загруженным акциям
    //    var all = await GetTopStocksAsync();
    //    return all.Where(s => s.Sector?.Equals(sector, StringComparison.OrdinalIgnoreCase) == true);
    //}

    //public Task<IEnumerable<string>> GetAvailableSymbolsAsync() =>
    //    Task.FromResult(_top.AsEnumerable());

    public async Task<IEnumerable<CandleStickData>> GetCandlestickDataAsync(string symbol)
    {
        try
        {
            var from = DateTime.UtcNow.AddDays(-360).ToString("yyyy-MM-dd");
            var to = DateTime.UtcNow.ToString("yyyy-MM-dd");
            var url = $"{Base}/engines/stock/markets/shares/securities/{symbol}/candles.json?from={from}&till={to}&interval=24";

            using var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync(url);

            var root = JsonConvert.DeserializeObject<CandelsRoot>(response);

            var result = root?.candles?.data?.Select(row => new CandleStickData
            {
                Open = Convert.ToDouble(row[0]),
                Close = Convert.ToDouble(row[1]),
                High = Convert.ToDouble(row[2]),
                Low = Convert.ToDouble(row[3]),
                Volume = Convert.ToDouble(row[5]),
                Date = DateTime.Parse(row[6].ToString())
            }) ?? Enumerable.Empty<CandleStickData>();

            return result.OrderBy(c => c.Date);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[ERROR] Candles {symbol}: {ex.Message}");
            return Enumerable.Empty<CandleStickData>();
        }
    }




    public async Task<CompanyInfoDetails> GetCompanyDetailsAsync(string symbol)
    {
        try
        {
            var url = $"{Base}/securities/{symbol}.json"; // Подразумевается, что URL возвращает блок `description`
            var json = await _client.GetStringAsync(url);
            using var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("description", out var desc)) return null;

            var cols = desc.GetProperty("columns").EnumerateArray().Select(c => c.GetString()).ToArray();
            var rowDict = desc.GetProperty("data")
                .EnumerateArray()
                .Select(row => row.EnumerateArray().ToArray())
                .ToDictionary(r => r[0].GetString(), r => r);

            string? TryGetString(string key) =>
                rowDict.TryGetValue(key, out var row) ? row[2].GetString() : null;

            long? TryGetLong(string key) =>
                long.TryParse(TryGetString(key), out var value) ? value : null;

            decimal? TryGetDecimal(string key) =>
                decimal.TryParse(TryGetString(key), out var value) ? value : null;

            DateTime? TryGetDate(string key) =>
                DateTime.TryParse(TryGetString(key), out var value) ? value : null;

            bool TryGetBool(string key) =>
                TryGetString(key) == "1";

            return new CompanyInfoDetails
            {
                SecId = TryGetString("SECID"),
                ShortName = TryGetString("SHORTNAME"),
                FullName = TryGetString("NAME"),
                ISIN = TryGetString("ISIN"),
                RegNumber = TryGetString("REGNUMBER"),
                IssueName = TryGetString("ISSUENAME"),
                TypeName = TryGetString("TYPENAME"),
                GroupName = TryGetString("GROUPNAME"),
                Currency = TryGetString("FACEUNIT"),
                IssueSize = TryGetLong("ISSUESIZE"),
                FaceValue = TryGetDecimal("FACEVALUE"),
                IssueDate = TryGetDate("ISSUEDATE"),
                IsForQualifiedInvestors = TryGetBool("ISQUALIFIEDINVESTORS"),
                ListingLevel = (int?)TryGetLong("LISTLEVEL")
            };
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[ERROR] GetCompanyDetailsAsync: {ex.Message}");
            return new CompanyInfoDetails();
        }
    }

    public async Task<IEnumerable<NewsArticle>> GetNewsAsync(string symbol)
    {
        try
        {
            var url = $"{Base}/sitenews.json";

            var response = await _client.GetStringAsync(url);

            var root = JsonConvert.DeserializeObject<RootSiteNews>(response);

            var news = new List<NewsArticle>();

            if (root?.sitenews?.data != null && root.sitenews.columns != null)
            {
                var columns = root.sitenews.columns;

                // Найдём индексы нужных полей по названию колонки
                int id = columns.IndexOf("id");
                int idxTitle = columns.IndexOf("title");
                int idxSummary = columns.IndexOf("summary");
                int idxPublishedAt = columns.IndexOf("published_at");
                int idxUrl = columns.IndexOf("url");
                int idxSource = columns.IndexOf("source");

                foreach (var row in root.sitenews.data)
                {
                    var item = new NewsArticle();



                    if (id >= 0 && id < row.Count && row[id] != null)
                    {
                        Int32.TryParse(row[id].ToString(), out int newId);
                        item.id = newId;
                    }

                    if (idxTitle >= 0 && idxTitle < row.Count && row[idxTitle] != null)
                        item.Title = row[idxTitle].ToString();

                    if (idxSummary >= 0 && idxSummary < row.Count && row[idxSummary] != null)
                        item.Summary = row[idxSummary].ToString();

                    if (idxPublishedAt >= 0 && idxPublishedAt < row.Count && row[idxPublishedAt] != null)
                    {
                        if (DateTime.TryParse(row[idxPublishedAt].ToString(), out var publishedAt))
                            item.PublishedAt = publishedAt;
                    }

                    if (idxUrl >= 0 && idxUrl < row.Count && row[idxUrl] != null)
                        item.Url = row[idxUrl].ToString();

                    if (idxSource >= 0 && idxSource < row.Count && row[idxSource] != null)
                        item.Source = row[idxSource].ToString();

                    news.Add(item);
                }

                foreach (var newItem in news)
                {
                    newItem.Summary = HtmlToPlainText(await GetNewsBodyByIdAsync(newItem.id));
                    newItem.Description = HtmlToPlainText(await GetNewsBodyByIdAsync(newItem.id));
                }

            }

            return news.OrderByDescending(n => n.PublishedAt);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[ERROR] News: {ex.Message}");
            return Enumerable.Empty<NewsArticle>();
        }
    }



    public async Task<string> GetNewsBodyByIdAsync(int id)
    {
        try
        {
            var url = $"{Base}/sitenews/{id}.json";
            var response = await _client.GetStringAsync(url);

            var root = JsonConvert.DeserializeObject<RootNewsBody.Root>(response);

            if (root?.content?.data != null && root.content.data.Count > 0 &&
                root.content.columns != null)
            {
                var columns = root.content.columns;
                int idxBody = columns.IndexOf("body");

                if (idxBody >= 0)
                {
                    var row = root.content.data[0];
                    if (idxBody < row.Count)
                        return row[idxBody]?.ToString() ?? string.Empty;
                }
            }

            return string.Empty;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[ERROR] GetNewsBodyByIdAsync: {ex.Message}");
            return string.Empty;
        }
    }



    public static string HtmlToPlainText(string html)
    {
        if (string.IsNullOrWhiteSpace(html))
            return string.Empty;

        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        return HtmlEntity.DeEntitize(doc.DocumentNode.InnerText);
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

    public class NewsArticle
    {
        public int id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string Summary { get; set; } = string.Empty;

        public string Url { get; set; }
        public string Source { get; set; }
        public DateTime PublishedAt { get; set; }
    }

    public class CompanyInfoDetails
    {
        public string SecId { get; set; }                // Код ценной бумаги (например, SBER)
        public string ShortName { get; set; }            // Краткое наименование (Сбербанк)
        public string FullName { get; set; }             // Полное наименование (Сбербанк России ПАО ао)
        public string ISIN { get; set; }                 // Международный идентификационный код
        public string RegNumber { get; set; }            // Номер гос. регистрации
        public string IssueName { get; set; }            // Тип ценной бумаги (Акции обыкновенные)
        public string TypeName { get; set; }             // Вид/категория бумаги (Акция обыкновенная)
        public string GroupName { get; set; }            // Тип инструмента (например, Акции)
        public string Currency { get; set; }             // Валюта номинала (например, RUB)
        public long? IssueSize { get; set; }             // Объем выпуска
        public decimal? FaceValue { get; set; }          // Номинальная стоимость
        public DateTime? IssueDate { get; set; }         // Дата начала торгов
        public bool IsForQualifiedInvestors { get; set; }// Только для квалифицированных инвесторов
        public int? ListingLevel { get; set; }           // Уровень листинга (1, 2, 3)
    }

}
using FinScope.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MoexMarketDataService;

namespace FinScope.Interfaces
{
    public interface IMarketDataService
    {
        Task<IEnumerable<Stock>> GetTopStocksAsync();
        Task<Stock?> GetStockBySymbolAsync(string symbol);
        Task<List<MarketIndex>> GetMarketIndicesAsync();
        //Task<IEnumerable<Stock>> GetStocksBySectorAsync(string sector);
        //Task<IEnumerable<string>> GetAvailableSymbolsAsync();
        Task<IEnumerable<CandleStickData>> GetCandlestickDataAsync(string symbol);
        Task<CompanyInfoDetails> GetCompanyDetailsAsync(string symbol);
        Task<IEnumerable<NewsArticle>> GetNewsAsync(string symbol);
        Task<string> GetNewsBodyByIdAsync(int id);
    }
}

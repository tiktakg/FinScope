using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinScope.Context;
using FinScope.Enitys;
using FinScope.Interfaces;
using FinScope.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using static MoexMarketDataService;

namespace FinScope.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly IAuthService _authService;
        private readonly FinScopeDbContext _dbContext;
        private readonly IMarketDataService _marketDataService;

        [ObservableProperty]
        private decimal _portfolioBalance;

        [ObservableProperty]
        private decimal _balanceChangePercent;

        [ObservableProperty]
        private decimal _totalProfit;

        [ObservableProperty]
        private decimal _profitPercent;

        [ObservableProperty]
        private ObservableCollection<PortfolioAsset> _topAssets = new();

        [ObservableProperty]
        private ObservableCollection<Transaction> _recentTransactions = new();

        [ObservableProperty]
        private ObservableCollection<NewsArticle> _marketNews = new();

        public string BalanceChangeColor => BalanceChangePercent >= 0 ? "#FF4CAF50" : "#FFF44336";
        public string ProfitColor => ProfitPercent >= 0 ? "#FF4CAF50" : "#FFF44336";

        public DashboardViewModel(
            IMarketDataService marketDataService,
       IAuthService authService,
     FinScopeDbContext dbContext)
        {
            _dbContext = dbContext;
            _authService = authService;
            _marketDataService = marketDataService;

            LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
            LoadDataCommand.Execute(null);
        }

        public IAsyncRelayCommand LoadDataCommand { get; }

        private async Task LoadDataAsync()
        {
            await Task.WhenAll(
                LoadPortfolioAsync(),
                LoadTransactionsAsync(),
                LoadNewsAsync());
        }

        private async Task LoadPortfolioAsync()
        {
            var userId = _authService.CurrentUser?.Id;
            if (userId == null)
                return;

            var assets = _dbContext.PortfolioAssets
                  .Where(a => a.UserId == userId)
                  .Select(a => new PortfolioAsset
                  {
                      Id = a.Id,
                      UserId = a.UserId,
                      StockId = a.StockId,
                      Stock = a.Stock,
                      Quantity = a.Quantity,
                      AvgPrice = a.AvgPrice,
                      CurrentPrice = a.CurrentPrice,
                      Profit = a.Profit,
                      ProfitPercent = a.ProfitPercent,
                      Value = a.Value
                  })
                  .ToList();

            if (assets?.Any() != true)
                return;

            // Обновляем топ-активы
            TopAssets.Clear();
            foreach (var asset in assets.OrderByDescending(a => a.Value).Take(5))
                TopAssets.Add(asset);

            // Баланс, прибыль и проценты
            PortfolioBalance = (decimal)assets.Sum(a => a.Value);
            TotalProfit = (decimal)assets.Sum(a => a.Profit);
            ProfitPercent = PortfolioBalance > 0 ? (TotalProfit / (PortfolioBalance - TotalProfit)) * 100 : 0;

        }

        private async Task LoadTransactionsAsync()
        {
            var userId = _authService.CurrentUser?.Id;
            if (userId == null)
                return;

            var transactions = await _dbContext.Transactions
                 .Include(t => t.Stock)
            .OrderByDescending(t => t.Date)
                 .Where(t => t.UserId == _authService.CurrentUser.Id)
                 .ToListAsync();

            RecentTransactions.Clear();
            foreach (var tx in transactions
                .OrderByDescending(t => t.Date)
                .Take(10))
            {
                RecentTransactions.Add(tx);
            }
        }

        private async Task LoadNewsAsync()
        {
            var newsItems = await _marketDataService.GetNewsAsync("MOEX"); // можно подставить индекс или главный актив
            MarketNews.Clear();
            foreach (var n in newsItems)
                MarketNews.Add(n);
        }
    }
}

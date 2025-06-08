using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MoexMarketDataService;

namespace FinScope.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
    // private readonly IPortfolioService _portfolioService;
    //private readonly IMarketDataService _marketDataService;

    [ObservableProperty]
    private decimal _portfolioBalance = 12500.75m;

    [ObservableProperty]
    private decimal _balanceChangePercent = 2.34m;

    [ObservableProperty]
    private decimal _totalProfit = 1250.50m;

    [ObservableProperty]
    private decimal _profitPercent = 12.5m;

    [ObservableProperty]
    private ObservableCollection<Asset> _topAssets = new()
        {
            new Asset { Symbol = "AAPL", Value = 4500.00m, ChangePercent = 1.23m },
            new Asset { Symbol = "MSFT", Value = 3800.00m, ChangePercent = -0.45m },
            new Asset { Symbol = "GOOGL", Value = 2200.00m, ChangePercent = 3.21m }
        };

    [ObservableProperty]
    private ObservableCollection<Transaction> _recentTransactions = new()
        {
            new Transaction { Date = DateTime.Now.AddDays(-1), Symbol = "AAPL", Type = "Покупка", Price = 1000.00m },
            new Transaction { Date = DateTime.Now.AddDays(-3), Symbol = "MSFT", Type = "Продажа", Price = -750.00m }
        };

    [ObservableProperty]
    private ObservableCollection<NewsItem> _marketNews = new()
        {
            new NewsItem { Title = "ФРС оставляет ставки без изменений", Source = "Bloomberg", Summary = "Федеральная резервная система приняла решение..." },
            new NewsItem { Title = "Apple представляет новые продукты", Source = "CNBC", Summary = "Компания Apple анонсировала новую линейку..." }
        };

    public string BalanceChangeColor => BalanceChangePercent >= 0 ? "#FF4CAF50" : "#FFF44336";
    public string ProfitColor => ProfitPercent >= 0 ? "#FF4CAF50" : "#FFF44336";

    public DashboardViewModel(
        //IPortfolioService portfolioService, IMarketDataService marketDataService
        )
    {
        //_portfolioService = portfolioService;
        //_marketDataService = marketDataService;

        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
        LoadDataCommand.Execute(null);
    }

    public IAsyncRelayCommand LoadDataCommand { get; }

    private async Task LoadDataAsync()
    {
        // Здесь будет реальная загрузка данных из сервисов
        // var portfolio = await _portfolioService.GetPortfolioAsync();
        // PortfolioBalance = portfolio.TotalValue;
        // и т.д.
    }
}

public class Asset : ObservableObject
{
    public string Symbol { get; set; }
    public decimal Value { get; set; }
    public decimal ChangePercent { get; set; }
    public string ChangeColor => ChangePercent >= 0 ? "#FF4CAF50" : "#FFF44336";
}

    public class Transaction
    {
        public DateTime Date { get; set; }
        public string Type { get; set; } = "";
        public string Symbol { get; set; } = "";
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total => Quantity * Price;
        public string? Exchange { get; set; }
        public string? Sector { get; set; }
        public string? Description { get; set; }
    }

}



using CommunityToolkit.Mvvm.ComponentModel;
using FinScope.Interfaces;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using static MoexMarketDataService;
using CommunityToolkit.Mvvm.Input;
using ScottPlot.Avalonia;
using ScottPlot;
using System.Linq;
using ScottPlot;
using ScottPlot.Plottables;
using SkiaSharp;
using FinScope.Context;
using Microsoft.EntityFrameworkCore;
using FinScope.Enitys;


namespace FinScope.ViewModels;

public partial class StockDetailViewModel : ObservableValidator
{
    private readonly FinScopeDbContext _dbContext; // Добавим через конструктор
    private readonly IMarketDataService _marketDataService;
    private readonly IAuthService _authService;

    public Stock Stock { get; private set; }

    [ObservableProperty]
    private CompanyInfoDetails _companyInfo;

    [ObservableProperty]
    private IReadOnlyList<OHLC> _ohlcData; // только данные

    [ObservableProperty]
    private bool isAddToPortfolioModalVisible;

    [ObservableProperty]
    private int addStockCount;

    public ObservableCollection<NewsArticle> News { get; } = new ObservableCollection<NewsArticle>();

    public IRelayCommand ShowAddToPortfolioModalCommand { get; }
    public IRelayCommand CancelAddToPortfolioCommand { get; }
    public StockDetailViewModel(
        IAuthService authService,
      IMarketDataService marketDataService,
      Stock stock,
      FinScopeDbContext dbContext)
    {
        _marketDataService = marketDataService;
        _authService = authService;
        _dbContext = dbContext;
        Stock = stock;


        ShowAddToPortfolioModalCommand = new RelayCommand(() => IsAddToPortfolioModalVisible = true);
        CancelAddToPortfolioCommand = new RelayCommand(() => IsAddToPortfolioModalVisible = false);


        // Создаем тестовые данные для свечей (OHLC)
     
    }
    public async Task InitAsync()
    {
        await LoadDataAsync();
    }
    private async Task LoadDataAsync()
    {
        try
        {
            await LoadCandlestickDataAsync();
            await LoadCompanyInfoAsync();
            await LoadNewsAsync();
        }
        catch (Exception ex)
        {

        }

    }
    private async Task LoadCompanyInfoAsync()
    {
        CompanyInfo = await _marketDataService.GetCompanyDetailsAsync(Stock.Symbol);
      
    }

    public async Task LoadCandlestickDataAsync()
    {
        var candles = (await _marketDataService.GetCandlestickDataAsync(Stock.Symbol))
            .OrderBy(c => c.Date)
            .ToList();

        _ohlcData = candles.Select(c => new OHLC(
            c.Open, c.High, c.Low, c.Close,
            c.Date,
            TimeSpan.FromDays(1)
        )).ToList();
    }




    private async Task LoadNewsAsync()
    {
        var newsItems = await _marketDataService.GetNewsAsync(Stock.Symbol);
        News.Clear();
        foreach (var n in newsItems)
        {
            News.Add(n);
        }
    }

    [RelayCommand]
    private async Task AddToPortfolioAsync()
    {
        if (AddStockCount <= 0)
            return;

        try
        {
            var currentUser = _authService.CurrentUser;
            if (currentUser == null)
            {
                return;
            }

            // Начинаем транзакцию
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                // Получаем актуальные данные пользователя с блокировкой для обновления
                var user = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.Id == currentUser.Id);

                if (user == null)
                {
                    return;
                }

                // Проверяем существование акции
                var stock = await _dbContext.Stocks
                    .FirstOrDefaultAsync(s => s.Symbol == Stock.Symbol);

                if (stock == null)
                {
                    return;
                }

                // Рассчитываем общую стоимость покупки
                var totalCost = AddStockCount * Stock.Price ?? 0m;



                // Находим существующий актив в портфеле
                var existingAsset = await _dbContext.PortfolioAssets
                    .FirstOrDefaultAsync(p => p.UserId == user.Id && p.StockId == stock.Id);

                if (existingAsset != null)
                {
                    // Обновляем существующий актив
                    var totalOldValue = existingAsset.Quantity * existingAsset.AvgPrice;
                    var totalNewValue = AddStockCount * Stock.Price.Value;

                    existingAsset.Quantity += AddStockCount;
                    existingAsset.AvgPrice = (totalOldValue + totalNewValue) / existingAsset.Quantity;
                    existingAsset.CurrentPrice = Stock.Price;
                }
                else
                {
                    // Добавляем новый актив
                    var newAsset = new PortfolioAsset
                    {
                        UserId = user.Id,
                        StockId = stock.Id,
                        Quantity = AddStockCount,
                        AvgPrice = Stock.Price.Value,
                        CurrentPrice = Stock.Price
                    };
                    await _dbContext.PortfolioAssets.AddAsync(newAsset);
                }

                // Списываем средства со счета пользователя

                // Создаем запись о транзакции
                var transactionRecord = new Transaction
                {
                    UserId = user.Id,
                    StockId = stock.Id,
                    Type = "Покупка",
                    Quantity = AddStockCount,
                    Price = Stock.Price.Value,
                    Total = totalCost,
                    Date = DateTime.UtcNow
                };
                await _dbContext.Transactions.AddAsync(transactionRecord);

                // Сохраняем все изменения
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Debug.WriteLine($"Ошибка транзакции: {ex}");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Ошибка: {ex}");
        }
}
}

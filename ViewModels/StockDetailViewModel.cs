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


namespace FinScope.ViewModels;

public partial class StockDetailViewModel : ObservableValidator
{
    private readonly IMarketDataService _marketDataService;

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
    public StockDetailViewModel(IMarketDataService marketDataService, Stock stock)
    {
        _marketDataService = marketDataService;
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
    private void AddToPortfolio()
    {
        // Здесь логика добавления в портфель
        Debug.WriteLine($"Добавлено {AddStockCount} акций {Stock.Symbol} в портфель");

        // Закрыть модальное окно после подтверждения
        IsAddToPortfolioModalVisible = false;
    }
}

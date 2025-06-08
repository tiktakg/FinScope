using CommunityToolkit.Mvvm.ComponentModel;
using FinScope.Interfaces;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using static MoexMarketDataService;
using CommunityToolkit.Mvvm.Input;



namespace FinScope.ViewModels;

public partial class StockDetailViewModel : ObservableValidator
{
    private readonly IMarketDataService _marketDataService;

    public Stock Stock { get; private set; }

    [ObservableProperty]
    private CompanyInfoDetails _companyInfo;
  
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

    }
    public async Task InitAsync()
    {
        await LoadDataAsync();
    }
    private async Task LoadDataAsync()
    {
        try
        {
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
   
    //private async Task LoadCandlestickDataAsync()
    //{
    //    var candles = (await _marketDataService.GetCandlestickDataAsync(Stock.Symbol))
    //             .OrderBy(c => c.Date)
    //             .ToList();

    //    Debug.WriteLine($"First candle date: {candles.First().Date}, Last candle date: {candles.Last().Date}");

    //    var plotModel = new PlotModel { Title = $"{Stock.Symbol} - Свечи" };

    //    // Ось дат
    //    var dateAxis = new DateTimeAxis
    //    {
    //        Position = AxisPosition.Bottom,
    //        StringFormat = "dd.MM",
    //        MajorGridlineStyle = LineStyle.Solid,
    //        MinorGridlineStyle = LineStyle.Dot,
    //        IsZoomEnabled = true,
    //        IsPanEnabled = true,
    //        Minimum = DateTimeAxis.ToDouble(candles.Min(c => c.Date)),
    //        Maximum = DateTimeAxis.ToDouble(candles.Max(c => c.Date))
    //    };

    //    // Ценовая ось
    //    var priceAxis = new LinearAxis
    //    {
    //        Position = AxisPosition.Left,
    //        MajorGridlineStyle = LineStyle.Solid,
    //        MinorGridlineStyle = LineStyle.Dot,
    //        IsZoomEnabled = true,
    //        IsPanEnabled = true,
    //        Minimum = candles.Min(c => c.Low) * 0.98,
    //        Maximum = candles.Max(c => c.High) * 1.02
    //    };

    //    // Свечная серия
    //    var candleSeries = new CandleStickSeries
    //    {
    //        ItemsSource = candles,
    //        DataFieldX = "X",  // Используем вычисляемое свойство X
    //        DataFieldHigh = "High",
    //        DataFieldLow = "Low",
    //        DataFieldOpen = "Open",
    //        DataFieldClose = "Close",
    //        IncreasingColor = OxyColors.Green,
    //        DecreasingColor = OxyColors.Red,
    //        StrokeThickness = 1,
    //          CandleWidth = 0.8
    //    };

    //    plotModel.Axes.Add(dateAxis);
    //    plotModel.Axes.Add(priceAxis);
    //    plotModel.Series.Add(candleSeries);

    //    // Тестовая линейная серия (для проверки)
    //    var testSeries = new LineSeries
    //    {
    //        ItemsSource = candles,
    //        DataFieldX = "X",
    //        DataFieldY = "Close",
    //        Color = OxyColors.Blue,
    //        StrokeThickness = 1
    //    };
    //    plotModel.Series.Add(testSeries);

    //    CandlestickPlotModel = plotModel;
    //    CandlestickPlotModel.InvalidatePlot(true);
    //}



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

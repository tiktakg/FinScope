using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using FinScope.Interfaces;
using FinScope.ViewModels;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using static MoexMarketDataService;


namespace FinScope.ViewModels;

  public partial class StockDetailViewModel : ObservableValidator
{
    private readonly IMarketDataService _marketDataService;

    public Stock Stock { get; private set; }

    public PlotModel CandlestickPlotModel { get; private set; }

    public ObservableCollection<NewsItem> News { get; } = new ObservableCollection<NewsItem>();

    public ICommand AddToPortfolioCommand { get; }

    public StockDetailViewModel(IMarketDataService marketDataService, Stock stock)
    {
        _marketDataService = marketDataService;
        Stock = stock;

        //AddToPortfolioCommand = ReactiveCommand.Create(() => AddToPortfolio());

        LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            await LoadCandlestickDataAsync();
            await LoadCompanyInfoAsync();
            await LoadNewsAsync();
        }
        catch(Exception ex)
        {

        }
      
    }

    private async Task LoadCandlestickDataAsync()
    {
        // Получаем данные по свечам (пример)
        var candles = await _marketDataService.GetCandlestickDataAsync(Stock.Symbol);

        var plotModel = new PlotModel { Title = $"{Stock.Symbol} - Свечи" };

        var dateAxis = new DateTimeAxis
        {
            Position = AxisPosition.Bottom,
            StringFormat = "dd.MM",
            MajorGridlineStyle = LineStyle.Solid,
            MinorGridlineStyle = LineStyle.Dot,
            IntervalType = DateTimeIntervalType.Days,
            MinorIntervalType = DateTimeIntervalType.Hours,
            IsZoomEnabled = true,
            IsPanEnabled = true,
        };

        var priceAxis = new LinearAxis
        {
            Position = AxisPosition.Left,
            MajorGridlineStyle = LineStyle.Solid,
            MinorGridlineStyle = LineStyle.Dot,
            IsZoomEnabled = true,
            IsPanEnabled = true,
        };

        var candleSeries = new CandleStickSeries
        {
            ItemsSource = candles,
            DataFieldX = "Date",
            DataFieldHigh = "High",
            DataFieldLow = "Low",
            DataFieldOpen = "Open",
            DataFieldClose = "Close",
            IncreasingColor = OxyColors.Green,
            DecreasingColor = OxyColors.Red,
        };

        plotModel.Axes.Add(dateAxis);
        plotModel.Axes.Add(priceAxis);
        plotModel.Series.Add(candleSeries);

        CandlestickPlotModel = plotModel;
    }

    private async Task LoadCompanyInfoAsync()
    {
        var info = await _marketDataService.GetCompanyInfoAsync(Stock.Symbol);
     
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

    private void AddToPortfolio()
    {
        // Логика добавления в портфель
    }
}

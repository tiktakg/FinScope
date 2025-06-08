using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using FinScope.ViewModels;
using static FinScope.ViewModels.NewsViewModel;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using static MoexMarketDataService;
using System.Linq;
using ScottPlot;
using ScottPlot.Avalonia;
using ScottPlot.AxisPanels;

namespace FinScope;

public partial class StockDetailView : UserControl

{
    public StockDetailView()
    {
        InitializeComponent();
        this.DataContextChanged += async (_, _) =>
        {
            if (DataContext is StockDetailViewModel vm)
            {
                await vm.LoadCandlestickDataAsync();

                AvaPlot.Plot.Clear();
                AvaPlot.Plot.Add.Candlestick(vm.OhlcData.ToArray());

                AvaPlot.Plot.Title($"{vm.Stock.Symbol} - Свечной график");
                AvaPlot.Plot.XLabel("Дата");
                AvaPlot.Plot.YLabel("Цена");

                //AvaPlot.Plot.FigureBackground.Color = Color.FromHex("#000000");
                AvaPlot.Plot.DataBackground.Color = Color.FromHex("#000000"); // задний фон самного грфика
                AvaPlot.Plot.Grid.MajorLineColor = Color.FromHex("#3d0c2f"); // лини графика
                AvaPlot.Plot.FigureBackground.Color = Color.FromHex("#000000"); // Окнотвка графика
                // the Style object contains helper methods to style many items at once
                AvaPlot.Plot.Axes.Color(Color.FromHex("#ffffff") );

                IXAxis xAxisSecondary = new TopAxis();
                IYAxis yAxisSecondary = new RightAxis();
                IXAxis xAxisPrimary = new BottomAxis();
                IYAxis yAxisPrimary = new LeftAxis();


                //AvaPlot.Plot.Axes.Color(xAxisPrimary, Color.FromHex("#a31c1c"));



                AvaPlot.Plot.Axes.DateTimeTicksBottom();

               

                AvaPlot.Refresh();
            }
        };

  
    }

    private void OnReadMoreClicked(object? sender, PointerPressedEventArgs e)
    {
        if (sender is TextBlock tb &&
            DataContext is StockDetailViewModel vm &&
            tb.DataContext is NewsArticle article &&
            !string.IsNullOrWhiteSpace(article.Url))
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = article.Url,
                    UseShellExecute = true
                });
            }
            catch { /* ????? ???????????? */ }
        }
    }
}
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using FinScope.ViewModels;
using static FinScope.ViewModels.NewsViewModel;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using static MoexMarketDataService;

namespace FinScope;

public partial class StockDetailView : UserControl

{
    public StockDetailView()
    {
        InitializeComponent();


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
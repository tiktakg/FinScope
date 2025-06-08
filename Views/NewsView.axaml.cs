using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using FinScope.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using static FinScope.ViewModels.NewsViewModel;
using System.Diagnostics;
using static MoexMarketDataService;

namespace FinScope;

public partial class NewsView : UserControl
{
    public NewsView()
    {
        InitializeComponent();
        DataContext = ((App)Application.Current).Services.GetRequiredService<NewsViewModel>();

    }
    private void OnReadMoreClicked(object? sender, PointerPressedEventArgs e)
    {
        if (sender is TextBlock tb &&
            DataContext is NewsViewModel vm &&
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
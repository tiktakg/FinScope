using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using FinScope.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace FinScope;

public partial class MarketOverviewView : UserControl
{
    public MarketOverviewView()
    {
        InitializeComponent();
        DataContext = ((App)Application.Current).Services.GetRequiredService<MarketOverviewViewModel>();

    }
}
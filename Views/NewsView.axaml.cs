using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using FinScope.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace FinScope;

public partial class NewsView : UserControl
{
    public NewsView()
    {
        InitializeComponent();
        DataContext = ((App)Application.Current).Services.GetRequiredService<NewsViewModel>();

    }
}
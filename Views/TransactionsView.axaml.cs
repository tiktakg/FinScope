using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using FinScope.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace FinScope;

public partial class TransactionsView : UserControl
{
    public TransactionsView()
    {
        InitializeComponent();
        DataContext = ((App)Application.Current).Services.GetRequiredService<TransactionsViewModel>();

    }
}
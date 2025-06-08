using Avalonia.Controls;
using FinScope.ViewModels;

namespace FinScope.Interfaces
{
    public interface INavigationService
    {
        void Navigate(UserControl view);
        void NavigateToStockDetail(Stock stock);
        void NavigateBack();
    }
}

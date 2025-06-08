using Avalonia.Controls;
using FinScope.Enitys;
using FinScope.Services;
using FinScope.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FinScope.Interfaces
{
    public interface INavigationService
    {
        void Navigate(UserControl view);
        void NavigateToStockDetail(Stock stock);
        void NavigateToLogin();


        void NavigateToRegister();
        void NavigateBack();
    }
}

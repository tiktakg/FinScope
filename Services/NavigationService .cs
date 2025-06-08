using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinScope.Interfaces;
using FinScope.Views;
using FinScope.ViewModels;

namespace FinScope.Services
{
    public class NavigationService : INavigationService
    {
        private Window _mainWindow;
        private readonly Lazy<MainWindowViewModel> _mainViewModelLazy;
        private readonly Stack<UserControl> _navigationStack = new Stack<UserControl>();
        private readonly IMarketDataService _marketDataService;

        public NavigationService(IMarketDataService marketDataService, Lazy<MainWindowViewModel> mainViewModelLazy)
        {
            _marketDataService = marketDataService;
            _mainViewModelLazy = mainViewModelLazy;
        }

        public void SetMainWindow(Window mainWindow)
        {
            _mainWindow = mainWindow;
        }
        public void Navigate(UserControl view)
        {
            _navigationStack.Push(view);
            SetContent(view);
        }
        public void NavigateToStockDetail(Stock stock)
        {
            var vm = new StockDetailViewModel(_marketDataService, stock);
            var view = new StockDetailView
            {
                DataContext = vm
            };
            _mainViewModelLazy.Value.CurrentView = view;
        }
        public void NavigateBack()
        {
            if (_navigationStack.Count > 1)
            {
                _navigationStack.Pop();
                var previousView = _navigationStack.Peek();

                SetContent(previousView);
            }
        }
        private void SetContent(UserControl content)
        {

            if (_mainWindow is MainWindow mainWindow)
            {
                mainWindow.Content = content;
            }
            else
            {
                throw new InvalidOperationException("MainWindow not set or is not of type MainWindow");
            }
        }
    }
}

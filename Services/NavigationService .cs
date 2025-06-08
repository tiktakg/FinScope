using Avalonia.Controls;
using System;
using System.Collections.Generic;
using FinScope.Interfaces;
using FinScope.Views;
using FinScope.ViewModels;
using FinScope.Enitys;
using FinScope.Context;
using Avalonia;
using Microsoft.Extensions.DependencyInjection;
namespace FinScope.Services
{
    public class NavigationService : INavigationService
    {
        private readonly FinScopeDbContext _dbContext; // Добавим через конструктор

        private Window _mainWindow;
        private readonly Lazy<MainWindowViewModel> _mainViewModelLazy;
        private readonly Stack<UserControl> _navigationStack = new Stack<UserControl>();
        private readonly IMarketDataService _marketDataService;
        private readonly IAuthService _authService;

        public NavigationService(IMarketDataService marketDataService,
            IAuthService authService,
        Lazy<MainWindowViewModel> mainViewModelLazy,
            FinScopeDbContext dbContext
           )
        {

            _dbContext = dbContext;
            _authService = authService;
            _marketDataService = marketDataService;
            _mainViewModelLazy = mainViewModelLazy;
            ;
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
            var vm = new StockDetailViewModel(_authService, _marketDataService, stock, _dbContext);
            var view = new StockDetailView
            {
                DataContext = vm
            };

            _mainViewModelLazy.Value.CurrentView = view;

            // Запускаем загрузку данных
            _ = vm.InitAsync(); // ⚠️ Fire-and-forget. Можно добавить лог или await если нужно
        }

        public void NavigateToLogin()
        {
            var vm = new LoginViewModel(_authService, this, _dbContext);
            var view = new LoginView
            {
                DataContext = vm
            };

            _mainViewModelLazy.Value.CurrentView = view;

        }

        public void NavigateToRegister()
        {

            var vm = ((App)Application.Current).Services.GetRequiredService<RegisterViewModel>();
            var view = new RegisterView
            {
                DataContext = vm
            };

            _mainViewModelLazy.Value.CurrentView = view;
        }
        public void NavigateBack()
        {
            var vm = new DashboardViewModel(_marketDataService);
            var view = new DashboardView
            {
                DataContext = vm
            };

            _mainViewModelLazy.Value.CurrentView = view;
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

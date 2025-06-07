using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinScope.Interfaces;
using FinScope.Views;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace FinScope.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private bool _isAuthenticated;

        [ObservableProperty]
        private bool _showLoginView = true;

        [ObservableProperty]
        private object _currentView;

        // Флаги видимости для каждого View
        [ObservableProperty] private bool _isDashboardVisible;
        [ObservableProperty] private bool _isMarketOverviewVisible;
        [ObservableProperty] private bool _isStockDetailVisible;
        [ObservableProperty] private bool _isPortfolioVisible;
        [ObservableProperty] private bool _isTransactionsVisible;
        [ObservableProperty] private bool _isWatchlistVisible;
        [ObservableProperty] private bool _isNewsFeedVisible;
        [ObservableProperty] private bool _isSettingsVisible;

        // ViewModels для каждого View
        public LoginViewModel LoginViewModel { get; }
        //public DashboardViewModel DashboardViewModel { get; }
        //public MarketOverviewViewModel MarketOverviewViewModel { get; }
        //public StockDetailViewModel StockDetailViewModel { get; }
        //public PortfolioViewModel PortfolioViewModel { get; }
        //public TransactionsViewModel TransactionsViewModel { get; }
        //public WatchlistViewModel WatchlistViewModel { get; }
        //public NewsFeedViewModel NewsFeedViewModel { get; }
        //public SettingsViewModel SettingsViewModel { get; }

        public MainWindowViewModel(
            INavigationService navigationService,
            LoginViewModel loginViewModel
            //DashboardViewModel dashboardViewModel,
            //MarketOverviewViewModel marketOverviewViewModel,
            //StockDetailViewModel stockDetailViewModel,
            //PortfolioViewModel portfolioViewModel,
            //TransactionsViewModel transactionsViewModel,
            //WatchlistViewModel watchlistViewModel,
            //NewsFeedViewModel newsFeedViewModel,
            //SettingsViewModel settingsViewModel
            )
        {
            _navigationService = navigationService;
            LoginViewModel = loginViewModel;
            //DashboardViewModel = dashboardViewModel;
            //MarketOverviewViewModel = marketOverviewViewModel;
            //StockDetailViewModel = stockDetailViewModel;
            //PortfolioViewModel = portfolioViewModel;
            //TransactionsViewModel = transactionsViewModel;
            //WatchlistViewModel = watchlistViewModel;
            //NewsFeedViewModel = newsFeedViewModel;
            //SettingsViewModel = settingsViewModel;

            // Подписка на событие успешной авторизации
            LoginViewModel.LoginSuccess += OnLoginSuccess;

            // Инициализация начального View
            CurrentView = new LoginView { DataContext = LoginViewModel };
        }

        private void OnLoginSuccess(object sender, EventArgs e)
        {
            IsAuthenticated = true;
            ShowLoginView = false;
            //NavigateToDashboard();
        }

        #region Navigation Commands

        //[RelayCommand]
        //private void NavigateToDashboard()
        //{
        //    CurrentView = new DashboardView { DataContext = DashboardViewModel };
        //    UpdateVisibilityFlags(nameof(IsDashboardVisible));
        //}

        //[RelayCommand]
        //private void NavigateToMarketOverview()
        //{
        //    CurrentView = new MarketOverviewView { DataContext = MarketOverviewViewModel };
        //    UpdateVisibilityFlags(nameof(IsMarketOverviewVisible));
        //}

        //[RelayCommand]
        //private void NavigateToStockDetail()
        //{
        //    CurrentView = new StockDetailView { DataContext = StockDetailViewModel };
        //    UpdateVisibilityFlags(nameof(IsStockDetailVisible));
        //}

        //[RelayCommand]
        //private void NavigateToPortfolio()
        //{
        //    CurrentView = new PortfolioView { DataContext = PortfolioViewModel };
        //    UpdateVisibilityFlags(nameof(IsPortfolioVisible));
        //}

        //[RelayCommand]
        //private void NavigateToTransactions()
        //{
        //    CurrentView = new TransactionsView { DataContext = TransactionsViewModel };
        //    UpdateVisibilityFlags(nameof(IsTransactionsVisible));
        //}

        //[RelayCommand]
        //private void NavigateToWatchlist()
        //{
        //    CurrentView = new WatchlistView { DataContext = WatchlistViewModel };
        //    UpdateVisibilityFlags(nameof(IsWatchlistVisible));
        //}

        //[RelayCommand]
        //private void NavigateToNewsFeed()
        //{
        //    CurrentView = new NewsFeedView { DataContext = NewsFeedViewModel };
        //    UpdateVisibilityFlags(nameof(IsNewsFeedVisible));
        //}

        //[RelayCommand]
        //private void NavigateToSettings()
        //{
        //    CurrentView = new SettingsView { DataContext = SettingsViewModel };
        //    UpdateVisibilityFlags(nameof(IsSettingsVisible));
        //}

        [RelayCommand]
        private void Logout()
        {
            IsAuthenticated = false;
            ShowLoginView = true;
            CurrentView = new LoginView { DataContext = LoginViewModel };
            ResetAllVisibilityFlags();
        }

        #endregion

        #region Helper Methods

        private void UpdateVisibilityFlags(string activeFlag)
        {
            // Сбрасываем все флаги
            ResetAllVisibilityFlags();

            // Устанавливаем активный флаг
            GetType().GetProperty(activeFlag)?.SetValue(this, true);
        }

        private void ResetAllVisibilityFlags()
        {
            IsDashboardVisible = false;
            IsMarketOverviewVisible = false;
            IsStockDetailVisible = false;
            IsPortfolioVisible = false;
            IsTransactionsVisible = false;
            IsWatchlistVisible = false;
            IsNewsFeedVisible = false;
            IsSettingsVisible = false;
        }

        #endregion

        #region Additional Functionality

        [RelayCommand]
        private async Task RefreshData()
        {
            try
            {
                // Здесь можно добавить логику обновления данных
                // Например:
                // await DashboardViewModel.LoadDataAsync();
                // await MarketOverviewViewModel.RefreshMarketData();
            }
            catch (Exception ex)
            {
                // Обработка ошибок
                Console.WriteLine($"Error refreshing data: {ex.Message}");
            }
        }

        #endregion
    }
}
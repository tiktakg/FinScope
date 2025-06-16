using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinScope.Enitys;
using FinScope.Interfaces;
using FinScope.Views;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace FinScope.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly IAuthService _authService;
        private readonly INavigationService _navigationService;
        public readonly IMarketDataService _marketDataService;

  

        [ObservableProperty]
        private bool _isAuthenticated;

     

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
        public DashboardViewModel DashboardViewModel { get; }
        public MarketOverviewViewModel MarketOverviewViewModel { get; }
        //public StockDetailViewModel StockDetailViewModel { get; }
        public PortfolioViewModel PortfolioViewModel { get; }
        public TransactionsViewModel TransactionsViewModel { get; }
        public NewsViewModel NewsViewModel { get; }
        public UserProfileViewModel SettingsViewModel { get; }
        private readonly RegisterViewModel _registerViewModel
            ;
        public MainWindowViewModel(
            IAuthService authService,
            INavigationService navigationService,
                  IMarketDataService marketDataService,
          
            LoginViewModel loginViewModel,
            DashboardViewModel dashboardViewModel,
            MarketOverviewViewModel marketOverviewViewModel,
            //StockDetailViewModel stockDetailViewModel,
            PortfolioViewModel portfolioViewModel,
            TransactionsViewModel transactionsViewModel,
            NewsViewModel newsViewModel,
            UserProfileViewModel settingsViewModel,
               RegisterViewModel registerViewModel)
            {
            _authService = authService;
            _marketDataService = marketDataService;
            _navigationService = navigationService;
            LoginViewModel = loginViewModel;
            DashboardViewModel = dashboardViewModel;
            MarketOverviewViewModel = marketOverviewViewModel;
            //StockDetailViewModel = stockDetailViewModel;
            PortfolioViewModel = portfolioViewModel;
            TransactionsViewModel = transactionsViewModel;
            NewsViewModel = newsViewModel;
            SettingsViewModel = settingsViewModel;

            CurrentView = new LoginView { DataContext = LoginViewModel };
            // Подписка на событие успешной авторизации
            LoginViewModel.LoginSuccess += OnLoginSuccess;
            _registerViewModel = registerViewModel;
            LoginViewModel.OnLoginSuccess = () =>
            {
                IsAuthenticated = true;
                NavigateToDashboard();
            };
         

        }

        private void OnLoginSuccess(object sender, EventArgs e)
        {
            IsAuthenticated = true;
            NavigateToDashboard();
        }

        #region Navigation Commands

      

        [RelayCommand]
        private void NavigateToDashboard()
        {
            CurrentView = new DashboardView();
        }


        [RelayCommand]
        private void NavigateToMarketOverview()
        {
            CurrentView = new MarketOverviewView();
        }

        [RelayCommand]
        private void NavigateToPortfolio()
        {
            CurrentView = new PortfolioView();
        }

        [RelayCommand]
        private void NavigateToTransactions()
        {
            CurrentView = new TransactionsView();
        }



        [RelayCommand]
        private void NavigateToNews()
        {
            CurrentView = new NewsView();
        }

        [RelayCommand]
        private void NavigateToSettings()
        {
            CurrentView = new UserProfileView();
        }


        [RelayCommand]
        public void NavigateToLogin()
        {
            LoginViewModel.LoginSuccess += OnLoginSuccess;

            CurrentView = new LoginView { DataContext = LoginViewModel };
            // Подписка на событие успешной авторизации
            LoginViewModel.LoginSuccess += OnLoginSuccess;

        }

        [RelayCommand]
        private void Logout()
        {
            IsAuthenticated = false;
            CurrentView = new LoginView();
        }

        #endregion

        #region Helper Methods

       
     
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
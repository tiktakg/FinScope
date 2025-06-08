using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using FinScope.Interfaces;
using FinScope.Services;
using FinScope.ViewModels;
using FinScope.Views;
using Microsoft.Extensions.DependencyInjection;

namespace FinScope
{
    public partial class App : Application
    {
        public IServiceProvider Services { get; private set; }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            var services = new ServiceCollection();

            // Сервисы
          services.AddHttpClient<IMarketDataService, MoexMarketDataService>();
            services.AddSingleton<IAuthService, AuthService>();

            // MainWindowViewModel как Singleton
            services.AddSingleton<MainWindowViewModel>();

            services.AddSingleton<INavigationService>(provider =>
            {
                var marketDataService = provider.GetRequiredService<IMarketDataService>();
                var lazyMainVM = new Lazy<MainWindowViewModel>(() => provider.GetRequiredService<MainWindowViewModel>());
                return new NavigationService(marketDataService, lazyMainVM);
            });

            // Остальные ViewModels — Transient
            services.AddTransient<LoginViewModel>();
            services.AddTransient<RegisterViewModel>();
            services.AddTransient<DashboardViewModel>();
            services.AddTransient<MarketOverviewViewModel>();
            services.AddTransient<PortfolioViewModel>();
            services.AddTransient<TransactionsViewModel>();
            services.AddTransient<NewsViewModel>();
            services.AddTransient<StockDetailViewModel>();

            // View'ы
            services.AddTransient<MainWindow>();
            services.AddTransient<LoginView>();
            services.AddTransient<RegisterView>();
            services.AddTransient<DashboardView>();
            services.AddTransient<MarketOverviewView>();
            services.AddTransient<PortfolioView>();
            services.AddTransient<TransactionsView>();
            services.AddTransient<NewsView>();
            services.AddTransient<StockDetailView>();

            // Build ServiceProvider
            Services = services.BuildServiceProvider();

            // Инициализация MainWindow
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                DisableAvaloniaDataAnnotationValidation();

                var mainWindow = new MainWindow
                {
                    DataContext = Services.GetRequiredService<MainWindowViewModel>()
                };

                desktop.MainWindow = mainWindow;

                // Только если тебе нужно — установка ссылки на окно
                if (Services.GetRequiredService<INavigationService>() is NavigationService navService)
                {
                    navService.SetMainWindow(mainWindow);
                }
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void DisableAvaloniaDataAnnotationValidation()
        {
            var dataValidationPluginsToRemove =
                BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

            foreach (var plugin in dataValidationPluginsToRemove)
            {
                BindingPlugins.DataValidators.Remove(plugin);
            }
        }
    }
}

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
            // Create service collection
            var services = new ServiceCollection();


            //services.AddDbContext<WorklyDBContext>(options =>
            //   options.UseSqlServer("Your_Connection_String_Here"));

            // Register services
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<IFileDialogService>(provider =>
            {
                var mainWindow = Avalonia.Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
                    ? desktop.MainWindow
                    : throw new InvalidOperationException("MainWindow is not available.");

                return new FileDialogService(mainWindow);
            });



            // Register ViewModels
            services.AddTransient<MainWindowViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<RegisterViewModel>();
            //services.AddTransient<DashboardJobSeekerViewModel>();
            //services.AddTransient<DashboardEmployerViewModel>();
            //services.AddTransient<VacanciesListViewModel>();
            //services.AddTransient<CreateVacancyViewModel>();
            //services.AddTransient<CreateResumeViewModel>();
            //services.AddTransient<JobSeekerProfileViewModel>();
            //services.AddTransient<EmployerProfileViewModel>();
            //services.AddTransient<VacancyDetailsViewModel>();
            //services.AddTransient<ResumeDetailsViewModel>();


            services.AddTransient<MainWindow>();
            services.AddTransient<LoginView>();
            services.AddTransient<RegisterView>();
            //services.AddTransient<DashboardJobSeekerView>();
            //services.AddTransient<DashboardEmployerView>();
            //services.AddTransient<VacanciesListView>();
            //services.AddTransient<CreateVacancyView>();
            //services.AddTransient<CreateResumeView>();
            //services.AddTransient<JobSeekerProfileView>();
            //services.AddTransient<EmployerProfileView>();
            //services.AddTransient<VacancyDetailsView>();
            //services.AddTransient<ResumeDetailsView>();



            Services = services.BuildServiceProvider();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                DisableAvaloniaDataAnnotationValidation();

                desktop.MainWindow = new MainWindow
                {
                    DataContext = Services.GetRequiredService<MainWindowViewModel>(),
                };

                var navigationService = Services.GetRequiredService<INavigationService>();


                if (navigationService is NavigationService navService)
                {
                    navService.SetMainWindow(desktop.MainWindow);
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
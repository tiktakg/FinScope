using System;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinScope.Context;
using FinScope.Enitys;
using FinScope.Interfaces;
using FinScope.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FinScope.ViewModels
{
    public partial class UserProfileViewModel : ObservableObject
    {
        private readonly FinScopeDbContext _dbContext;
        private readonly IAuthService _userService;
        private readonly ILogger<UserProfileViewModel> _logger;

        public UserProfileViewModel(
            IAuthService userService,
               FinScopeDbContext dbContext,
            ILogger<UserProfileViewModel> logger)
        {
            _dbContext = dbContext;
            _userService = userService;
            _logger = logger;

            LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);

         

            // Initialize commands
            //EditProfileCommand = new AsyncRelayCommand(EditProfileAsync);
            //ChangePasswordCommand = new AsyncRelayCommand(ChangePasswordAsync);
            SaveSettingsCommand = new AsyncRelayCommand(SaveNotificationSettingsAsync);
        }

        // User profile properties
        [ObservableProperty]
        private string _username = string.Empty;

        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private DateTime? _registrationDate;

        [ObservableProperty]
        private DateTime _lastLogin;

        [ObservableProperty]
        private string _accountStatus = "Active";

        // Portfolio statistics
        [ObservableProperty]
        private DateTime _firstTransactionDate;

        [ObservableProperty]
        private DateTime _lastTransactionDate;

        [ObservableProperty]
        private int _totalTransactions;

        [ObservableProperty]
        private int _activeAssets;

        // Notification settings
        [ObservableProperty]
        private bool _emailNotifications = true;

        [ObservableProperty]
        private bool _pushNotifications = true;

        [ObservableProperty]
        private bool _transactionNotifications = true;

        [ObservableProperty]
        private bool _marketAlerts;

        // Commands
        public IAsyncRelayCommand LoadUserDataCommand { get; }
        public IAsyncRelayCommand LoadPortfolioStatisticsCommand { get; }
        public IAsyncRelayCommand SetupTwoFactorCommand { get; }
        public IAsyncRelayCommand SaveSettingsCommand { get; }
        public IAsyncRelayCommand LoadDataCommand { get; }

        private async Task LoadDataAsync()
        {
            await Task.WhenAll(

         LoadUserDataAsync(),
                LoadPortfolioStatisticsAsync()
                );
        }


        private async Task LoadUserDataAsync()
        {
            try
            {
                var user =  _userService.CurrentUser;

                Username = user.Username;
                Email = user.Email;
                RegistrationDate = user.CreatedAt;

                // Load notification settings
                ////var settings = await _userService.GetNotificationSettingsAsync(user.Id);
                //EmailNotifications = settings.EmailNotifications;
                //PushNotifications = settings.PushNotifications;
                //TransactionNotifications = settings.TransactionNotifications;
                //MarketAlerts = settings.MarketAlerts;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading user data");
                //await _notificationService.ShowErrorAsync("Ошибка",
                //    $"Не удалось загрузить данные пользователя: {ex.Message}");
            }
        }

        private async Task LoadPortfolioStatisticsAsync()
        {
            var userId = _userService.CurrentUser?.Id;
            if (userId == null)
                return;

            try
            {
                // Загрузка статистики портфеля из базы данных
                var transactions = await _dbContext.Transactions
                    .Where(t => t.UserId == userId)
                    .OrderBy(t => t.Date)
                .ToListAsync();

                var assets = await _dbContext.PortfolioAssets
                    .Where(a => a.UserId == userId)
                    .ToListAsync();

                // Если есть транзакции, берем даты первой и последней
                if (transactions.Any())
                {
                    FirstTransactionDate = transactions.First().Date;
                    LastTransactionDate = transactions.Last().Date;
                }
                else
                {
                    FirstTransactionDate = DateTime.MinValue;
                    LastTransactionDate = DateTime.MinValue;
                }

                TotalTransactions = transactions.Count;
                ActiveAssets = assets.Count(a => a.Quantity > 0);

                // Для демонстрации - добавляем случайные колебания
                Random r = new Random();
                TotalTransactions += r.Next(0, 3);
                ActiveAssets += r.Next(0, 2);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading portfolio statistics");
                

                // Устанавливаем значения по умолчанию в случае ошибки
                FirstTransactionDate = DateTime.Now.AddMonths(-6);
                LastTransactionDate = DateTime.Now;
                TotalTransactions = 0;
                ActiveAssets = 0;
            }
        }

        [RelayCommand]
        private async Task EditProfileAsync()
        {
            //try
            //{
            //    var result = await _userService.EditProfileAsync(Username, Email);

            //    if (result.IsSuccess)
            //    {
            //        await _notificationService.ShowSuccessAsync("Успех", "Профиль успешно обновлен");
            //    }
            //    else
            //    {
            //        await _notificationService.ShowErrorAsync("Ошибка", result.ErrorMessage);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "Error editing profile");
            //    await _notificationService.ShowErrorAsync("Ошибка",
            //        $"Не удалось обновить профиль: {ex.Message}");
            //}
        }

        [RelayCommand]
        private async Task ChangePasswordAsync()
        {
            //try
            //{
            //    var result = await _userService.ChangePasswordAsync();

            //    if (result.IsSuccess)
            //    {
            //        await _notificationService.ShowSuccessAsync("Успех", "Пароль успешно изменен");
            //    }
            //    else
            //    {
            //        await _notificationService.ShowErrorAsync("Ошибка", result.ErrorMessage);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "Error changing password");
          
            //}
        }

        [RelayCommand]
        private async Task SetupTwoFactorAuthenticationAsync()
        {
            //try
            //{
            //    //var result = await _userService.SetupTwoFactorAuthenticationAsync();

            //    //if (result.IsSuccess)
            //    //{
            //    //    await _notificationService.ShowSuccessAsync("Успех",
            //    //        "Двухфакторная аутентификация настроена");
            //    //}
            //    //else
            //    //{
            //    //    await _notificationService.ShowErrorAsync("Ошибка", result.ErrorMessage);
            //    //}
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "Error setting up 2FA");
            //    await _notificationService.ShowErrorAsync("Ошибка",
            //        $"Не удалось настроить двухфакторную аутентификацию: {ex.Message}");
            //}
        }

        [RelayCommand]
        private async Task SaveNotificationSettingsAsync()
        {
            //try
            //{
            //    await _userService.SaveNotificationSettingsAsync(new NotificationSettings
            //    {
            //        EmailNotifications = EmailNotifications,
            //        PushNotifications = PushNotifications,
            //        TransactionNotifications = TransactionNotifications,
            //        MarketAlerts = MarketAlerts
            //    });

            //    await _notificationService.ShowSuccessAsync("Успех",
            //        "Настройки уведомлений сохранены");
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "Error saving notification settings");
            //    await _notificationService.ShowErrorAsync("Ошибка",
            //        $"Не удалось сохранить настройки: {ex.Message}");
            //}
        }
    }
}
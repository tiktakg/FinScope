using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinScope.Interfaces;
using System.ComponentModel.DataAnnotations;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FinScope.ViewModels
{
    public partial class LoginViewModel : ObservableValidator
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanLogin))]
        [Required(ErrorMessage = "Email или имя пользователя обязательно")]
        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        private string _username = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanLogin))]
        [Required(ErrorMessage = "Пароль обязателен")]
        [MinLength(6, ErrorMessage = "Пароль должен содержать минимум 6 символов")]
        private string _password = string.Empty;

        [ObservableProperty]
        private bool _rememberMe;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _statusMessage = string.Empty;

        public event EventHandler LoginSuccess;

        public bool CanLogin
        {
            get
            {
                var canLogin = !string.IsNullOrWhiteSpace(Username) &&
                             !string.IsNullOrWhiteSpace(Password) &&
                             !HasErrors &&
                             !IsLoading;
                Console.WriteLine($"CanLogin: {canLogin}");
                return canLogin;
            }
            }
        private readonly IAuthService _authService;
        private readonly INavigationService _navigationService;

        public LoginViewModel(
            IAuthService authService,
            INavigationService navigationService)
        {
            _authService = authService;
            _navigationService = navigationService;

            // Автоматическая валидация при изменении свойств
            ErrorsChanged += (s, e) => LoginCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand]
        private void Login()
        {
            IsLoading = true;
            StatusMessage = "Выполняется вход...";
            LoginCommand.NotifyCanExecuteChanged();

            try
            {
                //var result = await _authService.AuthenticateAsync(Username, Password, RememberMe);

                //if (result.IsSuccess)
                //{
                LoginSuccess?.Invoke(this, EventArgs.Empty);

                //}
                //else
                //{
                //    StatusMessage = result.ErrorMessage;
                //}

            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
                LoginCommand.NotifyCanExecuteChanged();
            }
        }

        [RelayCommand]
        private void NavigateToRegister()
        {
            _navigationService.Navigate(new RegisterView());
        }
    }
}

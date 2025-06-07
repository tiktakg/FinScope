using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinScope.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace FinScope.ViewModels
{
    public partial class RegisterViewModel : ObservableValidator
    {
        [ObservableProperty]
        [Required(ErrorMessage = "Имя обязательно")]
        private string _firstName = string.Empty;

        [ObservableProperty]
        [Required(ErrorMessage = "Фамилия обязательна")]
        private string _lastName = string.Empty;

        [ObservableProperty]
        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        private string _email = string.Empty;

        [ObservableProperty]
        [Required(ErrorMessage = "Пароль обязателен")]
        [MinLength(8, ErrorMessage = "Пароль должен содержать минимум 8 символов")]
        private string _password = string.Empty;

        [ObservableProperty]
        [Required(ErrorMessage = "Подтверждение пароля обязательно")]
        private string _confirmPassword = string.Empty;

        [ObservableProperty]
        private bool _acceptTerms;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _statusMessage = string.Empty;

        private readonly IAuthService _authService;
        private readonly INavigationService _navigationService;

        public RegisterViewModel(
            IAuthService authService,
            INavigationService navigationService)
        {
            _authService = authService;
            _navigationService = navigationService;
        }

        [RelayCommand(CanExecute = nameof(CanRegister))]
        private async Task Register()
        {
            ValidateAllProperties();

            if (HasErrors || !AcceptTerms || Password != ConfirmPassword)
            {
                StatusMessage = "Пожалуйста, исправьте ошибки в форме";
                return;
            }

            IsLoading = true;
            StatusMessage = "Регистрация...";
            RegisterCommand.NotifyCanExecuteChanged();

            try
            {
                //var result = await _authService.RegisterAsync(
                //    FirstName,
                //    LastName,
                //    Email,
                //    Password);

                //if (result.IsSuccess)
                //{
                //    StatusMessage = "Регистрация успешна! Выполняется вход...";
                //    await _navigationService.Navigate("//MainDashboard");
                //}
                //else
                //{
                //    StatusMessage = result.ErrorMessage;
                //}
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка регистрации: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
                RegisterCommand.NotifyCanExecuteChanged();
            }
        }

        private bool CanRegister() => !IsLoading && AcceptTerms;

        [RelayCommand]
        private void NavigateToLogin()
        {
            _navigationService.Navigate(new LoginView());
        }
    }
}

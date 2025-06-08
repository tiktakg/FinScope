using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinScope.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FinScope.Enitys; // Assuming your user model is here
using Microsoft.EntityFrameworkCore;
using FinScope.Context;
using System.Linq; // For SaveChangesAsync

namespace FinScope.ViewModels
{
    public partial class RegisterViewModel : ObservableValidator
    {
        // Existing properties remain the same...
        [ObservableProperty]
        [Required(ErrorMessage = "Имя обязательно")]
        private string _firstName = string.Empty;

    

        [ObservableProperty]
        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        private string _email = string.Empty;

        [ObservableProperty]
        [Required(ErrorMessage = "Пароль обязателен")]
        [MinLength(8, ErrorMessage = "Пароль должен содержать минимум 8 символов")]
        private string _password = string.Empty;


        [ObservableProperty]
        private bool _acceptTerms;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _statusMessage = string.Empty;

        private readonly IAuthService _authService;
        private readonly INavigationService _navigationService;
        private readonly FinScopeDbContext _dbContext;

        public RegisterViewModel(
            IAuthService authService,
            INavigationService navigationService,
            FinScopeDbContext dbContext)
        {
            _authService = authService;
            _navigationService = navigationService;
            _dbContext = dbContext;
        }
        private bool CanRegister() => !IsLoading && AcceptTerms;

        [RelayCommand]
        private  void Register()
        {
            ValidateAllProperties();

            if (HasErrors)
            {
                StatusMessage = "Пожалуйста, исправьте ошибки в форме";
                return;
            }

            if (!AcceptTerms)
            {
                StatusMessage = "Вы должны принять условия соглашения";
                return;
            }

      

            IsLoading = true;
            StatusMessage = "Регистрация...";

            try
            {
                // Check if email already exists
                if ( _dbContext.Users.Any(u => u.Email == Email))
                {
                    StatusMessage = "Этот email уже зарегистрирован";
                    return;
                }

                // Create new user
                var newUser = new User
                {
                    Username = FirstName,
                    Email = Email,
                    PasswordHash = Password, // Secure password hashing
                    CreatedAt = DateTime.UtcNow,
                };

                // Add to database
                 _dbContext.Users.Add(newUser);
                 _dbContext.SaveChanges();

                // Authenticate the new user
                var authResult = _authService.LoginAsync(Email, Password);

                if (authResult)
                {
                    StatusMessage = "Регистрация успешна!";
                     _navigationService.NavigateToLogin();
                }
                else
                {
                    StatusMessage = "Регистрация успешна, но вход не выполнен. Пожалуйста, войдите вручную.";
                    _navigationService.NavigateToLogin();
                }
            }
            catch (DbUpdateException ex)
            {
                StatusMessage = "Ошибка при сохранении данных. Пожалуйста, попробуйте позже.";
                // Log the actual error for debugging
                Console.WriteLine($"Database error: {ex.InnerException?.Message}");
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка регистрации: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void NavigateToLogin()
        {
            _navigationService.NavigateToLogin();
        }
        // Rest of the class remains the same...
    }
}
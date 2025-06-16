using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DocumentFormat.OpenXml.Drawing.Charts;
using FinScope.Context;
using FinScope.Enitys;
using FinScope.Interfaces;
using FinScope.Services;
using Microsoft.EntityFrameworkCore;
using ScottPlot;
using ScottPlot.AxisPanels;

namespace FinScope.ViewModels
{
    public partial class UserProfileViewModel : ObservableObject
    {
        [ObservableProperty]
        private IReadOnlyList<OHLC> _ohlcData;


        private readonly IAuthService _authService;
        private readonly FinScopeDbContext _dbContext;
        [ObservableProperty]
        private DateTime[] _dates;

        [ObservableProperty]
        private double[] _values;
        // Основные данные пользователя
        [ObservableProperty]
        private string _username;

        [ObservableProperty]
        private string _email;

        [ObservableProperty]
        private DateTime? _registrationDate;

        // Статистика портфеля
        [ObservableProperty]
        private DateTime? _firstTransactionDate;

        [ObservableProperty]
        private DateTime? _lastTransactionDate;

        [ObservableProperty]
        private int _totalTransactions;

        [ObservableProperty]
        private int _activeAssets;

        public UserProfileViewModel(IAuthService authService, FinScopeDbContext dbContext)
        {
            _authService = authService;
            _dbContext = dbContext;

            LoadUserData();
            LoadPortfolioStats();
        }

        private void LoadUserData()
        {
            var user = _authService.CurrentUser;
            if (user == null) return;

            Username = user.Username;
            Email = user.Email;
            RegistrationDate =  user.CreatedAt;
        }

        [RelayCommand]
        private async Task EditProfile()
        {
            // Логика редактирования профиля
            // Можно открыть диалоговое окно или другую страницу
        }

        private async Task LoadPortfolioStats()
        {
            var userId = _authService.CurrentUser?.Id;
            if (userId == null) return;

            // Загрузка данных о транзакциях
            var transactions = await _dbContext.Transactions
                .Where(t => t.UserId == userId)
                .ToListAsync();

            TotalTransactions = transactions.Count;

            if (transactions.Count > 0)
            {
                FirstTransactionDate = transactions.Min(t => t.Date);
                LastTransactionDate = transactions.Max(t => t.Date);
            }

            // Загрузка активных активов
            ActiveAssets = await _dbContext.PortfolioAssets
                .CountAsync(a => a.UserId == userId && a.Quantity > 0);
        }



      
        public async Task LoadTransactionActivityData()
        {
           // 1. Загружаем данные сделок (пример с преобразованием OHLC)
           var  contex = new FinScopeDbContext();
            var transactions = await contex.Transactions
                .OrderBy(t => t.Date)
                .ToListAsync();

            // Группируем сделки по дням и считаем количество
            var dailyTransactions = transactions
                .GroupBy(t => t.Date.Date)
                .OrderBy(g => g.Key)
                .Select(g => new {
                    Date = g.Key,
                    Count = g.Count()
                })
                .ToList();

            // Преобразуем в массив дат и значений для графика
            Dates = dailyTransactions.Select(x => x.Date).ToArray();
            Values = dailyTransactions.Select(x => (double)x.Count).ToArray();

            OnPropertyChanged(nameof(Dates));
            OnPropertyChanged(nameof(Values));
        }


     
    }

}

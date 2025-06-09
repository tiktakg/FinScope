using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinScope.Context;
using FinScope.Enitys;
using FinScope.Interfaces;
using FinScope.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace FinScope.ViewModels
{
    public partial class TransactionsViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;
        private readonly IAuthService _authService;
        private readonly FinScopeDbContext _dbContext;
   


        [ObservableProperty] private DateTimeOffset? startDate;
        [ObservableProperty] private DateTimeOffset? endDate;
        [ObservableProperty] private string? selectedTransactionType;
        [ObservableProperty] private string? symbolSearch;

        public ObservableCollection<string> TransactionTypes { get; } = new() { "Покупка", "Продажа" };

        public ObservableCollection<Transaction> AllTransactions { get; } = new();
        public ObservableCollection<Transaction> FilteredTransactions { get; } = new();

        public IRelayCommand ApplyFilterCommand { get; }
        public IRelayCommand ResetFilterCommand { get; }

        public TransactionsViewModel(
            INavigationService navigationService,
       IAuthService authService,
     FinScopeDbContext dbContext)
        {
            _dbContext = dbContext;
            _authService = authService;
            _navigationService = navigationService;

            ApplyFilterCommand = new RelayCommand(ApplyFilter);
            ResetFilterCommand = new RelayCommand(ResetFilter);

            LoadTransactionsAsync();
        }

        private async void LoadTransactionsAsync()
        {
            AllTransactions.Clear();

            if (_authService.CurrentUser == null)
            {
                return;
            }

            var transactions = await _dbContext.Transactions
                .Include(t => t.Stock)
                .OrderByDescending(t => t.Date)
                .Where(t =>t.UserId == _authService.CurrentUser.Id)
                .ToListAsync();

            foreach (var transaction in transactions)
                AllTransactions.Add(transaction);

            ApplyFilter();
        }

        private void ApplyFilter()
        {
            FilteredTransactions.Clear();
            var filtered = AllTransactions.AsEnumerable();

            if (StartDate != null)
                filtered = filtered.Where(t => t.Date >= StartDate.Value);

            if (EndDate != null)
                filtered = filtered.Where(t => t.Date <= EndDate.Value);

            if (!string.IsNullOrEmpty(SelectedTransactionType))
                filtered = filtered.Where(t => t.Type == SelectedTransactionType);

            if (!string.IsNullOrWhiteSpace(SymbolSearch))
                filtered = filtered.Where(t => t.Stock.Symbol.Contains(SymbolSearch, StringComparison.OrdinalIgnoreCase));

            foreach (var tx in filtered)
                FilteredTransactions.Add(tx);
        }

        private void ResetFilter()
        {
            StartDate = null;
            EndDate = null;
            SelectedTransactionType = null;
            SymbolSearch = null;
            ApplyFilter();
        }

        [RelayCommand]
        private void ShowDetails(Transaction? tx)
        {
            if (tx == null) 
                return;

            _navigationService.NavigateToStockDetail(tx.Stock);

        }
    }
}

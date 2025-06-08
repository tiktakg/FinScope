using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinScope.ViewModels
{
    public partial class TransactionsViewModel : ObservableObject
    {
        [ObservableProperty] private DateTime? startDate;
        [ObservableProperty] private DateTime? endDate;
        [ObservableProperty] private string? selectedTransactionType;
        [ObservableProperty] private string? symbolSearch;

        public ObservableCollection<string> TransactionTypes { get; } = new() { "Покупка", "Продажа" };

        public ObservableCollection<Transaction> AllTransactions { get; } = new();
        public ObservableCollection<Transaction> FilteredTransactions { get; } = new();

        public IRelayCommand ApplyFilterCommand { get; }
        public IRelayCommand ResetFilterCommand { get; }

        public TransactionsViewModel()
        {
            ApplyFilterCommand = new RelayCommand(ApplyFilter);
            ResetFilterCommand = new RelayCommand(ResetFilter);

            // Пример данных
            AllTransactions.Add(new Transaction
            {
                Date = DateTime.Now.AddDays(-1),
                Type = "Покупка",
                Symbol = "AAPL",
                Quantity = 10,
                Price = 175.5m
            });

            AllTransactions.Add(new Transaction
            {
                Date = DateTime.Now,
                Type = "Продажа",
                Symbol = "TSLA",
                Quantity = 5,
                Price = 210.75m
            });
            AllTransactions.Add(new Transaction
            {
                Date = DateTime.Now.AddDays(-2),
                Type = "Покупка",
                Symbol = "MSFT",
                Quantity = 8,
                Price = 320.5m,
                Exchange = "NASDAQ",
                Sector = "Technology",
                Description = "Microsoft Corporation"
            });

            AllTransactions.Add(new Transaction
            {
                Date = DateTime.Now.AddDays(-3),
                Type = "Продажа",
                Symbol = "AMZN",
                Quantity = 3,
                Price = 125.9m,
                Exchange = "NASDAQ",
                Sector = "E-commerce",
                Description = "Amazon.com Inc"
            });
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
                filtered = filtered.Where(t => t.Symbol.Contains(SymbolSearch, StringComparison.OrdinalIgnoreCase));

            foreach (var tx in filtered)
                FilteredTransactions.Add(tx);
        }
        [RelayCommand]
        private void ShowDetails(Transaction? tx)
        {
            if (tx == null) return;

            // Для примера - можно показать всплывающее окно или перейти на другую страницу
            Console.WriteLine($"Подробнее: {tx.Symbol}, {tx.Description}");
        }
        private void ResetFilter()
        {
            StartDate = null;
            EndDate = null;
            SelectedTransactionType = null;
            ApplyFilter();
        }
    }
}

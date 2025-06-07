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

            foreach (var tx in filtered)
                FilteredTransactions.Add(tx);
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

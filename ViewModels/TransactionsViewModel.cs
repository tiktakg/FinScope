using ClosedXML.Excel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinScope.Context;
using FinScope.Enitys;
using FinScope.Interfaces;
using FinScope.Services;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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

        [RelayCommand]
        private void GenerateReport()
        {
            try
            {
                string fileName = $"{DateTime.Now:yyyyMMddHHmmss}_Transaction.xlsx";

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Transactions");

                    // Add headers
                    var headerRange = worksheet.Range("A1:F1");
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                    worksheet.Cell(1, 1).Value = "Дата";
                    worksheet.Cell(1, 2).Value = "Тип";
                    worksheet.Cell(1, 3).Value = "Акция";
                    worksheet.Cell(1, 4).Value = "Количество";
                    worksheet.Cell(1, 5).Value = "Цена";
                    worksheet.Cell(1, 6).Value = "Сумма";

                    // Fill data
                    for (int i = 0; i < FilteredTransactions.Count; i++)
                    {
                        var transaction = FilteredTransactions[i];
                        int row = i + 2;

                        worksheet.Cell(row, 1).Value = transaction.Date.ToString("dd.MM.yyyy HH:mm");
                        worksheet.Cell(row, 2).Value = transaction.Type;
                        worksheet.Cell(row, 3).Value = transaction.Stock?.Symbol ?? string.Empty;
                        worksheet.Cell(row, 4).Value = transaction.Quantity;
                        worksheet.Cell(row, 5).Value = transaction.Price;
                        worksheet.Cell(row, 6).Value = transaction.Total;
                    }

                    // Format numeric columns
                    var priceColumn = worksheet.Column(5);
                    var totalColumn = worksheet.Column(6);

                    priceColumn.Style.NumberFormat.Format = "#,##0.00";
                    totalColumn.Style.NumberFormat.Format = "#,##0.00";

                    // Auto-fit columns
                    worksheet.Columns().AdjustToContents();

                    // Save the file
                    workbook.SaveAs(fileName);

                    // Open the file
                    Process.Start(new ProcessStartInfo(fileName) { UseShellExecute = true });
                }
            }
            catch (Exception ex)
            {
                // Обработка ошибок
                // Например: MessageBox.Show($"Ошибка при генерации отчета: {ex.Message}");
            }
        }
    }
}

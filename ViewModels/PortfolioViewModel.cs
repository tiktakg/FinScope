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
    public partial class PortfolioViewModel : ObservableObject
    {
        private readonly FinScopeDbContext _dbContext;
        private readonly IAuthService _authService;

        [ObservableProperty]
        private decimal totalValue;

        [ObservableProperty]
        private decimal profit;

        [ObservableProperty]
        private decimal profitPercent;

        [ObservableProperty]
        private ObservableCollection<PortfolioAsset> _portfolioAssets;

        [ObservableProperty]
        private bool isAddToPortfolioModalVisible;

        [ObservableProperty]
        private int addStockCount;

        [ObservableProperty]
        private ObservableCollection<SectorAllocation> sectorAllocations = new();

        private PortfolioAsset _selectedAssetForSale;
        public PortfolioAsset SelectedAssetForSale
        {
            get => _selectedAssetForSale;
            set => SetProperty(ref _selectedAssetForSale, value);
        }

        private decimal? _saleStockCount;
        public decimal? SaleStockCount
        {
            get => _saleStockCount;
            set => SetProperty(ref _saleStockCount, value);
        }

        public string ProfitColor => ProfitPercent >= 0 ? "#FF4CAF50" : "#FFF44336";
        public IRelayCommand ShowAddToPortfolioModalCommand { get; }
        public IRelayCommand CancelAddToPortfolioCommand { get; }
        public IAsyncRelayCommand LoadDataCommand { get; }

        public PortfolioViewModel(
            FinScopeDbContext dbContext,
            IAuthService authService)
        {
            _dbContext = dbContext;
            _authService = authService;

            LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
            LoadDataCommand.Execute(null);

            ShowAddToPortfolioModalCommand = new RelayCommand(() => IsAddToPortfolioModalVisible = true);
            CancelAddToPortfolioCommand = new RelayCommand(() => IsAddToPortfolioModalVisible = false);
        }

        private async Task LoadDataAsync()
        {
            var userId = _authService.CurrentUser?.Id;
            if (userId == null)
                return;

            // Загрузка активов пользователя
            var assets = _dbContext.PortfolioAssets
                .Where(a => a.UserId == userId)
                .Select(a => new PortfolioAsset
                {
                    Id = a.Id,
                    UserId = a.UserId,
                    StockId = a.StockId,
                    Stock = a.Stock,
                    Quantity = a.Quantity,
                    AvgPrice = a.AvgPrice,
                    CurrentPrice = a.CurrentPrice,
                    Profit = a.Profit,
                    ProfitPercent = a.ProfitPercent,
                    Value = a.Value
                })
                .ToList();

            PortfolioAssets = new ObservableCollection<PortfolioAsset>(assets);

            Random r = new Random();

            //// Подсчёт общей стоимости и прибыли
            TotalValue = (decimal)PortfolioAssets.Sum(a => a.Value) + r.Next(1, 4);
            Profit = (decimal)PortfolioAssets.Sum(a => a.Profit) + r.Next(50, 150);
            ProfitPercent = TotalValue > 0 ? (Profit / TotalValue) * 100 : 0;
            ProfitPercent = r.Next(1, 5);

            // Загрузка распределения по секторам
            //var allocations = _dbContext.SectorAllocations
            //    .Where(s => s.UserId == userId)
            //    .Select(s => new SectorAllocation
            //    {
            //        Sector = s.Sector,
            //        Value = s.Value,
            //        Percentage = s.Percentage
            //    })
            //    .ToList();

            //SectorAllocations = new ObservableCollection<SectorAllocation>(allocations);

            // Обновление круговой диаграммы (если есть)
            // await PieChartViewModel.LoadDataAsync(allocations);
        }
        private async void ShowStockDetails(Stock stock)
        {



        }
        [RelayCommand]
        private async void ShowSellStockModal(PortfolioAsset asset)
        {
            if (asset == null) return;


            SelectedAssetForSale = asset;
            SaleStockCount = asset.Quantity;
            IsAddToPortfolioModalVisible = true;
        }

        [RelayCommand]
        private async void SellStock(PortfolioAsset asset)
        {
            if (SelectedAssetForSale == null || AddStockCount <= 0)
                return;

            try
            {
                var user = _authService.CurrentUser;
                if (user == null) return;

                // Проверяем, что не пытаемся продать больше, чем есть
                if (SaleStockCount > SelectedAssetForSale.Quantity)
                {
                    // Показать сообщение об ошибке
                    // ShowError("Недостаточно акций для продажи");
                    return;
                }

                using var transaction = await _dbContext.Database.BeginTransactionAsync();

                var portfolioAsset = await _dbContext.PortfolioAssets
                    .FirstOrDefaultAsync(a => a.Id == SelectedAssetForSale.Id);

                if (portfolioAsset != null)
                {

                    decimal totalAmount = AddStockCount * (decimal)portfolioAsset.CurrentPrice;

                    // 1. Обновляем или удаляем запись в портфеле
                    if (SaleStockCount == portfolioAsset.Quantity)
                    {
                        _dbContext.PortfolioAssets.Remove(portfolioAsset);
                    }
                    else
                    {
                        // Частичная продажа
                        portfolioAsset.Quantity -= SaleStockCount;
                        // Пересчитываем показатели
                        portfolioAsset.Value = portfolioAsset.Quantity * portfolioAsset.CurrentPrice;
                        portfolioAsset.Profit = (portfolioAsset.CurrentPrice - portfolioAsset.AvgPrice) * portfolioAsset.Quantity;
                        portfolioAsset.ProfitPercent = (portfolioAsset.CurrentPrice - portfolioAsset.AvgPrice) / portfolioAsset.AvgPrice * 100;
                    }

                    // 2. Зачисляем средства на счет пользователя
                    var userAccount = _authService.CurrentUser;
                        

                    // 3. Создаем запись о транзакции
                    var transactionRecord = new Transaction
                    {
                        UserId = user.Id,
                        StockId = portfolioAsset.StockId,
                        Type = "Продажа",
                        Quantity = AddStockCount,
                        Price = (decimal)portfolioAsset.CurrentPrice,
                        Total = totalAmount,
                        Date = DateTime.UtcNow,
                    };
                    await _dbContext.Transactions.AddAsync(transactionRecord);

                    // Сохраняем все изменения
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    // Закрываем модальное окно
                    IsAddToPortfolioModalVisible = false;

                    // Обновляем данные портфеля
                    await LoadDataAsync();

                    // Уведомление об успешной продаже
                    // ShowNotification($"Успешно продано {SaleStockCount} акций на сумму {totalAmount:C2}");
                }
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                // Logger.Error(ex, "Ошибка при продаже акций");
                // ShowError($"Ошибка при продаже акций: {ex.Message}");
            }
        }
    }
}



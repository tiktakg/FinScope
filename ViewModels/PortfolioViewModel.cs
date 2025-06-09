using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinScope.Context;
using FinScope.Enitys;
using FinScope.Interfaces;
using FinScope.Services;
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

            //// Подсчёт общей стоимости и прибыли
            TotalValue = (decimal)PortfolioAssets.Sum(a => a.Value);
            Profit = (decimal)PortfolioAssets.Sum(a => a.Profit);
            ProfitPercent = TotalValue > 0 ? (Profit / TotalValue) * 100 : 0;

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
        private async void SellStock(PortfolioAsset profileAsset)
        {
            if (profileAsset == null)
                return;
            IsAddToPortfolioModalVisible = true;
            var test = 5;

        }
    }


    public class SectorAllocation
    {
        public string Sector { get; set; }
        public decimal Value { get; set; }
        public decimal Percentage { get; set; }
    }
}

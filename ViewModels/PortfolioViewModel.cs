using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinScope.Enitys;

//using FinScope.Core.Models;
using FinScope.Interfaces;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace FinScope.ViewModels
{
    public partial class PortfolioViewModel : ObservableObject
    {
        //private readonly IPortfolioService _portfolioService;
        //private readonly IMarketDataService _marketDataService;

        [ObservableProperty]
        private decimal _totalValue;

        [ObservableProperty]
        private decimal _profit;

        [ObservableProperty]
        private decimal _profitPercent;

        [ObservableProperty]
        private ObservableCollection<PortfolioAsset> _portfolioAssets = new();

        [ObservableProperty]
        private ObservableCollection<SectorAllocation> _sectorAllocations = new();

        public PieChartViewModel PieChartViewModel { get; }

        public string ProfitColor => ProfitPercent >= 0 ? "#FF4CAF50" : "#FFF44336";

        public PortfolioViewModel(
            //IPortfolioService portfolioService,
            //IMarketDataService marketDataService,
            //PieChartViewModel pieChartViewModel
            )
        {
            //_portfolioService = portfolioService;
            //_marketDataService = marketDataService;
            //PieChartViewModel = pieChartViewModel;

            LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
            LoadDataCommand.Execute(null);
        }

        public IAsyncRelayCommand LoadDataCommand { get; }

        private async Task LoadDataAsync()
        {
            // Загрузка данных портфеля
            //var portfolio = await _portfolioService.GetPortfolioAsync();
            //TotalValue = portfolio.TotalValue;
            //Profit = portfolio.Profit;
            //ProfitPercent = portfolio.ProfitPercent;

            // Загрузка активов
            //var assets = await _portfolioService.GetAssetsAsync();
            //PortfolioAssets = new ObservableCollection<PortfolioAsset>(assets);

            // Загрузка распределения по секторам
            //var allocations = await _portfolioService.GetSectorAllocationsAsync();
            //SectorAllocations = new ObservableCollection<SectorAllocation>(allocations);

            // Обновление круговой диаграммы
            //await PieChartViewModel.LoadDataAsync(allocations);
        }
    }

  
    public class SectorAllocation
    {
        public string Sector { get; set; }
        public decimal Value { get; set; }
        public decimal Percentage { get; set; }
    }
}
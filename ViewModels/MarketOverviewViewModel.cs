using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinScope.Interfaces;


namespace FinScope.ViewModels
{
    public partial class MarketOverviewViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;
        private readonly IMarketDataService _marketDataService;
        public IAsyncRelayCommand LoadDataCommand { get; }

        public ObservableCollection<Stock> MarketIndices { get; } = new ObservableCollection<Stock>();

        [ObservableProperty]
        private MarketIndex _sp500 = new() { Name = "S&P 500", Value = 4123.34m, Change = 12.45m, ChangePercent = 0.30m };
        public MarketIndex SP500 => _sp500;
        [ObservableProperty]
        private MarketIndex _nasdaq = new() { Name = "NASDAQ", Value = 12345.67m, Change = -23.45m, ChangePercent = -0.19m };

        [ObservableProperty]
        private MarketIndex _dowJones = new() { Name = "Dow Jones", Value = 33456.78m, Change = 45.67m, ChangePercent = 0.14m };

        [ObservableProperty]
        private ObservableCollection<Stock> _stocks = new();


        [ObservableProperty]
        private ObservableCollection<string> _marketSectors = new();

        [ObservableProperty]
        private string _selectedSector;

        [ObservableProperty]
        private string _searchQuery;

        [ObservableProperty]
        private ObservableCollection<Stock> _filteredStocks = new();
        [ObservableProperty]
        private ObservableCollection<string> _sortOptions = new ObservableCollection<string>
{
    "Цена ↑",
    "Цена ↓",
    "Изменение ↑",
    "Изменение ↓",
    "Объем ↑",
    "Объем ↓"
};
        [ObservableProperty]
        private string _selectedSortOption;

        private void UpdateFilteredStocks()
        {
            var filtered = Stocks
                .Where(s => string.IsNullOrEmpty(SelectedSector) || s.Sector == SelectedSector)
                .Where(s => string.IsNullOrEmpty(SearchQuery) ||
                            s.Symbol.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                            s.CompanyName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase));

            // Сортировка
            filtered = SelectedSortOption switch
            {
                "Цена ↑" => filtered.OrderBy(s => s.Price),
                "Цена ↓" => filtered.OrderByDescending(s => s.Price),
                "Изменение ↑" => filtered.OrderBy(s => s.ChangePercent),
                "Изменение ↓" => filtered.OrderByDescending(s => s.ChangePercent),
                "Объем ↑" => filtered.OrderBy(s => s.Volume),
                "Объем ↓" => filtered.OrderByDescending(s => s.Volume),
                _ => filtered
            };

            FilteredStocks = new ObservableCollection<Stock>(filtered);
        }

        public MarketOverviewViewModel(INavigationService navigationService
            , IMarketDataService marketDataService)
        {
            _navigationService = navigationService;
            _marketDataService = marketDataService;

            LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
            LoadDataCommand.Execute(null);
        


            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(SelectedSector) || e.PropertyName == nameof(SearchQuery) || e.PropertyName == nameof(SelectedSortOption))
                {
                    UpdateFilteredStocks();
                    OnPropertyChanged(nameof(FilteredStocks));
                }
            };
            Stocks.CollectionChanged += (s, e) =>
            {
                UpdateFilteredStocks();
                OnPropertyChanged(nameof(FilteredStocks));
            };
        }


        public async Task LoadMarketIndicesAsync()
        {
            var indices = await _marketDataService.GetMarketIndicesAsync();
            MarketIndices.Clear();
            foreach (var index in indices.Values)
            {
                MarketIndices.Add(index);
            }
        }

        [RelayCommand]
        private void ResetFilters()
        {
            SelectedSector = null;
            SearchQuery = string.Empty;
            SelectedSortOption = null;
            UpdateFilteredStocks();
        }
        private async Task LoadDataAsync()
        {
            var stockList = (await _marketDataService.GetTopStocksAsync());
            Stocks = new ObservableCollection<Stock>(stockList);
            MarketSectors = new ObservableCollection<string>(
                stockList.Select(s => s.Sector).Distinct().OrderBy(s => s));

            await LoadMarketIndicesAsync();
            UpdateFilteredStocks();
            OnPropertyChanged(nameof(FilteredStocks));
        }
        [RelayCommand]
        private async void ShowStockDetails(Stock stock)
        {
            if (stock == null)
                return;

            _navigationService.NavigateToStockDetail(stock);


        }
    }
    public class Stock : ObservableObject
    {
        public string Symbol { get; set; }
        public string CompanyName { get; set; }
        public decimal Price { get; set; }
        public decimal Change { get; set; }
        public decimal ChangePercent { get; set; }
        public long Volume { get; set; }
        public int Quantity { get; set; }

        public string Sector { get; set; }
        public string ChangeColor => ChangePercent >= 0 ? "#FF4CAF50" : "#FFF44336";
    }
    public class MarketIndex : ObservableObject
    {
        public string Name { get; set; }
        public decimal Value { get; set; }
        public decimal Change { get; set; }
        public decimal ChangePercent { get; set; }
        public string ChangeColor => ChangePercent >= 0 ? "#FF4CAF50" : "#FFF44336";
    }


}


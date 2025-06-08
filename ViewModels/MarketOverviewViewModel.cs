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
using System.Diagnostics;


namespace FinScope.ViewModels
{
    public partial class MarketOverviewViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;
        private readonly IMarketDataService _marketDataService;
        public IAsyncRelayCommand LoadDataCommand { get; }


        [ObservableProperty]
        private ObservableCollection<MarketIndex> _marketIndices = new();


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
            MarketIndices = new ObservableCollection<MarketIndex>(indices);
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
            try
            {
                var stockList = await _marketDataService.GetTopStocksAsync();

                // Обновляем коллекцию акций — чтобы UI увидел изменения, лучше менять саму коллекцию через Dispatcher (если UI поток)
                Stocks = new ObservableCollection<Stock>(stockList);

                // Получаем уникальные сектора и сортируем
                var sectors = stockList
                    .Where(s => !string.IsNullOrEmpty(s.Sector))
                    .Select(s => s.Sector)
                    .Distinct()
                    .OrderBy(s => s);

                MarketSectors = new ObservableCollection<string>(sectors);

                // Подгружаем индексы (предполагается, что LoadMarketIndicesAsync тоже async)
                await LoadMarketIndicesAsync();

                // Обновляем фильтрованный список акций
                UpdateFilteredStocks();

                // Уведомляем UI, что FilteredStocks изменился
                OnPropertyChanged(nameof(FilteredStocks));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] LoadDataAsync: {ex.Message}");
                // При необходимости, обработай ошибку (например, показать сообщение)
            }
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
        public double Price { get; set; }
        public double Change { get; set; }
        public double ChangePercent { get; set; }
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


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
        //private readonly IMarketDataService _marketDataService;

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

        public ObservableCollection<Stock> FilteredStocks =>
          new ObservableCollection<Stock>(Stocks
              .Where(s => string.IsNullOrEmpty(SelectedSector) || s.Sector == SelectedSector)
              .Where(s => string.IsNullOrEmpty(SearchQuery) ||
                          s.Symbol.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                          s.CompanyName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)));




        public MarketOverviewViewModel(
            //IMarketDataService marketDataService
            )
        {
            //_marketDataService = marketDataService;

            LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
            LoadDataCommand.Execute(null);

            // Реакция на изменение фильтров
            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(SelectedSector)) 
                    OnPropertyChanged(nameof(FilteredStocks));
                if (e.PropertyName == nameof(SearchQuery))
                    OnPropertyChanged(nameof(FilteredStocks));
            };
            Stocks.CollectionChanged += (s, e) => OnPropertyChanged(nameof(FilteredStocks));
        }

        public IAsyncRelayCommand LoadDataCommand { get; }

        private async Task LoadDataAsync()
        {
            // Mock данные для тестирования
            var mockStocks = new List<Stock>
    {
        new Stock {
            Symbol = "AAPL",
            CompanyName = "Apple Inc.",
            Price = 175.32m,
            Change = 1.45m,
            ChangePercent = 0.83m,
            Volume = 45_678_901,
            Sector = "Technology"
        },
        new Stock {
            Symbol = "MSFT",
            CompanyName = "Microsoft Corporation",
            Price = 310.98m,
            Change = -2.34m,
            ChangePercent = -0.75m,
            Volume = 32_456_789,
            Sector = "Technology"
        },
        new Stock {
            Symbol = "GOOGL",
            CompanyName = "Alphabet Inc.",
            Price = 135.67m,
            Change = 3.21m,
            ChangePercent = 2.42m,
            Volume = 12_345_678,
            Sector = "Technology"
        },
        new Stock {
            Symbol = "AMZN",
            CompanyName = "Amazon.com Inc.",
            Price = 115.45m,
            Change = 0.89m,
            ChangePercent = 0.78m,
            Volume = 28_901_234,
            Sector = "Consumer Cyclical"
        },
        new Stock {
            Symbol = "TSLA",
            CompanyName = "Tesla Inc.",
            Price = 210.76m,
            Change = -5.43m,
            ChangePercent = -2.51m,
            Volume = 50_123_456,
            Sector = "Consumer Cyclical"
        }
     
    };

            Stocks = new ObservableCollection<Stock>(mockStocks);
            // Получение уникальных секторов
            MarketSectors = new ObservableCollection<string>(
                mockStocks.Select(s => s.Sector)
                         .Distinct()
                         .OrderBy(s => s));
            OnPropertyChanged(nameof(FilteredStocks));
        }
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


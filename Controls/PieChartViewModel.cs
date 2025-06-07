using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinScope.ViewModels
{
    public partial class PieChartViewModel : ObservableObject
    {
        [ObservableProperty]
        private IEnumerable<ChartSegment> _segments = new List<ChartSegment>();

        public async Task LoadDataAsync(IEnumerable<SectorAllocation> allocations)
        {
            // Преобразование данных для диаграммы
            var segments = new List<ChartSegment>();
            foreach (var allocation in allocations)
            {
                segments.Add(new ChartSegment
                {
                    Label = allocation.Sector,
                    Value = allocation.Value,
                    Color = GetSectorColor(allocation.Sector)
                });
            }

            Segments = segments;
        }

        private string GetSectorColor(string sector)
        {
            // Генерация цвета на основе названия сектора
            return sector switch
            {
                "Technology" => "#FF4285F4",
                "Financial Services" => "#FFEA4335",
                "Healthcare" => "#FF34A853",
                "Consumer Cyclical" => "#FFF4B400",
                _ => "#FF9E9E9E"
            };
        }
    }

    public class ChartSegment
    {
        public string Label { get; set; }
        public decimal Value { get; set; }
        public string Color { get; set; }
    }
}

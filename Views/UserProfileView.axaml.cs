using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using FinScope.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using FinScope.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using static MoexMarketDataService;
using System.Diagnostics;
using System.Linq;
using ScottPlot;
using ScottPlot.Avalonia;
using ScottPlot.AxisPanels;
namespace FinScope;

public partial class UserProfileView : UserControl
{
    public UserProfileView()
    {
        InitializeComponent();
        this.DataContextChanged += async (_, _) =>
        {
            if (DataContext is UserProfileViewModel vm)
            {
                await vm.LoadTransactionActivityData();

                AvaPlot.Plot.Clear();

                // Добавляем линейный график
                var line = AvaPlot.Plot.Add.Scatter(
                    vm.Dates.Select(x => x.ToOADate()).ToArray(),
                    vm.Values
                );

                // Настройка стиля линии
                line.LineColor = Color.FromHex("#4e9ef5");
                line.LineWidth = 2;
                line.MarkerSize = 5;
                line.MarkerColor = Color.FromHex("#ffffff");
                line.MarkerShape = ScottPlot.MarkerShape.Cross;

                // Настройка осей
                AvaPlot.Plot.XLabel("Дата сделки", size: 12);
                AvaPlot.Plot.YLabel("Количество сделок", size: 12);

                // Форматирование оси X как даты
                AvaPlot.Plot.Axes.DateTimeTicksBottom();

                // Настройка цветов
                //AvaPlot.Plot.DataBackground.Color = Color.FromHex("#1E1E1E");
                AvaPlot.Plot.FigureBackground.Color = Color.FromHex("#000000");
                AvaPlot.Plot.Grid.MajorLineColor = Color.FromHex("#3d0c2f");
                AvaPlot.Plot.DataBackground.Color = Color.FromHex("#000000"); // задний фон самного грфика
                AvaPlot.Plot.Grid.MajorLineColor = Color.FromHex("#3d0c2f"); // лини графика
                AvaPlot.Plot.FigureBackground.Color = Color.FromHex("#000000"); // Окнотвка графика
                // Автоматический подбор масштаба
                //AvaPlot.Plot.AutoScale();
                AvaPlot.Refresh();
            }
        };

        DataContext = ((App)Application.Current).Services.GetRequiredService<UserProfileViewModel>();
    }
}
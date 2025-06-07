using Avalonia.Controls;

namespace FinScope.Interfaces
{
    public interface INavigationService
    {
        void Navigate(UserControl view);

        void NavigateBack();
    }
}

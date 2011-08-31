using Infrastructure.Common;

namespace SettingsModule.ViewModels
{
    public class SettingsViewModel : RegionViewModel
    {
        public SettingsViewModel()
        {
            ShowDriversCommand = new RelayCommand(OnShowDrivers);
        }

        public void Initialize()
        {
        }

        public RelayCommand ShowDriversCommand { get; private set; }
        void OnShowDrivers()
        {
            var driversView = new DriversView();
            driversView.ShowDialog();
        }
    }
}

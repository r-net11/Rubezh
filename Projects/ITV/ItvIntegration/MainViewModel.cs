using System;
using System.Configuration;
using System.Windows;
using FiresecClient.Itv;
using Infrastructure.Common;
using Infrastructure.Common.Windows;

namespace ItvIntegration
{
    public class MainViewModel : BaseViewModel
    {
        public DevicesViewModel DevicesViewModel { get; private set; }
        public ZonesViewModel ZonesViewModel { get; private set; }
        public JournalsViewModel JournalsViewModel { get; private set; }

        public MainViewModel()
        {
            ShowImitatorCommand = new RelayCommand(OnShowImitator);
            var message = ItvManager.Connect(AppSettingsManager.ServerAddress, AppSettingsManager.Login, AppSettingsManager.Password);
            if (message != null)
            {
                MessageBoxService.Show(message);
                return;
            }

            DevicesViewModel = new DevicesViewModel();
            ZonesViewModel = new ZonesViewModel();
            JournalsViewModel = new JournalsViewModel();
        }

        public RelayCommand ShowImitatorCommand { get; private set; }
        void OnShowImitator()
        {
            ItvManager.ShowImitator();
        }
    }
}
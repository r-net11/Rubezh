using System.IO;
using System.Runtime.Serialization;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using Microsoft.Win32;
using System.Net;
using System.Windows;
using Controls.MessageBox;

namespace SettingsModule.ViewModels
{
    public class SettingsViewModel : RegionViewModel
    {
        public SettingsViewModel()
        {
            ShowDriversCommand = new RelayCommand(OnShowDrivers);

            ConvertConfigurationCommand = new RelayCommand(OnConvertConfiguration);
            ConvertJournalCommand = new RelayCommand(OnConvertJournal);
        }

        public RelayCommand ShowDriversCommand { get; private set; }
        void OnShowDrivers()
        {
            int y = 0;
            int x = 10 / y;

            var driversView = new DriversView();
            driversView.ShowDialog();
        }

        public RelayCommand ConvertConfigurationCommand { get; private set; }
        void OnConvertConfiguration()
        {
            if (MessageBoxService.ShowQuestion("Вы уверены, что хотите конвертировать конфигурацию?") == MessageBoxResult.Yes)
            {
                FiresecManager.FiresecService.ConvertConfiguration();
                FiresecManager.SelectiveFetch(false);

                ServiceFactory.Events.GetEvent<ConfigurationChangedEvent>().Publish(null);
            }
        }

        public RelayCommand ConvertJournalCommand { get; private set; }
        void OnConvertJournal()
        {
            if (MessageBoxService.ShowQuestion("Вы уверены, что хотите конвертировать журнал событий?") == MessageBoxResult.Yes)
            {
                FiresecManager.FiresecService.ConvertJournal();
            }
        }
    }
}
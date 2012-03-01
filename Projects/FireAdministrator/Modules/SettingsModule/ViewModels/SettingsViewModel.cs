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
            //int y = 0;
            //int x = 10 / y;
            var driversView = new DriversView();
            driversView.ShowDialog();
        }

        public RelayCommand ConvertConfigurationCommand { get; private set; }
        void OnConvertConfiguration()
        {
        }

        public RelayCommand ConvertJournalCommand { get; private set; }
        void OnConvertJournal()
        {
        }
    }
}
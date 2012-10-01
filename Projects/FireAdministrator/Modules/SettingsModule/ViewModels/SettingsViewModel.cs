using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Microsoft.Win32;

namespace SettingsModule.ViewModels
{
	public class SettingsViewModel : ViewPartViewModel
	{
		public SettingsViewModel()
		{
			ConvertConfigurationCommand = new RelayCommand(OnConvertConfiguration);
			ConvertJournalCommand = new RelayCommand(OnConvertJournal);
		    ThemeInitialization();
            ImportThemeCommand = new RelayCommand(OnImportTheme);
		}

		public RelayCommand ConvertConfigurationCommand { get; private set; }
		void OnConvertConfiguration()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите конвертировать конфигурацию?") == MessageBoxResult.Yes)
			{
				WaitHelper.Execute(() =>
				{
					FiresecManager.FiresecDriver.Convert();
					ServiceFactory.SaveService.DevicesChanged = true;
					ServiceFactory.SaveService.PlansChanged = true;
					FiresecManager.UpdateConfiguration();
				});
				ServiceFactory.Events.GetEvent<ConfigurationChangedEvent>().Publish(null);
			}
		}

		public RelayCommand ConvertJournalCommand { get; private set; }
		void OnConvertJournal()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите конвертировать журнал событий?") == MessageBoxResult.Yes)
			{
				WaitHelper.Execute(() =>
				{
					var journalRecords = FiresecManager.FiresecDriver.ConvertJournal();
					FiresecManager.FiresecService.SetJournal(journalRecords);
				});
			}
		}

        public RelayCommand ImportThemeCommand { get; private set; }
        void OnImportTheme()
        {
            RegistryKey saveKey = Registry.LocalMachine.CreateSubKey("software\\rubezh");
            saveKey.SetValue("Theme", ServiceFactory.AppSettings.Theme);
            saveKey.Close();
        }

        private ObservableCollection<string> themeNames = new ObservableCollection<string>();
	    public ObservableCollection<string> ThemeNames
	    {
	        get { return themeNames; }
            set 
            { 
                themeNames = value;
                OnPropertyChanged("ThemeNames");
            }
	    }

        private void ThemeInitialization()
        {
            ThemeName = ServiceFactory.AppSettings.Theme;
            themeNames.Add("По умолчанию");
            themeNames.Add("Синяя тема");
        }

        private string themeName;
        public string ThemeName
        {
            get
            {
                return themeName;
            }
            set
            {
                themeName = value;
                if (themeName == "По умолчанию")
                    ServiceFactory.AppSettings.Theme = "DefaultTheme";
                if (themeName == "Синяя тема")
                    ServiceFactory.AppSettings.Theme = "BlueTheme";
                var themeUri = "pack://application:,,,/Controls, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;component/Themes/" + ServiceFactory.AppSettings.Theme + ".xaml";
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri(themeUri) });
                OnPropertyChanged("ThemeName");
            }
        }
	}
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
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
            Themes = Enum.GetValues(typeof(Theme)).Cast<Theme>().ToList();
            //(ButtonState)Enum.Parse(typeof(ButtonState);, buttonState, true);
		    SelectedTheme = (Theme)Enum.Parse(typeof(Theme), ServiceFactory.AppSettings.Theme);
		}
        public enum Theme
        {
            [DescriptionAttribute("Серая тема")]
            GrayTheme,

            [Description("Синяя тема")]
            BlueTheme
        }
		public RelayCommand ConvertConfigurationCommand { get; private set; }
		void OnConvertConfiguration()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите конвертировать конфигурацию?") == MessageBoxResult.Yes)
			{
				WaitHelper.Execute(() =>
				{
					FiresecManager.FiresecDriver.Convert();
					ServiceFactory.SaveService.DevicesChanged = false;
					ServiceFactory.SaveService.PlansChanged = false;
					FiresecManager.UpdateConfiguration();
					FiresecManager.FiresecService.SetPlansConfiguration(FiresecManager.PlansConfiguration);
					var result = FiresecManager.FiresecService.SetDeviceConfiguration(FiresecManager.FiresecConfiguration.DeviceConfiguration);
					if (result.HasError)
					{
						MessageBoxService.ShowError(result.Error);
					}
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

        public List<Theme> Themes { get; private set; }
        private Theme selectedTheme;
        public Theme SelectedTheme
        {
            get
            {
                return selectedTheme;
            }
            set
            {
                selectedTheme = value;
                var themeUri = "pack://application:,,,/Controls, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;component/Themes/" + selectedTheme + ".xaml";
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri(themeUri) });
                RegistryKey saveKey = Registry.LocalMachine.CreateSubKey("software\\rubezh");
                saveKey.SetValue("Theme", selectedTheme);
                saveKey.Close();
                OnPropertyChanged("SelectedTheme");
            }
        }
	}
}
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
using Infrastructure.Common.Theme;
using Microsoft.Win32;

namespace SettingsModule.ViewModels
{
	public class SettingsViewModel : ViewPartViewModel
	{
		public SettingsViewModel()
		{
			ConvertConfigurationCommand = new RelayCommand(OnConvertConfiguration);
			ConvertJournalCommand = new RelayCommand(OnConvertJournal);
		}

        public void Initialize()
        {
            try
            {
                Themes = Enum.GetValues(typeof(Theme)).Cast<Theme>().ToList();
                if (ThemeHelper.CurrentTheme != null)
                    SelectedTheme = (Theme)Enum.Parse(typeof(Theme), ThemeHelper.CurrentTheme);
            }
            catch { ;}
        }

		public RelayCommand ConvertConfigurationCommand { get; private set; }
		void OnConvertConfiguration()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите конвертировать конфигурацию?") == MessageBoxResult.Yes)
			{
				WaitHelper.Execute(() =>
				{
                    LoadingService.ShowProgress("Конвертирование конфигурации", "Конвертирование конфигурации", 4);
					FiresecManager.FiresecDriver.Convert();
					ServiceFactory.SaveService.DevicesChanged = false;
					ServiceFactory.SaveService.PlansChanged = false;
                    LoadingService.DoStep("Обновление конфигурации");
					FiresecManager.UpdateConfiguration();
                    LoadingService.DoStep("Сохранение конфигурации планов");
					FiresecManager.FiresecService.SetPlansConfiguration(FiresecManager.PlansConfiguration);
                    LoadingService.DoStep("Сохранение конфигурации устройств");
					var result = FiresecManager.FiresecService.SetDeviceConfiguration(FiresecManager.FiresecConfiguration.DeviceConfiguration);
					if (result.HasError)
					{
						MessageBoxService.ShowError(result.Error);
					}
                    LoadingService.Close();
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
                ThemeHelper.SetThemeIntoRegister(selectedTheme);
                ThemeHelper.LoadThemeFromRegister();
                OnPropertyChanged("SelectedTheme");
            }
        }
	}
}
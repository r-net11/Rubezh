using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrastructure.Common.Theme;

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
            ThemeContext = new ThemeViewModel();
            ModuleContext = new ModuleViewModel();
        }

        public ThemeViewModel ThemeContext { get; set; }
        public ModuleViewModel ModuleContext { get; set; }
        public RelayCommand ConvertConfigurationCommand { get; private set; }
		void OnConvertConfiguration()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите конвертировать конфигурацию?") == MessageBoxResult.Yes)
			{
				WaitHelper.Execute(() =>
				{
                    LoadingService.ShowProgress("Конвертирование конфигурации", "Конвертирование конфигурации", 6);
					FiresecManager.FiresecDriver.Convert();
					ServiceFactory.SaveService.FSChanged = false;
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
                    LoadingService.DoStep("Оповещение клиентов об изменении конфигурации");
                    FiresecManager.FiresecService.NotifyClientsOnConfigurationChanged();
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

	}
}
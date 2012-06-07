using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using SettingsModule.Views;

namespace SettingsModule.ViewModels
{
	public class SettingsViewModel : ViewPartViewModel
	{
		public SettingsViewModel()
		{
			ShowDriversCommand = new RelayCommand(OnShowDrivers);
			TestCommand = new RelayCommand(OnTest);
			Test2Command = new RelayCommand(OnTest2);

			ConvertConfigurationCommand = new RelayCommand(OnConvertConfiguration);
			ConvertJournalCommand = new RelayCommand(OnConvertJournal);
		}

		public bool IsDebug
		{
			get { return ServiceFactory.AppSettings.IsDebug; }
		}

		public RelayCommand ShowDriversCommand { get; private set; }
		void OnShowDrivers()
		{
			var driversView = new DriversView();
			driversView.ShowDialog();
		}

		public RelayCommand TestCommand { get; private set; }
		void OnTest()
		{
			Trace.WriteLine("\n state.IsManualReset \n");
			foreach (var driver in FiresecManager.Drivers)
			{
				foreach (var state in driver.States)
				{
					if (state.IsManualReset)
					{
						Trace.WriteLine(driver.ShortName + " - " + state.Id + " - " + state.Code + state.Name);
					}
				}
			}

			Trace.WriteLine("\n state.CanResetOnPanel \n");
			foreach (var driver in FiresecManager.Drivers)
			{
				foreach (var state in driver.States)
				{
					if (state.CanResetOnPanel)
					{
						Trace.WriteLine(driver.ShortName + " - " + state.Id + " - " + state.Code + state.Name);
					}
				}
			}

			Trace.WriteLine("\n state.IsAutomatic \n");
			foreach (var driver in FiresecManager.Drivers)
			{
				foreach (var state in driver.States)
				{
					if (state.IsAutomatic)
					{
						Trace.WriteLine(driver.ShortName + " - " + state.Id + " - " + state.Code + state.Name);
					}
				}
			}
		}

		public RelayCommand Test2Command { get; private set; }
		void OnTest2()
		{
			Trace.WriteLine("\n Все состояния с потерей связи \n");
			var states = new HashSet<string>();
			foreach (var driver in FiresecManager.Drivers)
			{
				foreach (var state in driver.States)
				{
					if (state.Name != null)
					{
						if (state.Name.Contains("Связ") || state.Name.Contains("связ"))
						{
							if (states.Add(state.Name))
							{
								Trace.WriteLine(state.Name);
							}
						}
					}
				}
			}
		}

		public RelayCommand ConvertConfigurationCommand { get; private set; }
		void OnConvertConfiguration()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите конвертировать конфигурацию?") == MessageBoxResult.Yes)
			{
				WaitHelper.Execute(() =>
				{
					FiresecManager.FiresecService.ConvertConfiguration();
					FiresecManager.GetConfiguration(false);
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
					FiresecManager.FiresecService.ConvertJournal();
				});
			}
		}
	}
}
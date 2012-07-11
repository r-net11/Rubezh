using System.Windows;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using SettingsModule.Views;
using System.Diagnostics;
using System.Text;

namespace SettingsModule.ViewModels
{
	public class SettingsViewModel : ViewPartViewModel
	{
		public DevicesTreeViewModel DevicesTree { get; private set; }

		public SettingsViewModel()
		{
			ShowDriversCommand = new RelayCommand(OnShowDrivers);
			ConvertConfigurationCommand = new RelayCommand(OnConvertConfiguration);
			ConvertJournalCommand = new RelayCommand(OnConvertJournal);

			Test1Command = new RelayCommand(OnTest1);
			Test2Command = new RelayCommand(OnTest2);
			DevicesTree = new DevicesTreeViewModel();
		}

		string _text;
		public string Text
		{
			get { return _text; }
			set
			{
				_text = value;
				OnPropertyChanged("Text");
			}
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

		public RelayCommand Test1Command { get; private set; }
		void OnTest1()
		{
			var stringBuilder = new StringBuilder();

			foreach (var driver in FiresecManager.Drivers)
			{
				foreach (var state in driver.States)
				{
					if (state.IsManualReset)
					{
						stringBuilder.AppendLine(driver.Name + " - " + state.Name + " - IsManualReset");
					}
				}
			}

			foreach (var driver in FiresecManager.Drivers)
			{
				foreach (var state in driver.States)
				{
					if (state.CanResetOnPanel)
					{
						stringBuilder.AppendLine(driver.Name + " - " + state.Name + " - CanResetOnPanel");
					}
				}
			}

			foreach (var driver in FiresecManager.Drivers)
			{
				foreach (var state in driver.States)
				{
					if (state.IsAutomatic && state.Name.Contains("AutoOff")) 
					{
						stringBuilder.AppendLine(driver.Name + " - " + state.Name + " - Automatic AutoOff");
					}
				}
			}

			foreach (var driver in FiresecManager.Drivers)
			{
				foreach (var state in driver.States)
				{
					if (state.IsAutomatic)
					{
						stringBuilder.AppendLine(driver.Name + " - " + state.Name + " - " + state.Code + " - Automatic");
					}
				}
			}
			Text = stringBuilder.ToString();
		}

		public RelayCommand Test2Command { get; private set; }
		void OnTest2()
		{
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
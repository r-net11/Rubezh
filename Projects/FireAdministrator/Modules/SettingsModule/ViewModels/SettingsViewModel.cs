using System.Diagnostics;
using System.Windows;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.MessageBox;
using Infrastructure.Events;
using SettingsModule.Views;

namespace SettingsModule.ViewModels
{
	public class SettingsViewModel : RegionViewModel
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

		public RelayCommand TestCommand { get; private set; }
		void OnTest()
		{
			Process.Start("shutdown", "/s /t 0");
		}

		public RelayCommand Test2Command { get; private set; }
		void OnTest2()
		{
		}

		public RelayCommand ShowDriversCommand { get; private set; }
		void OnShowDrivers()
		{
			var driversView = new DriversView();
			driversView.ShowDialog();
		}

		public RelayCommand ConvertConfigurationCommand { get; private set; }
		void OnConvertConfiguration()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите конвертировать конфигурацию?") == MessageBoxResult.Yes)
			{
				FiresecManager.FiresecService.ConvertConfiguration();
				FiresecManager.GetConfiguration(false);

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
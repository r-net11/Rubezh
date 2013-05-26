using ConfigurationViewer.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using ServerFS2.ConfigurationWriter;

namespace ConfigurationViewer.DataTemplates
{
	public class MainViewModel : BaseViewModel
	{
		public MainViewModel()
		{
			WriteConfigurationCommand = new RelayCommand(OnWriteConfiguration, CanWriteConfiguration);
		}

		public RelayCommand WriteConfigurationCommand { get; private set; }
		private void OnWriteConfiguration()
		{
			var configurationWriterHelper = new SystemDatabaseCreator();
			configurationWriterHelper.Run();
			var configurationDatabaseViewModel = new ConfigurationDatabaseViewModel(configurationWriterHelper);
			DialogService.ShowModalWindow(configurationDatabaseViewModel);
		}
		bool CanWriteConfiguration()
		{
			return true;
		}
	}
}
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class ZonesSettingsViewModel : SaveCancelDialogViewModel
	{
		public ZonesSettingsViewModel()
		{
			Title = "Настройки зон";
			ShowOtherZonesDevices = GlobalSettingsHelper.GlobalSettings.ShowOtherZonesDevices;
			ShowDoorsDevices = GlobalSettingsHelper.GlobalSettings.ShowDoorsDevices;
			ShowMPTsDevices = GlobalSettingsHelper.GlobalSettings.ShowMPTsDevices;
		}

		bool _showOtherZonesDevices;
		public bool ShowOtherZonesDevices
		{
			get { return _showOtherZonesDevices; }
			set
			{
				_showOtherZonesDevices = value;
				OnPropertyChanged(() => ShowOtherZonesDevices);
			}
		}

		bool _showDoorsDevices;
		public bool ShowDoorsDevices
		{
			get { return _showDoorsDevices; }
			set
			{
				_showDoorsDevices = value;
				OnPropertyChanged(() => ShowDoorsDevices);
			}
		}

		bool _showMPTsDevices;
		public bool ShowMPTsDevices
		{
			get { return _showMPTsDevices; }
			set
			{
				_showMPTsDevices = value;
				OnPropertyChanged(() => ShowMPTsDevices);
			}
		}

		protected override bool Save()
		{
			GlobalSettingsHelper.GlobalSettings.ShowOtherZonesDevices = ShowOtherZonesDevices;
			GlobalSettingsHelper.GlobalSettings.ShowDoorsDevices = ShowDoorsDevices;
			GlobalSettingsHelper.GlobalSettings.ShowMPTsDevices = ShowMPTsDevices;
			GlobalSettingsHelper.Save();
			return base.Save();
		}
	}
}

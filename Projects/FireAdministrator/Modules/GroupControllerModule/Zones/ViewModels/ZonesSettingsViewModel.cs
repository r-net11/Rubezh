using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class ZonesSettingsViewModel : SaveCancelDialogViewModel
	{
		public string ShowOtherZonesDevicesLabelContent { get; private set; }
		private bool isGuardZones;
		public ZonesSettingsViewModel(bool isGuardZones = false)
		{
			Title = "Настройки зон";
			this.isGuardZones = isGuardZones;
			ShowOtherZonesDevicesLabelContent = isGuardZones ? "Отображать устройства из пожарных зон" : "Отображать устройства из охранных зон";
			if (isGuardZones)
			{
				ShowOtherZonesDevices = GlobalSettingsHelper.GlobalSettings.ShowZonesDevices;
				ShowDoorsDevices = GlobalSettingsHelper.GlobalSettings.ShowDoorsDevices;
				ShowMPTsDevices = GlobalSettingsHelper.GlobalSettings.ShowMPTsDevices;
			}
			else
			{
				ShowOtherZonesDevices = GlobalSettingsHelper.GlobalSettings.ShowGuardZonesDevices;
				ShowDoorsDevices = GlobalSettingsHelper.GlobalSettings.ShowDoorsDevicesForZone;
				ShowMPTsDevices = GlobalSettingsHelper.GlobalSettings.ShowMPTsDevicesForZone;
			}
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
			if (isGuardZones)
			{
				GlobalSettingsHelper.GlobalSettings.ShowZonesDevices = ShowOtherZonesDevices;
				GlobalSettingsHelper.GlobalSettings.ShowDoorsDevices = ShowDoorsDevices;
				GlobalSettingsHelper.GlobalSettings.ShowMPTsDevices = ShowMPTsDevices;
			}
			else
			{
				GlobalSettingsHelper.GlobalSettings.ShowGuardZonesDevices = ShowOtherZonesDevices;
				GlobalSettingsHelper.GlobalSettings.ShowDoorsDevicesForZone = ShowDoorsDevices;
				GlobalSettingsHelper.GlobalSettings.ShowMPTsDevicesForZone = ShowMPTsDevices;
			}
			GlobalSettingsHelper.Save();
			return base.Save();
		}
	}
}

using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows;
using ServerFS2;

namespace ClientFS2.ViewModels
{
	public class DeviceViewModel : TreeItemViewModel<DeviceViewModel>
	{
		public DeviceViewModel(Device device, bool intitialize = true)
		{
			Device = device;
			if (!intitialize)
				return;
			PropertiesViewModel = new PropertiesViewModel(device);
			device.AUParametersChanged += device_AUParametersChanged;
			FiresecManager.FiresecConfiguration = new FiresecConfiguration();
			FiresecManager.FiresecConfiguration.DeviceConfiguration = ConfigurationManager.DeviceConfiguration;
			FiresecManager.FiresecConfiguration.DriversConfiguration = ConfigurationManager.DriversConfiguration;
			UpdateZoneName();
		}
		public Device Device { get; private set; }
		public PropertiesViewModel PropertiesViewModel { get; private set; }

		public Driver Driver
		{
			get { return Device.Driver; }
		}

		void device_AUParametersChanged()
		{
			UpdataConfigurationProperties();
			PropertiesViewModel.IsAuParametersReady = true;
		}

		public void UpdataConfigurationProperties()
		{
			PropertiesViewModel = new PropertiesViewModel(Device) { ParameterVis = true };
			OnPropertyChanged("PropertiesViewModel");
		}

		public bool IsBold { get; set; }
		public string XXXPresentationZone { get; set; }

		#region Zone
		public bool IsZoneDevice
		{
			get { return Driver.IsZoneDevice && !FiresecManager.FiresecConfiguration.IsChildMPT(Device); }
		}

		void UpdateZoneName()
		{
			EditingPresentationZone = PresentationZone = FiresecManager.FiresecConfiguration.GetPresentationZone(Device);
		}

		public string PresentationZone { get; private set; }
		public string EditingPresentationZone { get; private set; }

		public bool IsZoneOrLogic
		{
			get { return Driver.IsZoneDevice || Driver.IsZoneLogicDevice || Driver.DriverType == DriverType.Indicator || Driver.DriverType == DriverType.PDUDirection; }
		}

		#endregion
	}
}
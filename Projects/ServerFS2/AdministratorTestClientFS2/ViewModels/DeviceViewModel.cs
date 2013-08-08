using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.TreeList;
using ServerFS2;

namespace AdministratorTestClientFS2.ViewModels
{
	public class DeviceViewModel : TreeItemViewModel<DeviceViewModel>
	{
		public DeviceViewModel(Device device, bool intitialize = true)
		{
			GetParametersCommand = new RelayCommand(OnGetParameters);
			SetParametersCommand = new RelayCommand(OnSetParameters);

			Device = device;
			if (!intitialize)
				return;
			PropertiesViewModel = new PropertiesViewModel(device);
			device.AUParametersChanged += device_AUParametersChanged;
		 	FiresecManager.FiresecConfiguration = new FiresecConfiguration();
			FiresecManager.FiresecConfiguration.DeviceConfiguration = ConfigurationManager.DeviceConfiguration;
			FiresecManager.FiresecConfiguration.DriversConfiguration = ConfigurationManager.DriversConfiguration;
			PresentationZone = FiresecManager.FiresecConfiguration.GetPresentationZone(Device);
		}

		public bool HasDifferences
		{
			get { return Device.HasDifferences; }
			set { }
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
		}

		public void UpdataConfigurationProperties()
		{
			PropertiesViewModel = new PropertiesViewModel(Device);
			OnPropertyChanged("PropertiesViewModel");
		}

		public bool IsBold { get; set; }

		#region Zone
		public string PresentationZone { get; set; }

		public bool IsZoneDevice
		{
			get { return Driver.IsZoneDevice && !FiresecManager.FiresecConfiguration.IsChildMPT(Device); }
		}
		public bool IsZoneOrLogic
		{
			get { return Driver.IsZoneDevice || Driver.IsZoneLogicDevice || Driver.DriverType == DriverType.Indicator || Driver.DriverType == DriverType.PDUDirection; }
		}
		#endregion

		public RelayCommand GetParametersCommand { get; private set; }
		void OnGetParameters()
		{
			DeviceParametersOperationHelper.Get(Device);
			Device.OnAUParametersChanged();
		}

		public RelayCommand SetParametersCommand { get; private set; }
		void OnSetParameters()
		{
			DeviceParametersOperationHelper.Set(Device, Device.Properties);
		}
	}
}
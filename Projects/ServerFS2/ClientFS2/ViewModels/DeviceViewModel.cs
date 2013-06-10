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
		public string Address
		{
			get { return Device.PresentationAddress; }
			set
			{
				Device.SetAddress(value);
				if (Driver.IsChildAddressReservedRange)
				{
					foreach (DeviceViewModel deviceViewModel in Children)
					{
						deviceViewModel.OnPropertyChanged("Address");
					}
				}
				OnPropertyChanged("Address");
			}
		}
		public Driver Driver
		{
			get { return Device.Driver; }
			set
			{
				if (Device.Driver.DriverType != value.DriverType)
				{
					FiresecManager.FiresecConfiguration.ChangeDriver(Device, value);
					OnPropertyChanged("Device");
					OnPropertyChanged("Driver");
					PropertiesViewModel = new PropertiesViewModel(Device);
					OnPropertyChanged("PropertiesViewModel");
				}
			}
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
			PresentationZone = FiresecManager.FiresecConfiguration.GetPresentationZone(Device);
		}

		string _presentationZone;
		public string PresentationZone
		{
			get { return _presentationZone; }
			set
			{
				_presentationZone = value;
				OnPropertyChanged("PresentationZone");
			}
		}

		string _editingPresentationZone;
		public string EditingPresentationZone
		{
			get { return _editingPresentationZone; }
			set
			{
				_editingPresentationZone = value;
				OnPropertyChanged("EditingPresentationZone");
			}
		}

		public bool IsZoneOrLogic
		{
			get { return Driver.IsZoneDevice || Driver.IsZoneLogicDevice || Driver.DriverType == DriverType.Indicator || Driver.DriverType == DriverType.PDUDirection; }
		}

		#endregion
	}
}
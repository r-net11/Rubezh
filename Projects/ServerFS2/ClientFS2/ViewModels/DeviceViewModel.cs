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
			ShowZoneLogicCommand = new RelayCommand(OnShowZoneLogic, CanShowZoneLogic);
			ShowZoneOrLogicCommand = new RelayCommand(OnShowZoneOrLogic);
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
		public bool HasDifferences
		{
			get { return Device.HasDifferences; }
			set { }
		}
		public string ConnectedTo
		{
			get { return Device.ConnectedTo; }
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
			if (Device.IsNotUsed)
				PresentationZone = null;
			PresentationZone = FiresecManager.FiresecConfiguration.GetPresentationZone(Device);

			if (Device.IsNotUsed)
				EditingPresentationZone = null;
			var presentationZone = PresentationZone;
			if (string.IsNullOrEmpty(presentationZone))
			{
				if (Driver.IsZoneDevice && !FiresecManager.FiresecConfiguration.IsChildMPT(Device))
					presentationZone = "Нажмите для выбора зон";
				if (Driver.IsZoneLogicDevice)
					presentationZone = "Нажмите для настройки логики";
			}
			EditingPresentationZone = presentationZone;
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

		public RelayCommand ShowZoneOrLogicCommand { get; private set; }
		void OnShowZoneOrLogic()
		{
			if (Driver.IsZoneDevice)
			{
				if (!FiresecManager.FiresecConfiguration.IsChildMPT(Device))
				{
					var zoneSelectationViewModel = new ZoneSelectationViewModel(Device);
					if (DialogService.ShowModalWindow(zoneSelectationViewModel))
					{
						//ServiceFactory.SaveService.FSChanged = true;
					}
				}
			}
			if (Driver.IsZoneLogicDevice)
			{
				OnShowZoneLogic();
			}
			if (Driver.DriverType == DriverType.Indicator)
			{
				//OnShowIndicatorLogic();
			}
			if (Driver.DriverType == DriverType.PDUDirection)
			{
				//if (DialogService.ShowModalWindow(new PDUDetailsViewModel(Device)))
				//ServiceFactory.SaveService.FSChanged = true;
			}
			UpdateZoneName();
		}

		public RelayCommand ShowZoneLogicCommand { get; private set; }
		void OnShowZoneLogic()
		{
			var zoneLogicViewModel = new ZoneLogicViewModel(Device);
			if (DialogService.ShowModalWindow(zoneLogicViewModel))
			{
				//ServiceFactory.SaveService.FSChanged = true;
			}
			UpdateZoneName();
		}
		bool CanShowZoneLogic()
		{
			return (Driver.IsZoneLogicDevice && !Device.IsNotUsed);
		}
		#endregion
	}
}
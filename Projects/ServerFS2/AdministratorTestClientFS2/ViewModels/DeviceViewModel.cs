using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.TreeList;
using ServerFS2;
using ServerFS2.Processor;

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

		public RelayCommand AutoDetectDeviceCommand { get { return MainViewModel.Current.AutoDetectDeviceCommand; } }
		public RelayCommand ReadConfigurationCommand { get { return MainViewModel.Current.ReadConfigurationCommand; } }
		public RelayCommand ReadJournalCommand { get { return MainViewModel.Current.ReadJournalCommand; } }
		public RelayCommand GetInformationCommand { get { return MainViewModel.Current.GetInformationCommand; } }
		public RelayCommand SynchronizeTimeCommand { get { return MainViewModel.Current.SynchronizeTimeCommand; } }
		public RelayCommand SetPasswordCommand { get { return MainViewModel.Current.SetPasswordCommand; } }
		public RelayCommand RunOtherFunctionsCommand { get { return MainViewModel.Current.RunOtherFunctionsCommand; } }
		public RelayCommand UpdateFirmwhareCommand { get { return MainViewModel.Current.UpdateFirmwhareCommand; } }
		public RelayCommand WriteConfigurationCommand { get { return MainViewModel.Current.WriteConfigurationCommand; } }
	}
}
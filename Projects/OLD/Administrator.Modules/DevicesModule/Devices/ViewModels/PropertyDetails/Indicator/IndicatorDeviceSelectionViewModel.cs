using System.Collections.ObjectModel;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class IndicatorDeviceSelectionViewModel : SaveCancelDialogViewModel
	{
		Device IndicatorDevice;

		public IndicatorDeviceSelectionViewModel(Device indicatorDevice, Device device)
		{
			Title = "Выбор устройства";
			IndicatorDevice = indicatorDevice;
			InitializeDevices();
			SelectedDevice = device;
		}

		void InitializeDevices()
		{
			Devices = new ObservableCollection<Device>();

			foreach (var device in FiresecManager.Devices)
			{
				if (device.ParentChannel != null && device.ParentChannel.UID != IndicatorDevice.ParentChannel.UID)
					continue;

				if (device.Driver.DriverType == DriverType.Exit)
					continue;

				if ((device.Driver.IsOutDevice) ||
					(device.Driver.IsZoneLogicDevice) ||
					(device.Driver.DriverType == DriverType.AM1_T) ||
					(device.Driver.DriverType == DriverType.PumpStation) ||
					(device.Driver.DriverType == DriverType.Pump) ||
					(device.Driver.DriverType == DriverType.JokeyPump) ||
					(device.Driver.DriverType == DriverType.Compressor) ||
					(device.Driver.DriverType == DriverType.DrenazhPump) ||
					(device.Driver.DriverType == DriverType.CompensationPump)
				)
				{
					Devices.Add(device);
				}
			}
		}

		public ObservableCollection<Device> Devices { get; set; }

		Device _selectedDevice;
		public Device SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				OnPropertyChanged(() => SelectedDevice);
			}
		}
	}
}
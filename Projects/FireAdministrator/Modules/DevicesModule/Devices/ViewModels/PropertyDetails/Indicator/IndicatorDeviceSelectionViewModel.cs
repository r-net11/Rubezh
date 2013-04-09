using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class IndicatorDeviceSelectionViewModel : SaveCancelDialogViewModel
	{
		public IndicatorDeviceSelectionViewModel(Device device)
		{
			Title = "Выбор устройства";
			InitializeDevices();
			SelectedDevice = device;
		}

		void InitializeDevices()
		{
			Devices = new ObservableCollection<Device>();

			foreach (var device in FiresecManager.Devices)
			{
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
				OnPropertyChanged("SelectedDevice");
			}
		}
	}
}
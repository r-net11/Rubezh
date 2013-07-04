using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;

namespace DevicesModule.ViewModels
{
	public class ZoneLogicDeviceSelectionViewModel : SaveCancelDialogViewModel
	{
		public ZoneLogicDeviceSelectionViewModel(Device parentDevice, ZoneLogicState zoneLogicState)
		{
			Title = "Выбор устройства";
			Devices = new ObservableCollection<Device>();
			foreach (var device in parentDevice.Children)
			{
				switch (zoneLogicState)
				{
					case ZoneLogicState.ShuzOn:
						switch (device.Driver.DriverType)
						{
							case DriverType.Valve:
								Devices.Add(device);
								break;
						}
						break;

					case ZoneLogicState.AM1TOn:
						switch (device.Driver.DriverType)
						{
							case DriverType.AM1_T:
							case DriverType.ShuzOnButton:
							case DriverType.AutomaticButton:
							case DriverType.ShuzOffButton:
							case DriverType.MDU:
								Devices.Add(device);
								break;
						}
						break;
				}
			}
			SelectedDevice = Devices.FirstOrDefault();
		}

		public ObservableCollection<Device> Devices { get; private set; }

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
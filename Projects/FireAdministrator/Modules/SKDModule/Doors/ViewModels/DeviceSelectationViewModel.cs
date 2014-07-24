using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class DeviceSelectationViewModel : SaveCancelDialogViewModel
	{
		public DeviceSelectationViewModel(Guid deviceUID, DoorType doorType)
		{
			Title = "Выбор устройства";
			Devices = new ObservableCollection<SKDDevice>();
			foreach (var skdDevice in SKDManager.Devices)
			{
				if (skdDevice.DriverType == SKDDriverType.Reader)
				{
					if (doorType == DoorType.TwoWay && (skdDevice.IntAddress % 2) == 1)
						continue;

					Devices.Add(skdDevice);
				}
			}
			SelectedDevice = Devices.FirstOrDefault(x => x.UID == deviceUID);
		}

		public ObservableCollection<SKDDevice> Devices { get; private set; }

		SKDDevice _selectedDevice;
		public SKDDevice SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				OnPropertyChanged(() => SelectedDevice);
			}
		}

		protected override bool Save()
		{
			return SelectedDevice != null;
		}
	}
}
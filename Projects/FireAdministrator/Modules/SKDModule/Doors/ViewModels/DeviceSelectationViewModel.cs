using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;
using FiresecAPI.SKD;

namespace SKDModule.ViewModels
{
	public class DeviceSelectationViewModel : SaveCancelDialogViewModel
	{
		public DeviceSelectationViewModel(Guid deviceUID)
		{
			Title = "Выбор устройства";
			Devices = new ObservableCollection<SKDDevice>();
			foreach (var skdDevice in SKDManager.Devices)
			{
				if (skdDevice.DriverType == SKDDriverType.Reader)
				{
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
				OnPropertyChanged("SelectedDevice");
			}
		}

		protected override bool Save()
		{
			return SelectedDevice != null;
		}
	}
}
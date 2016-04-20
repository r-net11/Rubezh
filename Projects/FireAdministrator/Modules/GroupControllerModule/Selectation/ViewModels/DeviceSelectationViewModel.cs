using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace GKModule.ViewModels
{
	[SaveSizeAttribute]
	public class DeviceSelectationViewModel : SaveCancelDialogViewModel
	{
		public DeviceSelectationViewModel(GKDevice selectedDevice, IEnumerable<GKDevice> sourceDevices = null)
		{
			Title = "Выбор устройства";
			Devices = new ObservableCollection<GKDevice>(sourceDevices);
			if (selectedDevice != null)
				SelectedDevice = Devices.FirstOrDefault(x => x.UID == selectedDevice.UID);
		}

		public ObservableCollection<GKDevice> Devices { get; private set; }

		GKDevice _selectedDevice;
		public GKDevice SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				OnPropertyChanged(() => SelectedDevice);
			}
		}

		public string Description
		{
			get
			{
				return SelectedDevice != null ? SelectedDevice.Description : "";
			}
			set
			{
				if (SelectedDevice != null)
				{
					SelectedDevice.Description = value;
					var device = DevicesViewModel.Current.AllDevices.FirstOrDefault(x => x.Device == SelectedDevice);
					if (device != null)
						device.Update();
				}
				OnPropertyChanged(() => Description);
			}
		}
	}
}
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using Infrastructure.Common.Windows;

namespace GKModule.ViewModels
{
	[SaveSizeAttribute]
	public class DeviceSelectationViewModel : SaveCancelDialogViewModel
	{
		public DeviceSelectationViewModel(List<XDevice> devices, XDevice selectedDevice)
		{
			Title = "Выбор устройства";

			Devices = new ObservableCollection<DeviceViewModel>();
			foreach (var device in devices)
			{
				var deviceViewModel = new DeviceViewModel(device);
				Devices.Add(deviceViewModel);
			}

			SelectedDevice = Devices.FirstOrDefault(x => x.Device == selectedDevice);
		}

		public ObservableCollection<DeviceViewModel> Devices { get; private set; }

		DeviceViewModel _selectedDevice;
		public DeviceViewModel SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				OnPropertyChanged("SelectedDevice");
			}
		}

		protected override bool CanSave()
		{
			return SelectedDevice != null;
		}
	}
}
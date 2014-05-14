using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	[SaveSizeAttribute]
	public class DeviceSelectationViewModel : SaveCancelDialogViewModel
	{
		public DeviceSelectationViewModel(XDevice selectedDevice, List<XDevice> sourceDevices = null)
		{
			Title = "Выбор устройства";

			Devices = new ObservableCollection<XDevice>(sourceDevices);
			if (selectedDevice != null)
				SelectedDevice = Devices.FirstOrDefault(x => x.BaseUID == selectedDevice.BaseUID);
		}

		public ObservableCollection<XDevice> Devices { get; private set; }

		XDevice _selectedDevice;
		public XDevice SelectedDevice
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
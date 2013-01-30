using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;

namespace DevicesModule.ViewModels
{
	public class ZoneLogicDeviceSelectionViewModel : SaveCancelDialogViewModel
	{
		public ZoneLogicDeviceSelectionViewModel(Device parentDevice)
		{
			Title = "Выбор устройства";
			Devices = new ObservableCollection<Device>(parentDevice.Children.Where(x => x.Driver.DriverType == DriverType.AM1_T));
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
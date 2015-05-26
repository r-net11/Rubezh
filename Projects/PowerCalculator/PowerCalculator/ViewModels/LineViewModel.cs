using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using PowerCalculator.Models;

namespace PowerCalculator.ViewModels
{
	public class LineViewModel : BaseViewModel
	{
		public LineViewModel(Line line)
		{
			Line = line;
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			Devices = new ObservableCollection<DeviceViewModel>();
		}

		public Line Line { get; set; }
		public ObservableCollection<DeviceViewModel> Devices { get; private set; }

		DeviceViewModel _selectedDevice;
		public DeviceViewModel SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				OnPropertyChanged(() => SelectedDevice);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var newDeviceViewModel = new NewDeviceViewModel();
			if (DialogService.ShowModalWindow(newDeviceViewModel))
			{
				for (int i = 0; i < newDeviceViewModel.Count; i++)
				{
					var device = new Device();
					device.DriverType = newDeviceViewModel.SelectedDeviceType.DriverType;
					Line.Devices.Add(device);
					var deviceViewModel = new DeviceViewModel(device);
					Devices.Add(deviceViewModel);
				}
				SelectedDevice = Devices.LastOrDefault();
			}
		}
		bool CanAdd()
		{
			return Devices.Count < 255;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			Line.Devices.Add(SelectedDevice.Device);
			Devices.Remove(SelectedDevice);
			SelectedDevice = Devices.FirstOrDefault();
		}
		bool CanRemove()
		{
			return SelectedDevice != null;
		}
	}
}
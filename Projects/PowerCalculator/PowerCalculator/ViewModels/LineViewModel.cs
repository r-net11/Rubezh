using System;
using System.Linq;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using PowerCalculator.Models;

namespace PowerCalculator.ViewModels
{
	public class LineViewModel : BaseViewModel
	{
		public LineViewModel()
		{
			Name = "Название АЛС";
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			Devices = new ObservableCollection<DeviceViewModel>();
		}

		public string Name { get; set; }

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
				var alsDevice = new AlsDevice();
				alsDevice.DeviceType = newDeviceViewModel.SelectedDeviceType.DeviceType;
				var deviceViewModel = new DeviceViewModel(alsDevice);
				Devices.Add(deviceViewModel);
				SelectedDevice = deviceViewModel;
			}
		}
		bool CanAdd()
		{
			return true;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			Devices.Remove(SelectedDevice);
			SelectedDevice = Devices.FirstOrDefault();
		}
		bool CanRemove()
		{
			return SelectedDevice != null;
		}
	}
}
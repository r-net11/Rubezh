using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Models;
using Infrastructure.Common;
using ClientFS2;

namespace MonitorClientFS2.ViewModels
{
	public class DevicesViewModel : DialogViewModel
	{
		public DevicesViewModel()
		{
			SetIgnoreCommand = new RelayCommand(OnSetIgnore, CanSetIgnore);
			ResetIgnoreCommand = new RelayCommand(OnResetIgnore, CanResetIgnore);

			BuildTree();
			if (RootDevice != null)
			{
				RootDevice.IsExpanded = true;
				SelectedDevice = RootDevice;
			}
			OnPropertyChanged("RootDevices");
		}

		DeviceViewModel _selectedDevice;
		public DeviceViewModel SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				if (value != null)
					value.ExpantToThis();
				OnPropertyChanged("SelectedDevice");
			}
		}
		DeviceViewModel _rootDevice;
		public DeviceViewModel RootDevice
		{
			get { return _rootDevice; }
			private set
			{
				_rootDevice = value;
				OnPropertyChanged("RootDevice");
			}
		}
		public DeviceViewModel[] RootDevices
		{
			get { return new[] { RootDevice }; }
		}
		void BuildTree()
		{
			RootDevice = AddDeviceInternal(ConfigurationManager.DeviceConfiguration.RootDevice, null);
		}
		private static DeviceViewModel AddDeviceInternal(Device device, TreeItemViewModel<DeviceViewModel> parentDeviceViewModel)
		{
			var deviceViewModel = new DeviceViewModel(device);
			if (parentDeviceViewModel != null)
				parentDeviceViewModel.Children.Add(deviceViewModel);

			foreach (var childDevice in device.Children)
				AddDeviceInternal(childDevice, deviceViewModel);
			return deviceViewModel;
		}

		public RelayCommand SetIgnoreCommand { get; private set; }
		void OnSetIgnore()
		{

		}
		bool CanSetIgnore()
		{
			return SelectedDevice != null;
		}

		public RelayCommand ResetIgnoreCommand { get; private set; }
		void OnResetIgnore()
		{

		}
		bool CanResetIgnore()
		{
			return SelectedDevice != null;
		}
	}
}
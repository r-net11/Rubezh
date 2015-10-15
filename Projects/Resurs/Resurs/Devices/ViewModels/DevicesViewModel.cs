using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Resurs.Processor;
using ResursAPI;
using ResursDAL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class DevicesViewModel : BaseViewModel
	{
		public DevicesViewModel()
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);

			BuildTree();
			if (RootDevice != null)
			{
				RootDevice.IsExpanded = true;
				SelectedDevice = RootDevice;
				foreach (var child in RootDevice.Children)
				{
					if (true)
					{
						child.IsExpanded = true;
					}
				}
			}

			foreach (var device in AllDevices)
			{
				if (true)
					device.ExpandToThis();
			}

			OnPropertyChanged(() => RootDevices);
		}

		DeviceViewModel _selectedDevice;
		public DeviceViewModel SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				if(_selectedDevice != null)
					_selectedDevice.Load();
				OnPropertyChanged(() => SelectedDevice);
			}
		}

		DeviceViewModel _rootDevice;
		public DeviceViewModel RootDevice
		{
			get { return _rootDevice; }
			private set
			{
				_rootDevice = value;
				OnPropertyChanged(() => RootDevice);
			}
		}

		public DeviceViewModel[] RootDevices
		{
			get { return new[] { RootDevice }; }
		}

		void BuildTree()
		{
			RootDevice = AddDeviceInternal(ResursDAL.DBCash.RootDevice, null);
			FillAllDevices();
		}

		public DeviceViewModel AddDevice(Device device, DeviceViewModel parentDeviceViewModel)
		{
			var deviceViewModel = AddDeviceInternal(device, parentDeviceViewModel);
			FillAllDevices();
			return deviceViewModel;
		}
		private DeviceViewModel AddDeviceInternal(Device device, DeviceViewModel parentDeviceViewModel)
		{
			var deviceViewModel = new DeviceViewModel(device);
			if (parentDeviceViewModel != null)
				parentDeviceViewModel.AddChild(deviceViewModel);

			foreach (var childDevice in device.Children)
			{
				AddDeviceInternal(childDevice, deviceViewModel);
			}
			return deviceViewModel;
		}

		public List<DeviceViewModel> AllDevices;

		public void FillAllDevices()
		{
			AllDevices = new List<DeviceViewModel>();
			AddChildPlainDevices(RootDevice);
		}

		void AddChildPlainDevices(DeviceViewModel parentViewModel)
		{
			AllDevices.Add(parentViewModel);
			foreach (var childViewModel in parentViewModel.Children)
				AddChildPlainDevices(childViewModel);
		}

		public void Select(Guid deviceUID)
		{
			if (deviceUID != Guid.Empty)
			{
				FillAllDevices();
				var deviceViewModel = AllDevices.FirstOrDefault(x => x.Device.UID == deviceUID);
				if (deviceViewModel != null)
					deviceViewModel.ExpandToThis();
				Bootstrapper.MainViewModel.SelectedTabIndex = 0;
				SelectedDevice = deviceViewModel;
			}
		}

		public bool IsVisibility
		{
			get { return DBCash.CurrentUser.UserPermissions.Any(x => x.PermissionType == PermissionType.Device); }
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var parent = SelectedDevice;
			if (parent.Device.Driver.Children.Count == 0)
				parent = SelectedDevice.Parent;
			bool isCreateDevice = true;
			var driverType = new DriverType();
			if (parent.Device.Driver.Children.Count > 1)
			{
				var driverTypesViewModel = new DriverTypesViewModel(parent.Device.DriverType);
				if (DialogService.ShowModalWindow(driverTypesViewModel))
				{
					driverType = driverTypesViewModel.SelectedDriverType;
				}
				else
					isCreateDevice = false;
			}
			else
				driverType = parent.Device.Driver.Children.FirstOrDefault();
			if(isCreateDevice)
			{
				var deviceDetailsViewModel = new DeviceDetailsViewModel(driverType, parent.Device);
				if (DialogService.ShowModalWindow(deviceDetailsViewModel))
				{
					var deviceViewModel = new DeviceViewModel(deviceDetailsViewModel.Device);
					parent.AddChild(deviceViewModel);
					parent.IsExpanded = true;
					AllDevices.Add(deviceViewModel);
					SelectedDevice = deviceViewModel;
				}
			}
		}
		bool CanAdd()
		{
			return SelectedDevice != null && DBCash.CurrentUser.UserPermissions.Any(x => x.PermissionType == PermissionType.EditDevice);
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var deviceDetailsViewModel = new DeviceDetailsViewModel(SelectedDevice.Device);
			if (DialogService.ShowModalWindow(deviceDetailsViewModel))
			{
				var deviceViewModel = new DeviceViewModel(deviceDetailsViewModel.Device);
				SelectedDevice.Update(deviceDetailsViewModel.Device);
				foreach (var child in SelectedDevice.Children)
				{
					child.Update();
				}

			}
		}
		bool CanEdit()
		{
			return SelectedDevice != null && SelectedDevice.Parent != null && DBCash.CurrentUser.UserPermissions.Any(x => x.PermissionType == PermissionType.EditDevice);;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить устройство?"))
			{
				var selectedDevice = SelectedDevice;
				var parent = selectedDevice.Parent;
				if (parent != null && DBCash.DeleteDevice(selectedDevice.Device))
				{
					var index = selectedDevice.VisualIndex;
					parent.Nodes.Remove(selectedDevice);
					index = Math.Min(index, parent.ChildrenCount - 1);
					foreach (var childDeviceViewModel in selectedDevice.GetAllChildren(true))
					{
						AllDevices.Remove(childDeviceViewModel);
					}
					SelectedDevice = index >= 0 ? parent.GetChildByVisualIndex(index) : parent;
				}
			}
		}
		bool CanRemove()
		{
			return SelectedDevice != null && SelectedDevice.Parent != null && DBCash.CurrentUser.UserPermissions.Any(x => x.PermissionType == PermissionType.EditDevice);
		}
	}
}
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Resurs.Processor;
using ResursAPI;
using ResursDAL;
using ResursNetwork.Networks;
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
			SetActiveCommand = new RelayCommand(OnSetActive, CanSetActive);
			UnSetActiveCommand = new RelayCommand(OnUnSetActive, CanUnSetActive);
			
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
					DeviceProcessor.Instance.AddToMonitoring(child.Device);
					foreach (var counter in child.Children)
					{
						DeviceProcessor.Instance.AddToMonitoring(counter.Device);
					}
				}
			}

			OnPropertyChanged(() => RootDevices);

			DeviceProcessor.Instance.IsActiveChanged -= OnIsActiveChanged;
			DeviceProcessor.Instance.IsActiveChanged += OnIsActiveChanged;
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
				SelectedDevice = deviceViewModel;
				Bootstrapper.MainViewModel.SelectedTabIndex = 0;
			}
		}

		public bool IsVisible
		{
			get { return DBCash.CheckPermission(PermissionType.ViewDevice); }
		}

		void UpdateBillViewModels(Device device, Bill oldBill = null)
		{
			var newBill = device.Bill;
			if (oldBill == newBill)
				return;
			if (oldBill != null)
			{
				var billViewModel = Bootstrapper.MainViewModel.ConsumersViewModel.FindBillViewModel(oldBill.UID);
				if (billViewModel != null)
				{
					var deviceViewModel = billViewModel.Devices.FirstOrDefault(x => x.Device.UID == device.UID);
					if (deviceViewModel != null)
						billViewModel.Devices.Remove(deviceViewModel);
				}
			}
			if (newBill != null)
			{
				var billViewModel = Bootstrapper.MainViewModel.ConsumersViewModel.FindBillViewModel(newBill.UID);
				if (billViewModel != null)
				{
					var deviceViewModel = billViewModel.Devices.FirstOrDefault(x => x.Device.UID == device.UID);
					if (deviceViewModel == null)
						billViewModel.Devices.Add(new DeviceViewModel(device));
				}
			}
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
					DeviceProcessor.Instance.AddToMonitoring(deviceViewModel.Device);
					UpdateBillViewModels(SelectedDevice.Device);
				}
			}
		}
		bool CanAdd()
		{
			return SelectedDevice != null && DBCash.CheckPermission(PermissionType.EditDevice);
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var oldBill = SelectedDevice.Device.Bill;
			var deviceDetailsViewModel = new DeviceDetailsViewModel(SelectedDevice.Device);
			if (DialogService.ShowModalWindow(deviceDetailsViewModel))
			{
				var deviceViewModel = new DeviceViewModel(deviceDetailsViewModel.Device);
				SelectedDevice.Update(deviceDetailsViewModel.Device);
				foreach (var child in SelectedDevice.Children)
				{
					child.Update();
				}
				UpdateBillViewModels(SelectedDevice.Device, oldBill);
			}
		}
		bool CanEdit()
		{
			return SelectedDevice != null && SelectedDevice.Parent != null && DBCash.CheckPermission(PermissionType.EditDevice);
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
					DeviceProcessor.Instance.DeleteFromMonitoring(selectedDevice.Device);
					parent.Nodes.Remove(selectedDevice);
					index = Math.Min(index, parent.ChildrenCount - 1);
					foreach (var childDeviceViewModel in selectedDevice.GetAllChildren(true))
					{
						AllDevices.Remove(childDeviceViewModel);
					}
					UpdateBillViewModels(SelectedDevice.Device, SelectedDevice.Device.Bill);
					SelectedDevice = index >= 0 ? parent.GetChildByVisualIndex(index) : parent;
				}
			}
		}
		bool CanRemove()
		{
			return SelectedDevice != null && SelectedDevice.Parent != null && DBCash.CheckPermission(PermissionType.EditDevice);
		}

		public RelayCommand SetActiveCommand { get; private set; }
		void OnSetActive()
		{
			if (DeviceProcessor.Instance.WriteParameters(SelectedDevice.Device))
			{
				DeviceProcessor.Instance.SetStatus(SelectedDevice.Device, true);
			}
		}
		bool CanSetActive()
		{
			return SelectedDevice != null && 
				SelectedDevice.Parent != null &&
				!SelectedDevice.IsActive && 
				DBCash.CurrentUser.UserPermissions.Any(x => x.PermissionType == PermissionType.EditDevice);
		}

		public RelayCommand UnSetActiveCommand { get; private set; }
		void OnUnSetActive()
		{
			DeviceProcessor.Instance.SetStatus(SelectedDevice.Device, false);
		}
		bool CanUnSetActive()
		{
			return SelectedDevice != null &&
				SelectedDevice.Parent != null &&
				SelectedDevice.IsActive && 
				DBCash.CurrentUser.UserPermissions.Any(x => x.PermissionType == PermissionType.EditDevice);
		}

		void OnIsActiveChanged(object sender, IsActiveChangedEventArgs args)
		{
			var device = RootDevice.GetAllChildren().FirstOrDefault(x => x.Device.UID == args.DeviceUID);
			if(device != null)
			{
				SelectedDevice.Device.IsActive = args.IsActive;
				DBCash.SaveDevice(SelectedDevice.Device);
				SelectedDevice.Update();
			}
		}
	}
}
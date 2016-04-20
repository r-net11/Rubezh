using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;
using ResursDAL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Resurs.ViewModels
{
	public class SelectDeviceViewModel : SaveCancelDialogViewModel
	{
		public SelectDeviceViewModel(List<Guid> exceptDeviceUids)
		{
			Title = "Выбор устройства";
			BuildTree(exceptDeviceUids);
			if (RootDevice != null)
			{
				RootDevice.IsExpanded = true;
				SelectedDevice = RootDevice;
				foreach (var child in RootDevice.Children)
					child.IsExpanded = true;
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
					_selectedDevice.Update();
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

		void BuildTree(List<Guid> exceptDeviceUids)
		{
			RootDevice = AddDeviceInternal(ResursDAL.DbCache.RootDevice, null, exceptDeviceUids);
		}
				
		private DeviceViewModel AddDeviceInternal(Device device, DeviceViewModel parentDeviceViewModel, List<Guid> exceptDeviceUids)
		{
			var deviceViewModel = new DeviceViewModel(device);
			foreach (var childDevice in device.Children)
			{
				if (!exceptDeviceUids.Any(x => x == childDevice.UID))
					AddDeviceInternal(childDevice, deviceViewModel, exceptDeviceUids);
			}
			if (parentDeviceViewModel != null && (deviceViewModel.ChildrenCount > 0 || device.Children.Count == 0))
				parentDeviceViewModel.AddChild(deviceViewModel);
			return deviceViewModel;
		}

		protected override bool CanSave()
		{
			return SelectedDevice != null && SelectedDevice.Device.DeviceType == DeviceType.Counter;
		}

		public bool IsVisibility
		{
			get { return DbCache.CurrentUser.UserPermissions.Any(x => x.PermissionType == PermissionType.ViewDevice); }
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.Models.Layouts;
using RubezhAPI;

namespace GKModule.ViewModels
{
	public class DevicesViewModel : ViewPartViewModel, ISelectable<Guid>
	{
		public static DevicesViewModel Current { get; private set; }
		public DevicesViewModel()
		{
			Current = this;
			IsVisibleBottomPanel = true;
		}

		public void Initialize()
		{
			BuildTree();
			if (RootDevice != null)
			{
				RootDevice.IsExpanded = true;
				SelectedDevice = RootDevice;
			}

			foreach (var device in AllDevices)
			{
				if (device.Device.DriverType == GKDriverType.RSR2_KAU)
					device.ExpandToThis();
			}

			RootDevices = null;
			OnPropertyChanged(() => RootDevices);
			RootDevices = new DeviceViewModel[] { RootDevice };
			OnPropertyChanged(() => RootDevices);
		}

		#region DeviceSelection
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
				SelectedDevice = AllDevices.FirstOrDefault(x => x.Device.UID == deviceUID);
			}
		}
		#endregion

		DeviceViewModel _selectedDevice;
		public DeviceViewModel SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				if (value != null)
					value.ExpandToThis();
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

		public DeviceViewModel[] RootDevices { get; private set; }
		LayoutPartAdditionalProperties _properties;
		public LayoutPartAdditionalProperties Properties
		{
			get { return _properties; }
			set
			{
				_properties = value;
				IsVisibleBottomPanel = (_properties != null) ? _properties.IsVisibleBottomPanel : false;
			}
		}
		bool _isVisibleBottomPanel;
		public bool IsVisibleBottomPanel
		{
			get { return _isVisibleBottomPanel; }
			set
			{
				_isVisibleBottomPanel = value;
				OnPropertyChanged(() => IsVisibleBottomPanel);
			}
		}
		void BuildTree()
		{
			RootDevice = AddDeviceInternal(GKManager.DeviceConfiguration.RootDevice, null);
			FillAllDevices();
		}

		private DeviceViewModel AddDeviceInternal(GKDevice device, DeviceViewModel parentDeviceViewModel)
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
	}
}
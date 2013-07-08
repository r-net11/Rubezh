using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using DevicesModule.DeviceProperties;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrastructure.ViewModels;
using KeyboardKey = System.Windows.Input.Key;
using Infrustructure.Plans.Elements;
using DevicesModule.Plans.Designer;
using Infrustructure.Plans.Events;
using Common;
using DevicesModule.Plans;

namespace DevicesModule.ViewModels
{
	public class DeviceParametersViewModel : MenuViewPartViewModel, ISelectable<Guid>
	{
		bool _lockSelection;

		public DeviceParametersViewModel()
		{
			Menu = new DeviceParametersMenuViewModel(this);
			WriteCommand = new RelayCommand(OnWrite);
			ReadCommand = new RelayCommand(OnRead);
		}

		public void Initialize()
		{
			foreach (var device in FiresecManager.Devices)
			{
				if (device.SystemParameters == null)
				{
					device.SystemParameters = new List<Property>();
				}
				if (device.DeviceParameters == null)
				{
					device.DeviceParameters = new List<Property>();
				}
			}
			BuildTree();
			if (RootDevice != null)
			{
				RootDevice.IsExpanded = true;
				SelectedDevice = RootDevice;
			}
			OnPropertyChanged("RootDevices");
		}

		#region Tree
		public List<DeviceViewModel> AllDevices;

		public void FillAllDevices()
		{
			AllDevices = new List<DeviceViewModel>();
			AddChildPlainDevices(RootDevice);
		}

		void AddChildPlainDevices(DeviceViewModel parentViewModel)
		{
			AllDevices.Add(parentViewModel);
			foreach (DeviceViewModel childViewModel in parentViewModel.Children)
				AddChildPlainDevices(childViewModel);
		}

		public void Select(Guid deviceUID)
		{
			if (deviceUID != Guid.Empty)
			{
				FillAllDevices();
				var deviceViewModel = AllDevices.FirstOrDefault(x => x.Device.UID == deviceUID);
				if (deviceViewModel != null)
				{
					deviceViewModel.ExpandToThis();
					SelectedDevice = deviceViewModel;
				}
			}
		}

		DeviceViewModel _selectedDevice;
		public DeviceViewModel SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				OnPropertyChanged("SelectedDevice");
				if (!_lockSelection && _selectedDevice != null && _selectedDevice.Device.PlanElementUIDs.Count > 0)
					ServiceFactory.Events.GetEvent<FindElementEvent>().Publish(_selectedDevice.Device.PlanElementUIDs);

				if (value != null)
				{
					OneDeviceParameterViewModel = new OneDeviceParameterViewModel(value.Device);
				}
				else
				{
					OneDeviceParameterViewModel = new OneDeviceParameterViewModel(null);
				}
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
			get { return new DeviceViewModel[] { RootDevice }; }
		}

		void BuildTree()
		{
			RootDevice = AddDeviceInternal(FiresecManager.FiresecConfiguration.DeviceConfiguration.RootDevice, null);
			FillAllDevices();
		}

		public DeviceViewModel AddDevice(Device device, DeviceViewModel parentDeviceViewModel)
		{
			var deviceViewModel = AddDeviceInternal(device, parentDeviceViewModel);
			FillAllDevices();
			return deviceViewModel;
		}
		DeviceViewModel AddDeviceInternal(Device device, DeviceViewModel parentDeviceViewModel)
		{
			var deviceViewModel = new DeviceViewModel(device);
			if (parentDeviceViewModel != null)
				parentDeviceViewModel.AddChild(deviceViewModel);

			foreach (var childDevice in device.Children)
				AddDeviceInternal(childDevice, deviceViewModel);
			return deviceViewModel;
		}
		#endregion

		OneDeviceParameterViewModel _oneDeviceParameterViewModel;
		public OneDeviceParameterViewModel OneDeviceParameterViewModel
		{
			get { return _oneDeviceParameterViewModel; }
			set
			{
				_oneDeviceParameterViewModel = value;
				OnPropertyChanged("OneDeviceParameterViewModel");
			}
		}

		public RelayCommand WriteCommand { get; private set; }
		void OnWrite()
		{

		}

		public RelayCommand ReadCommand { get; private set; }
		void OnRead()
		{
			
		}

		public override void OnShow()
		{
			base.OnShow();
			Initialize();
		}

		public override void OnHide()
		{
			base.OnHide();
		}
	}
}
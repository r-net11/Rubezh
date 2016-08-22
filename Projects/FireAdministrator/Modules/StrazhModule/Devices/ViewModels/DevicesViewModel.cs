using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using Infrustructure.Plans.Events;
using StrazhAPI.Models;
using StrazhAPI.Plans.Elements;
using StrazhAPI.SKD;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Localization.Strazh.Common;
using Localization.Strazh.ViewModels;
using KeyboardKey = System.Windows.Input.Key;

namespace StrazhModule.ViewModels
{
	public class DevicesViewModel : MenuViewPartViewModel, ISelectable<Guid>
	{
		private DeviceViewModel _selectedDevice;
		private bool _lockSelection;
		private DeviceViewModel _rootDevice;
		public List<DeviceViewModel> AllDevices;

		public DevicesViewModel()
		{
			Menu = new DevicesMenuViewModel(this);
			Current = this;
			DeviceCommandsViewModel = new DeviceCommandsViewModel(this);
			RegisterShortcuts();
			IsRightPanelEnabled = true;
			SubscribeEvents();
			SetRibbonItems();
		}

		#region Properties

		public static DevicesViewModel Current { get; private set; }
		public DeviceCommandsViewModel DeviceCommandsViewModel { get; private set; }

		public DeviceViewModel SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				OnPropertyChanged(() => SelectedDevice);
				UpdateRibbonItems();
				if (!_lockSelection && _selectedDevice != null && _selectedDevice.Device.PlanElementUIDs.Count > 0)
					ServiceFactoryBase.Events.GetEvent<FindElementEvent>().Publish(_selectedDevice.Device.PlanElementUIDs);
			}
		}

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

		#endregion

		public void Initialize()
		{
			BuildTree();
			if (RootDevice != null)
			{
				RootDevice.IsExpanded = true;
				SelectedDevice = RootDevice;
				foreach (var child in RootDevice.Children)
				{
					if (child.Driver.DriverType == SKDDriverType.Controller)
						child.IsExpanded = true;
				}
			}

			foreach (var device in AllDevices)
			{
				if (device.Device.Driver.DriverType == SKDDriverType.Controller)
					device.ExpandToThis();
			}

			OnPropertyChanged(() => RootDevices);
		}

		public void FillAllDevices()
		{
			AllDevices = new List<DeviceViewModel>();
			AddChildPlainDevices(RootDevice);
		}

		private void AddChildPlainDevices(DeviceViewModel parentViewModel)
		{
			AllDevices.Add(parentViewModel);
			foreach (var childViewModel in parentViewModel.Children)
				AddChildPlainDevices(childViewModel);
		}

		public void Select(Guid deviceUID)
		{
			if (deviceUID == Guid.Empty) return;

			FillAllDevices();
			var deviceViewModel = AllDevices.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceViewModel != null)
				deviceViewModel.ExpandToThis();

			SelectedDevice = deviceViewModel;
		}

		private void BuildTree()
		{
			RootDevice = AddDeviceInternal(SKDManager.SKDConfiguration.RootDevice, null);
			FillAllDevices();
		}

		public DeviceViewModel AddDevice(SKDDevice device, DeviceViewModel parentDeviceViewModel)
		{
			var deviceViewModel = AddDeviceInternal(device, parentDeviceViewModel);
			FillAllDevices();
			return deviceViewModel;
		}
		private static DeviceViewModel AddDeviceInternal(SKDDevice device, DeviceViewModel parentDeviceViewModel)
		{
			var deviceViewModel = new DeviceViewModel(device);

			if (parentDeviceViewModel != null)
				parentDeviceViewModel.AddChild(deviceViewModel);

			foreach (var childDevice in device.Children)
				AddDeviceInternal(childDevice, deviceViewModel);

			return deviceViewModel;
		}

		private void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), () =>
			{
				if (SelectedDevice != null)
				{
					if (SelectedDevice.AddCommand.CanExecute(null))
						SelectedDevice.AddCommand.Execute();
				}
			});
			RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), () =>
			{
				if (SelectedDevice != null)
				{
					if (SelectedDevice.RemoveCommand.CanExecute(null))
						SelectedDevice.RemoveCommand.Execute();
				}
			});
			RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), () =>
			{
				if (SelectedDevice != null)
				{
					if (SelectedDevice.ShowPropertiesCommand.CanExecute(null))
						SelectedDevice.ShowPropertiesCommand.Execute();
				}
			});
			RegisterShortcut(new KeyGesture(KeyboardKey.Right, ModifierKeys.Control), () =>
			{
				if (SelectedDevice != null)
				{
					if (SelectedDevice.HasChildren && !SelectedDevice.IsExpanded)
						SelectedDevice.IsExpanded = true;
				}
			});
			RegisterShortcut(new KeyGesture(KeyboardKey.Left, ModifierKeys.Control), () =>
			{
				if (SelectedDevice != null)
				{
					if (SelectedDevice.HasChildren && SelectedDevice.IsExpanded)
						SelectedDevice.IsExpanded = false;
				}
			});
		}

		private void SubscribeEvents()
		{
			ServiceFactoryBase.Events.GetEvent<ElementAddedEvent>().Unsubscribe(OnElementChanged);
			ServiceFactoryBase.Events.GetEvent<ElementRemovedEvent>().Unsubscribe(OnElementChanged);
			ServiceFactoryBase.Events.GetEvent<ElementChangedEvent>().Subscribe(OnElementChanged);
			ServiceFactoryBase.Events.GetEvent<ElementSelectedEvent>().Unsubscribe(OnElementSelected);

			ServiceFactoryBase.Events.GetEvent<ElementAddedEvent>().Subscribe(OnElementChanged);
			ServiceFactoryBase.Events.GetEvent<ElementRemovedEvent>().Subscribe(OnElementChanged);
			ServiceFactoryBase.Events.GetEvent<ElementChangedEvent>().Subscribe(OnElementChanged);
			ServiceFactoryBase.Events.GetEvent<ElementSelectedEvent>().Subscribe(OnElementSelected);
		}
		private void OnDeviceChanged(Guid deviceUID)
		{
			var device = AllDevices.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (device == null) return;

			device.Update();
			// TODO: FIX IT
			if (!_lockSelection)
			{
				device.ExpandToThis();
				SelectedDevice = device;
			}
		}
		private void OnElementChanged(List<ElementBase> elements)
		{
			_lockSelection = true;
			elements.ForEach(element =>
			{
				var elementDevice = element as ElementSKDDevice;
				if (elementDevice != null)
					OnDeviceChanged(elementDevice.DeviceUID);
			});
			_lockSelection = false;
		}
		private void OnElementSelected(ElementBase element)
		{
			var elementSKDDevice = element as ElementSKDDevice;
			if (elementSKDDevice == null) return;

			_lockSelection = true;
			Select(elementSKDDevice.DeviceUID);
			_lockSelection = false;
		}

		protected override void UpdateRibbonItems()
		{
			base.UpdateRibbonItems();
			RibbonItems[0][0].Command = SelectedDevice == null ? null : SelectedDevice.AddCommand;
			RibbonItems[0][1].Command = SelectedDevice == null ? null : SelectedDevice.ShowPropertiesCommand;
			RibbonItems[0][2].Command = SelectedDevice == null ? null : SelectedDevice.RemoveCommand;
		}
		private void SetRibbonItems()
		{
			RibbonItems = new List<RibbonMenuItemViewModel>
			{
				new RibbonMenuItemViewModel(CommonViewModels.Edition, new ObservableCollection<RibbonMenuItemViewModel>
				{
					new RibbonMenuItemViewModel(CommonResources.Add, "BAdd"),
					new RibbonMenuItemViewModel(CommonResources.Edit, "BEdit"),
					new RibbonMenuItemViewModel(CommonResources.Delete, "BDelete"),
				}, "BEdit") { Order = 1 } ,
				new RibbonMenuItemViewModel(CommonResources.Device, new ObservableCollection<RibbonMenuItemViewModel>
				{
					new RibbonMenuItemViewModel(CommonViewModels.Devices_ControllerParams, DeviceCommandsViewModel.ShowControllerConfigurationCommand, "BParametersWrite"),
					new RibbonMenuItemViewModel(CommonViewModels.Devices_DoorParams, DeviceCommandsViewModel.ShowLockConfigurationCommand, "BInformation") { IsNewGroup = true },
				}, "BDevice") { Order = 2 }
			};
		}
	}
}
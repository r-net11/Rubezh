using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DevicesModule.DeviceProperties;
using DevicesModule.Views;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrastructure.ViewModels;
using System.Windows.Input;
using KeyboardKey = System.Windows.Input.Key;

namespace DevicesModule.ViewModels
{
	public class DevicesViewModel : MenuViewPartViewModel, ISelectable<Guid>
	{
		public static DevicesViewModel Current { get; private set; }
		public DeviceCommandsViewModel DeviceCommandsViewModel { get; private set; }
		public PropertiesViewModel PropMenu { get; private set; }
		public DevicesViewModel()
		{
			Current = this;
			CopyCommand = new RelayCommand(OnCopy, CanCutCopy);
			CutCommand = new RelayCommand(OnCut, CanCutCopy);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
			PasteAsCommand = new RelayCommand(OnPasteAs, CanPasteAs);
			DeviceCommandsViewModel = new DeviceCommandsViewModel(this);
			Menu = new DevicesMenuViewModel(this);
			PropMenu = new PropertiesViewModel(this);
		}

		public void Initialize()
		{
			RegisterShortcuts();
			BuildTree();
			if (Devices.Count > 0)
			{
				CollapseChild(Devices[0]);
				ExpandChild(Devices[0]);
				SelectedDevice = Devices[0];
			}
			UpdateGuardVisibility();
		}
		private void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.C, ModifierKeys.Control), CopyCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.V, ModifierKeys.Control), PasteCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.X, ModifierKeys.Control), CutCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.Insert, ModifierKeys.Control), () =>
				{
					if (SelectedDevice == null)
						return;
					SelectedDevice.AddCommand.Execute();
				});
			RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), () =>
				{
					if (SelectedDevice == null)
						return;
					SelectedDevice.RemoveCommand.Execute();
				});
			RegisterShortcut(new KeyGesture(KeyboardKey.Right, ModifierKeys.Control), () =>
				{
					if (SelectedDevice == null)
						return;
					if (SelectedDevice.HasChildren && !SelectedDevice.IsExpanded)
						SelectedDevice.IsExpanded = true;
				});
			RegisterShortcut(new KeyGesture(KeyboardKey.Left, ModifierKeys.Control), () =>
				{
					if (SelectedDevice == null)
						return;
					if (SelectedDevice.HasChildren && SelectedDevice.IsExpanded)
						SelectedDevice.IsExpanded = false;
				});

			//if (e.Key == Key.P && Keyboard.Modifiers == ModifierKeys.Control)
			//    PressButton(showPropertiesButton);
			//if (e.Key == Key.F && Keyboard.Modifiers == ModifierKeys.Control)
			//    PressButton(autoDetectButton);
			//if (e.Key == Key.R && Keyboard.Modifiers == ModifierKeys.Control)
			//    PressButton(readDeviceButton, false);
			//if (e.Key == Key.R && Keyboard.Modifiers == ModifierKeys.Alt)
			//    PressButton(usbReadDeviceButton, true);
			//if (e.Key == Key.W && Keyboard.Modifiers == ModifierKeys.Control)
			//    PressButton(writeDeviceButton, false);
			//if (e.Key == Key.W && Keyboard.Modifiers == ModifierKeys.Alt)
			//    PressButton(usbWriteDeviceButton, true);
			//if (e.Key == Key.W && ((int)Keyboard.Modifiers == ((int)ModifierKeys.Control + (int)ModifierKeys.Shift)))
			//    PressButton(writeAllDeviceButton);
			//if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
			//    PressButton(setPasswordButton, false);
			//if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Alt)
			//    PressButton(usbSetPasswordButton, true);
			//if (e.Key == Key.U && Keyboard.Modifiers == ModifierKeys.Control)
			//    PressButton(updateSoftButton, false);
			//if (e.Key == Key.U && Keyboard.Modifiers == ModifierKeys.Alt)
			//    PressButton(usbUpdateSoftButton, true);
		}

		#region DeviceSelection
		public List<DeviceViewModel> AllDevices;

		public void FillAllDevices()
		{
			AllDevices = new List<DeviceViewModel>();
			AddChildPlainDevices(Devices[0]);
		}

		void AddChildPlainDevices(DeviceViewModel parentViewModel)
		{
			AllDevices.Add(parentViewModel);
			foreach (var childViewModel in parentViewModel.Children)
			{
				AddChildPlainDevices(childViewModel);
			}
		}

		public void Select(Guid deviceUID)
		{
			if (deviceUID != Guid.Empty)
			{
				FillAllDevices();

				var deviceViewModel = AllDevices.FirstOrDefault(x => x.Device.UID == deviceUID);
				if (deviceViewModel != null)
					deviceViewModel.ExpantToThis();
				SelectedDevice = deviceViewModel;
			}
		}
		#endregion

		ObservableCollection<DeviceViewModel> _devices;
		public ObservableCollection<DeviceViewModel> Devices
		{
			get { return _devices; }
			set
			{
				_devices = value;
				OnPropertyChanged("Devices");
			}
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

		void BuildTree()
		{
			Devices = new ObservableCollection<DeviceViewModel>();
			AddDevice(FiresecManager.FiresecConfiguration.DeviceConfiguration.RootDevice, null);
		}

		public DeviceViewModel AddDevice(Device device, DeviceViewModel parentDeviceViewModel)
		{
			var deviceViewModel = new DeviceViewModel(device, Devices);
			deviceViewModel.Parent = parentDeviceViewModel;

			var indexOf = Devices.IndexOf(parentDeviceViewModel);
			Devices.Insert(indexOf + 1, deviceViewModel);

			foreach (var childDevice in device.Children)
			{
				var childDeviceViewModel = AddDevice(childDevice, deviceViewModel);
				deviceViewModel.Children.Add(childDeviceViewModel);
			}

			return deviceViewModel;
		}

		public void CollapseChild(DeviceViewModel parentDeviceViewModel)
		{
			parentDeviceViewModel.IsExpanded = false;
			foreach (var deviceViewModel in parentDeviceViewModel.Children)
			{
				CollapseChild(deviceViewModel);
			}
		}

		public void ExpandChild(DeviceViewModel parentDeviceViewModel)
		{
			if (parentDeviceViewModel.Device.Driver.Category != DeviceCategoryType.Device)
			{
				parentDeviceViewModel.IsExpanded = true;
				foreach (var deviceViewModel in parentDeviceViewModel.Children)
				{
					ExpandChild(deviceViewModel);
				}
			}
		}

		Device _deviceToCopy;
		bool _isFullCopy;

		bool CanCutCopy()
		{
			return !(SelectedDevice == null || SelectedDevice.Parent == null ||
				SelectedDevice.Driver.IsAutoCreate || SelectedDevice.Parent.Driver.AutoChild == SelectedDevice.Driver.UID
				|| SelectedDevice.Driver.DriverType == DriverType.MPT);
		}

		public RelayCommand CopyCommand { get; private set; }
		void OnCopy()
		{
			_deviceToCopy = FiresecManager.FiresecConfiguration.CopyDevice(SelectedDevice.Device, false);
		}

		public RelayCommand CutCommand { get; private set; }
		void OnCut()
		{
			_deviceToCopy = FiresecManager.FiresecConfiguration.CopyDevice(SelectedDevice.Device, true);
			SelectedDevice.RemoveCommand.Execute();

			FiresecManager.FiresecConfiguration.DeviceConfiguration.Update();
			ServiceFactory.SaveService.DevicesChanged = true;
			UpdateGuardVisibility();
		}

		bool CanPaste()
		{
			return (SelectedDevice != null && _deviceToCopy != null && SelectedDevice.Driver.AvaliableChildren.Contains(_deviceToCopy.Driver.UID));
		}

		public RelayCommand PasteCommand { get; private set; }
		void OnPaste()
		{
			var pasteDevice = FiresecManager.FiresecConfiguration.CopyDevice(_deviceToCopy, _isFullCopy);
			PasteDevice(pasteDevice);
		}

		bool CanPasteAs()
		{
			return (SelectedDevice != null && _deviceToCopy != null &&
				(DriversHelper.PanelDrivers.Contains(_deviceToCopy.Driver.DriverType) || DriversHelper.UsbPanelDrivers.Contains(_deviceToCopy.Driver.DriverType)) &&
				(SelectedDevice.Driver.DriverType == DriverType.Computer || SelectedDevice.Driver.DriverType == DriverType.USB_Channel_1 || SelectedDevice.Driver.DriverType == DriverType.USB_Channel_2));
		}

		public RelayCommand PasteAsCommand { get; private set; }
		void OnPasteAs()
		{
			var pasteAsViewModel = new PasteAsViewModel(SelectedDevice.Driver.DriverType);
			if (DialogService.ShowModalWindow(pasteAsViewModel))
			{
				var pasteDevice = FiresecManager.FiresecConfiguration.CopyDevice(_deviceToCopy, _isFullCopy);

				pasteDevice.DriverUID = pasteAsViewModel.SelectedDriver.UID;
				pasteDevice.Driver = pasteAsViewModel.SelectedDriver;

				FiresecManager.FiresecConfiguration.SynchronizeChildern(pasteDevice);

				PasteDevice(pasteDevice);
			}
		}

		void PasteDevice(Device device)
		{
			SelectedDevice.Device.Children.Add(device);
			device.Parent = SelectedDevice.Device;

			var newDevice = AddDevice(device, SelectedDevice);
			SelectedDevice.Children.Add(newDevice);
			CollapseChild(newDevice);

			FiresecManager.FiresecConfiguration.DeviceConfiguration.Update();
			ServiceFactory.SaveService.DevicesChanged = true;
			UpdateGuardVisibility();
		}

		public static void UpdateGuardVisibility()
		{
			var hasSecurityDevice = FiresecManager.Devices.Any(x => x.Driver.DeviceType == DeviceType.Sequrity);
			ServiceFactory.Events.GetEvent<GuardVisibilityChangedEvent>().Publish(hasSecurityDevice);
		}
	}
}
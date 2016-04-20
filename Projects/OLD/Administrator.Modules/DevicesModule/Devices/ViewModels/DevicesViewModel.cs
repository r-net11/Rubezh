using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using DevicesModule.DeviceProperties;
using DevicesModule.Plans;
using DevicesModule.Plans.Designer;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrastructure.ViewModels;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using KeyboardKey = System.Windows.Input.Key;

namespace DevicesModule.ViewModels
{ 
	public class DevicesViewModel : MenuViewPartViewModel, ISelectable<Guid>
	{
		public static DevicesViewModel Current { get; private set; }
		public DeviceCommandsViewModel DeviceCommandsViewModel { get; private set; }
		//public FS2DeviceCommandsViewModel FS2DeviceCommandsViewModel { get; private set; }
		public PropertiesViewModel PropMenu { get; private set; }
		private bool _lockSelection;

		public DevicesViewModel()
		{
			_lockSelection = false;
			Current = this;
			CopyCommand = new RelayCommand(OnCopy, CanCutCopy);
			CutCommand = new RelayCommand(OnCut, CanCutCopy);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
			PasteAsCommand = new RelayCommand(OnPasteAs, CanPasteAs);
			DeviceCommandsViewModel = new DeviceCommandsViewModel(this);
			//FS2DeviceCommandsViewModel = new FS2DeviceCommandsViewModel(this);
			Menu = new DevicesMenuViewModel(this);
			PropMenu = new PropertiesViewModel(this);
			RegisterShortcuts();
			IsRightPanelEnabled = true;
			IsRightPanelVisible = true;
			SubscribeEvents();
			SetRibbonItems();
		}

		public void Initialize()
		{
			BuildTree();
			if (RootDevice != null)
			{
				RootDevice.IsExpanded = true;
				SelectedDevice = RootDevice;
			}
			OnPropertyChanged(() => RootDevices);
			UpdateGuardVisibility();
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
					//deviceViewModel.IsSelected = true;
				}
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
				OnPropertyChanged(() => SelectedDevice);
				UpdateRibbonItems();
				if (!_lockSelection && _selectedDevice != null && _selectedDevice.Device.PlanElementUIDs.Count > 0)
					ServiceFactory.Events.GetEvent<FindElementEvent>().Publish(_selectedDevice.Device.PlanElementUIDs);
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

		Device _deviceToCopy;
		bool _planUpdateRequired;
		public RelayCommand CopyCommand { get; private set; }
		void OnCopy()
		{
			_deviceToCopy = FiresecManager.FiresecConfiguration.CopyDevice(SelectedDevice.Device, false);
			_planUpdateRequired = false;
		}

		public RelayCommand CutCommand { get; private set; }
		void OnCut()
		{
			_deviceToCopy = FiresecManager.FiresecConfiguration.CopyDevice(SelectedDevice.Device, true);
			SelectedDevice.RemoveCommand.Execute();

			FiresecManager.FiresecConfiguration.DeviceConfiguration.Update();
			ServiceFactory.SaveService.FSChanged = true;
			UpdateGuardVisibility();
			_planUpdateRequired = true;
		}
		bool CanCutCopy()
		{
			return !(SelectedDevice == null || SelectedDevice.Parent == null ||
				SelectedDevice.Driver.IsAutoCreate || SelectedDevice.Parent.Driver.AutoChild == SelectedDevice.Driver.UID
				|| FiresecManager.FiresecConfiguration.IsChildMPT(SelectedDevice.Device));
		}

		public RelayCommand PasteCommand { get; private set; }
		void OnPaste()
		{
			var pasteDevice = FiresecManager.FiresecConfiguration.CopyDevice(_deviceToCopy, false);
			if (CanDoPaste(SelectedDevice))
				PasteDevice(pasteDevice, SelectedDevice);
			else if (SelectedDevice != null && CanDoPaste(SelectedDevice.Parent))
				PasteDevice(pasteDevice, SelectedDevice.Parent);
		}
		bool CanPaste()
		{
			return CanDoPaste(SelectedDevice) || (SelectedDevice != null && CanDoPaste(SelectedDevice.Parent));
		}
		bool CanDoPaste(DeviceViewModel deviceViewModel)
		{
			return (deviceViewModel != null && _deviceToCopy != null && deviceViewModel.Driver.AvaliableChildren.Contains(_deviceToCopy.Driver.UID) && !deviceViewModel.Driver.IsChildAddressReservedRange && !FiresecManager.FiresecConfiguration.IsChildMPT(deviceViewModel.Device));
		}

		public RelayCommand PasteAsCommand { get; private set; }
		void OnPasteAs()
		{
			var pasteAsViewModel = new PasteAsViewModel(SelectedDevice.Driver.DriverType);
			if (DialogService.ShowModalWindow(pasteAsViewModel))
			{
				var pasteDevice = FiresecManager.FiresecConfiguration.CopyDevice(_deviceToCopy, false);
				pasteDevice.DriverUID = pasteAsViewModel.SelectedDriver.UID;
				pasteDevice.Driver = pasteAsViewModel.SelectedDriver;
				FiresecManager.FiresecConfiguration.SynchronizeChildern(pasteDevice);
				PasteDevice(pasteDevice);
			}
		}

		bool CanPasteAs()
		{
			return (SelectedDevice != null && _deviceToCopy != null &&
				(DriversHelper.PanelDrivers.Contains(_deviceToCopy.Driver.DriverType) || DriversHelper.UsbPanelDrivers.Contains(_deviceToCopy.Driver.DriverType)) &&
				(SelectedDevice.Driver.DriverType == DriverType.Computer || SelectedDevice.Driver.DriverType == DriverType.USB_Channel_1 || SelectedDevice.Driver.DriverType == DriverType.USB_Channel_2));
		}

		void PasteDevice(Device device, DeviceViewModel deviceViewModel = null)
		{
			if (deviceViewModel == null)
				deviceViewModel = SelectedDevice;
			deviceViewModel.Device.Children.Add(device);
			device.Parent = deviceViewModel.Device;

			var newDevice = AddDeviceInternal(device, deviceViewModel);
			newDevice.CollapseChildren();

			FiresecManager.FiresecConfiguration.DeviceConfiguration.Update();
			ServiceFactory.SaveService.FSChanged = true;
			UpdateGuardVisibility();
			FillAllDevices();
			UpdatePlans(device);
		}

		void UpdatePlans(Device device)
		{
			Helper.BuildMap();
			if (_planUpdateRequired)
			{
				CopyPlanUIDs(device, _deviceToCopy);
				var uids = new List<Guid>();
				UpdatePlans(device, uids);
				PlanExtension.Invalidate(uids);
				_planUpdateRequired = false;
			}
		}
		void CopyPlanUIDs(Device device, Device originalPanelDevice)
		{
			device.PlanElementUIDs = originalPanelDevice.PlanElementUIDs;
			for (int i = 0; i < device.Children.Count; i++)
			{
				var newDevice = device.Children[i];
				var originalDevice = originalPanelDevice.Children.FirstOrDefault(x => x.IntAddress == newDevice.IntAddress);
				if (originalDevice != null)
				{
					CopyPlanUIDs(newDevice, originalDevice);
				}
			}
		}
		void UpdatePlans(Device device, List<Guid> uids)
		{
			foreach (var uid in device.PlanElementUIDs)
				foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
					foreach (var elementDevice in plan.ElementDevices)
						if (uid == elementDevice.UID)
						{
							elementDevice.DeviceUID = device.UID;
							uids.Add(uid);
						}
			foreach (var child in device.Children)
				UpdatePlans(child, uids);
		}
		public static void UpdateGuardVisibility()
		{
			var hasSecurityDevice = FiresecManager.Devices.Any(x => x.Driver.DeviceType == DeviceType.Sequrity);
			ServiceFactory.Events.GetEvent<GuardVisibilityChangedEvent>().Publish(hasSecurityDevice);
		}

		void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.J, ModifierKeys.Control), () => SelectedDevice = RootDevice);

			RegisterShortcut(new KeyGesture(KeyboardKey.C, ModifierKeys.Control), CopyCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.V, ModifierKeys.Control), PasteCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.X, ModifierKeys.Control), CutCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), () =>
				{
					if (SelectedDevice != null)
					{
						if (SelectedDevice.AddCommand.CanExecute(null))
							SelectedDevice.AddCommand.Execute();
					}
				});
			RegisterShortcut(new KeyGesture(KeyboardKey.M, ModifierKeys.Control), () =>
			{
				if (SelectedDevice != null)
				{
					if (SelectedDevice.AddToParentCommand.CanExecute(null))
						SelectedDevice.AddToParentCommand.Execute();
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

		void SubscribeEvents()
		{
			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Unsubscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Unsubscribe(OnElementRemoved);
			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Unsubscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementSelectedEvent>().Unsubscribe(OnElementSelected);

			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Subscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Subscribe(OnElementRemoved);
			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Subscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementSelectedEvent>().Subscribe(OnElementSelected);
		}
		void OnDeviceChanged(Guid deviceUID)
		{
			var device = AllDevices.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (device != null)
			{
				device.Update();
				// TODO: FIX IT
				if (!_lockSelection)
				{
					device.ExpandToThis();
					SelectedDevice = device;
				}
			}
		}
		void OnElementRemoved(List<ElementBase> elements)
		{
			elements.OfType<ElementDevice>().ToList().ForEach(element => Helper.ResetDevice(element));
			OnElementChanged(elements);
		}
		void OnElementChanged(List<ElementBase> elements)
		{
			Guid guid = Guid.Empty;
			_lockSelection = true;
			elements.ForEach(element =>
			{
				ElementDevice elementDevice = element as ElementDevice;
				if (elementDevice != null)
				{
					OnDeviceChanged(elementDevice.DeviceUID);
					//if (guid != Guid.Empty)
					//OnDeviceChanged(guid);
					//guid = elementDevice.DeviceUID;
				}
			});
			_lockSelection = false;
			//if (guid != Guid.Empty)
			//	OnDeviceChanged(guid);
		}
		void OnElementSelected(ElementBase element)
		{
			ElementDevice elementDevice = element as ElementDevice;
			if (elementDevice != null)
			{
				_lockSelection = true;
				Select(elementDevice.DeviceUID);
				_lockSelection = false;
			}
		}

		public bool IsFS2Enabled
		{
			//get { return FiresecManager.IsFS2Enabled; }
			get { return false; }
		}
		public bool IsFSAgentEnabled
		{
			//get { return !FiresecManager.IsFS2Enabled; }
			get { return true; }
		}

		protected override void UpdateRibbonItems()
		{
			base.UpdateRibbonItems();
			RibbonItems[0][0].Command = SelectedDevice == null ? null : SelectedDevice.AddCommand;
			RibbonItems[0][1].Command = SelectedDevice == null ? null : SelectedDevice.ShowPropertiesCommand;
			RibbonItems[0][2].Command = SelectedDevice == null ? null : SelectedDevice.RemoveCommand;
			RibbonItems[1][10].IsVisible = DeviceCommandsViewModel.IsAlternativeUSB;
		}
		void SetRibbonItems()
		{
			//if (FiresecManager.IsFS2Enabled)
			//{
			//	RibbonItems = new List<RibbonMenuItemViewModel>()
			//	{
			//		new RibbonMenuItemViewModel("????????????????????????????", new ObservableCollection<RibbonMenuItemViewModel>()
			//		{
			//			new RibbonMenuItemViewModel("????????????????", "BAdd"),
			//			new RibbonMenuItemViewModel("??????????????????????????", "BEdit"),
			//			new RibbonMenuItemViewModel("??????????????", "BDelete"),
			//			new RibbonMenuItemViewModel("????????????????????", CopyCommand, "BCopy"),
			//			new RibbonMenuItemViewModel("????????????????", CutCommand, "BCut"),
			//			new RibbonMenuItemViewModel("????????????????", PasteCommand, "BPaste"),
			//		}, "BEdit") { Order = 1 } ,
			//		new RibbonMenuItemViewModel("????????????????????", new ObservableCollection<RibbonMenuItemViewModel>()
			//		{
			//			new RibbonMenuItemViewModel("??????????????????", FS2DeviceCommandsViewModel.AutoDetectCommand, "BSearch"),
			//			new RibbonMenuItemViewModel("??????????????????", FS2DeviceCommandsViewModel.AutoDetectCommand, "BSearch"),
			//			new RibbonMenuItemViewModel("?????????????? ???????????????????????? ???? ??????????????", FS2DeviceCommandsViewModel.ReadDeviceCommand,false, "BParametersRead"),
			//			new RibbonMenuItemViewModel("???????????????????? ?? ??????????????", FS2DeviceCommandsViewModel.GetDescriptionCommand, false, "BInformation"),
			//			new RibbonMenuItemViewModel("???????????? ??????????????", FS2DeviceCommandsViewModel.GetDeviceJournalCommand, false, "BJournal"),
			//			new RibbonMenuItemViewModel("???????????????? ???????????????????????? ?? ????????????", FS2DeviceCommandsViewModel.WriteDeviceCommand, false, "BParametersWrite") { IsNewGroup = true },
			//			new RibbonMenuItemViewModel("???????????????? ???????????????????????? ???? ?????? ??????????????", FS2DeviceCommandsViewModel.WriteAllDeviceCommand , "BParametersWriteAll"),
			//			new RibbonMenuItemViewModel("???????????????????????????????? ???????? ?????????????? ?? ????????????????????", FS2DeviceCommandsViewModel.SynchronizeDeviceCommand, false, "BWatch")  { IsNewGroup = true },
			//			new RibbonMenuItemViewModel("???????????????????? ????", FS2DeviceCommandsViewModel.UpdateSoftCommand, false, "BParametersSync"),
			//			new RibbonMenuItemViewModel("???????????? ????????????", FS2DeviceCommandsViewModel.SetPasswordCommand, false, "BPassword"),
			//			new RibbonMenuItemViewModel("??????????????????", FS2DeviceCommandsViewModel.BindMsCommand, "BLink"),
			//			new RibbonMenuItemViewModel("???????????? ??????????????", FS2DeviceCommandsViewModel.ExecuteCustomAdminFunctionsCommand, false, "BSettings"),
			//			new RibbonMenuItemViewModel("USB", new ObservableCollection<RibbonMenuItemViewModel>()
			//			{
			//				new RibbonMenuItemViewModel("?????????????? ???????????????????????? ???? ??????????????", FS2DeviceCommandsViewModel.ReadDeviceCommand, true, "BParametersRead"),
			//				new RibbonMenuItemViewModel("???????????????????? ?? ??????????????", FS2DeviceCommandsViewModel.GetDescriptionCommand, true, "BInformation"),
			//				new RibbonMenuItemViewModel("???????????? ??????????????", FS2DeviceCommandsViewModel.GetDeviceJournalCommand, true, "BJournal"),
			//				new RibbonMenuItemViewModel("???????????????? ???????????????????????? ?? ????????????", FS2DeviceCommandsViewModel.WriteDeviceCommand, true, "BParametersWrite") { IsNewGroup = true },
			//				new RibbonMenuItemViewModel("???????????????????????????????? ???????? ?????????????? ?? ????????????????????", FS2DeviceCommandsViewModel.SynchronizeDeviceCommand, true, "BWatch") { IsNewGroup = true },
			//				new RibbonMenuItemViewModel("???????????????????? ????", FS2DeviceCommandsViewModel.UpdateSoftCommand, true, "BParametersSync"),
			//				new RibbonMenuItemViewModel("???????????? ????????????", FS2DeviceCommandsViewModel.SetPasswordCommand, true, "BPassword"),
			//				new RibbonMenuItemViewModel("???????????? ??????????????", FS2DeviceCommandsViewModel.ExecuteCustomAdminFunctionsCommand, true, "BInformation"),
			//			}, "BUsb") { IsNewGroup = true },
			//			new RibbonMenuItemViewModel("?????????????? ???????????? ?????????????? ???? ??????????", FS2DeviceCommandsViewModel.ReadJournalFromFileCommand, "BJournal"),
			//		}, "BDevice") { Order = 2 }
			//	};
			//}
			//else
			{
				RibbonItems = new List<RibbonMenuItemViewModel>()
				{
					new RibbonMenuItemViewModel("????????????????????????????", new ObservableCollection<RibbonMenuItemViewModel>()
					{
						new RibbonMenuItemViewModel("????????????????", "BAdd"),
						new RibbonMenuItemViewModel("??????????????????????????", "BEdit"),
						new RibbonMenuItemViewModel("??????????????", "BDelete"),
						new RibbonMenuItemViewModel("????????????????????", CopyCommand, "BCopy"),
						new RibbonMenuItemViewModel("????????????????", CutCommand, "BCut"),
						new RibbonMenuItemViewModel("????????????????", PasteCommand, "BPaste"),
					}, "BEdit") { Order = 1 } ,
					new RibbonMenuItemViewModel("????????????????????", new ObservableCollection<RibbonMenuItemViewModel>()
					{
						new RibbonMenuItemViewModel("??????????????????", DeviceCommandsViewModel.AutoDetectCommand, "BSearch"),
						new RibbonMenuItemViewModel("?????????????? ???????????????????????? ???? ??????????????", DeviceCommandsViewModel.ReadDeviceCommand,false, "BParametersRead"),
						new RibbonMenuItemViewModel("???????????????????? ?? ??????????????", DeviceCommandsViewModel.GetDescriptionCommand, false, "BInformation"),
						new RibbonMenuItemViewModel("???????????? ??????????????", DeviceCommandsViewModel.GetDeviceJournalCommand, false, "BJournal"),
						new RibbonMenuItemViewModel("???????????????? ???????????????????????? ?? ????????????", DeviceCommandsViewModel.WriteDeviceCommand, false, "BParametersWrite") { IsNewGroup = true },
						new RibbonMenuItemViewModel("???????????????? ???????????????????????? ???? ?????? ??????????????", DeviceCommandsViewModel.WriteAllDeviceCommand , "BParametersWriteAll"),
						new RibbonMenuItemViewModel("???????????????????????????????? ???????? ?????????????? ?? ????????????????????", DeviceCommandsViewModel.SynchronizeDeviceCommand, false, "BWatch")  { IsNewGroup = true },
						new RibbonMenuItemViewModel("???????????????????? ????", DeviceCommandsViewModel.UpdateSoftCommand, false, "BParametersSync"),
						new RibbonMenuItemViewModel("???????????? ????????????", DeviceCommandsViewModel.SetPasswordCommand, false, "BPassword"),
						new RibbonMenuItemViewModel("??????????????????", DeviceCommandsViewModel.BindMsCommand, "BLink"),
						new RibbonMenuItemViewModel("???????????? ??????????????", DeviceCommandsViewModel.ExecuteCustomAdminFunctionsCommand, false, "BSettings"),
						new RibbonMenuItemViewModel("USB", new ObservableCollection<RibbonMenuItemViewModel>()
						{
							new RibbonMenuItemViewModel("?????????????? ???????????????????????? ???? ??????????????", DeviceCommandsViewModel.ReadDeviceCommand, true, "BParametersRead"),
							new RibbonMenuItemViewModel("???????????????????? ?? ??????????????", DeviceCommandsViewModel.GetDescriptionCommand, true, "BInformation"),
							new RibbonMenuItemViewModel("???????????? ??????????????", DeviceCommandsViewModel.GetDeviceJournalCommand, true, "BJournal"),
							new RibbonMenuItemViewModel("???????????????? ???????????????????????? ?? ????????????", DeviceCommandsViewModel.WriteDeviceCommand, true, "BParametersWrite") { IsNewGroup = true },
							new RibbonMenuItemViewModel("???????????????????????????????? ???????? ?????????????? ?? ????????????????????", DeviceCommandsViewModel.SynchronizeDeviceCommand, true, "BWatch") { IsNewGroup = true },
							new RibbonMenuItemViewModel("???????????????????? ????", DeviceCommandsViewModel.UpdateSoftCommand, true, "BParametersSync"),
							new RibbonMenuItemViewModel("???????????? ????????????", DeviceCommandsViewModel.SetPasswordCommand, true, "BPassword"),
							new RibbonMenuItemViewModel("???????????? ??????????????", DeviceCommandsViewModel.ExecuteCustomAdminFunctionsCommand, true, "BInformation"),
						}, "BUsb") { IsNewGroup = true },
						new RibbonMenuItemViewModel("???????????????????????????? ???? Firesec-1", DeviceCommandsViewModel.ConvertCommand, false, "BSettings"),
					}, "BDevice") { Order = 2 }
				};
			}
		}
	}
}
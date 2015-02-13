using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Input;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecClient;
using GKModule.Models;
using GKModule.Plans;
using GKModule.Plans.Designer;
using GKProcessor;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using Microsoft.Win32;
using KeyboardKey = System.Windows.Input.Key;
using System.Xml.Serialization;
using System.Diagnostics;

namespace GKModule.ViewModels
{
	public class DevicesViewModel : MenuViewPartViewModel, ISelectable<Guid>
	{
		public static DevicesViewModel Current { get; private set; }
		public DeviceCommandsViewModel DeviceCommandsViewModel { get; private set; }
		private bool _lockSelection;

		public DevicesViewModel()
		{
			_lockSelection = false;
			Menu = new DevicesMenuViewModel(this);
			Current = this;
			CopyCommand = new RelayCommand(OnCopy, CanCutCopy);
			CutCommand = new RelayCommand(OnCut, CanCutCopy);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
			DeviceCommandsViewModel = new DeviceCommandsViewModel(this);
			ReadJournalFromFileCommand = new RelayCommand(OnReadJournalFromFile);
			RegisterShortcuts();
			IsRightPanelEnabled = true;
			SubscribeEvents();
			SetRibbonItems();
			DevicesToCopy = new List<GKDevice>();
		}

		protected override bool IsRightPanelVisibleByDefault
		{
			get { return true; }
		}

		public void Initialize()
		{
			BuildTree();
			if (RootDevice != null)
			{
				RootDevice.IsExpanded = true;
				SelectedDevice = RootDevice;
				foreach (var child in RootDevice.Children)
				{
					if (child.Driver.DriverType == GKDriverType.GK)
						child.IsExpanded = true;
				}
			}

			foreach (var device in AllDevices)
			{
				if (device.Device.Driver.DriverType == GKDriverType.KAU || device.Device.Driver.DriverType == GKDriverType.RSR2_KAU)
					device.ExpandToThis();
			}

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
				FillAllDevices();
				var deviceViewModel = AllDevices.FirstOrDefault(x => x.Device.UID == deviceUID);
				if (deviceViewModel != null)
					deviceViewModel.ExpandToThis();
				SelectedDevice = deviceViewModel;
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
			RootDevice = AddDeviceInternal(GKManager.DeviceConfiguration.RootDevice, null);
			FillAllDevices();
		}

		public DeviceViewModel AddDevice(GKDevice device, DeviceViewModel parentDeviceViewModel)
		{
			var deviceViewModel = AddDeviceInternal(device, parentDeviceViewModel);
			FillAllDevices();
			return deviceViewModel;
		}
		private DeviceViewModel AddDeviceInternal(GKDevice device, DeviceViewModel parentDeviceViewModel)
		{
			var deviceViewModel = new DeviceViewModel(device);
			if (parentDeviceViewModel != null)
				parentDeviceViewModel.AddChild(deviceViewModel);

			foreach (var childDevice in device.Children)
				AddDeviceInternal(childDevice, deviceViewModel);
			return deviceViewModel;
		}

		#region CopyPaste
		public List<GKDevice> DevicesToCopy { get; set; }

		bool CanCutCopy()
		{
			return !(SelectedDevice == null || SelectedDevice.Parent == null ||
				SelectedDevice.Driver.IsAutoCreate || SelectedDevice.Parent.Driver.IsGroupDevice);
		}

		public RelayCommand CopyCommand { get; private set; }
		void OnCopy()
		{
			DevicesToCopy = new List<GKDevice>() { GKManager.CopyDevice(SelectedDevice.Device, false) };
		}

		public RelayCommand CutCommand { get; private set; }
		void OnCut()
		{
			DevicesToCopy = new List<GKDevice>() { GKManager.CopyDevice(SelectedDevice.Device, true) };
			SelectedDevice.RemoveCommand.Execute();

			GKManager.DeviceConfiguration.Update();
			ServiceFactory.SaveService.GKChanged = true;
		}

		public RelayCommand PasteCommand { get; private set; }
		void OnPaste()
		{
			using (new WaitWrapper())
			{
				using (var cache = new ElementDeviceUpdater())
				{
					foreach (var deviceToCopy in DevicesToCopy)
					{
						var pasteDevice = GKManager.CopyDevice(deviceToCopy, true);
						var device = PasteDevice(pasteDevice);
						device.UID = pasteDevice.UID;
						if (device != null)
							cache.UpdateDeviceBinding(device);
					}
					if (SelectedDevice.Device.IsConnectedToKAURSR2OrIsKAURSR2)
					{
						GKManager.RebuildRSR2Addresses(SelectedDevice.Device.KAURSR2Parent);
						GKManager.UpdateConfiguration();
					}
				}
				GKManager.DeviceConfiguration.Update();
				GKPlanExtension.Instance.Cache.BuildSafe<GKDevice>();
				GKPlanExtension.Instance.InvalidateCanvas();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}
		bool CanPaste()
		{
			if (DevicesToCopy.Count > 0 && SelectedDevice != null)
			{
				if (SelectedDevice.Device.DriverType == GKDriverType.RSR2_KAU || SelectedDevice.Device.DriverType == GKDriverType.KAUIndicator)
					return false;

				if (SelectedDevice.Device.IsConnectedToKAURSR2OrIsKAURSR2)
				{
					if (SelectedDevice.Parent == null)
						return false;
					foreach (var deviceToCopy in DevicesToCopy)
					{
						if (!SelectedDevice.Driver.Children.Contains(deviceToCopy.DriverType) && !SelectedDevice.Parent.Driver.Children.Contains(deviceToCopy.DriverType))
							return false;
					}
					return true;
				}
				else
				{
					foreach (var deviceToCopy in DevicesToCopy)
					{
						if (!SelectedDevice.Driver.Children.Contains(deviceToCopy.DriverType))
							return false;
					}
					return true;
				}
			}
			return false;
		}

		GKDevice PasteDevice(GKDevice device)
		{
			if (SelectedDevice.Device.DriverType == GKDriverType.RSR2_KAU || SelectedDevice.Device.DriverType == GKDriverType.KAUIndicator)
				return null;
			if (SelectedDevice.Device.IsConnectedToKAURSR2OrIsKAURSR2)
			{
				var kauDeviceShleifdevice = SelectedDevice.Device.KAURSR2ShleifParent;
				int maxAddress = NewDeviceHelper.GetMinAddress(device.Driver, kauDeviceShleifdevice);
				if (maxAddress >= 255)
					return null;

				if (SelectedDevice.Device.DriverType == GKDriverType.RSR2_KAU_Shleif)
				{
					var addedDevice = GKManager.AddChild(SelectedDevice.Device, null, device.Driver, (byte)(maxAddress));
					GKManager.CopyDevice(device, addedDevice);
					addedDevice.IntAddress = (byte)(maxAddress);
					var addedDeviceViewModel = NewDeviceHelper.AddDevice(addedDevice, SelectedDevice, false);
					AllDevices.Add(addedDeviceViewModel);
					return addedDevice;
				}
				else
				{
					var addedDevice = GKManager.AddChild(SelectedDevice.Parent.Device, SelectedDevice.Device, device.Driver, (byte)(maxAddress));
					GKManager.CopyDevice(device, addedDevice);
					addedDevice.IntAddress = (byte)(maxAddress);
					var addedDeviceViewModel = NewDeviceHelper.InsertDevice(addedDevice, SelectedDevice);
					AllDevices.Add(addedDeviceViewModel);
					return addedDevice;
				}
			}
			else
			{
				SelectedDevice.Device.Children.Add(device);
				device.Parent = SelectedDevice.Device;
				AddDevice(device, SelectedDevice);
				return device;
			}
		}
		#endregion

		public RelayCommand ReadJournalFromFileCommand { get; private set; }
		void OnReadJournalFromFile()
		{
			var openDialog = new OpenFileDialog()
			{
				Filter = "Журнал событий|*.fscj",
				DefaultExt = "Журнал событий|*.fscj"
			};
			if (openDialog.ShowDialog().Value)
			{
				using (var fileStream = new FileStream(openDialog.FileName, FileMode.Open, FileAccess.Read))
				{
					var xmlSerializer = new XmlSerializer(typeof(JournalItemsCollection));
					var journalItemsCollection = (JournalItemsCollection)xmlSerializer.Deserialize(fileStream);
					if (journalItemsCollection != null)
					{
						DialogService.ShowModalWindow(new JournalFromFileViewModel(journalItemsCollection));
					}
				}
			}
		}

		private void RegisterShortcuts()
		{
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
			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Unsubscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Unsubscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Unsubscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementSelectedEvent>().Unsubscribe(OnElementSelected);

			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Subscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Subscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Subscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementSelectedEvent>().Subscribe(OnElementSelected);
		}
		private void OnDeviceChanged(Guid deviceUID)
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
		private void OnElementChanged(List<ElementBase> elements)
		{
			_lockSelection = true;
			elements.ForEach(element =>
			{
				ElementGKDevice elementDevice = element as ElementGKDevice;
				if (elementDevice != null)
					OnDeviceChanged(elementDevice.DeviceUID);
			});
			_lockSelection = false;
		}
		private void OnElementSelected(ElementBase element)
		{
			ElementGKDevice elementXDevice = element as ElementGKDevice;
			if (elementXDevice != null)
			{
				_lockSelection = true;
				Select(elementXDevice.DeviceUID);
				_lockSelection = false;
			}
		}

		public override void OnShow()
		{
			base.OnShow();
		}
		public override void OnHide()
		{
			base.OnHide();
		}

		protected override void UpdateRibbonItems()
		{
			base.UpdateRibbonItems();
			RibbonItems[0][0].Command = SelectedDevice == null ? null : SelectedDevice.AddCommand;
			RibbonItems[0][1].Command = SelectedDevice == null ? null : SelectedDevice.ShowPropertiesCommand;
			RibbonItems[0][2].Command = SelectedDevice == null ? null : SelectedDevice.RemoveCommand;

			RibbonItems[1][9][0].Command = SelectedDevice == null ? null : SelectedDevice.ReadCommand;
			RibbonItems[1][9][1].Command = SelectedDevice == null ? null : SelectedDevice.WriteCommand;
			RibbonItems[1][9][2].Command = SelectedDevice == null ? null : SelectedDevice.ReadAllCommand;
			RibbonItems[1][9][3].Command = SelectedDevice == null ? null : SelectedDevice.WriteAllCommand;
			RibbonItems[1][9][4].Command = SelectedDevice == null ? null : SelectedDevice.SyncFromDeviceToSystemCommand;
			RibbonItems[1][9][5].Command = SelectedDevice == null ? null : SelectedDevice.SyncFromAllDeviceToSystemCommand;
			RibbonItems[1][9][6].Command = SelectedDevice == null ? null : SelectedDevice.CopyParamCommand;
			RibbonItems[1][9][7].Command = SelectedDevice == null ? null : SelectedDevice.PasteParamCommand;
			RibbonItems[1][9][8].Command = SelectedDevice == null ? null : SelectedDevice.PasteAllParamCommand;
			RibbonItems[1][9][9].Command = SelectedDevice == null ? null : SelectedDevice.PasteTemplateCommand;
			RibbonItems[1][9][10].Command = SelectedDevice == null ? null : SelectedDevice.PasteAllTemplateCommand;
		}
		private void SetRibbonItems()
		{
			RibbonItems = new List<RibbonMenuItemViewModel>()
			{
				new RibbonMenuItemViewModel("Редактирование", new ObservableCollection<RibbonMenuItemViewModel>()
				{
					new RibbonMenuItemViewModel("Добавить", "BAdd"),
					new RibbonMenuItemViewModel("Редактировать", "BEdit"),
					new RibbonMenuItemViewModel("Удалить", "BDelete"),
					new RibbonMenuItemViewModel("Копировать", CopyCommand, "BCopy"),
					new RibbonMenuItemViewModel("Вырезать", CutCommand, "BCut"),
					new RibbonMenuItemViewModel("Вставить", PasteCommand, "BPaste"),
				}, "BEdit") { Order = 1 } ,
				new RibbonMenuItemViewModel("Устройство", new ObservableCollection<RibbonMenuItemViewModel>()
				{
					new RibbonMenuItemViewModel("Считать конфигурацию", DeviceCommandsViewModel.ReadConfigFileCommand, "BParametersReadFromFile"),
					new RibbonMenuItemViewModel("Считать конфигурацию дескрипторов", DeviceCommandsViewModel.ReadConfigurationCommand, "BParametersRead"),
					new RibbonMenuItemViewModel("Записать конфигурацию", DeviceCommandsViewModel.WriteConfigCommand, "BParametersWrite"),
					new RibbonMenuItemViewModel("Информация об устройстве", DeviceCommandsViewModel.ShowInfoCommand, "BInformation") { IsNewGroup = true },
					new RibbonMenuItemViewModel("Синхронизация времени", DeviceCommandsViewModel.SynchroniseTimeCommand, "BWatch"),
					new RibbonMenuItemViewModel("Журнал событий", DeviceCommandsViewModel.ReadJournalCommand, "BJournal"),
					new RibbonMenuItemViewModel("Обновление ПО", DeviceCommandsViewModel.UpdateFirmwhareCommand, "BParametersSync"),
					new RibbonMenuItemViewModel("Автопоиск устройств", DeviceCommandsViewModel.AutoSearchCommand, "BSearch"),
					new RibbonMenuItemViewModel("Актуализация пользователей прибора", DeviceCommandsViewModel.ActualizeUsersCommand, "BSettings"),
					new RibbonMenuItemViewModel("Параметры", new ObservableCollection<RibbonMenuItemViewModel>
					{
						new RibbonMenuItemViewModel("Считать параметры", "BParametersRead"),
						new RibbonMenuItemViewModel("Записать параметры", "BParametersWrite"),
						new RibbonMenuItemViewModel("Считать параметры дочерних устройств", "BParametersReadAll"),
						new RibbonMenuItemViewModel("Записать параметры дочерних устройств", "BParametersWriteAll"),

						new RibbonMenuItemViewModel("Копировать параметры из устройства в систему", "BLeft"),
						new RibbonMenuItemViewModel("Копировать параметры из всех дочерних устройств в систему", "BLeftLeft"),
						
						new RibbonMenuItemViewModel("Копировать параметры", "BCopy"),
						new RibbonMenuItemViewModel("Вставить параметры", "BPaste"),
						new RibbonMenuItemViewModel("Вставить параметры во все дочерние устройства", "BPasteAll"),
						
						new RibbonMenuItemViewModel("Применить шаблон", "BBriefcase"),
						new RibbonMenuItemViewModel("Применить шаблон ко всем дочерним устройствам", "BBriefcaseAll"),
					}, "BParametersReadWrite"),
					new RibbonMenuItemViewModel("Считать журнал событий из файла", ReadJournalFromFileCommand, "BJournal"),
				}, "BDevice") { Order = 2 }
			};
		}
	}
}
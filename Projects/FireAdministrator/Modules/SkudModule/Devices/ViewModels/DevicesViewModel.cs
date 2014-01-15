using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using FiresecAPI;
using Infrustructure.Plans.Events;
using FiresecClient;
using Infrastructure;
using System.Windows.Input;
using System.IO;
using Infrastructure.Common;
using Microsoft.Win32;
using Infrastructure.Common.Windows;
using KeyboardKey = System.Windows.Input.Key;
using Infrustructure.Plans.Elements;
using Infrastructure.Common.Ribbon;
using System.Collections.ObjectModel;
using XFiresecAPI;

namespace SkudModule.ViewModels
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
			DevicesToCopy = new List<SKDDevice>();
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
					if (child.Driver.DriverType == SKDDriverType.Controller)
						child.IsExpanded = true;
				}
			}

			foreach (var device in AllDevices)
			{
				if (device.Device.Driver.DriverType == SKDDriverType.Controller)
					device.ExpandToThis();
			}

			OnPropertyChanged("RootDevices");
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
				OnPropertyChanged("RootDevice");
			}
		}

		public DeviceViewModel[] RootDevices
		{
			get { return new DeviceViewModel[] { RootDevice }; }
		}

		void BuildTree()
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
		private DeviceViewModel AddDeviceInternal(SKDDevice device, DeviceViewModel parentDeviceViewModel)
		{
			var deviceViewModel = new DeviceViewModel(device);
			if (parentDeviceViewModel != null)
				parentDeviceViewModel.AddChild(deviceViewModel);

			foreach (var childDevice in device.Children)
				AddDeviceInternal(childDevice, deviceViewModel);
			return deviceViewModel;
		}

		#region CopyPaste
		public List<SKDDevice> DevicesToCopy { get; set; }

		bool CanCutCopy()
		{
			return !(SelectedDevice == null || SelectedDevice.Parent == null ||
				SelectedDevice.Driver.IsAutoCreate || SelectedDevice.Parent.Driver.IsGroupDevice);
		}

		public RelayCommand CopyCommand { get; private set; }
		void OnCopy()
		{
			//DevicesToCopy = new List<SKDDevice>() { XManager.CopyDevice(SelectedDevice.Device, false) };
		}

		public RelayCommand CutCommand { get; private set; }
		void OnCut()
		{
			//DevicesToCopy = new List<SKDDevice>() { XManager.CopyDevice(SelectedDevice.Device, true) };
			SelectedDevice.RemoveCommand.Execute();

			XManager.DeviceConfiguration.Update();
			ServiceFactory.SaveService.GKChanged = true;
		}

		public RelayCommand PasteCommand { get; private set; }
		void OnPaste()
		{
			foreach (var deviceToCopy in DevicesToCopy)
			{
				//var pasteDevice = XManager.CopyDevice(deviceToCopy, false);
				//PasteDevice(pasteDevice);
			}
		}
		bool CanPaste()
		{
			if (DevicesToCopy.Count > 0 && SelectedDevice != null)
			{
				foreach (var deviceToCopy in DevicesToCopy)
				{
					if (!SelectedDevice.Driver.Children.Contains(deviceToCopy.DriverType))
						return false;
				}
				return true;
			}
			return false;
		}

		void PasteDevice(SKDDevice device)
		{
			SelectedDevice.Device.Children.Add(device);
			device.Parent = SelectedDevice.Device;
			AddDevice(device, SelectedDevice);

			XManager.DeviceConfiguration.Update();
			ServiceFactory.SaveService.GKChanged = true;
		}
		#endregion

		public RelayCommand ReadJournalFromFileCommand { get; private set; }
		void OnReadJournalFromFile()
		{
			var openDialog = new OpenFileDialog()
			{
				Filter = "Журнал событий Firesec|*.fscj",
				DefaultExt = "Журнал событий Firesec|*.fscj"
			};
			if (openDialog.ShowDialog().Value)
			{
				using (var fileStream = new FileStream(openDialog.FileName, FileMode.Open, FileAccess.Read))
				{
					//var dataContractSerializer = new DataContractSerializer(typeof(JournalItemsCollection));
					//var journalItemsCollection = (JournalItemsCollection)dataContractSerializer.ReadObject(fileStream);
					//if (journalItemsCollection != null)
					//{
					//    DialogService.ShowModalWindow(new JournalFromFileViewModel(journalItemsCollection));
					//}
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
			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Unsubscribe(OnElementRemoved);
			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Subscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementSelectedEvent>().Unsubscribe(OnElementSelected);

			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Subscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Subscribe(OnElementRemoved);
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
		private void OnElementRemoved(List<ElementBase> elements)
		{
			//elements.OfType<ElementXDevice>().ToList().ForEach(element => Helper.ResetXDevice(element));
			OnElementChanged(elements);
		}
		private void OnElementChanged(List<ElementBase> elements)
		{
			_lockSelection = true;
			elements.ForEach(element =>
			{
				//ElementXDevice elementDevice = element as ElementXDevice;
				//if (elementDevice != null)
				//    OnDeviceChanged(elementDevice.XDeviceUID);
			});
			_lockSelection = false;
		}
		private void OnElementSelected(ElementBase element)
		{
			//ElementXDevice elementXDevice = element as ElementXDevice;
			//if (elementXDevice != null)
			//{
			//    _lockSelection = true;
			//    Select(elementXDevice.XDeviceUID);
			//    _lockSelection = false;
			//}
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

			//RibbonItems[1][6][0].Command = SelectedDevice == null ? null : SelectedDevice.ReadCommand;
			//RibbonItems[1][6][1].Command = SelectedDevice == null ? null : SelectedDevice.WriteCommand;
			//RibbonItems[1][6][2].Command = SelectedDevice == null ? null : SelectedDevice.ReadAllCommand;
			//RibbonItems[1][6][3].Command = SelectedDevice == null ? null : SelectedDevice.WriteAllCommand;
			//RibbonItems[1][6][4].Command = SelectedDevice == null ? null : SelectedDevice.SyncFromDeviceToSystemCommand;
			//RibbonItems[1][6][5].Command = SelectedDevice == null ? null : SelectedDevice.SyncFromAllDeviceToSystemCommand;
			//RibbonItems[1][6][6].Command = SelectedDevice == null ? null : SelectedDevice.CopyParamCommand;
			//RibbonItems[1][6][7].Command = SelectedDevice == null ? null : SelectedDevice.PasteParamCommand;
			//RibbonItems[1][6][8].Command = SelectedDevice == null ? null : SelectedDevice.PasteAllParamCommand;
			//RibbonItems[1][6][9].Command = SelectedDevice == null ? null : SelectedDevice.PasteTemplateCommand;
			//RibbonItems[1][6][10].Command = SelectedDevice == null ? null : SelectedDevice.PasteAllTemplateCommand;
		}
		private void SetRibbonItems()
		{
			RibbonItems = new List<RibbonMenuItemViewModel>()
			{
				new RibbonMenuItemViewModel("Редактирование", new ObservableCollection<RibbonMenuItemViewModel>()
				{
					new RibbonMenuItemViewModel("Добавить", "/Controls;component/Images/BAdd.png"),
					new RibbonMenuItemViewModel("Редактировать", "/Controls;component/Images/BEdit.png"),
					new RibbonMenuItemViewModel("Удалить", "/Controls;component/Images/BDelete.png"),
					new RibbonMenuItemViewModel("Копировать", CopyCommand, "/Controls;component/Images/BCopy.png"),
					new RibbonMenuItemViewModel("Вырезать", CutCommand, "/Controls;component/Images/BCut.png"),
					new RibbonMenuItemViewModel("Вставить", PasteCommand, "/Controls;component/Images/BPaste.png"),
				}, "/Controls;component/Images/BEdit.png") { Order = 1 } ,
				new RibbonMenuItemViewModel("Устройство", new ObservableCollection<RibbonMenuItemViewModel>()
				{
					new RibbonMenuItemViewModel("Считать конфигурацию дескрипторов", DeviceCommandsViewModel.ReadConfigurationCommand, "/Controls;component/Images/BParametersRead.png"),
					new RibbonMenuItemViewModel("Записать конфигурацию", DeviceCommandsViewModel.WriteConfigCommand, "/Controls;component/Images/BParametersWrite.png"),
					new RibbonMenuItemViewModel("Информация об устройстве", DeviceCommandsViewModel.ShowInfoCommand, "/Controls;component/Images/BInformation.png") { IsNewGroup = true },
					new RibbonMenuItemViewModel("Синхронизация времени", DeviceCommandsViewModel.SynchroniseTimeCommand, "/Controls;component/Images/BWatch.png"),
					new RibbonMenuItemViewModel("Журнал событий", DeviceCommandsViewModel.ReadJournalCommand, "/Controls;component/Images/BJournal.png"),
					new RibbonMenuItemViewModel("Обновление ПО", DeviceCommandsViewModel.UpdateFirmwhareCommand, "/Controls;component/Images/BParametersSync.png"),
					new RibbonMenuItemViewModel("Параметры", new ObservableCollection<RibbonMenuItemViewModel>
                    {
                        new RibbonMenuItemViewModel("Считать параметры", "/Controls;component/Images/BParametersRead.png"),
                        new RibbonMenuItemViewModel("Записать параметры", "/Controls;component/Images/BParametersWrite.png"),
                        new RibbonMenuItemViewModel("Считать параметры дочерних устройств", "/Controls;component/Images/BParametersReadAll.png"),
                        new RibbonMenuItemViewModel("Записать параметры дочерних устройств", "/Controls;component/Images/BParametersWriteAll.png"),

                        new RibbonMenuItemViewModel("Копировать параметры из устройства в систему", "/Controls;component/Images/BLeft.png"),
                        new RibbonMenuItemViewModel("Копировать параметры из всех дочерних устройств в систему", "/Controls;component/Images/BLeftLeft.png"),
                        
                        new RibbonMenuItemViewModel("Копировать параметры", "/Controls;component/Images/BCopy.png"),
                        new RibbonMenuItemViewModel("Вставить параметры", "/Controls;component/Images/BPaste.png"),
                        new RibbonMenuItemViewModel("Вставить параметры во все дочерние устройства", "/Controls;component/Images/BPasteAll.png"),
                        
                        new RibbonMenuItemViewModel("Применить шаблон", "/Controls;component/Images/BBriefcase.png"),
                        new RibbonMenuItemViewModel("Применить шаблон ко всем дочерним устройствам", "/Controls;component/Images/BBriefcaseAll.png"),
                    }, "/Controls;component/Images/BParametersReadWrite.png"),
                    new RibbonMenuItemViewModel("Считать журнал событий из файла", ReadJournalFromFileCommand, "/Controls;component/Images/BJournal.png"),
				}, "/Controls;component/Images/BDevice.png") { Order = 2 }
			};
		}
	}
}
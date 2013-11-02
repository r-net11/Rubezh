using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using FiresecAPI.XModels;
using FiresecClient;
using GKProcessor;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrastructure.ViewModels;
using XFiresecAPI;
using KeyboardKey = System.Windows.Input.Key;


namespace GKModule.ViewModels
{
	public class DeviceParametersViewModel : MenuViewPartViewModel, ISelectable<Guid>
	{
		public DeviceParametersViewModel()
		{
			Menu = new DeviceParametersMenuViewModel(this);
			ReadCommand = new RelayCommand(OnRead, CanReadWrite);
			WriteCommand = new RelayCommand(OnSyncFromSystemToDevice, CanSync);
			ReadAllCommand = new RelayCommand(OnReadAll, CanReadWriteAll);
			WriteAllCommand = new RelayCommand(SyncFromAllSystemToDevice, CanSyncAll);

			CopyCommand = new RelayCommand(OnCopy, CanCopy);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
			PasteAllCommand = new RelayCommand(OnPasteAll, CanPasteAll);
			PasteTemplateCommand = new RelayCommand(OnPasteTemplate, CanPasteTemplate);
			PasteAllTemplateCommand = new RelayCommand(OnPasteAllTemplate, CanPasteAllTemplate);

			SyncFromSystemToDeviceCommand = new RelayCommand(OnSyncFromSystemToDevice, CanSync);
			SyncFromDeviceToSystemCommand = new RelayCommand(OnSyncFromDeviceToSystem, CanSync);
			SyncFromAllSystemToDeviceCommand = new RelayCommand(SyncFromAllSystemToDevice, CanSyncAll);
			SyncFromAllDeviceToSystemCommand = new RelayCommand(OnSyncFromAllDeviceToSystem, CanSyncAll);

			Invalidate();
			SetRibbonItems();
			RegisterShortcuts();
		}

		void FiresecDriverAuParametersHelper_Progress(string value, int percentsCompleted)
		{
			ProgressCaption = value;
			PercentsCompleted = percentsCompleted;
			OnPropertyChanged("ProgressCaption");
			OnPropertyChanged("PercentsCompleted");
		}

		public string ProgressCaption { get; set; }
		public int PercentsCompleted { get; set; }

		public void Initialize()
		{
			BuildTree();
			if (RootDevice != null)
			{
				RootDevice.IsExpanded = true;
				SelectedDevice = RootDevice;
			}
			OnPropertyChanged("RootDevices");

			foreach (var deviceViewModel in AllDevices)
			{
				deviceViewModel.Device.AUParametersChanged += () => { UpdateDeviceParameterMissmatch(); };
			}
			UpdateDeviceParameterMissmatch();
		}

		#region Tree
		public List<DeviceParameterViewModel> AllDevices;

		public void FillAllDevices()
		{
			AllDevices = new List<DeviceParameterViewModel>();
			AddChildPlainDevices(RootDevice);
		}

		void AddChildPlainDevices(DeviceParameterViewModel parentViewModel)
		{
			AllDevices.Add(parentViewModel);
			foreach (DeviceParameterViewModel childViewModel in parentViewModel.Children)
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

		DeviceParameterViewModel _rootDevice;
		public DeviceParameterViewModel RootDevice
		{
			get { return _rootDevice; }
			private set
			{
				_rootDevice = value;
				OnPropertyChanged("RootDevice");
			}
		}

		public DeviceParameterViewModel[] RootDevices
		{
			get { return new[] { RootDevice }; }
		}

		void BuildTree()
		{
			RootDevice = AddDeviceInternal(XManager.DeviceConfiguration.RootDevice, null);
			FillAllDevices();
		}

		public DeviceParameterViewModel AddDevice(XDevice device, DeviceParameterViewModel parentDeviceViewModel)
		{
			var deviceViewModel = AddDeviceInternal(device, parentDeviceViewModel);
			FillAllDevices();
			return deviceViewModel;
		}
		DeviceParameterViewModel AddDeviceInternal(XDevice device, DeviceParameterViewModel parentDeviceViewModel)
		{
			var deviceViewModel = new DeviceParameterViewModel(device);
			if (parentDeviceViewModel != null)
				parentDeviceViewModel.AddChild(deviceViewModel);

			foreach (var childDevice in device.Children)
				AddDeviceInternal(childDevice, deviceViewModel);
			return deviceViewModel;
		}
		#endregion

		DeviceParameterViewModel _selectedDevice;
		public DeviceParameterViewModel SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				OnPropertyChanged("SelectedDevice");

				if (value != null)
				{
					value.Update();
				}
			}
		}

		#region Read and Write
		public RelayCommand ReadCommand { get; private set; }
		void OnRead()
		{
			if (CheckNeedSave())
			{
				//SelectedDevice.Device.Properties.Clear();
				ReadDevices(new List<XDevice>() { SelectedDevice.Device });
				SelectedDevice.Update();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		bool CanReadWrite()
		{
			return SelectedDevice != null && SelectedDevice.HasAUParameters;
		}

		public RelayCommand ReadAllCommand { get; private set; }
		void OnReadAll()
		{
			if (CheckNeedSave())
			{
				var devices = GetRealChildren();
				devices.Add(SelectedDevice.Device);
				ReadDevices(devices);
				foreach (var device in devices)
				{
					var deviceViewModel = AllDevices.FirstOrDefault(x => x.Device == device);
					if (deviceViewModel != null)
					{
						deviceViewModel.Update();
					}
				}
			}
		}

		bool CanReadWriteAll()
		{
			return SelectedDevice != null && SelectedDevice.Children.Count() > 0;
		}

		bool IsPropertyValid(XProperty property, XDriverProperty driverProperty)
		{
			int value;
			return
					driverProperty != null &&
					driverProperty.IsAUParameter &&
					driverProperty.DriverPropertyType == XDriverPropertyTypeEnum.IntType &&
					(!int.TryParse(Convert.ToString(property.Value), out value) ||
					(value < driverProperty.Min || value > driverProperty.Max));
		}

		void WriteDevices(List<XDevice> devices)
		{
			ParametersHelper.ErrorLog = "";
			LoadingService.Show("Запись параметров");
			DatabaseManager.Convert();
			var i = 0;
			foreach (var device in devices)
			{
				i++;
				FiresecDriverAuParametersHelper_Progress("Запись параметров в устройство " + device.PresentationDriverAndAddress, (i * 100) / devices.Count);
				ParametersHelper.SetSingleParameter(device);
				Thread.Sleep(100);
			}
			LoadingService.Close();
			if (ParametersHelper.ErrorLog != "")
				MessageBoxService.ShowError("Ошибка при записи параметров в следующие устройства:" + ParametersHelper.ErrorLog);
			FiresecDriverAuParametersHelper_Progress("Запись параметров в устройство ", 0);
			ServiceFactory.SaveService.GKChanged = true;
		}

		void ReadDevices(List<XDevice> devices)
		{
			ParametersHelper.ErrorLog = "";
			LoadingService.Show("Запрос параметров");
			DatabaseManager.Convert();
			var i = 0;
			foreach (var device in devices)
			{
				i++;
				FiresecDriverAuParametersHelper_Progress("Чтение параметров устройства " + device.PresentationDriverAndAddress, (i * 100) / devices.Count);
				ParametersHelper.GetSingleParameter(device);
			}
			LoadingService.Close();
			if (ParametersHelper.ErrorLog != "")
				MessageBoxService.ShowError("Ошибка при получении параметров следующих устройств:" + ParametersHelper.ErrorLog);
			FiresecDriverAuParametersHelper_Progress("Чтение параметров устройства ", 0);
			ServiceFactory.SaveService.GKChanged = true;
		}
		#endregion

		#region Capy and Paste
		XDriver DriverCopy;
		List<XProperty> PropertiesCopy;

		public RelayCommand CopyCommand { get; private set; }
		void OnCopy()
		{
			DriverCopy = SelectedDevice.Device.Driver;
			PropertiesCopy = new List<XProperty>();
			foreach (var property in SelectedDevice.Device.Properties)
			{
				var driverProperty = SelectedDevice.Device.Driver.Properties.FirstOrDefault(x => x.Name == property.Name);
				if (driverProperty != null && driverProperty.IsAUParameter)
				{
					var propertyCopy = new XProperty()
					{
						StringValue = property.StringValue,
						Name = property.Name,
						Value = property.Value
					};
					PropertiesCopy.Add(propertyCopy);
				}
			}
		}
		bool CanCopy()
		{
			return SelectedDevice != null && SelectedDevice.HasAUParameters;
		}

		public RelayCommand PasteCommand { get; private set; }
		void OnPaste()
		{
			CopyParametersFromBuffer(SelectedDevice.Device);
			SelectedDevice.Update();
			UpdateDeviceParameterMissmatch();
		}
		bool CanPaste()
		{
			return SelectedDevice != null && DriverCopy != null && SelectedDevice.Device.DriverType == DriverCopy.DriverType && SelectedDevice.Children.Count() == 0;
		}

		public RelayCommand PasteAllCommand { get; private set; }
		void OnPasteAll()
		{
			foreach (var device in XManager.GetAllDeviceChildren(SelectedDevice.Device))
			{
				CopyParametersFromBuffer(device);
			}
			SelectedDevice.Update();
			UpdateDeviceParameterMissmatch();
		}
		bool CanPasteAll()
		{
			return SelectedDevice != null && SelectedDevice.Children.Count() > 0 && DriverCopy != null;
		}

		void CopyParametersFromBuffer(XDevice device)
		{
			foreach (var property in PropertiesCopy)
			{
				var deviceProperty = device.Properties.FirstOrDefault(x => x.Name == property.Name);
				if (deviceProperty != null)
				{
					deviceProperty.Value = property.Value;
				}
			}
			ServiceFactory.SaveService.GKChanged = true;
		}
		#endregion

		#region Template
		public RelayCommand PasteTemplateCommand { get; private set; }
		void OnPasteTemplate()
		{
			var parameterTemplateSelectationViewModel = new ParameterTemplateSelectationViewModel();
			if (DialogService.ShowModalWindow(parameterTemplateSelectationViewModel))
			{
				CopyParametersFromTemplate(parameterTemplateSelectationViewModel.SelectedParameterTemplate, SelectedDevice.Device);
				SelectedDevice.Update();
			}
			UpdateDeviceParameterMissmatch();
		}
		bool CanPasteTemplate()
		{
			return SelectedDevice != null && SelectedDevice.Children.Count() == 0 && SelectedDevice.HasAUParameters;
		}

		public RelayCommand PasteAllTemplateCommand { get; private set; }
		void OnPasteAllTemplate()
		{
			var parameterTemplateSelectationViewModel = new ParameterTemplateSelectationViewModel();
			if (DialogService.ShowModalWindow(parameterTemplateSelectationViewModel))
			{
				foreach (var device in XManager.GetAllDeviceChildren(SelectedDevice.Device))
				{
					CopyParametersFromTemplate(parameterTemplateSelectationViewModel.SelectedParameterTemplate, device);
				}
				SelectedDevice.Update();
			}
			UpdateDeviceParameterMissmatch();
		}
		bool CanPasteAllTemplate()
		{
			return SelectedDevice != null && SelectedDevice.Children.Count() > 0;
		}

		void CopyParametersFromTemplate(XParameterTemplate parameterTemplate, XDevice device)
		{
			var deviceParameterTemplate = parameterTemplate.DeviceParameterTemplates.FirstOrDefault(x => x.XDevice.DriverUID == device.Driver.UID);
			if (deviceParameterTemplate != null)
			{
				foreach (var property in deviceParameterTemplate.XDevice.Properties)
				{
					var deviceProperty = device.Properties.FirstOrDefault(x => x.Name == property.Name);
					if (deviceProperty != null)
					{
						deviceProperty.Value = property.Value;
					}
				}
			}
		}
		#endregion

		#region Syncronization
		public RelayCommand WriteCommand { get; private set; }
		public RelayCommand SyncFromSystemToDeviceCommand { get; private set; }
		void OnSyncFromSystemToDevice()
		{
			if (CheckNeedSave())
			{
				var devices = new List<XDevice>() { SelectedDevice.Device };
				if (Validate(devices))
				{
					CopyFromSystemToDevice(SelectedDevice.Device);
					SelectedDevice.Update();
					UpdateDeviceParameterMissmatch();
					WriteDevices(devices);
				}
			}
		}

		public RelayCommand WriteAllCommand { get; private set; }
		public RelayCommand SyncFromAllSystemToDeviceCommand { get; private set; }
		void SyncFromAllSystemToDevice()
		{
			if (CheckNeedSave())
			{
				var devices = GetRealChildren();
				devices.Add(SelectedDevice.Device);
				if (Validate(devices))
				{
					foreach (var device in devices)
					{
						CopyFromSystemToDevice(device);
						var deviceViewModel = AllDevices.FirstOrDefault(x => x.Device == device);
						if (deviceViewModel != null)
							deviceViewModel.Update();
					}
					UpdateDeviceParameterMissmatch();
					WriteDevices(devices);
				}
			}
		}

		public RelayCommand SyncFromDeviceToSystemCommand { get; private set; }
		void OnSyncFromDeviceToSystem()
		{
			if (CheckNeedSave())
			{
				CopyFromDeviceToSystem(SelectedDevice.Device);
				SelectedDevice.Update();
				UpdateDeviceParameterMissmatch();
			}
		}

		public RelayCommand SyncFromAllDeviceToSystemCommand { get; private set; }
		void OnSyncFromAllDeviceToSystem()
		{
			if (CheckNeedSave())
			{
				var devices = GetRealChildren();
				devices.Add(SelectedDevice.Device);
				foreach (var device in devices)
				{
					CopyFromDeviceToSystem(device);
					var deviceViewModel = AllDevices.FirstOrDefault(x => x.Device == device);
					if (deviceViewModel != null)
						deviceViewModel.Update();
				}
				UpdateDeviceParameterMissmatch();
			}
		}

		bool CanSync()
		{
			return SelectedDevice != null && SelectedDevice.HasAUParameters;
		}

		bool CanSyncAll()
		{
			return SelectedDevice != null && SelectedDevice.Children.Count() > 0;
		}

		void CopyFromDeviceToSystem(XDevice device)
		{
			device.Properties.RemoveAll(x => x.Name != "IPAddress");
			foreach (var property in device.DeviceProperties)
			{
				var clonedProperty = new XProperty()
				{
					Name = property.Name,
					Value = property.Value
				};
				device.Properties.Add(clonedProperty);
			}
			ServiceFactory.SaveService.GKChanged = true;
		}

		void CopyFromSystemToDevice(XDevice device)
		{
			device.DeviceProperties.Clear();
			foreach (var property in device.Properties)
			{
				var clonedProperty = new XProperty()
				{
					Name = property.Name,
					Value = property.Value
				};
				device.DeviceProperties.Add(clonedProperty);
			}
			ServiceFactory.SaveService.GKChanged = true;
		}
		#endregion

		bool Validate(List<XDevice> devices)
		{
			foreach (var device in devices)
			{
				foreach (var property in device.Properties)
				{
					var driverProperty = device.Driver.Properties.FirstOrDefault(x => x.Name == property.Name);
					if (IsPropertyValid(property, driverProperty))
					{
						MessageBoxService.Show("Устройство " + device.PresentationDriverAndAddress + "\nЗначение параметра\n" + driverProperty.Caption + "\nдолжно быть целым числом " + "в диапазоне от " + driverProperty.Min.ToString() + " до " + driverProperty.Max.ToString(), "Firesec");
						return false;
					}
				}
			}
			return true;
		}

		void UpdateDeviceParameterMissmatch()
		{
			AllDevices.ForEach(x => x.DeviceParameterMissmatchType = DeviceParameterMissmatchType.Equal);
			foreach (var deviceViewModel in AllDevices)
			{
				deviceViewModel.UpdateDeviceParameterMissmatchType();
				if (deviceViewModel.DeviceParameterMissmatchType == DeviceParameterMissmatchType.Unknown)
				{
					deviceViewModel.GetAllParents().ForEach(x => x.DeviceParameterMissmatchType = DeviceParameterMissmatchType.Unknown);
				}
			}
			foreach (var deviceViewModel in AllDevices)
			{
				deviceViewModel.UpdateDeviceParameterMissmatchType();
				if (deviceViewModel.DeviceParameterMissmatchType == DeviceParameterMissmatchType.Unequal)
				{
					deviceViewModel.GetAllParents().ForEach(x => x.DeviceParameterMissmatchType = DeviceParameterMissmatchType.Unequal);
				}
			}
		}

		void Invalidate()
		{
			foreach (var device in XManager.Devices)
			{
				if (device.Properties == null)
					device.Properties = new List<XProperty>();
				foreach (var driverProperty in device.Driver.Properties)
				{
					var property = device.Properties.FirstOrDefault(x => x.Name == driverProperty.Name);
					if (property == null)
					{
						property = new XProperty()
						{
							Name = driverProperty.Name,
							Value = driverProperty.Default,
							StringValue = driverProperty.StringDefault
						};
						device.Properties.Add(property);
					}
					property.DriverProperty = driverProperty;
				}
				device.Properties.RemoveAll(x => x.DriverProperty == null);
			}
		}

		void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.C, ModifierKeys.Control), CopyCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.V, ModifierKeys.Control), PasteCommand);
		}

		List<XDevice> GetRealChildren()
		{
			var devices = XManager.GetAllDeviceChildren(SelectedDevice.Device).Where(device => device.Driver.Properties.Any(x => x.IsAUParameter)).ToList();
			return devices;
		}

		bool CheckNeedSave()
		{
			if (ServiceFactory.SaveService.FSChanged)
			{
				if (MessageBoxService.ShowQuestion("Для выполнения этой операции необходимо применить конфигурацию. Применить сейчас?") == System.Windows.MessageBoxResult.Yes)
				{
					var cancelEventArgs = new CancelEventArgs();
					ServiceFactory.Events.GetEvent<SetNewConfigurationEvent>().Publish(cancelEventArgs);
					return !cancelEventArgs.Cancel;
				}
				return false;
			}
			return true;
		}

		int FSChangesCount;
		public override void OnShow()
		{
			base.OnShow();
			if (ServiceFactory.SaveService.FSChangesCount > FSChangesCount)
			{
				FSChangesCount = ServiceFactory.SaveService.FSChangesCount;
				Initialize();
			}
			AllDevices.ForEach(x => x.Device.Changed += UpdateDeviceParameterMissmatch);
		}
		public override void OnHide()
		{
			base.OnHide();
			AllDevices.ForEach(x => x.Device.Changed -= UpdateDeviceParameterMissmatch);
		}

		private void SetRibbonItems()
		{
			RibbonItems = new List<RibbonMenuItemViewModel>()
			{
				new RibbonMenuItemViewModel("Параметры", new ObservableCollection<RibbonMenuItemViewModel>()
				{
					new RibbonMenuItemViewModel("Считать параметры", ReadCommand, "/Controls;component/Images/BParametersRead.png"),
					new RibbonMenuItemViewModel("Записать параметры", WriteCommand, "/Controls;component/Images/BParametersWrite.png"),
					new RibbonMenuItemViewModel("Считать параметры дочерних устройств", ReadAllCommand, "/Controls;component/Images/BParametersReadAll.png"),
					new RibbonMenuItemViewModel("Записать параметры дочерних устройств", WriteAllCommand, "/Controls;component/Images/BParametersWriteAll.png"),
					new RibbonMenuItemViewModel("Копировать параметры", CopyCommand, "/Controls;component/Images/BCopy.png"),
					new RibbonMenuItemViewModel("Вставить параметры", PasteCommand, "/Controls;component/Images/BPaste.png"),
					new RibbonMenuItemViewModel("Синхронизация", new ObservableCollection<RibbonMenuItemViewModel>()
					{
						new RibbonMenuItemViewModel("Из системы в устройство", SyncFromSystemToDeviceCommand, "/Controls;component/Images/Right.png"),
						new RibbonMenuItemViewModel("Из всех дочерних устройств системы в устройства", SyncFromAllSystemToDeviceCommand, "/Controls;component/Images/RightRight.png"),
						new RibbonMenuItemViewModel("Из устройства в систему", SyncFromDeviceToSystemCommand, "/Controls;component/Images/Left.png") { IsNewGroup = true },
						new RibbonMenuItemViewModel("Из всех дочерних устройств прибора в систему", SyncFromAllDeviceToSystemCommand, "/Controls;component/Images/LeftLeft.png"),
					}, "/Controls;component/Images/BParametersSync.png"),
				}, "/Controls;component/Images/BAllParameters.png") { Order = 2 }
			};
		}
	}
}
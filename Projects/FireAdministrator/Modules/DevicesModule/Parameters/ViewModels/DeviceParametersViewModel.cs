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
using FiresecAPI;
using System.ComponentModel;
using Infrastructure.Common.Ribbon;
using System.Collections.ObjectModel;

namespace DevicesModule.ViewModels
{
	public class DeviceParametersViewModel : MenuViewPartViewModel, ISelectable<Guid>
	{
		public DeviceParametersViewModel()
		{
			Menu = new DeviceParametersMenuViewModel(this);
			ReadCommand = new RelayCommand(OnRead, CanReadWrite);
			WriteCommand = new RelayCommand(OnWrite, CanReadWrite);
			ReadAllCommand = new RelayCommand(OnReadAll, CanReadWriteAll);
			WriteAllCommand = new RelayCommand(OnWriteAll, CanReadWriteAll);

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
		}

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
				deviceViewModel.Device.AUParametersChanged += new Action(() => { UpdateIsMissmatch(); });
			}
			UpdateIsMissmatch();
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
			get { return new DeviceParameterViewModel[] { RootDevice }; }
		}

		void BuildTree()
		{
			RootDevice = AddDeviceInternal(FiresecManager.FiresecConfiguration.DeviceConfiguration.RootDevice, null);
			FillAllDevices();
		}

		public DeviceParameterViewModel AddDevice(Device device, DeviceParameterViewModel parentDeviceViewModel)
		{
			var deviceViewModel = AddDeviceInternal(device, parentDeviceViewModel);
			FillAllDevices();
			return deviceViewModel;
		}
		DeviceParameterViewModel AddDeviceInternal(Device device, DeviceParameterViewModel parentDeviceViewModel)
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
				WaitHelper.Execute(() =>
				{
					SelectedDevice.Device.DeviceAUProperties.Clear();
					SelectedDevice.Update();
					ReadOneDevice(SelectedDevice.Device);
				});
				ServiceFactory.SaveService.FSParametersChanged = true;
			}
		}

		public RelayCommand WriteCommand { get; private set; }
		void OnWrite()
		{
			if (CheckNeedSave())
			{
				WaitHelper.Execute(() =>
				{
					WriteOneDevice(SelectedDevice.Device);
				});
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
				WaitHelper.Execute(() =>
				{
					foreach (var device in SelectedDevice.Device.GetAllChildren())
					{
						device.DeviceAUProperties.Clear();
						var deviceViewModel = AllDevices.FirstOrDefault(x => x.Device == device);
						if (deviceViewModel != null)
						{
							deviceViewModel.Update();
						}
						ReadOneDevice(device);
					}
				});
			}
		}

		public RelayCommand WriteAllCommand { get; private set; }
		void OnWriteAll()
		{
			if (CheckNeedSave())
			{
				WaitHelper.Execute(() =>
				{
					foreach (var device in SelectedDevice.Device.GetAllChildren())
					{
						WriteOneDevice(device);
					}
				});
			}
		}

		bool CanReadWriteAll()
		{
			return SelectedDevice != null && SelectedDevice.Children.Count() > 0;
		}

		bool IsPropertyValid(Property property, DriverProperty driverProperty)
		{
			int value;
			return
					driverProperty != null &&
					driverProperty.IsAUParameter &&
					driverProperty.DriverPropertyType == DriverPropertyTypeEnum.IntType &&
					(!int.TryParse(property.Value, out value) ||
					(value < driverProperty.Min || value > driverProperty.Max));
		}

		void WriteOneDevice(Device device)
		{
			foreach (var property in device.SystemAUProperties)
			{
				var driverProperty = device.Driver.Properties.FirstOrDefault(x => x.Name == property.Name);
				if (IsPropertyValid(property, driverProperty))
				{
					MessageBoxService.Show("Значение параметра \n" + driverProperty.Caption + "\nдолжно быть целым числом " + "в диапазоне от " + driverProperty.Min.ToString() + " до " + driverProperty.Max.ToString(), "Firesec");
					return;
				}
			}
			Firesec_50.FiresecDriverAuParametersHelper.SetConfigurationParameters(device.UID, device.SystemAUProperties);
		}

		void ReadOneDevice(Device device)
		{
			OperationResult<bool> result = Firesec_50.FiresecDriverAuParametersHelper.BeginGetConfigurationParameters(device);
			if (result.HasError)
			{
				MessageBoxService.Show("При вызове метода на сервере возникло исключение " + result.Error);
				return;
			}
			SelectedDevice.IsAuParametersReady = false;
		}
		#endregion

		#region Capy and Paste
		Driver DriverCopy;
		List<Property> PropertiesCopy;

		public RelayCommand CopyCommand { get; private set; }
		void OnCopy()
		{
			DriverCopy = SelectedDevice.Device.Driver;
			PropertiesCopy = new List<Property>();
			foreach (var property in SelectedDevice.Device.SystemAUProperties)
			{
				var driverProperty = SelectedDevice.Device.Driver.Properties.FirstOrDefault(x => x.Name == property.Name);
				if (driverProperty != null && driverProperty.IsAUParameter)
				{
					var propertyCopy = new Property()
					{
						DriverProperty = property.DriverProperty,
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
			UpdateIsMissmatch();
		}
		bool CanPaste()
		{
			return SelectedDevice != null && DriverCopy != null && SelectedDevice.Device.Driver.DriverType == DriverCopy.DriverType && SelectedDevice.Children.Count() == 0;
		}

		public RelayCommand PasteAllCommand { get; private set; }
		void OnPasteAll()
		{
			foreach (var device in SelectedDevice.Device.GetAllChildren())
			{
				CopyParametersFromBuffer(device);
			}
			SelectedDevice.Update();
			UpdateIsMissmatch();
		}
		bool CanPasteAll()
		{
			return SelectedDevice != null && SelectedDevice.Children.Count() > 0 && DriverCopy != null;
		}

		void CopyParametersFromBuffer(Device device)
		{
			foreach (var property in PropertiesCopy)
			{
				var deviceProperty = device.SystemAUProperties.FirstOrDefault(x => x.Name == property.Name);
				if (deviceProperty != null)
				{
					deviceProperty.Value = property.Value;
				}
			}
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
			UpdateIsMissmatch();
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
				foreach (var device in SelectedDevice.Device.GetAllChildren())
				{
					CopyParametersFromTemplate(parameterTemplateSelectationViewModel.SelectedParameterTemplate, device);
				}
				SelectedDevice.Update();
			}
			UpdateIsMissmatch();
		}
		bool CanPasteAllTemplate()
		{
			return SelectedDevice != null && SelectedDevice.Children.Count() > 0;
		}

		void CopyParametersFromTemplate(ParameterTemplate parameterTemplate, Device device)
		{
			var deviceParameterTemplate = parameterTemplate.DeviceParameterTemplates.FirstOrDefault(x => x.Device.DriverUID == device.Driver.UID);
			if (deviceParameterTemplate != null)
			{
				foreach (var property in deviceParameterTemplate.Device.SystemAUProperties)
				{
					var deviceProperty = device.SystemAUProperties.FirstOrDefault(x => x.Name == property.Name);
					if (deviceProperty != null)
					{
						deviceProperty.Value = property.Value;
					}
				}
			}
		}
		#endregion

		#region Syncronization
		public RelayCommand SyncFromSystemToDeviceCommand { get; private set; }
		void OnSyncFromSystemToDevice()
		{
			if (CheckNeedSave())
			{
				CopyFromSystemToDevice(SelectedDevice.Device);
				SelectedDevice.Update();
				UpdateIsMissmatch();
			}
		}

		public RelayCommand SyncFromAllSystemToDeviceCommand { get; private set; }
		void SyncFromAllSystemToDevice()
		{
			if (CheckNeedSave())
			{
				foreach (var device in SelectedDevice.Device.GetAllChildren())
				{
					CopyFromSystemToDevice(device);
					var deviceViewModel = AllDevices.FirstOrDefault(x => x.Device == device);
					if (deviceViewModel != null)
						deviceViewModel.Update();
				}
				UpdateIsMissmatch();
			}
		}

		public RelayCommand SyncFromDeviceToSystemCommand { get; private set; }
		void OnSyncFromDeviceToSystem()
		{
			if (CheckNeedSave())
			{
				CopyFromDeviceToSystem(SelectedDevice.Device);
				SelectedDevice.Update();
				UpdateIsMissmatch();
			}
		}

		public RelayCommand SyncFromAllDeviceToSystemCommand { get; private set; }
		void OnSyncFromAllDeviceToSystem()
		{
			if (CheckNeedSave())
			{
				foreach (var device in SelectedDevice.Device.GetAllChildren())
				{
					CopyFromDeviceToSystem(device);
					var deviceViewModel = AllDevices.FirstOrDefault(x => x.Device == device);
					if (deviceViewModel != null)
						deviceViewModel.Update();
				}
				UpdateIsMissmatch();
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

		void CopyFromDeviceToSystem(Device device)
		{
			device.SystemAUProperties.Clear();
			foreach (var property in device.DeviceAUProperties)
			{
				var clonedProperty = new Property()
				{
					Name = property.Name,
					Value = property.Value
				};
				device.SystemAUProperties.Add(clonedProperty);
			}
		}

		void CopyFromSystemToDevice(Device device)
		{
			device.DeviceAUProperties.Clear();
			foreach (var property in device.SystemAUProperties)
			{
				var clonedProperty = new Property()
				{
					Name = property.Name,
					Value = property.Value
				};
				device.DeviceAUProperties.Add(clonedProperty);
			}
			WriteOneDevice(device);
		}
		#endregion

		void UpdateIsMissmatch()
		{
			AllDevices.ForEach(x => x.IsMissmatch = false);
			foreach (var deviceViewModel in AllDevices)
			{
				deviceViewModel.UpdateIsMissmatch();
				if (deviceViewModel.IsMissmatch)
				{
					deviceViewModel.GetAllParents().ForEach(x => x.IsMissmatch = true);
				}
			}
		}

		void Invalidate()
		{
			foreach (var device in FiresecManager.Devices)
			{
				foreach (var driverProperty in device.Driver.Properties)
				{
					if (driverProperty.IsAUParameter)
					{
						var property = device.Properties.FirstOrDefault(x => x.Name == driverProperty.Name);
						if (property == null)
						{
							property = new Property()
							{
								Name = driverProperty.Name,
								Value = driverProperty.Default
							};
							device.SystemAUProperties.Add(property);
						}
						property.DriverProperty = driverProperty;
					}
				}
				if (device.SystemAUProperties != null)
					device.SystemAUProperties.RemoveAll(x => x.DriverProperty == null);
			}
		}

		bool CheckNeedSave()
		{
			if (ServiceFactory.SaveService.FSChanged)
			{
				if (MessageBoxService.ShowQuestion("Для выполнения этой операции необходимо применить конфигурацию. Примнить сейчас?") == System.Windows.MessageBoxResult.Yes)
				{
					var cancelEventArgs = new CancelEventArgs();
					ServiceFactory.Events.GetEvent<SetNewConfigurationEvent>().Publish(cancelEventArgs);
					return !cancelEventArgs.Cancel;
				}
				else
				{
					return false;
				}
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
			AllDevices.ForEach(x => x.Device.Changed += UpdateIsMissmatch);
		}
		public override void OnHide()
		{
			base.OnHide();
			AllDevices.ForEach(x => x.Device.Changed -= UpdateIsMissmatch);
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
						new RibbonMenuItemViewModel("Из системы в устройство", SyncFromSystemToDeviceCommand),
						new RibbonMenuItemViewModel("Из всех дочерних устройств системы в устройства", SyncFromAllSystemToDeviceCommand),
						new RibbonMenuItemViewModel("Из устройства в систему", SyncFromDeviceToSystemCommand) { IsNewGroup = true },
						new RibbonMenuItemViewModel("Из всех дочерних устройств прибора в систему", SyncFromAllDeviceToSystemCommand),
					}, "/Controls;component/Images/BParametersSync.png"),
				}, "/Controls;component/Images/BAllParameters.png") { Order = 2 }
			};
		}
	}
}
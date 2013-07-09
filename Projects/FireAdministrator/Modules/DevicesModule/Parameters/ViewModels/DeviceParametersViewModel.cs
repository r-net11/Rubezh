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
			SyncFromAllSystemToDeviceCommand = new RelayCommand(SyncFromAllSystemToDevice, CanSyncAll);
			SyncFromDeviceToSystemCommand = new RelayCommand(OnSyncFromDeviceToSystem, CanSync);
			SyncFromAllDeviceToSystemCommand = new RelayCommand(OnSyncFromAllDeviceToSystem, CanSyncAll);
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

		public RelayCommand ReadCommand { get; private set; }
		void OnRead()
		{
			WaitHelper.Execute(() =>
			{
				SelectedDevice.Device.DeviceParameters.Clear();
				SelectedDevice.Update();
				ReadOneDevice(SelectedDevice.Device);
			});
			ServiceFactory.SaveService.FSParametersChanged = true;
		}

		public RelayCommand WriteCommand { get; private set; }
		void OnWrite()
		{
			WaitHelper.Execute(() =>
			{
				WriteOneDevice(SelectedDevice.Device);
			});
		}

		bool CanReadWrite()
		{
			return SelectedDevice != null && SelectedDevice.HasAUParameters;
		}

		public RelayCommand ReadAllCommand { get; private set; }
		void OnReadAll()
		{
			WaitHelper.Execute(() =>
			{
				foreach (var device in SelectedDevice.Device.GetAllChildren())
				{
					device.DeviceParameters.Clear();
					var deviceViewModel = AllDevices.FirstOrDefault(x => x.Device == device);
					if (deviceViewModel != null)
					{
						deviceViewModel.Update();
					}
					ReadOneDevice(device);
				}
			});
		}

		public RelayCommand WriteAllCommand { get; private set; }
		void OnWriteAll()
		{
			WaitHelper.Execute(() =>
			{
				foreach (var device in SelectedDevice.Device.GetAllChildren())
				{
					WriteOneDevice(device);
				}
			});
		}

		bool CanReadWriteAll()
		{
			return SelectedDevice != null && SelectedDevice.Children.Count() > 0;
		}

		bool IsValidSet(Property property, DriverProperty driverProperty)
		{
			int value;
			return
					driverProperty != null &&
					driverProperty.IsAUParameter &&
					driverProperty.DriverPropertyType == DriverPropertyTypeEnum.IntType &&
					(!int.TryParse(property.Value, out value) ||
					(value < driverProperty.Min || value > driverProperty.Max));
		}

		Driver DriverCopy;
		List<Property> PropertiesCopy;

		public RelayCommand CopyCommand { get; private set; }
		void OnCopy()
		{
			DriverCopy = SelectedDevice.Device.Driver;
			PropertiesCopy = new List<Property>();
			foreach (var property in SelectedDevice.Device.Properties)
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
		}
		bool CanPasteAll()
		{
			return SelectedDevice != null && SelectedDevice.Children.Count() > 0 && DriverCopy != null;
		}

		public RelayCommand PasteTemplateCommand { get; private set; }
		void OnPasteTemplate()
		{
			var parameterTemplateSelectationViewModel = new ParameterTemplateSelectationViewModel();
			if (DialogService.ShowModalWindow(parameterTemplateSelectationViewModel))
			{
				CopyParametersFromTemplate(parameterTemplateSelectationViewModel.SelectedParameterTemplate, SelectedDevice.Device);
				SelectedDevice.Update();
			}
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
		}
		bool CanPasteAllTemplate()
		{
			return SelectedDevice != null && SelectedDevice.Children.Count() > 0;
		}

		public RelayCommand SyncFromSystemToDeviceCommand { get; private set; }
		void OnSyncFromSystemToDevice()
		{
			CopyFromSystemToDevice(SelectedDevice.Device);
			SelectedDevice.Update();
		}

		public RelayCommand SyncFromAllSystemToDeviceCommand { get; private set; }
		void SyncFromAllSystemToDevice()
		{
			foreach (var device in SelectedDevice.Device.GetAllChildren())
			{
				CopyFromSystemToDevice(device);
				var deviceViewModel = AllDevices.FirstOrDefault(x => x.Device == device);
				if (deviceViewModel != null)
					deviceViewModel.Update();
			}
		}

		public RelayCommand SyncFromDeviceToSystemCommand { get; private set; }
		void OnSyncFromDeviceToSystem()
		{
			CopyFromDeviceToSystem(SelectedDevice.Device);
			SelectedDevice.Update();
		}

		public RelayCommand SyncFromAllDeviceToSystemCommand { get; private set; }
		void OnSyncFromAllDeviceToSystem()
		{
			foreach (var device in SelectedDevice.Device.GetAllChildren())
			{
				CopyFromDeviceToSystem(device);
				var deviceViewModel = AllDevices.FirstOrDefault(x => x.Device == device);
				if (deviceViewModel != null)
					deviceViewModel.Update();
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
			foreach (var deviceParameter in device.DeviceParameters)
			{
				var deviceDriverProperty = device.Driver.Properties.FirstOrDefault(x => x.Name == deviceParameter.Name);
				if (deviceDriverProperty != null && deviceDriverProperty.IsAUParameter)
				{
					var driverProperty = device.Properties.FirstOrDefault(x => x.Name == deviceParameter.Name);
					if (driverProperty == null)
					{
						driverProperty = new Property()
						{
							Name = deviceParameter.Name
						};
						device.Properties.Add(driverProperty);
					}
					driverProperty.Value = deviceParameter.Value;
				}
			}
		}

		void CopyFromSystemToDevice(Device device)
		{
			foreach (var property in device.Properties)
			{
				var driverProperty = device.Driver.Properties.FirstOrDefault(x => x.Name == property.Name);
				if (driverProperty != null)
				{
					var deviceProperty = device.DeviceParameters.FirstOrDefault(x => x.Name == property.Name);
					if (deviceProperty == null)
					{
						deviceProperty = new Property()
						{
							Name = property.Name
						};
						device.DeviceParameters.Add(deviceProperty);
					}
					deviceProperty.Value = property.Value;
				}
			}
		}

		void WriteOneDevice(Device device)
		{
			foreach (var property in device.Properties)
			{
				var driverProperty = device.Driver.Properties.FirstOrDefault(x => x.Name == property.Name);
				if (IsValidSet(property, driverProperty))
				{
					MessageBoxService.Show("Значение параметра \n" + driverProperty.Caption + "\nдолжно быть целым числом " + "в диапазоне от " + driverProperty.Min.ToString() + " до " + driverProperty.Max.ToString(), "Firesec");
					return;
				}
			}
			Firesec_50.FiresecDriverAuParametersHelper.SetConfigurationParameters(device.UID, device.Properties);
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

		void CopyParametersFromBuffer(Device device)
		{
			foreach (var property in PropertiesCopy)
			{
				var deviceProperty = device.Properties.FirstOrDefault(x => x.Name == property.Name);
				if (deviceProperty != null)
				{
					deviceProperty.Value = property.Value;
				}
			}
		}

		void CopyParametersFromTemplate(ParameterTemplate parameterTemplate, Device device)
		{
			var deviceParameterTemplate = parameterTemplate.DeviceParameterTemplates.FirstOrDefault(x => x.Device.DriverUID == device.Driver.UID);
			if (deviceParameterTemplate != null)
			{
				foreach (var property in deviceParameterTemplate.Device.Properties)
				{
					var deviceProperty = device.Properties.FirstOrDefault(x => x.Name == property.Name);
					if (deviceProperty != null)
					{
						deviceProperty.Value = property.Value;
					}
				}
			}
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
		}

		public override void OnHide()
		{
			base.OnHide();
		}
	}
}
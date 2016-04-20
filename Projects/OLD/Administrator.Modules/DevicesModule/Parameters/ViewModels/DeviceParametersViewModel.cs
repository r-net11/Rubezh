using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Ribbon;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using Infrastructure.Events;
using Infrastructure.ViewModels;

namespace DevicesModule.ViewModels
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

			//if (FiresecManager.IsFS2Enabled)
			//{
			//	FS2AuParametersHelper.Progress -= new Action<string, int>(FiresecDriverAuParametersHelper_Progress);
			//	FS2AuParametersHelper.Progress += new Action<string, int>(FiresecDriverAuParametersHelper_Progress);
			//}
			//else
			{
				Firesec_50.FiresecDriverAuParametersHelper.Progress -= new Action<string, int>(FiresecDriverAuParametersHelper_Progress);
				Firesec_50.FiresecDriverAuParametersHelper.Progress += new Action<string, int>(FiresecDriverAuParametersHelper_Progress);
			}
		}

		void FiresecDriverAuParametersHelper_Progress(string value, int percentsCompleted)
		{
			ProgressCaption = value;
			PercentsCompleted = percentsCompleted;
			OnPropertyChanged(() => ProgressCaption);
			OnPropertyChanged(() => PercentsCompleted);
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
			OnPropertyChanged(() => RootDevices);

			foreach (var deviceViewModel in AllDevices)
			{
				deviceViewModel.Device.AUParametersChanged += new Action(() => { UpdateDeviceParameterMissmatch(); });
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
				OnPropertyChanged(() => RootDevice);
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
				OnPropertyChanged(() => SelectedDevice);

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
				SelectedDevice.Device.DeviceAUProperties.Clear();
				SelectedDevice.Update();
				ReadDevices(new List<Device>() { SelectedDevice.Device });
				ServiceFactory.SaveService.FSParametersChanged = true;
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
				foreach (var device in devices)
				{
					device.DeviceAUProperties.Clear();
					var deviceViewModel = AllDevices.FirstOrDefault(x => x.Device == device);
					if (deviceViewModel != null)
					{
						deviceViewModel.Update();
					}
				}
				ReadDevices(devices);
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

		void WriteDevices(List<Device> devices)
		{
			//if (FiresecManager.IsFS2Enabled)
			//{
			//	FS2AuParametersHelper.BeginSetAuParameters(devices);
			//}
			//else
			{
				Firesec_50.FiresecDriverAuParametersHelper.BeginSetAuParameters(devices);
			}
		}

		void ReadDevices(List<Device> devices)
		{
			//if (FiresecManager.IsFS2Enabled)
			//{
			//	FS2AuParametersHelper.BeginGetAuParameters(devices);
			//}
			//else
			{
				Firesec_50.FiresecDriverAuParametersHelper.BeginGetAuParameters(devices);
			}
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
			UpdateDeviceParameterMissmatch();
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
			UpdateDeviceParameterMissmatch();
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
			ServiceFactory.SaveService.FSParametersChanged = true;
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
				foreach (var device in SelectedDevice.Device.GetAllChildren())
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
		public RelayCommand WriteCommand { get; private set; }
		public RelayCommand SyncFromSystemToDeviceCommand { get; private set; }
		void OnSyncFromSystemToDevice()
		{
			if (CheckNeedSave())
			{
				var devices = new List<Device>() { SelectedDevice.Device };
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
			ServiceFactory.SaveService.FSParametersChanged = true;
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
			ServiceFactory.SaveService.FSParametersChanged = true;
		}
		#endregion

		bool Validate(List<Device> devices)
		{
			foreach (var device in devices)
			{
				foreach (var property in device.SystemAUProperties)
				{
					var driverProperty = device.Driver.Properties.FirstOrDefault(x => x.Name == property.Name);
					if (IsPropertyValid(property, driverProperty))
					{
						MessageBoxService.Show("???????????????? ?????????????????? \n" + driverProperty.Caption + "\n???????????? ???????? ?????????? ???????????? " + "?? ?????????????????? ???? " + driverProperty.Min.ToString() + " ???? " + driverProperty.Max.ToString(), "Firesec");
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
			foreach (var device in FiresecManager.Devices)
			{
				if (device.SystemAUProperties == null)
					device.SystemAUProperties = new List<Property>();
				foreach (var driverProperty in device.Driver.Properties)
				{
					if (driverProperty.IsAUParameter)
					{
						var property = device.SystemAUProperties.FirstOrDefault(x => x.Name == driverProperty.Name);
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
				device.SystemAUProperties.RemoveAll(x => x.DriverProperty == null);
			}
		}

		List<Device> GetRealChildren()
		{
			var devices = new List<Device>();
			foreach (var device in SelectedDevice.Device.GetAllChildren())
			{
				if (device.Driver.Properties.Any(x => x.IsAUParameter))
				{
					devices.Add(device);
				}
			}
			return devices;
		}

		bool CheckNeedSave()
		{
			if (ServiceFactory.SaveService.FSChanged)
			{
				if (MessageBoxService.ShowQuestion("?????? ???????????????????? ???????? ???????????????? ???????????????????? ?????????????????? ????????????????????????. ?????????????????? ?????????????"))
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
				new RibbonMenuItemViewModel("??????????????????", new ObservableCollection<RibbonMenuItemViewModel>()
				{
					new RibbonMenuItemViewModel("?????????????? ??????????????????", ReadCommand, "BParametersRead"),
					new RibbonMenuItemViewModel("???????????????? ??????????????????", WriteCommand, "BParametersWrite"),
					new RibbonMenuItemViewModel("?????????????? ?????????????????? ???????????????? ??????????????????", ReadAllCommand, "BParametersReadAll"),
					new RibbonMenuItemViewModel("???????????????? ?????????????????? ???????????????? ??????????????????", WriteAllCommand, "BParametersWriteAll"),
					new RibbonMenuItemViewModel("???????????????????? ??????????????????", CopyCommand, "BCopy"),
					new RibbonMenuItemViewModel("???????????????? ??????????????????", PasteCommand, "BPaste"),
					new RibbonMenuItemViewModel("??????????????????????????", new ObservableCollection<RibbonMenuItemViewModel>()
					{
						new RibbonMenuItemViewModel("???? ?????????????? ?? ????????????????????", SyncFromSystemToDeviceCommand, "Right"),
						new RibbonMenuItemViewModel("???? ???????? ???????????????? ?????????????????? ?????????????? ?? ????????????????????", SyncFromAllSystemToDeviceCommand, "RightRight"),
						new RibbonMenuItemViewModel("???? ???????????????????? ?? ??????????????", SyncFromDeviceToSystemCommand, "Left") { IsNewGroup = true },
						new RibbonMenuItemViewModel("???? ???????? ???????????????? ?????????????????? ?????????????? ?? ??????????????", SyncFromAllDeviceToSystemCommand, "LeftLeft"),
					}, "BParametersSync"),
				}, "BAllParameters") { Order = 2 }
			};
		}
	}
}
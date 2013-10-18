using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Shapes;
using Common.GK;
using DeviceControls;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Painters;
using XFiresecAPI;
using Infrustructure.Plans.Helper;

namespace GKModule.ViewModels
{
	public class DeviceViewModel : TreeNodeViewModel<DeviceViewModel>
	{
		public XDevice Device { get; private set; }
		public PropertiesViewModel PropertiesViewModel { get; private set; }

		public DeviceViewModel(XDevice device)
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			AddToParentCommand = new RelayCommand(OnAddToParent, CanAddToParent);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties, CanShowProperties);
			ShowLogicCommand = new RelayCommand(OnShowLogic, CanShowLogic);
			ShowZonesCommand = new RelayCommand(OnShowZones, CanShowZones);
			ShowZoneOrLogicCommand = new RelayCommand(OnShowZoneOrLogic, CanShowZoneOrLogic);
			ShowZoneCommand = new RelayCommand(OnShowZone, CanShowZone);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan);
			ShowParentCommand = new RelayCommand(OnShowParent, CanShowParent);

			ReadCommand = new RelayCommand(OnRead, CanReadWrite);
			WriteCommand = new RelayCommand(OnSyncFromSystemToDevice, CanSync);
			ReadAllCommand = new RelayCommand(OnReadAll, CanReadWriteAll);
			WriteAllCommand = new RelayCommand(SyncFromAllSystemToDevice, CanSyncAll);
			SyncFromDeviceToSystemCommand = new RelayCommand(OnSyncFromDeviceToSystem, CanSync);
			SyncFromAllDeviceToSystemCommand = new RelayCommand(OnSyncFromAllDeviceToSystem, CanSyncAll);

			ResetAUPropertiesCommand = new RelayCommand(OnResetAUProperties);
			CreateDragObjectCommand = new RelayCommand<DataObject>(OnCreateDragObjectCommand, CanCreateDragObjectCommand);
			CreateDragVisual = OnCreateDragVisual;
			AllowMultipleVizualizationCommand = new RelayCommand<bool>(OnAllowMultipleVizualizationCommand, CanAllowMultipleVizualizationCommand);

			Device = device;
			PropertiesViewModel = new PropertiesViewModel(device);
			device.Changed += new System.Action(OnChanged);

			AvailvableDrivers = new ObservableCollection<XDriver>();
			UpdateDriver();
			device.AUParametersChanged += () => { UpdateProperties(); };
		}

		public RelayCommand WriteCommand { get; private set; }
		public RelayCommand SyncFromSystemToDeviceCommand { get; private set; }
		void OnSyncFromSystemToDevice()
		{
			if (CheckNeedSave())
			{
				var devices = new List<XDevice>() { Device };
				if (Validate(devices))
				{
					CopyFromSystemToDevice(Device);
					PropertiesViewModel.Update();
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
				devices.Add(Device);
				if (Validate(devices))
				{
					foreach (var device in devices)
					{
						CopyFromSystemToDevice(device);
						var deviceViewModel = DevicesViewModel.Current.AllDevices.FirstOrDefault(x => x.Device == device);
						if (deviceViewModel != null)
							deviceViewModel.PropertiesViewModel.Update();
					}
					WriteDevices(devices);
				}
			}
		}

		public RelayCommand SyncFromDeviceToSystemCommand { get; private set; }
		void OnSyncFromDeviceToSystem()
		{
			if (CheckNeedSave())
			{
				CopyFromDeviceToSystem(Device);
				PropertiesViewModel.Update();
				//UpdateDeviceParameterMissmatch();
			}
		}
		//void UpdateDeviceParameterMissmatch()
		//{
		//    AllDevices.ForEach(x => x.DeviceParameterMissmatchType = DeviceParameterMissmatchType.Equal);
		//    foreach (var deviceViewModel in AllDevices)
		//    {
		//        deviceViewModel.UpdateDeviceParameterMissmatchType();
		//        if (deviceViewModel.DeviceParameterMissmatchType == DeviceParameterMissmatchType.Unknown)
		//        {
		//            deviceViewModel.GetAllParents().ForEach(x => x.DeviceParameterMissmatchType = DeviceParameterMissmatchType.Unknown);
		//        }
		//    }
		//    foreach (var deviceViewModel in AllDevices)
		//    {
		//        deviceViewModel.UpdateDeviceParameterMissmatchType();
		//        if (deviceViewModel.DeviceParameterMissmatchType == DeviceParameterMissmatchType.Unequal)
		//        {
		//            deviceViewModel.GetAllParents().ForEach(x => x.DeviceParameterMissmatchType = DeviceParameterMissmatchType.Unequal);
		//        }
		//    }
		//}
		public RelayCommand SyncFromAllDeviceToSystemCommand { get; private set; }
		void OnSyncFromAllDeviceToSystem()
		{
			if (CheckNeedSave()) 
			{
				var devices = GetRealChildren();
				devices.Add(Device);
				foreach (var device in devices)
				{
					CopyFromDeviceToSystem(device);
					var deviceViewModel = DevicesViewModel.Current.AllDevices.FirstOrDefault(x => x.Device == device);
					if (deviceViewModel != null)
						deviceViewModel.PropertiesViewModel.Update();
				}
			}
		}

		bool CanSync()
		{
			return HasAUProperties;
		}

		bool CanSyncAll()
		{
			return Children.Count() > 0;
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
			ServiceFactory.SaveService.FSParametersChanged = true;
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
			ServiceFactory.SaveService.FSParametersChanged = true;
		}

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

	
		public RelayCommand ReadCommand { get; private set; }
		void OnRead()
		{
			if (CheckNeedSave())
			{
				ReadDevices(new List<XDevice>() { Device });
				PropertiesViewModel.Update();
				ServiceFactory.SaveService.FSParametersChanged = true; // TODO Для ГК свой флаг
			}
		}

		bool CanReadWrite()
		{
			return HasAUProperties;
		}

		public RelayCommand ReadAllCommand { get; private set; }
		void OnReadAll()
		{
			if (CheckNeedSave())
			{
				var devices = GetRealChildren();
				devices.Add(Device);
				ReadDevices(devices);
			}
		}

		bool CanReadWriteAll()
		{
			return Children.Count() > 0;
		}

		List<XDevice> GetRealChildren()
		{
			var devices = XManager.GetAllDeviceChildren(Device).Where(device => device.Driver.Properties.Any(x => x.IsAUParameter)).ToList();
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

		void OnChanged()
		{
			OnPropertyChanged("PresentationAddress");
			OnPropertyChanged("PresentationZone");
			OnPropertyChanged("EditingPresentationZone");
		}

		public void UpdateProperties()
		{
			PropertiesViewModel = new PropertiesViewModel(Device);
			OnPropertyChanged("PropertiesViewModel");
		}

		public void Update()
		{
			OnPropertyChanged("HasChildren");
			OnPropertyChanged("IsOnPlan");
			OnPropertyChanged(() => VisualizationState);
		}

		public string Address
		{
			get { return Device.Address; }
			set
			{
				Device.SetAddress(value);
				if (Driver.IsGroupDevice)
				{
					foreach (var deviceViewModel in Children)
					{
						deviceViewModel.OnPropertyChanged("Address");
						deviceViewModel.OnPropertyChanged("PresentationAddress");
					}
				}
				OnPropertyChanged("Address");
				OnPropertyChanged("PresentationAddress");
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public string PresentationAddress
		{
			get { return Device.PresentationAddress; }
		}

		public string Description
		{
			get { return Device.Description; }
			set
			{
				Device.Description = value;
				OnPropertyChanged("Description");
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public bool IsUsed
		{
			get { return !Device.IsNotUsed; }
			set
			{
				Device.IsNotUsed = !value;
				XManager.ChangeDeviceLogic(Device, new XDeviceLogic());
				OnPropertyChanged("IsUsed");
				OnPropertyChanged("ShowOnPlan");
				OnPropertyChanged("PresentationZone");
				OnPropertyChanged("EditingPresentationZone");
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			NewDeviceViewModelBase newDeviceViewModel;
			if (Device.IsConnectedToKAURSR2OrIsKAURSR2)
				newDeviceViewModel = new RSR2NewDeviceViewModel(this);
			else
				newDeviceViewModel = new NewDeviceViewModel(this);

			if (newDeviceViewModel.Drivers.Count == 1)
			{
				newDeviceViewModel.SaveCommand.Execute();
				DevicesViewModel.Current.SelectedDevice = newDeviceViewModel.AddedDevice;
				ServiceFactory.SaveService.GKChanged = true;
				return;
			}
			if (DialogService.ShowModalWindow(newDeviceViewModel))
			{
				ServiceFactory.SaveService.GKChanged = true;
				DevicesViewModel.Current.AllDevices.Add(newDeviceViewModel.AddedDevice);
				GKModule.Plans.Designer.Helper.BuildMap();
			}
		}
		public bool CanAdd()
		{
			if(Device.AllParents.Any(x=>x.Driver.DriverType == XDriverType.RSR2_KAU))
				return true;
			if (Driver.Children.Count > 0)
				return true;
			if (Driver.DriverType == XDriverType.MPT && Parent != null && Parent.Driver.IsKauOrRSR2Kau)
				return true;
			return false;
		}

		public RelayCommand AddToParentCommand { get; private set; }
		void OnAddToParent()
		{
			Parent.AddCommand.Execute();
		}
		public bool CanAddToParent()
		{
			return ((Parent != null) && (Parent.AddCommand.CanExecute(null)));
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			XManager.RemoveDevice(Parent.Device, Device);
			var parent = Parent;
			if (parent != null)
			{
				var index = DevicesViewModel.Current.SelectedDevice.VisualIndex;
				parent.Nodes.Remove(this);
				parent.Update();

				ServiceFactory.SaveService.GKChanged = true;

				index = Math.Min(index, parent.ChildrenCount - 1);
				DevicesViewModel.Current.AllDevices.Remove(this);
				DevicesViewModel.Current.SelectedDevice = index >= 0 ? parent.GetChildByVisualIndex(index) : parent;
			}
			GKModule.Plans.Designer.Helper.BuildMap();
		}
		bool CanRemove()
		{
			return !(Driver.IsAutoCreate || Parent == null || (Parent.Driver.IsGroupDevice && Parent.Driver.GroupDeviceChildType == Driver.DriverType));
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			var pumpStationViewModel = new PumpStationViewModel(Device);
			if (DialogService.ShowModalWindow(pumpStationViewModel))
			{
				ServiceFactory.SaveService.GKChanged = true;
			}
		}
		bool CanShowProperties()
		{
			return Device.Driver.DriverType == XDriverType.PumpStation;
		}

		public string PresentationZone
		{
			get
			{
				if (Device.IsNotUsed)
					return null;
				return XManager.GetPresentationZone(Device);
			}
		}

		public string EditingPresentationZone
		{
			get
			{
				if (Device.IsNotUsed)
					return null;
				var presentationZone = XManager.GetPresentationZone(Device);
				IsZoneGrayed = string.IsNullOrEmpty(presentationZone);
				if (string.IsNullOrEmpty(presentationZone))
				{
					if (Driver.HasZone)
						presentationZone = "Нажмите для выбора зон";
					if (Driver.HasLogic)
						presentationZone = "Нажмите для настройки логики";
				}
				return presentationZone;
			}
		}

		bool _isZoneGrayed = false;
		public bool IsZoneGrayed
		{
			get { return _isZoneGrayed; }
			set
			{
				_isZoneGrayed = value;
				OnPropertyChanged("IsZoneGrayed");
			}
		}

		public bool IsOnPlan
		{
			get { return Device.PlanElementUIDs.Count > 0; }
		}
		public bool ShowOnPlan
		{
			get { return !Device.IsNotUsed && (Device.Driver.IsDeviceOnShleif || Device.Children.Count > 0); }
		}
		public VisualizationState VisualizationState
		{
			get { return Driver != null && Driver.IsPlaceable ? (IsOnPlan ? (Device.AllowMultipleVizualization ? VisualizationState.Multiple : VisualizationState.Single) : VisualizationState.NotPresent) : VisualizationState.Prohibit; }
		}

		public RelayCommand<DataObject> CreateDragObjectCommand { get; private set; }
		private void OnCreateDragObjectCommand(DataObject dataObject)
		{
			IsSelected = true;
			var plansElement = new ElementXDevice()
			{
				XDeviceUID = Device.UID
			};
			dataObject.SetData("DESIGNER_ITEM", plansElement);
		}
		private bool CanCreateDragObjectCommand(DataObject dataObject)
		{
			return VisualizationState == VisualizationState.NotPresent || VisualizationState == VisualizationState.Multiple;
		}

		public Converter<IDataObject, UIElement> CreateDragVisual { get; private set; }
		private UIElement OnCreateDragVisual(IDataObject dataObject)
		{
			var brush = DevicePictureCache.GetXBrush(Device);
			return new Rectangle()
			{
				Fill = brush,
				Height = PainterCache.PointZoom * PainterCache.Zoom,
				Width = PainterCache.PointZoom * PainterCache.Zoom,
			};
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			if (Device.PlanElementUIDs.Count > 0)
				ServiceFactory.Events.GetEvent<FindElementEvent>().Publish(Device.PlanElementUIDs);
		}

		public RelayCommand<bool> AllowMultipleVizualizationCommand { get; private set; }
		private void OnAllowMultipleVizualizationCommand(bool isAllow)
		{
			Device.AllowMultipleVizualization = isAllow;
			Update();
		}
		private bool CanAllowMultipleVizualizationCommand(bool isAllow)
		{
			return Device.AllowMultipleVizualization != isAllow;
		}

		public RelayCommand ShowLogicCommand { get; private set; }
		void OnShowLogic()
		{
			if (DialogService.ShowModalWindow(new DeviceLogicViewModel(Device)))
			{
				OnPropertyChanged("PresentationZone");
				ServiceFactory.SaveService.GKChanged = true;
			}
		}
		bool CanShowLogic()
		{
			return (Driver.HasLogic && !Device.IsNotUsed);
		}

		public RelayCommand ShowZonesCommand { get; private set; }
		void OnShowZones()
		{
			var zonesSelectationViewModel = new ZonesSelectationViewModel(Device.Zones, true);
			if (DialogService.ShowModalWindow(zonesSelectationViewModel))
			{
				XManager.ChangeDeviceZones(Device, zonesSelectationViewModel.Zones);
				OnPropertyChanged("PresentationZone");
				ServiceFactory.SaveService.GKChanged = true;
			}
		}
		bool CanShowZones()
		{
			return (Driver.HasZone);
		}

		public RelayCommand ShowZoneOrLogicCommand { get; private set; }
		void OnShowZoneOrLogic()
		{
			IsSelected = true;
			if (CanShowZones())
				OnShowZones();

			if (CanShowLogic())
				OnShowLogic();
		}
		bool CanShowZoneOrLogic()
		{
			return (Driver.HasZone || Driver.HasLogic);
		}

		public bool IsZoneOrLogic
		{
			get { return CanShowZoneOrLogic(); }
		}

		public RelayCommand ShowZoneCommand { get; private set; }
		void OnShowZone()
		{
			var zone = Device.Zones.FirstOrDefault();
			if (zone != null)
			{
				ServiceFactory.Events.GetEvent<ShowXZoneEvent>().Publish(zone.UID);
			}
		}
		bool CanShowZone()
		{
			return Device.Zones.Count == 1;
		}

		public RelayCommand ShowParentCommand { get; private set; }
		void OnShowParent()
		{
			ServiceFactory.Events.GetEvent<ShowXDeviceEvent>().Publish(Device.Parent.UID);
		}
		bool CanShowParent()
		{
			return Device.Parent != null;
		}

		public XDriver Driver
		{
			get { return Device.Driver; }
			set
			{
				if (Device.Driver.DriverType != value.DriverType)
				{
					XManager.ChangeDriver(Device, value);
					this.Nodes.Clear();
					foreach (var childDevice in Device.Children)
					{
						DevicesViewModel.Current.AddDevice(childDevice, this);
					}
					OnPropertyChanged("Device");
					OnPropertyChanged("Driver");
					OnPropertyChanged("Device");
					OnPropertyChanged("Children");
					OnPropertyChanged("EditingPresentationZone");
					PropertiesViewModel = new PropertiesViewModel(Device);
					OnPropertyChanged("PropertiesViewModel");
					if (Device.KAURSR2Parent != null)
						XManager.RebuildRSR2Addresses(Device.KAURSR2Parent);
					XManager.DeviceConfiguration.Update();
					Update();
					ServiceFactory.SaveService.GKChanged = true;
				}
			}
		}

		public ObservableCollection<XDriver> AvailvableDrivers { get; private set; }

		void UpdateDriver()
		{
			AvailvableDrivers.Clear();
			if (CanChangeDriver)
			{
				switch (Device.Parent.Driver.DriverType)
				{
					case XDriverType.AM_4:
						AvailvableDrivers.Add(XManager.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.AM_1));
						AvailvableDrivers.Add(XManager.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.AM1_T));
						break;

					case XDriverType.AMP_4:
						AvailvableDrivers.Add(XManager.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.AMP_1));
						break;

					default:
						foreach (var driverType in Device.Parent.Driver.Children)
						{
							var driver = XManager.Drivers.FirstOrDefault(x => x.DriverType == driverType);
							if (CanDriverBeChanged(driver))
							{
								AvailvableDrivers.Add(driver);
							}
						}
						break;
				}
			}
		}

		public bool CanDriverBeChanged(XDriver driver)
		{
			if (driver == null || Device.Parent == null)
				return false;

			if (Device.Parent.Driver.DriverType == XDriverType.AM_4)
				return true;

			if (driver.IsAutoCreate)
				return false;
			//if (driver.IsGroupDevice)
			//    return false;
			if (Device.Parent.Driver.IsGroupDevice)
				return false;
			return driver.IsDeviceOnShleif;
		}

		public bool CanChangeDriver
		{
			get { return CanDriverBeChanged(Device.Driver); }
		}

		public bool IsBold { get; set; }

		public RelayCommand CopyCommand { get { return DevicesViewModel.Current.CopyCommand; } }
		public RelayCommand CutCommand { get { return DevicesViewModel.Current.CutCommand; } }
		public RelayCommand PasteCommand { get { return DevicesViewModel.Current.PasteCommand; } }

		public bool HasAUProperties
		{
			get { return Device.Driver.Properties.Count(x => x.IsAUParameter) > 0; }
		}

		//public RelayCommand GetAUPropertiesCommand { get; private set; }
		//void OnGetAUProperties()
		//{
		//    DatabaseManager.Convert();
		//    ParametersHelper.GetSingleParameter(Device);
		//    ServiceFactory.SaveService.GKChanged = true;
		//}

		void ReadDevices(List<XDevice> devices)
		{
			ParametersHelper.ErrorLog = "";
			LoadingService.Show("Запрос параметров");
			DatabaseManager.Convert();
			var i = 0;
			foreach (var device in devices)
			{
				i++;
				//FiresecDriverAuParametersHelper_Progress("Чтение параметров устройства " + device.PresentationDriverAndAddress, (i * 100) / devices.Count);
				ParametersHelper.GetSingleParameter(device);
			}
			LoadingService.Close();
			if (ParametersHelper.ErrorLog != "")
				MessageBoxService.ShowError("Ошибка при получении параметров следующих устройств:" + ParametersHelper.ErrorLog);
			//FiresecDriverAuParametersHelper_Progress("Чтение параметров устройства ", 0);
			ServiceFactory.SaveService.GKChanged = true;
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
				//FiresecDriverAuParametersHelper_Progress("Запись параметров в устройство " + device.PresentationDriverAndAddress, (i * 100) / devices.Count);
				ParametersHelper.SetSingleParameter(device);
				Thread.Sleep(100);
			}
			LoadingService.Close();
			if (ParametersHelper.ErrorLog != "")
				MessageBoxService.ShowError("Ошибка при записи параметров в следующие устройства:" + ParametersHelper.ErrorLog);
			//FiresecDriverAuParametersHelper_Progress("Запись параметров в устройство ", 0);
			ServiceFactory.SaveService.GKChanged = true;
		}

		//public RelayCommand SetAUPropertiesCommand { get; private set; }
		//void OnSetAUProperties()
		//{
		//    DatabaseManager.Convert();
		//    ParametersHelper.SetSingleParameter(Device);
		//}

		public RelayCommand ResetAUPropertiesCommand { get; private set; }
		void OnResetAUProperties()
		{
			foreach (var property in Device.Properties)
			{
				var driverProperty = Device.Driver.Properties.FirstOrDefault(x => x.Name == property.Name);
				if (driverProperty != null)
				{
					property.Value = driverProperty.Default;
				}
			}
			PropertiesViewModel = new PropertiesViewModel(Device);
			OnPropertyChanged("PropertiesViewModel");
		}
		#region OPC
		public bool CanOPCUsed
		{
			get { return Device.Driver.IsPlaceable; }
		}

		public bool IsOPCUsed
		{
			get { return Device.IsOPCUsed; }
			set
			{
				Device.IsOPCUsed = value;
				OnPropertyChanged(() => IsOPCUsed);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}
		#endregion
	}
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DevicesModule.DeviceProperties;
using DevicesModule.ViewModels;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections;
using Infrastructure.Common.TreeList;

namespace DiagnosticsModule.ViewModels
{
	public class DevicesTreeViewModel : DialogViewModel
	{
		public DevicesTreeViewModel()
		{
			Title = "Дерево устройств";
			Devices = BuildTree();
			Devices2 = BuildTree();
			ExpandCollapse(Devices2);

			Zones = new ObservableCollection<ZoneViewModel>(
				from zone in FiresecManager.Zones
				orderby zone.No
				select new ZoneViewModel(zone));
			if (Zones.Count > 0)
				for (int i = 0; i < 1000; i++)
					Zones.Add(new ZoneViewModel(new Zone() { No = i }));
		}

		public ObservableCollection<ZoneViewModel> Zones { get; private set; }

		public ObservableCollection<DeviceViewModel2> Devices { get; private set; }
		public ObservableCollection<DeviceViewModel2> Devices2 { get; private set; }

		private ObservableCollection<DeviceViewModel2> BuildTree()
		{
			var devices = new ObservableCollection<DeviceViewModel2>();
			var deviceViewModel = AddDevice(FiresecManager.FiresecConfiguration.DeviceConfiguration.RootDevice);
			devices.Add(deviceViewModel);
			if (deviceViewModel.Children.Count > 0 && deviceViewModel.Children[0].Children.Count > 0)
				for (int i = 0; i < 1000; i++)
					deviceViewModel.Children[0].Children.Add(deviceViewModel.Children[0].Children[0]);
			return devices;
		}
		private void ExpandCollapse(ObservableCollection<DeviceViewModel2> devices)
		{
			foreach (var device in devices)
			{
				device.IsExpanded = true;
				//ExpandCollapse(device.Children);
				//device.IsExpanded = false;
			}
		}

		public DeviceViewModel2 AddDevice(Device device)
		{
			var deviceViewModel = new DeviceViewModel2(device);
			//foreach (var childDevice in device.Children)
			//    deviceViewModel.Children.Add(AddDevice(childDevice));
			return deviceViewModel;
		}
	}


	public class DeviceViewModel2 : TreeItemViewModel<DeviceViewModel2> , ITreeNodeModel
	{
		public Device Device { get; private set; }
		public PropertiesViewModel PropertiesViewModel { get; private set; }

		public DeviceViewModel2(Device device)
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			AddToParentCommand = new RelayCommand(OnAddToParent, CanAddToParent);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			ShowZoneLogicCommand = new RelayCommand(OnShowZoneLogic, CanShowZoneLogic);
			ShowZoneOrLogicCommand = new RelayCommand(OnShowZoneOrLogic);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties, CanShowProperties);
			ShowZoneCommand = new RelayCommand(OnShowZone);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan);

			Device = device;
			PropertiesViewModel = new PropertiesViewModel(device);

			AvailvableDrivers = new ObservableCollection<Driver>();
			UpdateDriver();
			device.Changed += new Action(device_Changed);
			device.AUParametersChanged += new Action(device_AUParametersChanged);
		}

		void device_Changed()
		{
			OnPropertyChanged("Address");
			OnPropertyChanged("PresentationZone");
			OnPropertyChanged("EditingPresentationZone");
			OnPropertyChanged("HasExternalDevices");
		}

		void device_AUParametersChanged()
		{
			UpdataConfigurationProperties();
			PropertiesViewModel.IsAuParametersReady = true;
		}

		public void UpdataConfigurationProperties()
		{
			PropertiesViewModel = new PropertiesViewModel(Device) { ParameterVis = true };
			OnPropertyChanged("PropertiesViewModel");
		}

		public void Update()
		{
			IsExpanded = false;
			IsExpanded = true;
			OnPropertyChanged("HasChildren");
			OnPropertyChanged("IsOnPlan");
		}

		public string Address
		{
			get { return Device.PresentationAddress; }
			set
			{
				Device.SetAddress(value);
				if (Driver.IsChildAddressReservedRange)
				{
					foreach (var deviceViewModel in Children)
					{
						deviceViewModel.OnPropertyChanged("Address");
					}
				}
				ServiceFactory.SaveService.FSChanged = true;
				OnPropertyChanged("Address");
			}
		}

		public bool HasDifferences
		{
			get { return Device.HasDifferences; }
			set { }
		}

		public string XXXPresentationZone { get; set; }

		public string Description
		{
			get { return Device.Description; }
			set
			{
				Device.Description = value;
				Device.OnChanged();
				OnPropertyChanged("Description");
				ServiceFactory.SaveService.FSChanged = true;
			}
		}

		#region Zone
		public bool IsZoneDevice
		{
			get { return Driver.IsZoneDevice && !FiresecManager.FiresecConfiguration.IsChildMPT(Device); }
		}

		public string PresentationZone
		{
			get { return FiresecManager.FiresecConfiguration.GetPresentationZone(Device); }
		}

		public string EditingPresentationZone
		{
			get
			{
				return "";
				var presentationZone = FiresecManager.FiresecConfiguration.GetPresentationZone(Device);
				if (string.IsNullOrEmpty(presentationZone))
				{
					if (Driver.IsZoneDevice && !FiresecManager.FiresecConfiguration.IsChildMPT(Device))
						presentationZone = "Нажмите для выбора зон";
					if (Driver.IsZoneLogicDevice)
						presentationZone = "Нажмите для настройки логики";
				}
				return presentationZone;
			}
		}

		public bool IsZoneOrLogic
		{
			get { return Driver.IsZoneDevice || Driver.IsZoneLogicDevice || Driver.DriverType == DriverType.Indicator || Driver.DriverType == DriverType.PDUDirection; }
		}

		public RelayCommand ShowZoneOrLogicCommand { get; private set; }
		void OnShowZoneOrLogic()
		{
			if (Driver.IsZoneDevice)
			{
				if (!FiresecManager.FiresecConfiguration.IsChildMPT(Device))
				{
					var zoneSelectationViewModel = new ZoneSelectationViewModel(Device);
					if (DialogService.ShowModalWindow(zoneSelectationViewModel))
					{
						ServiceFactory.SaveService.FSChanged = true;
					}
				}
			}
			if (Driver.IsZoneLogicDevice)
			{
				OnShowZoneLogic();
			}
			if (Driver.DriverType == DriverType.Indicator)
			{
				OnShowIndicatorLogic();
			}
			if (Driver.DriverType == DriverType.PDUDirection)
			{
				if (DialogService.ShowModalWindow(new PDUDetailsViewModel(Device)))
					ServiceFactory.SaveService.FSChanged = true;
			}
			OnPropertyChanged("PresentationZone");
		}

		public RelayCommand ShowZoneLogicCommand { get; private set; }
		void OnShowZoneLogic()
		{
			var zoneLogicViewModel = new ZoneLogicViewModel(Device);
			if (DialogService.ShowModalWindow(zoneLogicViewModel))
			{
				ServiceFactory.SaveService.FSChanged = true;
			}
			OnPropertyChanged("PresentationZone");
		}
		bool CanShowZoneLogic()
		{
			return (Driver.IsZoneLogicDevice && !Device.IsNotUsed);
		}
		#endregion

		public bool HasExternalDevices
		{
			get { return Device.HasExternalDevices; }
		}

		public string ConnectedTo
		{
			get { return Device.ConnectedTo; }
		}

		public bool IsUsed
		{
			get { return !Device.IsNotUsed; }
			set
			{
				FiresecManager.FiresecConfiguration.SetIsNotUsed(Device, !value);
				OnPropertyChanged("IsUsed");
				OnPropertyChanged("ShowOnPlan");
				ServiceFactory.SaveService.FSChanged = true;
			}
		}

		public bool IsOnPlan
		{
			get { return Device.PlanElementUIDs.Count > 0; }
		}
		public bool ShowOnPlan
		{
			get { return !Device.IsNotUsed && (Device.Driver.IsPlaceable || Device.Children.Count > 0); }
		}

		void OnShowIndicatorLogic()
		{
			var indicatorDetailsViewModel = new IndicatorDetailsViewModel(Device);
			if (DialogService.ShowModalWindow(indicatorDetailsViewModel))
				ServiceFactory.SaveService.FSChanged = true;
			OnPropertyChanged("PresentationZone");
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
		}
		public bool CanAdd()
		{
			if (FiresecManager.FiresecConfiguration.IsChildMPT(Device))
				return false;
			return (Driver.CanAddChildren && Driver.AutoChild == Guid.Empty);
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
		}
		bool CanRemove()
		{
			return !(Driver.IsAutoCreate || Parent == null || Parent.Driver.AutoChild == Driver.UID);
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			switch (Device.Driver.DriverType)
			{
				case DriverType.Indicator:
					OnShowIndicatorLogic();
					break;

				case DriverType.Valve:
					if (DialogService.ShowModalWindow(new ValveDetailsViewModel(Device)))
					{
						ServiceFactory.SaveService.FSChanged = true;
						OnPropertyChanged("HasExternalDevices");
					}
					break;

				case DriverType.Pump:
				case DriverType.JokeyPump:
				case DriverType.Compressor:
				case DriverType.CompensationPump:
					if (DialogService.ShowModalWindow(new PumpDetailsViewModel(Device)))
						ServiceFactory.SaveService.FSChanged = true;
					break;

				case DriverType.PDUDirection:
					if (DialogService.ShowModalWindow(new PDUDetailsViewModel(Device)))
						ServiceFactory.SaveService.FSChanged = true;
					break;

				case DriverType.UOO_TL:
					if (DialogService.ShowModalWindow(new UOOTLDetailsViewModel(Device)))
						ServiceFactory.SaveService.FSChanged = true;
					break;

				case DriverType.MPT:
					if (DialogService.ShowModalWindow(new MptDetailsViewModel(Device)))
						ServiceFactory.SaveService.FSChanged = true;
					break;
			}
			OnPropertyChanged("PresentationZone");
		}
		bool CanShowProperties()
		{
			switch (Device.Driver.DriverType)
			{
				case DriverType.Indicator:
				case DriverType.Valve:
				case DriverType.Pump:
				case DriverType.JokeyPump:
				case DriverType.Compressor:
				case DriverType.CompensationPump:
				case DriverType.PDUDirection:
				case DriverType.UOO_TL:
				case DriverType.MPT:
					return true;
			}
			return false;
		}

		public RelayCommand ShowZoneCommand { get; private set; }
		void OnShowZone()
		{
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
		}

		public Driver Driver
		{
			get { return Device.Driver; }
			set
			{
				if (Device.Driver.DriverType != value.DriverType)
				{
					FiresecManager.FiresecConfiguration.ChangeDriver(Device, value);
					OnPropertyChanged("Device");
					OnPropertyChanged("Driver");
					PropertiesViewModel = new PropertiesViewModel(Device);
					OnPropertyChanged("PropertiesViewModel");
					Update();
					ServiceFactory.SaveService.FSChanged = true;
				}
			}
		}

		public ObservableCollection<Driver> AvailvableDrivers { get; private set; }

		void UpdateDriver()
		{
			AvailvableDrivers.Clear();
			if (CanChangeDriver)
			{
				switch (Device.Parent.Driver.DriverType)
				{
					case DriverType.AM4:
						AvailvableDrivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.AM_1));
						AvailvableDrivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.StopButton));
						AvailvableDrivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.StartButton));
						AvailvableDrivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.AutomaticButton));
						AvailvableDrivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.ShuzOnButton));
						AvailvableDrivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.ShuzOffButton));
						AvailvableDrivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.ShuzUnblockButton));
						AvailvableDrivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.AM1_O));
						AvailvableDrivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.AM1_T));
						break;

					case DriverType.AM4_P:
						AvailvableDrivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.AM1_O));
						AvailvableDrivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.AMP_4));
						break;

					default:
						foreach (var driverUID in Device.Parent.Driver.AvaliableChildren)
						{
							var driver = FiresecManager.Drivers.FirstOrDefault(x => x.UID == driverUID);
							if (CanDriverBeChanged(driver))
							{
								AvailvableDrivers.Add(driver);
							}
						}
						break;
				}
			}
		}

		public bool CanDriverBeChanged(Driver driver)
		{
			if (driver == null || Device.Parent == null)
				return false;

			if (Device.Parent.Driver.DriverType == DriverType.AM4)
				return true;
			if (Device.Parent.Driver.DriverType == DriverType.AM4_P)
				return true;

			if (driver.IsAutoCreate)
				return false;
			if (Device.Parent.Driver.IsChildAddressReservedRange)
				return false;
			return (driver.Category == DeviceCategoryType.Sensor) || (driver.Category == DeviceCategoryType.Effector);
		}

		public bool CanChangeDriver
		{
			get { return CanDriverBeChanged(Device.Driver); }
		}

		public bool IsBold { get; set; }

		public RelayCommand CopyCommand { get { return DevicesViewModel.Current.CopyCommand; } }
		public RelayCommand CutCommand { get { return DevicesViewModel.Current.CutCommand; } }
		public RelayCommand PasteCommand { get { return DevicesViewModel.Current.PasteCommand; } }
		public RelayCommand PasteAsCommand { get { return DevicesViewModel.Current.PasteAsCommand; } }

		#region ITreeNodeModel Members


		public IEnumerable GetChildren()
		{
			return Children;
		}

		public bool HasChild()
		{
			return HasChildren;
		}

		#endregion
	}


	public class DeviceViewModel3 : BaseViewModel
	{
		public bool IsExpanded { get; set; }
		public bool IsSelected { get; set; }

		ObservableCollection<DeviceViewModel3> _children;
		public ObservableCollection<DeviceViewModel3> Children
		{
			get { return _children; }
			set
			{
				_children = value;
				OnPropertyChanged("Children");
			}
		}

		public Device Device { get; private set; }
		public PropertiesViewModel PropertiesViewModel { get; private set; }

		public DeviceViewModel3(Device device)
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			AddToParentCommand = new RelayCommand(OnAddToParent, CanAddToParent);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			ShowZoneLogicCommand = new RelayCommand(OnShowZoneLogic, CanShowZoneLogic);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties, CanShowProperties);
			ShowZoneCommand = new RelayCommand(OnShowZone);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan);

			Children = new ObservableCollection<DeviceViewModel3>();

			Device = device;
			PropertiesViewModel = new PropertiesViewModel(device);

			AvailvableDrivers = new ObservableCollection<Driver>();
			UpdateDriver();
			device.Changed += new Action(device_Changed);
		}

		void device_Changed()
		{
			OnPropertyChanged("Address");
			OnPropertyChanged("PresentationZone");
			OnPropertyChanged("HasExternalDevices");
		}

		public void UpdataConfigurationProperties()
		{
			PropertiesViewModel = new PropertiesViewModel(Device) { ParameterVis = true };
			OnPropertyChanged("PropertiesViewModel");
		}

		public void Update()
		{
			IsExpanded = false;
			IsExpanded = true;
			OnPropertyChanged("HasChildren");
			OnPropertyChanged("IsOnPlan");
		}

		public string Address
		{
			get { return Device.PresentationAddress; }
			set
			{
				if (Device.Parent.Children.Where(x => x != Device).Any(x => x.PresentationAddress == value))
				{
					MessageBoxService.Show("Устройство с таким адресом уже существует");
				}
				else
				{
					Device.SetAddress(value);
					if (Driver.IsChildAddressReservedRange)
					{
						foreach (var deviceViewModel in Children)
						{
							deviceViewModel.OnPropertyChanged("Address");
						}
					}
					ServiceFactory.SaveService.FSChanged = true;
				}
				OnPropertyChanged("Address");
			}
		}

		public string Description
		{
			get { return Device.Description; }
			set
			{
				Device.Description = value;
				Device.OnChanged();
				OnPropertyChanged("Description");
				ServiceFactory.SaveService.FSChanged = true;
			}
		}

		public bool IsZoneDevice
		{
			get { return Driver.IsZoneDevice && !FiresecManager.FiresecConfiguration.IsChildMPT(Device); }
		}

		public IEnumerable<Zone> Zones
		{
			get { return from Zone zone in FiresecManager.Zones orderby zone.No select zone; }
		}

		public Zone Zone
		{
			get { return FiresecManager.Zones.FirstOrDefault(x => x.UID == Device.ZoneUID); }
			set
			{
				if (Device.ZoneUID != value.UID)
				{
					FiresecManager.FiresecConfiguration.AddDeviceToZone(Device, value);
					OnPropertyChanged("Zone");
					ServiceFactory.SaveService.FSChanged = true;
				}
			}
		}

		public string PresentationZone
		{
			get { return FiresecManager.FiresecConfiguration.GetPresentationZone(Device); }
		}

		public bool HasExternalDevices
		{
			get { return Device.HasExternalDevices; }
		}

		public string ConnectedTo
		{
			get { return Device.ConnectedTo; }
		}

		public bool IsUsed
		{
			get { return !Device.IsNotUsed; }
			set
			{
				FiresecManager.FiresecConfiguration.SetIsNotUsed(Device, !value);
				OnPropertyChanged("IsUsed");
				OnPropertyChanged("ShowOnPlan");
				ServiceFactory.SaveService.FSChanged = true;
			}
		}

		public bool IsOnPlan
		{
			get { return Device.PlanElementUIDs.Count > 0; }
		}
		public bool ShowOnPlan
		{
			get { return !Device.IsNotUsed && (Device.Driver.IsPlaceable || Device.Children.Count > 0); }
		}

		public RelayCommand ShowZoneLogicCommand { get; private set; }
		void OnShowZoneLogic()
		{
			var zoneLogicViewModel = new ZoneLogicViewModel(Device);
			if (DialogService.ShowModalWindow(zoneLogicViewModel))
			{
				ServiceFactory.SaveService.FSChanged = true;
			}
			OnPropertyChanged("PresentationZone");
		}
		bool CanShowZoneLogic()
		{
			return (Driver.IsZoneLogicDevice && !Device.IsNotUsed);
		}

		void OnShowIndicatorLogic()
		{
			var indicatorDetailsViewModel = new IndicatorDetailsViewModel(Device);
			if (DialogService.ShowModalWindow(indicatorDetailsViewModel))
				ServiceFactory.SaveService.FSChanged = true;
			OnPropertyChanged("PresentationZone");
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
		}
		public bool CanAdd()
		{
			if (FiresecManager.FiresecConfiguration.IsChildMPT(Device))
				return false;
			return (Driver.CanAddChildren && Driver.AutoChild == Guid.Empty);
		}

		public RelayCommand AddToParentCommand { get; private set; }
		void OnAddToParent()
		{
		}
		public bool CanAddToParent()
		{
			return true;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
		}
		bool CanRemove()
		{
			return true;
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
		}
		bool CanShowProperties()
		{
			return true;
		}

		public RelayCommand ShowZoneCommand { get; private set; }
		void OnShowZone()
		{
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
		}

		public Driver Driver
		{
			get { return Device.Driver; }
			set
			{
			}
		}

		public ObservableCollection<Driver> AvailvableDrivers { get; private set; }

		void UpdateDriver()
		{
			AvailvableDrivers.Clear();
			if (CanChangeDriver)
			{
				switch (Device.Parent.Driver.DriverType)
				{
					case DriverType.AM4:
						AvailvableDrivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.AM_1));
						AvailvableDrivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.StopButton));
						AvailvableDrivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.StartButton));
						AvailvableDrivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.AutomaticButton));
						AvailvableDrivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.ShuzOnButton));
						AvailvableDrivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.ShuzOffButton));
						AvailvableDrivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.ShuzUnblockButton));
						AvailvableDrivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.AM1_O));
						AvailvableDrivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.AM1_T));
						break;

					case DriverType.AM4_P:
						AvailvableDrivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.AM1_O));
						AvailvableDrivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.AMP_4));
						break;

					default:
						foreach (var driverUID in Device.Parent.Driver.AvaliableChildren)
						{
							var driver = FiresecManager.Drivers.FirstOrDefault(x => x.UID == driverUID);
							if (CanDriverBeChanged(driver))
							{
								AvailvableDrivers.Add(driver);
							}
						}
						break;
				}
			}
		}

		public bool CanDriverBeChanged(Driver driver)
		{
			if (driver == null || Device.Parent == null)
				return false;

			if (Device.Parent.Driver.DriverType == DriverType.AM4)
				return true;
			if (Device.Parent.Driver.DriverType == DriverType.AM4_P)
				return true;

			if (driver.IsAutoCreate)
				return false;
			if (Device.Parent.Driver.IsChildAddressReservedRange)
				return false;
			return (driver.Category == DeviceCategoryType.Sensor) || (driver.Category == DeviceCategoryType.Effector);
		}

		public bool CanChangeDriver
		{
			get { return CanDriverBeChanged(Device.Driver); }
		}

		public RelayCommand CopyCommand { get { return DevicesViewModel.Current.CopyCommand; } }
		public RelayCommand CutCommand { get { return DevicesViewModel.Current.CutCommand; } }
		public RelayCommand PasteCommand { get { return DevicesViewModel.Current.PasteCommand; } }
		public RelayCommand PasteAsCommand { get { return DevicesViewModel.Current.PasteAsCommand; } }
	}
}
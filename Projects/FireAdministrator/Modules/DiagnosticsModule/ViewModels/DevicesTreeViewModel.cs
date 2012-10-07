using System.Linq;
using System.Collections.ObjectModel;
using DevicesModule.ViewModels;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;
using DevicesModule.DeviceProperties;
using Infrastructure.Common;
using System;
using Infrastructure.Common.Windows;
using Infrastructure;
using System.Collections.Generic;

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
			for (int i = 0; i < 1000; i++)
				Zones.Add(Zones[0]);
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
				ExpandCollapse(device.Children);
				//device.IsExpanded = false;
			}
		}

		public DeviceViewModel2 AddDevice(Device device)
		{
			var deviceViewModel = new DeviceViewModel2(device);
			foreach (var childDevice in device.Children)
				deviceViewModel.Children.Add(AddDevice(childDevice));
			return deviceViewModel;
		}
	}
	public class DeviceViewModel2 : BaseViewModel
	{
		public bool IsExpanded { get; set; }

		ObservableCollection<DeviceViewModel2> _children;
		public ObservableCollection<DeviceViewModel2> Children
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

		public DeviceViewModel2(Device device)
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			AddToParentCommand = new RelayCommand(OnAddToParent, CanAddToParent);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			ShowZoneLogicCommand = new RelayCommand(OnShowZoneLogic, CanShowZoneLogic);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties, CanShowProperties);
			ShowZoneCommand = new RelayCommand(OnShowZone);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan);

			Children = new ObservableCollection<DeviceViewModel2>();

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
					ServiceFactory.SaveService.DevicesChanged = true;
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
				ServiceFactory.SaveService.DevicesChanged = true;
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
					ServiceFactory.SaveService.DevicesChanged = true;
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
				ServiceFactory.SaveService.DevicesChanged = true;
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
				ServiceFactory.SaveService.DevicesChanged = true;
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
				ServiceFactory.SaveService.DevicesChanged = true;
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
				if (Device.Driver.DriverType != value.DriverType)
				{
					FiresecManager.FiresecConfiguration.ChangeDriver(Device, value);
					OnPropertyChanged("Device");
					OnPropertyChanged("Driver");
					PropertiesViewModel = new PropertiesViewModel(Device);
					OnPropertyChanged("PropertiesViewModel");
					Update();
					ServiceFactory.SaveService.DevicesChanged = true;
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

		public RelayCommand CopyCommand { get { return DevicesViewModel.Current.CopyCommand; } }
		public RelayCommand CutCommand { get { return DevicesViewModel.Current.CutCommand; } }
		public RelayCommand PasteCommand { get { return DevicesViewModel.Current.PasteCommand; } }
		public RelayCommand PasteAsCommand { get { return DevicesViewModel.Current.PasteAsCommand; } }
	}

}
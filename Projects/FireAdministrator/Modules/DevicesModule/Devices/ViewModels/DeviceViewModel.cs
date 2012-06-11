using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DevicesModule.DeviceProperties;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Events;

namespace DevicesModule.ViewModels
{
	public class DeviceViewModel : TreeBaseViewModel<DeviceViewModel>
	{
		public Device Device { get; private set; }
		public PropertiesViewModel PropertiesViewModel { get; private set; }

		public DeviceViewModel(Device device, ObservableCollection<DeviceViewModel> sourceDevices)
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			AddManyCommand = new RelayCommand(OnAddMany, CanAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			ShowZoneLogicCommand = new RelayCommand(OnShowZoneLogic);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties, CanShowProperties);
			ShowZoneCommand = new RelayCommand(OnShowZone);

			Children = new ObservableCollection<DeviceViewModel>();

			Source = sourceDevices;
			Device = device;
			PropertiesViewModel = new PropertiesViewModel(device);

			AvailvableDrivers = new ObservableCollection<Driver>();
			UpdateDriver();
		}

		public void Update()
		{
			IsExpanded = false;
			IsExpanded = true;
			OnPropertyChanged("HasChildren");
		}

		public Driver Driver
		{
			get { return Device.Driver; }
			set
			{
				if (Device.Driver.DriverType != value.DriverType)
				{
					Device.Driver = value;
					Device.DriverUID = value.UID;
					OnPropertyChanged("Device");
					OnPropertyChanged("Device.Driver");
					OnPropertyChanged("PresentationZone");
					ServiceFactory.SaveService.DevicesChanged = true;
					DevicesViewModel.Current.UpdateExternalDevices();
				}
			}
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
				OnPropertyChanged("Description");

				ServiceFactory.SaveService.DevicesChanged = true;
			}
		}

		public IEnumerable<Zone> Zones
		{
			get
			{
				return from Zone zone in FiresecManager.DeviceConfiguration.Zones
					   orderby zone.No
					   select zone;
			}
		}

		public Zone Zone
		{
			get { return FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == Device.ZoneNo); }
			set
			{
				if (Device.ZoneNo != value.No)
				{
					Device.ZoneNo = value.No;
					OnPropertyChanged("Zone");
					ServiceFactory.SaveService.DevicesChanged = true;
					DevicesViewModel.Current.UpdateExternalDevices();
				}
			}
		}

		public string PresentationZone
		{
			get { return Device.GetPersentationZone(); }
		}

		public bool HasExternalDevices
		{
			get { return FiresecManager.HasExternalDevices(Device); }
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
				Device.IsNotUsed = !value;
				OnPropertyChanged("IsUsed");
				ServiceFactory.SaveService.DevicesChanged = true;
			}
		}

		public RelayCommand ShowZoneLogicCommand { get; private set; }
		void OnShowZoneLogic()
		{
			var zoneLogicViewModel = new ZoneLogicViewModel(Device);
			if (DialogService.ShowModalWindow(zoneLogicViewModel))
			{
				ServiceFactory.SaveService.DevicesChanged = true;
				DevicesViewModel.Current.UpdateExternalDevices();
			}
			OnPropertyChanged("PresentationZone");
		}

		void OnShowIndicatorLogic()
		{
			var indicatorDetailsViewModel = new IndicatorDetailsViewModel(Device);
			if (DialogService.ShowModalWindow(indicatorDetailsViewModel))
				ServiceFactory.SaveService.DevicesChanged = true;
			OnPropertyChanged("PresentationZone");
		}

		public bool CanAdd()
		{
			return (Driver.CanAddChildren && Driver.AutoChild == Guid.Empty);
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			if (DialogService.ShowModalWindow(new NewDeviceViewModel(this)))
			{
				ServiceFactory.SaveService.DevicesChanged = true;
				DevicesViewModel.UpdateGuardVisibility();
			}
		}

		public RelayCommand AddManyCommand { get; private set; }
		void OnAddMany()
		{
			if (DialogService.ShowModalWindow(new NewDeviceRangeViewModel(this)))
			{
				ServiceFactory.SaveService.DevicesChanged = true;
				DevicesViewModel.UpdateGuardVisibility();
			}
		}

		bool CanRemove()
		{
			return !(Driver.IsAutoCreate || Parent == null || Parent.Driver.AutoChild == Driver.UID);
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			var index = DevicesViewModel.Current.Devices.IndexOf(DevicesViewModel.Current.SelectedDevice);

			Parent.IsExpanded = false;
			Parent.Device.Children.Remove(Device);
			Parent.Children.Remove(this);
			Parent.Update();
			Parent.IsExpanded = true;
			Parent = null;

			FiresecManager.DeviceConfiguration.Update();
			ServiceFactory.SaveService.DevicesChanged = true;
			DevicesViewModel.UpdateGuardVisibility();
			FiresecManager.InvalidateConfiguration();

			index = Math.Min(index, DevicesViewModel.Current.Devices.Count - 1);
			DevicesViewModel.Current.SelectedDevice = DevicesViewModel.Current.Devices[index];

			DevicesViewModel.Current.UpdateExternalDevices();
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
				case DriverType.Direction:
				case DriverType.UOO_TL:
				case DriverType.MPT:
					return true;
			}
			return false;
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			switch (Device.Driver.DriverType)
			{
				case DriverType.Indicator:
					OnShowIndicatorLogic();
					FiresecManager.InvalidateConfiguration();
					break;

				case DriverType.Valve:
					if (DialogService.ShowModalWindow(new ValveDetailsViewModel(Device)))
					{
						ServiceFactory.SaveService.DevicesChanged = true;
						OnPropertyChanged("HasExternalDevices");
					}
					break;

				case DriverType.Pump:
				case DriverType.JokeyPump:
				case DriverType.Compressor:
				case DriverType.CompensationPump:
					if (DialogService.ShowModalWindow(new PumpDetailsViewModel(Device)))
						ServiceFactory.SaveService.DevicesChanged = true;
					break;

				case DriverType.Direction:
					if (DialogService.ShowModalWindow(new GroupDetailsViewModel(Device)))
						ServiceFactory.SaveService.DevicesChanged = true;
					break;

				case DriverType.UOO_TL:
					if (DialogService.ShowModalWindow(new TelephoneLineDetailsViewModel(Device)))
						ServiceFactory.SaveService.DevicesChanged = true;
					break;

				case DriverType.MPT:
					if (DialogService.ShowModalWindow(new MptDetailsViewModel(Device)))
						ServiceFactory.SaveService.DevicesChanged = true;
					break;
			}
			OnPropertyChanged("PresentationZone");
		}

		public RelayCommand ShowZoneCommand { get; private set; }
		void OnShowZone()
		{
			if (Device.ZoneNo.HasValue)
				ServiceFactory.Events.GetEvent<ShowZoneEvent>().Publish(Device.ZoneNo.Value);
		}

		public ObservableCollection<Driver> AvailvableDrivers { get; private set; }

		void UpdateDriver()
		{
			AvailvableDrivers.Clear();
			if (CanChangeDriver)
			{
				foreach (var driverUID in Device.Parent.Driver.AvaliableChildren)
				{
					var driver = FiresecManager.Drivers.FirstOrDefault(x => x.UID == driverUID);
					if (CanDriverBeChanged(driver))
					{
						AvailvableDrivers.Add(driver);
					}
				}
			}
		}

		public bool CanDriverBeChanged(Driver driver)
		{
			if (driver == null)
				return false;
			if (driver.IsAutoCreate)
				return false;
			if (Parent != null && Parent.Driver.IsChildAddressReservedRange)
				return false;
			return (driver.Category == DeviceCategoryType.Sensor) || (driver.Category == DeviceCategoryType.Effector);
		}

		public bool CanChangeDriver
		{
			get
			{
				return CanDriverBeChanged(Device.Driver);
			}
		}

		public RelayCommand CopyCommand { get { return DevicesViewModel.Current.CopyCommand; } }
		public RelayCommand CutCommand { get { return DevicesViewModel.Current.CutCommand; } }
		public RelayCommand PasteCommand { get { return DevicesViewModel.Current.PasteCommand; } }
		public RelayCommand PasteAsCommand { get { return DevicesViewModel.Current.PasteAsCommand; } }
	}
}
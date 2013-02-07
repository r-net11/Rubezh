using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Events;
using XFiresecAPI;
using Infrastructure.Events;
using System;
using System.Collections.Generic;

namespace GKModule.ViewModels
{
	public class DeviceViewModel : TreeItemViewModel<DeviceViewModel>
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

			Device = device;
			PropertiesViewModel = new PropertiesViewModel(device);
			device.Changed += new System.Action(OnChanged);

			AvailvableDrivers = new ObservableCollection<XDriver>();
			UpdateDriver();
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
			IsExpanded = false;
			IsExpanded = true;
			OnPropertyChanged("HasChildren");
			OnPropertyChanged("IsOnPlan");
		}

		public string Address
		{
			get { return Device.Address; }
			set
			{
				if (Device.Parent.Children.Where(x => (x != Device) && (x.Driver.IsAutoCreate == false)).Any(x => x.Address == value))
				{
					MessageBoxService.Show("Устройство с таким адресом уже существует");
				}
				else
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
            var newDeviceViewModel = new NewDeviceViewModel(this);
            if (newDeviceViewModel.Drivers.Count == 1)
            {
                newDeviceViewModel.SaveCommand.Execute();
				DevicesViewModel.Current.SelectedDevice = newDeviceViewModel.AddedDevice;
                ServiceFactory.SaveService.GKChanged = true;
                return;
            }
			if (DialogService.ShowModalWindow(newDeviceViewModel))
			{
				DevicesViewModel.Current.SelectedDevice = newDeviceViewModel.AddedDevice;
				ServiceFactory.SaveService.GKChanged = true;
			}
		}
        public bool CanAdd()
        {
			if (Driver.Children.Count > 0)
				return true;
			if (Driver.DriverType == XDriverType.MPT && Parent != null && Parent.Driver.DriverType == XDriverType.KAU)
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
			Parent.IsExpanded = false;
			XManager.RemoveDevice(Parent.Device, Device);
			var parent = Parent;
			if (parent != null)
			{
				var index = parent.Children.IndexOf(this);
				parent.Children.Remove(this);
				parent.Update();

				ServiceFactory.SaveService.GKChanged = true;

				index = Math.Min(index, parent.Children.Count - 1);
				DevicesViewModel.Current.AllDevices.Remove(this);
				DevicesViewModel.Current.SelectedDevice = index >= 0 ? parent.Children[index] : parent;
			}
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

		public bool IsOnPlan
		{
			get { return Device.PlanElementUIDs.Count > 0; }
		}
		public bool ShowOnPlan
		{
			get { return !Device.IsNotUsed && (Device.Driver.IsDeviceOnShleif || Device.Children.Count > 0); }
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			if (Device.PlanElementUIDs.Count > 0)
				ServiceFactory.Events.GetEvent<FindElementEvent>().Publish(Device.PlanElementUIDs[0]);
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
			var zonesSelectationViewModel = new ZonesSelectationViewModel(Device.Zones);
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
					OnPropertyChanged("Device");
					OnPropertyChanged("Driver");
					PropertiesViewModel = new PropertiesViewModel(Device);
					OnPropertyChanged("PropertiesViewModel");
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
						AvailvableDrivers.Add(XManager.DriversConfiguration.XDrivers.FirstOrDefault(x => x.DriverType == XDriverType.AM_1));
						AvailvableDrivers.Add(XManager.DriversConfiguration.XDrivers.FirstOrDefault(x => x.DriverType == XDriverType.AM1_O));
						AvailvableDrivers.Add(XManager.DriversConfiguration.XDrivers.FirstOrDefault(x => x.DriverType == XDriverType.AM1_T));
						break;

					case XDriverType.AMP_4:
						AvailvableDrivers.Add(XManager.DriversConfiguration.XDrivers.FirstOrDefault(x => x.DriverType == XDriverType.AM1_O));
						AvailvableDrivers.Add(XManager.DriversConfiguration.XDrivers.FirstOrDefault(x => x.DriverType == XDriverType.AMP_1));
						break;

					default:
						foreach (var driverType in Device.Parent.Driver.Children)
						{
							var driver = XManager.DriversConfiguration.XDrivers.FirstOrDefault(x => x.DriverType == driverType);
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
			if (Device.Parent.Driver.DriverType == XDriverType.AMP_4)
				return true;

			if (driver.IsAutoCreate)
				return false;
			if (driver.IsGroupDevice)
				return false;
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
	}
}
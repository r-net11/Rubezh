using System;
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
	public class DeviceViewModel : TreeItemViewModel<DeviceViewModel>
	{
		public Device Device { get; private set; }
		public PropertiesViewModel PropertiesViewModel { get; private set; }

		public DeviceViewModel(Device device)
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
		}

		void device_Changed()
		{
			OnPropertyChanged("Address");
			OnPropertyChanged("PresentationZone");
            OnPropertyChanged("EditingPresentationZone");
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

		public bool IsLocal
		{
			get { return Device.IsLocal; }
			set { }
		}
		public bool IsRemote
		{
			get { return Device.IsRemote; }
			set { }
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
            var newDeviceViewModel = new NewDeviceViewModel(this);
			if (DialogService.ShowModalWindow(newDeviceViewModel))
			{
                //DevicesViewModel.Current.Select(newDeviceViewModel.CreatedDeviceViewModel.Device.UID);
				ServiceFactory.SaveService.FSChanged = true;
				DevicesViewModel.UpdateGuardVisibility();
			}
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
			if (Driver.IsPanel)
			{
				if (MessageBoxService.ShowQuestion("Вы действительно хотите удалить устройство") != System.Windows.MessageBoxResult.Yes)
					return;
			}

			FiresecManager.FiresecConfiguration.RemoveDevice(Device);
			var parent = Parent;
			if (parent != null)
			{
				var index = parent.Children.IndexOf(DevicesViewModel.Current.SelectedDevice);
				parent.Children.Remove(this);
				parent.Update();

				ServiceFactory.SaveService.FSChanged = true;
				DevicesViewModel.UpdateGuardVisibility();

				index = Math.Min(index, parent.Children.Count - 1);
				DevicesViewModel.Current.SelectedDevice = index >= 0 ? parent.Children[index] : parent;
			}
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
			if (Device.ZoneUID != Guid.Empty)
				ServiceFactory.Events.GetEvent<ShowZoneEvent>().Publish(Device.ZoneUID);
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			if (Device.PlanElementUIDs.Count > 0)
				ServiceFactory.Events.GetEvent<Infrustructure.Plans.Events.FindElementEvent>().Publish(Device.PlanElementUIDs[0]);
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

		public RelayCommand CopyCommand { get { return DevicesViewModel.Current.CopyCommand; } }
		public RelayCommand CutCommand { get { return DevicesViewModel.Current.CutCommand; } }
		public RelayCommand PasteCommand { get { return DevicesViewModel.Current.PasteCommand; } }
		public RelayCommand PasteAsCommand { get { return DevicesViewModel.Current.PasteAsCommand; } }

		public bool IsBold { get; set; }
	}
}
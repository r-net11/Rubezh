using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using XFiresecAPI;
using System.Collections.Generic;
using System;

namespace GKModule.ViewModels
{
	public class DeviceViewModel : TreeBaseViewModel<DeviceViewModel>
	{
		public XDevice Device { get; private set; }
		public PropertiesViewModel PropertiesViewModel { get; private set; }

		public DeviceViewModel(XDevice device, ObservableCollection<DeviceViewModel> sourceDevices)
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties, CanShowProperties);
			ShowLogicCommand = new RelayCommand(OnShowLogic, CanShowLogic);
            ShowZonesCommand = new RelayCommand(OnShowZones, CanShowZones);
            ShowZoneOrLogicCommand = new RelayCommand(OnShowZoneOrLogic, CanShowZoneOrLogic);

			Children = new ObservableCollection<DeviceViewModel>();

			Source = sourceDevices;
			Device = device;
			PropertiesViewModel = new PropertiesViewModel(device);
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
		}

		public XDriver Driver
		{
			get { return Device.Driver; }
		}

		public string Address
		{
			get { return Device.Address; }
			set
			{
				if (Device.Parent.Children.Where(x => x != Device).Any(x => x.Address == value))
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
						}
					}
				}
				OnPropertyChanged("Address");
				ServiceFactory.SaveService.XDevicesChanged = true;
			}
		}

		public string Description
		{
			get { return Device.Description; }
			set
			{
				Device.Description = value;
				OnPropertyChanged("Description");
			}
		}

		public bool CanAdd()
		{
			return (Driver.Children.Count > 0);
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			if (DialogService.ShowModalWindow(new NewDeviceViewModel(this)))
			{
				ServiceFactory.SaveService.XDevicesChanged = true;
			}
		}

		bool CanRemove()
		{
			return !(Driver.IsAutoCreate || Parent == null || (Parent.Driver.IsGroupDevice && Parent.Driver.GroupDeviceChildType == Driver.DriverType));
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			Parent.IsExpanded = false;
			Parent.Device.Children.Remove(Device);
			Parent.Children.Remove(this);
			Parent.Update();
			Parent.IsExpanded = true;
			Parent = null;

			XManager.DeviceConfiguration.Update();
			ServiceFactory.SaveService.XDevicesChanged = true;
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
		}
        bool CanShowProperties()
        {
            return false;
        }

        public string PresentationZone
        {
            get { return XManager.GetPresentationZone(Device); }
        }

		public RelayCommand ShowLogicCommand { get; private set; }
		void OnShowLogic()
		{
            if (DialogService.ShowModalWindow(new DeviceLogicViewModel(Device)))
            {
                OnPropertyChanged("PresentationZone");
                ServiceFactory.SaveService.XDevicesChanged = true;
            }
		}
        bool CanShowLogic()
        {
            return (Driver.HasLogic);
        }

        public RelayCommand ShowZonesCommand { get; private set; }
        void OnShowZones()
        {
            var zonesSelectationViewModel = new ZonesSelectationViewModel(Device.Zones);
            if (DialogService.ShowModalWindow(zonesSelectationViewModel))
            {
                Device.Zones = zonesSelectationViewModel.Zones;
                OnPropertyChanged("PresentationZone");
                ServiceFactory.SaveService.XDevicesChanged = true;
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

		public RelayCommand CopyCommand { get { return DevicesViewModel.Current.CopyCommand; } }
		public RelayCommand CutCommand { get { return DevicesViewModel.Current.CutCommand; } }
		public RelayCommand PasteCommand { get { return DevicesViewModel.Current.PasteCommand; } }
	}
}
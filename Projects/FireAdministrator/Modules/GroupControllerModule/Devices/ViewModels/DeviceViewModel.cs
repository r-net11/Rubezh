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

		public IEnumerable<XZone> Zones
		{
			get
			{
				return from XZone zone in XManager.DeviceConfiguration.Zones
					   orderby zone.No
					   select zone;
			}
		}

		public XZone Zone
		{
			get
			{
				var firstZoneUID = Device.Zones.FirstOrDefault();
				if (firstZoneUID != Guid.Empty)
				{
					return XManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == firstZoneUID);
				}
				return null;
			}
			set
			{
				var firstZoneUID = Device.Zones.FirstOrDefault();
				if (firstZoneUID != value.UID)
				{
					firstZoneUID = value.UID;
					OnPropertyChanged("Zone");
					ServiceFactory.SaveService.DevicesChanged = true;
				}
			}
		}

		public string PresentationZone
		{
			get { return XManager.GetPresentationZone(Device); }
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

		bool CanShowProperties()
		{
			return false;
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
		}

		bool CanShowLogic()
		{
			return (Driver.HasLogic);
		}

		public RelayCommand ShowLogicCommand { get; private set; }
		void OnShowLogic()
		{
			if (DialogService.ShowModalWindow(new DeviceLogicViewModel(Device)))
				ServiceFactory.SaveService.XDevicesChanged = true;
		}

		public RelayCommand CopyCommand { get { return DevicesViewModel.Current.CopyCommand; } }
		public RelayCommand CutCommand { get { return DevicesViewModel.Current.CutCommand; } }
		public RelayCommand PasteCommand { get { return DevicesViewModel.Current.PasteCommand; } }
	}
}
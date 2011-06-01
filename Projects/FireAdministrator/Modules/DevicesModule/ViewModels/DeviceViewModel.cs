using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using FiresecClient;
using Infrastructure.Common;
using System.Windows.Controls;
using System.Windows.Data;
using DevicesModule.PropertyBindings;
using System.Windows;
using DevicesModule.Views;
using Infrastructure;

namespace DevicesModule.ViewModels
{
    public class DeviceViewModel : TreeBaseViewModel<DeviceViewModel>
    {
        public Device Device;
        public Firesec.Metadata.drvType Driver;

        public DeviceViewModel()
        {
            Children = new ObservableCollection<DeviceViewModel>();
            AddCommand = new RelayCommand(OnAdd);
            AddManyCommand = new RelayCommand(OnAddMany);
            RemoveCommand = new RelayCommand(OnRemove);
            ShowZoneLogicCommand = new RelayCommand(OnShowZoneLogic);
            ShowIndicatorLogicCommand = new RelayCommand(OnShowIndicatorLogic);
        }

        public void Initialize(Device device, ObservableCollection<DeviceViewModel> sourceDevices)
        {
            Source = sourceDevices;

            Device = device;
            Driver = FiresecManager.CurrentConfiguration.Metadata.drv.FirstOrDefault(x => x.id == device.DriverId);

            SetProperties();

            Address = device.Address;
            Description = device.Description;
        }

        public List<StringProperty> StringProperties { get; set; }
        public List<BoolProperty> BoolProperties { get; set; }
        public List<EnumProperty> EnumProperties { get; set; }

        void SetProperties()
        {
            StringProperties = new List<StringProperty>();
            BoolProperties = new List<BoolProperty>();
            EnumProperties = new List<EnumProperty>();

            if (Driver.propInfo != null)
            {
                foreach (Firesec.Metadata.propInfoType propertyInfo in Driver.propInfo)
                {
                    if (propertyInfo.hidden == "1")
                        continue;
                    if ((propertyInfo.caption == "Заводской номер") || (propertyInfo.caption == "Версия микропрограммы"))
                        continue;

                    if (propertyInfo.param != null)
                    {
                        EnumProperties.Add(new EnumProperty(propertyInfo, Device));
                    }
                    else
                    {
                        switch (propertyInfo.type)
                        {
                            case "String":
                            case "Int":
                            case "Byte":
                                StringProperties.Add(new StringProperty(propertyInfo, Device));
                                break;
                            case "Bool":
                                BoolProperties.Add(new BoolProperty(propertyInfo, Device));
                                break;
                            default:
                                throw new Exception("Неизвестный тип свойства");
                        }
                    }
                }
            }
        }

        public void Update()
        {
            OnPropertyChanged("HasChildren");
        }

        public string DriverId
        {
            get
            {
                if (Device != null)
                    return Device.DriverId;
                return null;
            }
        }

        public ObservableCollection<ZoneViewModel> Zones
        {
            get
            {
                ObservableCollection<ZoneViewModel> ZoneViewModels = new ObservableCollection<ZoneViewModel>();
                foreach (Zone zone in FiresecManager.CurrentConfiguration.Zones)
                {
                    ZoneViewModel zoneViewModel = new ZoneViewModel(zone);
                    ZoneViewModels.Add(zoneViewModel);
                }
                return ZoneViewModels;
            }
        }

        public ZoneViewModel Zone
        {
            get
            {
                Zone zone = FiresecManager.CurrentConfiguration.Zones.FirstOrDefault(x => x.No == Device.ZoneNo);
                if (zone != null)
                {
                    ZoneViewModel zoneViewModel = new ZoneViewModel(zone);
                    return zoneViewModel;
                }
                return null;
            }
            set
            {
                Device.ZoneNo = value.No;
                OnPropertyChanged("Zone");
            }
        }

        public bool IsZoneDevice
        {
            get
            {
                return !((Driver.minZoneCardinality == "0") && (Driver.maxZoneCardinality == "0"));
            }
        }

        public bool IsIndicatorDevice
        {
            get
            {
                return (Driver.name == "Индикатор");
            }
        }

        public bool IsZoneLogicDevice
        {
            get
            {
                return ((Driver.options != null) && (Driver.options.Contains("ExtendedZoneLogic")));
            }
        }

        public bool CanAddChildren
        {
            get
            {
                List<Firesec.Metadata.drvType> childDrivers = new List<Firesec.Metadata.drvType>();

                foreach (Firesec.Metadata.drvType childDriver in FiresecManager.CurrentConfiguration.Metadata.drv)
                {
                    Firesec.Metadata.classType childClass = FiresecManager.CurrentConfiguration.Metadata.@class.FirstOrDefault(x => x.clsid == childDriver.clsid);
                    if ((childClass.parent != null) && (childClass.parent.Any(x => x.clsid == Driver.clsid)))
                    {
                        if ((childDriver.lim_parent != null) && (childDriver.lim_parent != Driver.id))
                            continue;
                        if (childDriver.acr_enabled == "1")
                            continue;
                        childDrivers.Add(childDriver);
                    }
                }

                return (childDrivers.Count > 0);
            }
        }

        public string ShortDriverName
        {
            get
            {
                return Driver.shortName;
            }
        }

        public string DriverName
        {
            get
            {
                return Driver.name;
            }
        }

        public bool HasAddress
        {
            get
            {
                return (!string.IsNullOrEmpty(Address));
            }
        }

        string address;
        public string Address
        {
            get { return address; }
            set
            {
                address = value;
                OnPropertyChanged("Address");
            }
        }

        public bool CanEditAddress
        {
            get
            {
                if (Driver.ar_no_addr != null)
                {
                    if (Driver.ar_no_addr == "1")
                        return false;

                    if (Driver.acr_enabled == "1")
                        return false;
                }
                return true;
            }
        }

        string description;
        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                OnPropertyChanged("Description");
            }
        }

        public string ConnectedTo
        {
            get
            {
                if (Parent == null)
                    return null;
                else
                {
                    string parentPart = Parent.ShortDriverName;
                    if (Parent.Driver.ar_no_addr != "1")
                        parentPart += " - " + Parent.Address;

                    if (Parent.ConnectedTo == null)
                        return parentPart;

                    if (Parent.Parent.ConnectedTo == null)
                        return parentPart;

                    return parentPart + @"\" + Parent.ConnectedTo;
                }
            }
        }

        public bool HasImage
        {
            get
            {
                return (ImageSource != @"C:/Program Files/Firesec/Icons/Device_Device.ico");
            }
        }

        public string ImageSource
        {
            get
            {
                string ImageName;
                if (!string.IsNullOrEmpty(Driver.dev_icon))
                {
                    ImageName = Driver.dev_icon;
                }
                else
                {
                    Firesec.Metadata.classType metadataClass = FiresecManager.CurrentConfiguration.Metadata.@class.FirstOrDefault(x => x.clsid == Driver.clsid);
                    ImageName = metadataClass.param.FirstOrDefault(x => x.name == "Icon").value;
                }

                return @"C:/Program Files/Firesec/Icons/" + ImageName + ".ico";
                //return @"pack://application:,,,/Icons/" + ImageName + ".ico";
            }
        }

        public RelayCommand ShowZoneLogicCommand { get; private set; }
        void OnShowZoneLogic()
        {
            ZoneLogicViewModel zoneLogicViewModel = new ZoneLogicViewModel();
            zoneLogicViewModel.Initialize(Device.ZoneLogic);
            bool result = ServiceFactory.UserDialogs.ShowModalWindow(zoneLogicViewModel);
            if (result)
            {
                Device.ZoneLogic = zoneLogicViewModel.Save();
            }
        }

        public RelayCommand ShowIndicatorLogicCommand { get; private set; }
        void OnShowIndicatorLogic()
        {
            IndicatorDetailsViewModel indicatorDetailsViewModel = new IndicatorDetailsViewModel();
            indicatorDetailsViewModel.Initialize(Device);
            bool result = ServiceFactory.UserDialogs.ShowModalWindow(indicatorDetailsViewModel);
            if (result)
            {
                ;
            }
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            NewDeviceViewModel newDeviceViewModel = new NewDeviceViewModel();
            newDeviceViewModel.Initialize(this);
            ServiceFactory.UserDialogs.ShowModalWindow(newDeviceViewModel);
        }

        public RelayCommand AddManyCommand { get; private set; }
        void OnAddMany()
        {
            NewDeviceViewModel newDeviceViewModel = new NewDeviceViewModel();
            newDeviceViewModel.Initialize(this);
            ServiceFactory.UserDialogs.ShowModalWindow(newDeviceViewModel);
        }

        public RelayCommand RemoveCommand { get; private set; }
        void OnRemove()
        {
            if (Parent != null)
            {
                Parent.IsExpanded = false;
                Parent.Children.Remove(this);
                Parent.Update();
                Parent.IsExpanded = true;
            }
        }
    }
}

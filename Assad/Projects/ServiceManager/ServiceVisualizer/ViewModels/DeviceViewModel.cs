using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Common;
using ClientApi;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;
using ServiceApi;

namespace ServiceVisualizer
{
    public class DeviceViewModel : BaseViewModel
    {
        public DeviceDetailsView View { get; set; }
        public Device Device;
        Firesec.Metadata.drvType driver;

        public DeviceViewModel()
        {
            Children = new ObservableCollection<DeviceViewModel>();
            AddCommand = new RelayCommand(OnAddCommand);
            RemoveCommand = new RelayCommand(OnRemoveCommand);
            ShowZoneLogicCommand = new RelayCommand(OnShowZoneLogicCommand);
        }

        bool isExpanded;
        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                isExpanded = value;

                if (isExpanded)
                {
                    AddChildren(this);
                }
                else
                {
                    RemoveChildren(this);
                }

                OnPropertyChanged("IsExpanded");
            }
        }

        void RemoveChildren(DeviceViewModel parentDeviceViewModel)
        {
            foreach (DeviceViewModel deviceViewModel in parentDeviceViewModel.Children)
            {
                if (ViewModel.Current.AllDeviceViewModels.Contains(deviceViewModel))
                    ViewModel.Current.AllDeviceViewModels.Remove(deviceViewModel);
                RemoveChildren(deviceViewModel);
            }
        }

        void AddChildren(DeviceViewModel parentDeviceViewModel)
        {
            if (parentDeviceViewModel.IsExpanded)
            {
                int indexOf = ViewModel.Current.AllDeviceViewModels.IndexOf(parentDeviceViewModel);
                for (int i = 0; i < parentDeviceViewModel.Children.Count; i++)
                {
                    if (ViewModel.Current.AllDeviceViewModels.Contains(parentDeviceViewModel.Children[i]) == false)
                    {
                        ViewModel.Current.AllDeviceViewModels.Insert(indexOf + 1 + i, parentDeviceViewModel.Children[i]);
                    }
                }

                foreach (DeviceViewModel deviceViewModel in parentDeviceViewModel.Children)
                {
                    AddChildren(deviceViewModel);
                }
            }
        }

        public bool HasChildren
        {
            get
            {
                return (Children.Count > 0);
            }
        }

        public int Level
        {
            get
            {
            if (Parent == null)
            {
                return 0;
            }
            else
            {
                return Parent.Level + 1;
            }
            }
        }

        public void Update()
        {
            OnPropertyChanged("HasChildren");
        }

        public RelayCommand ShowZoneLogicCommand { get; private set; }
        void OnShowZoneLogicCommand(object obj)
        {
            ZoneLogicView zoneLogicView = new ZoneLogicView();
            ZoneLogicViewModel zoneLogicViewModel = new ZoneLogicViewModel();
            zoneLogicViewModel.RequestClose += delegate { zoneLogicView.Close(); };
            zoneLogicViewModel.SetDevice(this);
            zoneLogicView.DataContext = zoneLogicViewModel;
            zoneLogicView.ShowDialog();
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAddCommand(object obj)
        {
            NewDeviceViewModel newDeviceViewModel = new NewDeviceViewModel();
            newDeviceViewModel.Init(this);
            NewDeviceView newDeviceView = new NewDeviceView();
            newDeviceView.DataContext = newDeviceViewModel;
            newDeviceViewModel.RequestClose += delegate { newDeviceView.Close(); };
            newDeviceView.ShowDialog();

            if (newDeviceViewModel.SelectedAvailableDevice != null)
            {
                string driverId = newDeviceViewModel.SelectedAvailableDevice.DriverId;
                DeviceViewModel deviceViewModel = new DeviceViewModel();
                Device device = new Device();
                device.Properties = new List<Property>();
                device.DriverId = driverId;
                device.PresentationAddress = "0.0";
                deviceViewModel.SetDevice(device);
                deviceViewModel.Parent = this;
                this.Children.Add(deviceViewModel);

                Update();
                IsExpanded = false;
                IsExpanded = true;
            }
        }

        public RelayCommand RemoveCommand { get; private set; }
        void OnRemoveCommand(object obj)
        {
            if (Parent != null)
            {
                Parent.IsExpanded = false;
                Parent.Children.Remove(this);
                Parent.Update();
                Parent.IsExpanded = true;
            }
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

        StackPanel propStackPanel;
        public StackPanel PropStackPanel
        {
            get { return propStackPanel; }
            set
            {
                propStackPanel = value;
                OnPropertyChanged("PropStackPanel");
            }
        }

        StringProperty _textBinding { get; set; }

        public List<StringProperty> StringProperties { get; set; }
        public List<BoolProperty> BoolProperties { get; set; }
        public List<EnumProperty> EnumProperties { get; set; }


        void SetProperties()
        {
            StringProperties = new List<StringProperty>();
            BoolProperties = new List<BoolProperty>();
            EnumProperties = new List<EnumProperty>();

            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            StackPanel _PropStackPanel = new StackPanel();
            _PropStackPanel.Children.Clear();

            if (driver.propInfo != null)
            {
                foreach (Firesec.Metadata.propInfoType propertyInfo in driver.propInfo)
                {
                    if (propertyInfo.hidden == "1")
                        continue;
                    if ((propertyInfo.caption == "Заводской номер") || (propertyInfo.caption == "Версия микропрограммы"))
                        continue;

                    UIElement uiElement = null;

                    if (propertyInfo.param != null)
                    {
                        EnumProperty enumProperty = new EnumProperty();
                        enumProperty.PropertyName = propertyInfo.name;
                        enumProperty.Values = new List<string>();
                        ComboBox comboBox = new ComboBox();
                        foreach (Firesec.Metadata.paramType propertyParameter in propertyInfo.param)
                        {
                            enumProperty.Values.Add(propertyParameter.name);
                            comboBox.Items.Add(propertyParameter.name);
                        }

                        Binding b = new Binding();
                        b.Source = enumProperty;
                        b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                        b.Path = new PropertyPath("SelectedValue");
                        comboBox.SetBinding(ComboBox.SelectedValueProperty, b);

                        if (Device.Properties.Any(x => x.Name == propertyInfo.name))
                        {
                            enumProperty.SelectedValue = Device.Properties.FirstOrDefault(x => x.Name == propertyInfo.name).Value;
                            //string selectedValueIndex = device.DeviceProperties.FirstOrDefault(x => x.Name == propertyInfo.name).Value;
                            //enumProperty.SelectedValue = propertyInfo.param.FirstOrDefault(x => x.value == selectedValueIndex).name;
                        }
                        else
                        {
                            string selectedValueIndex = propertyInfo.@default;
                            enumProperty.SelectedValue = propertyInfo.param.FirstOrDefault(x => x.value == selectedValueIndex).name;
                        }

                        EnumProperties.Add(enumProperty);
                        uiElement = comboBox;
                    }
                    else
                    {
                        switch (propertyInfo.type)
                        {
                            case "String":
                            case "Int":
                                TextBox textBox = new TextBox();

                                StringProperty stringProperty = new StringProperty();
                                stringProperty.PropertyName = propertyInfo.name;
                                Binding b = new Binding();
                                b.Source = stringProperty;
                                b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                                b.Path = new System.Windows.PropertyPath("Text");
                                textBox.SetBinding(TextBox.TextProperty, b);

                                if (Device.Properties.Any(x => x.Name == propertyInfo.name))
                                    stringProperty.Text = Device.Properties.FirstOrDefault(x => x.Name == propertyInfo.name).Value;
                                else
                                    stringProperty.Text = propertyInfo.@default;

                                StringProperties.Add(stringProperty);
                                uiElement = textBox;
                                break;
                            case "Bool":
                                CheckBox checkBox = new CheckBox();

                                BoolProperty boolProperty = new BoolProperty();
                                boolProperty.PropertyName = propertyInfo.name;
                                Binding b2 = new Binding();
                                b2.Source = boolProperty;
                                b2.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                                b2.Path = new PropertyPath("IsChecked");
                                checkBox.SetBinding(CheckBox.IsCheckedProperty, b2);

                                if (Device.Properties.Any(x => x.Name == propertyInfo.name))
                                    boolProperty.IsChecked = (Device.Properties.FirstOrDefault(x => x.Name == propertyInfo.name).Value == "1") ? true : false;
                                else
                                    boolProperty.IsChecked = (propertyInfo.@default == "1") ? true : false;

                                BoolProperties.Add(boolProperty);
                                uiElement = checkBox;
                                break;
                            default:
                                throw new Exception("Неизвестный тип свойства");
                        }
                    }

                    grid.RowDefinitions.Add(new RowDefinition());
                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = propertyInfo.caption;
                    grid.Children.Add(uiElement);
                    grid.Children.Add(textBlock);
                    Grid.SetColumn(textBlock, 0);
                    Grid.SetColumn(uiElement, 1);
                    Grid.SetRow(textBlock, grid.RowDefinitions.Count - 1);
                    Grid.SetRow(uiElement, grid.RowDefinitions.Count - 1);
                }
            }
            _PropStackPanel.Children.Add(grid);
            PropStackPanel = _PropStackPanel;
        }

        public void SetDevice(Device device)
        {
            this.Device = device;
            driver = ServiceClient.CurrentConfiguration.Metadata.drv.FirstOrDefault(x => x.id == device.DriverId);

            SetProperties();

            DriverName = driver.name;
            ShortDriverName = driver.shortName;
            Address = device.PresentationAddress;
            Description = device.Description;

            string ImageName;
            if (!string.IsNullOrEmpty(driver.dev_icon))
            {
                ImageName = driver.dev_icon;
            }
            else
            {
                Firesec.Metadata.classType metadataClass = ServiceClient.CurrentConfiguration.Metadata.@class.FirstOrDefault(x => x.clsid == driver.clsid);
                ImageName = metadataClass.param.FirstOrDefault(x => x.name == "Icon").value;
            }
            ImageSource = @"C:\Program Files\Firesec\Icons\" + ImageName + ".ico";

            if (driver.ar_no_addr != null)
            {
                CanEditAddress = (driver.ar_no_addr == "1") ? false : true;
            }
            else
            {
                CanEditAddress = true;
            }
        }

        public void SetZone()
        {
            if (Device.ZoneNo != null)
            {
                Zone = ViewModel.Current.ZoneViewModels.FirstOrDefault(x => x.ZoneNo == Device.ZoneNo);
            }
        }

        public ObservableCollection<ZoneViewModel> Zones
        {
            get { return ViewModel.Current.ZoneViewModels; }
        }

        public bool IsZoneDevice
        {
            get
            {
                if ((driver.minZoneCardinality == "0") && (driver.maxZoneCardinality == "0"))
                {
                    return false;
                }
                return true;
            }
        }

        public bool IsZoneLogicDevice
        {
            get
            {
                if ((driver.options != null) && (driver.options.Contains("ExtendedZoneLogic")))
                {
                    return true;
                }
                return false;
            }
        }

        public bool CanAddChildren
        {
            get
            {
                List<Firesec.Metadata.drvType> childDrivers = new List<Firesec.Metadata.drvType>();

                foreach (Firesec.Metadata.drvType childDriver in ServiceClient.CurrentConfiguration.Metadata.drv)
                {
                    Firesec.Metadata.classType childClass = ServiceClient.CurrentConfiguration.Metadata.@class.FirstOrDefault(x => x.clsid == childDriver.clsid);
                    if ((childClass.parent != null) && (childClass.parent.Any(x => x.clsid == driver.clsid)))
                    {
                        childDrivers.Add(childDriver);
                    }
                }

                return (childDrivers.Count > 0);


                //FiresecMetadata.DriverItem driverItem = ViewModel.Current.treeBuilder.Drivers.FirstOrDefault(x => x.DriverId == Device.DriverId);
                //if (driverItem == null)
                //    return false;
                //if (driverItem.Children.Count > 0)
                //    return true;
                //return false;
            }
        }

        string shortDriverName;
        public string ShortDriverName
        {
            get { return shortDriverName; }
            set
            {
                shortDriverName = value;
                OnPropertyChanged("ShortDriverName");
            }
        }

        string driverName;
        public string DriverName
        {
            get { return driverName; }
            set
            {
                driverName = value;
                OnPropertyChanged("DriverName");
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

        ZoneViewModel zone;
        public ZoneViewModel Zone
        {
            get { return zone; }
            set
            {
                zone = value;
                OnPropertyChanged("Zone");
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

        //string state;
        //public string State
        //{
        //    get { return state; }
        //    set
        //    {
        //        state = value;
        //        OnPropertyChanged("State");
        //    }
        //}

        //ObservableCollection<string> states;
        //public ObservableCollection<string> States
        //{
        //    get { return states; }
        //    set
        //    {
        //        states = value;
        //        OnPropertyChanged("States");
        //    }
        //}

        //ObservableCollection<ServiceApi.Parameter> parameters;
        //public ObservableCollection<ServiceApi.Parameter> Parameters
        //{
        //    get { return parameters; }
        //    set
        //    {
        //        parameters = value;
        //        OnPropertyChanged("Parameters");
        //    }
        //}

        string imageSource;
        public string ImageSource
        {
            get { return imageSource; }
            set
            {
                imageSource = value;
                OnPropertyChanged("ImageSource");
            }
        }

        public bool CanEditAddress { get; private set; }

        public DeviceViewModel Parent { get; set; }

        ObservableCollection<DeviceViewModel> children;
        public ObservableCollection<DeviceViewModel> Children
        {
            get { return children; }
            set
            {
                children = value;
                OnPropertyChanged("Children");
            }
        }

        bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                OnPropertyChanged("IsSelected");
                ViewModel.Current.SelectedDeviceViewModel = this;
            }
        }
    }
}

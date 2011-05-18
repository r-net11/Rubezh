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
        public Device _device;
        public Firesec.Metadata.drvType Driver;

        public DeviceViewModel()
        {
            Children = new ObservableCollection<DeviceViewModel>();
            AddCommand = new RelayCommand(OnAdd);
            AddManyCommand = new RelayCommand(OnAddMany);
            RemoveCommand = new RelayCommand(OnRemove);
            ShowZoneLogicCommand = new RelayCommand(OnShowZoneLogic);
        }

        public void Initialize(Device device, ObservableCollection<DeviceViewModel> sourceDevices)
        {
            Source = sourceDevices;

            _device = device;
            Driver = FiresecManager.CurrentConfiguration.Metadata.drv.FirstOrDefault(x => x.id == device.DriverId);

            SetProperties();

            Address = device.Address;
            Description = device.Description;
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

            if (Driver.propInfo != null)
            {
                foreach (Firesec.Metadata.propInfoType propertyInfo in Driver.propInfo)
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

                        if (_device.Properties.Any(x => x.Name == propertyInfo.name))
                        {
                            enumProperty.SelectedValue = _device.Properties.FirstOrDefault(x => x.Name == propertyInfo.name).Value;
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
                            case "Byte":
                                TextBox textBox = new TextBox();

                                StringProperty stringProperty = new StringProperty();
                                stringProperty.PropertyName = propertyInfo.name;
                                Binding b = new Binding();
                                b.Source = stringProperty;
                                b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                                b.Path = new System.Windows.PropertyPath("Text");
                                textBox.SetBinding(TextBox.TextProperty, b);

                                if (_device.Properties.Any(x => x.Name == propertyInfo.name))
                                    stringProperty.Text = _device.Properties.FirstOrDefault(x => x.Name == propertyInfo.name).Value;
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

                                if (_device.Properties.Any(x => x.Name == propertyInfo.name))
                                    boolProperty.IsChecked = (_device.Properties.FirstOrDefault(x => x.Name == propertyInfo.name).Value == "1") ? true : false;
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

        public void Update()
        {
            OnPropertyChanged("HasChildren");
        }

        public string DriverId
        {
            get
            {
                if (_device != null)
                    return _device.DriverId;
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
                Zone zone = FiresecManager.CurrentConfiguration.Zones.FirstOrDefault(x => x.No == _device.ZoneNo);
                if (zone != null)
                {
                    ZoneViewModel zoneViewModel = new ZoneViewModel(zone);
                    return zoneViewModel;
                }
                return null;
            }
            set
            {
                _device.ZoneNo = value.No;
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
            zoneLogicViewModel.Initialize(this);
            ServiceFactory.UserDialogs.ShowModalWindow(zoneLogicViewModel);
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

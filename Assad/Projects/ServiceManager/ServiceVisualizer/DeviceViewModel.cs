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

namespace ServiceVisualizer
{
    public class DeviceViewModel : BaseViewModel
    {
        public DeviceDetailsView View { get; set; }

        public DeviceViewModel()
        {
            Children = new ObservableCollection<DeviceViewModel>();
            AddCommand = new RelayCommand(OnAddCommand);
            RemoveCommand = new RelayCommand(OnRemoveCommand);
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
                device.DriverId = driverId;
                device.PresentationAddress = "0.0";
                deviceViewModel.SetDevice(device);
                deviceViewModel.Parent = this;
                this.Children.Add(deviceViewModel);
            }
        }

        public RelayCommand RemoveCommand { get; private set; }
        void OnRemoveCommand(object obj)
        {
            Parent.Children.Remove(this);
        }

        Device device;
        Firesec.Metadata.drvType driver;

        public string DriverId
        {
            get
            {
                if (device != null)
                    return device.DriverId;
                return null;
            }
        }

        string availableProperties;
        public string AvailableProperties
        {
            get { return availableProperties; }
            set
            {
                availableProperties = value;
                OnPropertyChanged("AvailableProperties");
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

                TextBinding _textBinding { get; set; }

        public void SetDevice(Device device)
        {
            this.device = device;
            driver = ServiceClient.Configuration.Metadata.drv.FirstOrDefault(x => x.id == device.DriverId);

            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            StackPanel _PropStackPanel = new StackPanel();
            _PropStackPanel.Children.Clear();

            string _availableProperties = "";
            if (driver.propInfo != null)
            {
                foreach (Firesec.Metadata.propInfoType propertyInfo in driver.propInfo)
                {
                    if (propertyInfo.hidden == "1")
                        continue;
                    if ((propertyInfo.caption == "Заводской номер") || (propertyInfo.caption == "Версия микропрограммы"))
                        continue;
                    _availableProperties += propertyInfo.caption + " - " + propertyInfo.type + "\n";



                    TextBox textBox = new TextBox();
                    TextBinding textBinding = new TextBinding();
                    Binding b = new Binding();
                    b.Source = textBinding;
                    b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    b.Path = new System.Windows.PropertyPath("Text");
                    textBox.SetBinding(TextBox.TextProperty, b);

                    textBox.Text = propertyInfo.caption;

                    //_PropStackPanel.Children.Add(textBox);
                    //_PropStackPanel.Children.Add(new CheckBox());
                    //View.propertiesStackPanel.Children.Add(new TextBox());

                    _textBinding = textBinding;

                    grid.RowDefinitions.Add(new RowDefinition());
                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = "Hello";
                    grid.Children.Add(textBox);
                    grid.Children.Add(textBlock);
                    Grid.SetColumn(textBlock, 0);
                    Grid.SetColumn(textBox, 1);
                    Grid.SetRow(textBlock, grid.RowDefinitions.Count -1);
                    Grid.SetRow(textBox, grid.RowDefinitions.Count - 1);
                }
            }
            _PropStackPanel.Children.Add(grid);
            AvailableProperties = _availableProperties;
            PropStackPanel = _PropStackPanel;

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
                Firesec.Metadata.classType metadataClass = ServiceClient.Configuration.Metadata.@class.FirstOrDefault(x => x.clsid == driver.clsid);
                ImageName = metadataClass.param.FirstOrDefault(x => x.name == "Icon").value;
            }
            ImageSource = @"C:\Program Files\Firesec\Icons\" + ImageName + ".ico";

            State = device.State;
            States = new ObservableCollection<string>();
            if (device.States != null)
            {
                foreach (string state in device.States)
                {
                    States.Add(state);
                }
            }
            Parameters = new ObservableCollection<Parameter>();
            if (device.Parameters != null)
            {
                foreach (Parameter parameter in device.Parameters)
                {
                    Parameters.Add(parameter);
                }
            }
        }

        public void SetZone()
        {
            if (device._Zone != null)
            {
                Zone = ViewModel.Current.ZoneViewModels.FirstOrDefault(x => x.ZoneNo == device._Zone.No);
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
                FiresecMetadata.DriverItem driverItem = ViewModel.Current.treeBuilder.Drivers.FirstOrDefault(x => x.DriverId == device.DriverId);
                if (driverItem == null)
                    return false;
                if (driverItem.Children.Count > 0)
                    return true;
                return false;
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

        string state;
        public string State
        {
            get { return state; }
            set
            {
                state = value;
                OnPropertyChanged("State");
            }
        }

        ObservableCollection<string> states;
        public ObservableCollection<string> States
        {
            get { return states; }
            set
            {
                states = value;
                OnPropertyChanged("States");
            }
        }

        ObservableCollection<Parameter> parameters;
        public ObservableCollection<Parameter> Parameters
        {
            get { return parameters; }
            set
            {
                parameters = value;
                OnPropertyChanged("Parameters");
            }
        }

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

        bool isExpanded;
        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                isExpanded = value;
                OnPropertyChanged("IsExpanded");
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Collections.ObjectModel;
using System.Windows;
using ClientApi;

namespace ServiceVisualizer
{
    public class NewDeviceViewModel : BaseViewModel
    {
        public NewDeviceViewModel()
        {
            AddCommand = new RelayCommand(OnAddCommand);
        }

        public event Action RequestClose;
        void OnRequestClose()
        {
            if (RequestClose != null)
                RequestClose();
        }

        DeviceViewModel ParentDeviceViewModel;

        public void Init(DeviceViewModel deviceViewModel)
        {
            ParentDeviceViewModel = deviceViewModel;
            AvailableDevices = new ObservableCollection<AvailableDevice>();

            FiresecMetadata.DriverItem driverItem = ViewModel.Current.treeBuilder.Drivers.FirstOrDefault(x => x.DriverId == deviceViewModel.DriverId);
            foreach (FiresecMetadata.DriverItem childDriverItem in driverItem.Children)
            {
                Firesec.Metadata.drvType driver = ServiceClient.CurrentConfiguration.Metadata.drv.FirstOrDefault(x => x.id == childDriverItem.DriverId);
                AvailableDevice availableDevice = new AvailableDevice();
                availableDevice.DriverId = childDriverItem.DriverId;
                availableDevice.DriverName = driver.shortName;

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

                availableDevice.ImageSource = @"C:\Program Files\Firesec\Icons\" + ImageName + ".ico";
                AvailableDevices.Add(availableDevice);
            }
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAddCommand(object obj)
        {
            OnRequestClose();
        }

        ObservableCollection<AvailableDevice> availableDevices;
        public ObservableCollection<AvailableDevice> AvailableDevices
        {
            get { return availableDevices; }
            set
            {
                availableDevices = value;
                OnPropertyChanged("AvailableDevices");
            }
        }

        AvailableDevice selectedAvailableDevice;
        public AvailableDevice SelectedAvailableDevice
        {
            get { return selectedAvailableDevice; }
            set
            {
                selectedAvailableDevice = value;
                OnPropertyChanged("SelectedAvailableDevice");
            }
        }

        bool closeSignal = false;
        bool CloseSignal
        {
            get { return closeSignal; }
            set
            {
                closeSignal = value;
                OnPropertyChanged("CloseSignal");
            }
        }
        

    }

    public class AvailableDevice
    {
        public string DriverId { get; set; }
        public string DriverName { get; set; }
        public string ImageSource { get; set; }
    }


    public static class WindowCloseBehaviour
    {
        public static void SetClose(DependencyObject target, bool value)
        {
            target.SetValue(CloseProperty, value);
        }
        public static readonly DependencyProperty CloseProperty =
        DependencyProperty.RegisterAttached(
        "Close",
        typeof(bool),
        typeof(WindowCloseBehaviour),
        new UIPropertyMetadata(false, OnClose));
        private static void OnClose(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool && ((bool)e.NewValue))
            {
                Window window = GetWindow(sender);
                if (window != null)
                    window.Close();
            }
        }
        private static Window GetWindow(DependencyObject sender)
        {
            Window window = null;
            if (sender is Window)
                window = (Window)sender;
            if (window == null)
                window = Window.GetWindow(sender);
            return window;
        }
    }

}

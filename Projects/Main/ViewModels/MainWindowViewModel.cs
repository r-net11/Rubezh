using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Threading;
using AssadDdevices;
using ComDevices;

namespace Main
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public static MainWindowViewModel ViewModel { get; set; }
        public MainWindowView mainWindowView { get; set; }

        NetManager netManager;

        public MainWindowViewModel()
        {
            ViewModel = this;

            Logger.Logger.Create();
            AssadDeviceTypesManager.LoadTypes();
            netManager = new NetManager();


            ComDeviceManager.Load();
            //RefreshTreeView();

            ComDeviceWather deviceWather = new ComDeviceWather();
            deviceWather.PropertyChanged += new Action<string>(deviceWather_PropertyChanged);
            deviceWather.Start();

            ComDevices = new ObservableCollection<ComDevice>();
            ComDevices.Add(ComDeviceManager.Devices[0]);

            ConnectCommand = new RelayCommand(OnConnect);
            ShowLogsCommand = new RelayCommand(OnShowLogs);
        }

        void deviceWather_PropertyChanged(string obj)
        {
            //Invoke(new Action<string>(RefreshDeviceState), obj);
        }

        public RelayCommand ConnectCommand { get; private set; }
        void OnConnect(object parameter)
        {
            netManager.Start();
            netManager.SendBroadcastUdp();

            AssadEmulator.App.Create();
        }

        public RelayCommand ShowLogsCommand { get; private set; }
        void OnShowLogs(object parameters)
        {
            Logger.Logger.form.Show();
        }

        ObservableCollection<AssadDevice> devices;
        public ObservableCollection<AssadDevice> Devices
        {
            get { return devices; }
            set
            {
                devices = value;
                OnPropertyChanged("Devices");
            }
        }

        AssadDevice selectedDevice;
        public AssadDevice SelectedDevice
        {
            get { return selectedDevice; }
            set
            {
                selectedDevice = value;
                OnPropertyChanged("SelectedDevice");
            }
        }

        ObservableCollection<ComDevice> comDevices;
        public ObservableCollection<ComDevice> ComDevices
        {
            get { return comDevices; }
            set
            {
                comDevices = value;
                OnPropertyChanged("ComDevices");
            }
        }

        ComDevice selectedComDevice;
        public ComDevice SelectedComDevice
        {
            get { return selectedComDevice; }
            set
            {
                selectedComDevice = value;
                OnPropertyChanged("SelectedComDevice");
            }
        }

        public void RefreshTree()
        {
            //mainWindowView.Dispatcher.Invoke(new Action(OnRefresh), DispatcherPriority.Normal);
            mainWindowView.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(OnRefresh));
        }

        public void OnRefresh()
        {
            AssadDevice device = AssadDeviceManager.Devices[0];
            Devices = new ObservableCollection<AssadDevice>();
            Devices.Add(device);
        }

        void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}

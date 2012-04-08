using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using FiresecClient;
using System.Configuration;

namespace ItvIntegration
{
    public class DevicesViewModel : INotifyPropertyChanged
    {
        public void Initialize()
        {
            string serverAddress = ConfigurationManager.AppSettings["ServiceAddress"] as string;
            string defaultLogin = ConfigurationManager.AppSettings["DefaultLogin"] as string;
            string defaultPassword = ConfigurationManager.AppSettings["DefaultPassword"] as string;
            string result = ItvManager.Connect(serverAddress, defaultLogin, defaultPassword);
            if (result != null)
            {
                MessageBox.Show(result);
                return;
            }

            Devices = new ObservableCollection<DeviceViewModel>();
            foreach (var deviceState in ItvManager.DeviceStates.DeviceStates)
            {
                var deviceViewModel = new DeviceViewModel(deviceState);
                Devices.Add(deviceViewModel);
            }
        }

        public ObservableCollection<DeviceViewModel> Devices { get; set; }

        DeviceViewModel _selectedDevice;
        public DeviceViewModel SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                _selectedDevice = value;
                OnPropertyChanged("StateType");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}

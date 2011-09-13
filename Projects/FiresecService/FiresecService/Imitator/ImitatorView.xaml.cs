using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace FiresecService.Imitator
{
    public partial class ImitatorView : Window, INotifyPropertyChanged
    {
        public ImitatorView()
        {
            InitializeComponent();
            DataContext = this;

            Devices = new ObservableCollection<DeviceViewModel>();

            foreach (var deviceState in FiresecManager.DeviceConfigurationStates.DeviceStates)
            {
                var deviceViewModel = new DeviceViewModel(deviceState);
                Devices.Add(deviceViewModel);
            }
        }

        public ObservableCollection<DeviceViewModel> Devices { get; private set; }

        DeviceViewModel _selectedDevice;
        public DeviceViewModel SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                _selectedDevice = value;
                OnPropertyChanged("SelectedDevice");
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

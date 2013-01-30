using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ServerFS2;

namespace ClientFS2
{
    public class ClientViewModel:BaseViewModel
    {
        public ObservableCollection<Device> Devices { get; private set; }
        public ClientViewModel()
        {
            ReadJournalCommand = new RelayCommand(OnReadJournal);
            SendRequestCommand = new RelayCommand(OnSendRequest);
            AutoDetectDeviceCommand = new RelayCommand(OnAutoDetectDevice);
            Devices = new ObservableCollection<Device>();
        }
        private Device _selectedDevice;
        public Device SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                _selectedDevice = value;
                OnPropertyChanged("SelectedDevice");
            }
        }

        private string _textBoxRequest;
        public string TextBoxRequest
        {
            get { return _textBoxRequest; }
            set
            {
                _textBoxRequest = value;
                OnPropertyChanged("TextBoxRequest");
            }
        }

        private string _textBoxResponse;
        public string TextBoxResponse
        {
            get { return _textBoxResponse; }
            set
            {
                _textBoxResponse = value;
                OnPropertyChanged("TextBoxResponse");
            }
        }

        public RelayCommand ReadJournalCommand { get; private set; }
        void OnReadJournal()
        {
            var device = SelectedDevice;
            ServerHelper.GetJournalItems(device);
            ShowJournal(device);
        }

        void ShowJournal(Device device)
        {
            var win = new Window();
            var dataGrid = new DataGrid { ItemsSource = device.JournalItems };
            win.Content = dataGrid;
            win.Show();
        }

        public RelayCommand SendRequestCommand { get; private set; }
        private void OnSendRequest()
        {
            var bytes = TextBoxRequest.Split()
                   .Select(t => byte.Parse(t, NumberStyles.AllowHexSpecifier)).ToList();
            var inbytes = ServerHelper.SendRequest(bytes);
            foreach (var b in inbytes)
                TextBoxResponse += b.ToString("X2") + " ";
        }

        public RelayCommand AutoDetectDeviceCommand { get; private set; }
        private void OnAutoDetectDevice()
        {
            ServerHelper.AutoDetectDevice(Devices);
            OnPropertyChanged("Devices");
        }
    }
}

using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using ServerFS2;
using FiresecClient;
using Device = FiresecAPI.Models.Device;


namespace ClientFS2.ViewModels
{
    public class ClientViewModel:BaseViewModel
    {
        public ObservableCollection<Device> Devices { get; private set; }
        public ClientViewModel()
        {
            ReadJournalCommand = new RelayCommand(OnReadJournal);
            SendRequestCommand = new RelayCommand(OnSendRequest);
            AutoDetectDeviceCommand = new RelayCommand(OnAutoDetectDevice);
            ShowDevicesTreeCommand = new RelayCommand(OnShowDevicesTree);
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
			ShowJournal(ServerHelper.GetJournalItems(device));
        }

        static void ShowJournal(List<JournalItem> journalItems)
        {
			DialogService.ShowModalWindow(new JournalViewModel(journalItems));
        }

        public RelayCommand ShowDevicesTreeCommand { get; private set; }
        static void OnShowDevicesTree()
        {
            DialogService.ShowModalWindow(new DevicesViewModel());
        }

        public RelayCommand SendRequestCommand { get; private set; }
        private void OnSendRequest()
        {
            var bytes = TextBoxRequest.Split()
                   .Select(t => byte.Parse(t, NumberStyles.AllowHexSpecifier)).ToList();
            var inbytes = ServerHelper.SendRequest(bytes);
            foreach (var b in inbytes)
                TextBoxResponse += b.ToString ("X2") + " ";
        }
        public RelayCommand AutoDetectDeviceCommand { get; private set; }
        private void OnAutoDetectDevice()
        {
            ServerHelper.AutoDetectDevice(Devices);
            OnPropertyChanged("Devices");
        }
    }
}

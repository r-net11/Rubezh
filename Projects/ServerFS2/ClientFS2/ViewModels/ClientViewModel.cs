using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Microsoft.Win32;
using ServerFS2;
using Device = ServerFS2.Device;

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
            GetConfigurationFromFileCommand = new RelayCommand(OnGetConfigurationFromFile);
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

        static void ShowJournal(Device device)
        {
            DialogService.ShowModalWindow(new JournalViewModel(device));
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

        public RelayCommand GetConfigurationFromFileCommand { get; private set; }
        private static void OnGetConfigurationFromFile()
        {
            try
            {
                var openDialog = new OpenFileDialog()
                                     {
                                         Filter = "firesec2 files|*.fscp",
                                         DefaultExt = "firesec2 files|*.fscp"
                                     };
                if (openDialog.ShowDialog().Value)
                {
                    WaitHelper.Execute(() =>
                                           {
                                               //ZipConfigActualizeHelper.Actualize(openDialog.FileName);
                                               //var folderName = AppDataFolderHelper.GetFolder("Administrator/Configuration");
                                               //var configFileName = Path.Combine(folderName, "Config.fscp");
                                               //if (Directory.Exists(folderName))
                                               //	Directory.Delete(folderName, true);
                                               //Directory.CreateDirectory(folderName);
                                               //File.Copy(openDialog.FileName, configFileName);

                                               FiresecManager.LoadFromZipFile(openDialog.FileName);
                                               FiresecManager.UpdateConfiguration();
                                               DeviceConfiguration deviceConfiguration = FiresecManager.FiresecConfiguration.DeviceConfiguration;

                                           });
                }
            }
            catch{}
        }
    }
}

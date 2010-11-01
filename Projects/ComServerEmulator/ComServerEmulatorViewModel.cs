using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows;
using System.ComponentModel;
using Common;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ComServerEmulator
{
    public class ComServerEmulatorViewModel : INotifyPropertyChanged
    {
        public ComServerEmulatorViewModel()
        {
            ComDeviceManager.Load();
            devices = new ObservableCollection<ComDevice>();
            devices.Add(ComDeviceManager.Devices[0]);

            GetStateCommand = new RelayCommand(OnGetState);
        }

        public RelayCommand GetStateCommand { get; private set; }


        ComServer.CoreState.config config = new ComServer.CoreState.config();

        public void OnGetState(object parameter)
        {
            config = new ComServer.CoreState.config();
            TreeBase[] _devices = Devices[0].FindAllChildren().FindAll(x => ((x as ComDevice).States.FindAll(y => y.IsActive == true)).Count() > 0).ToArray();

            if (_devices.Length > 0)
            {
                config.dev = new ComServer.CoreState.devType[_devices.Length];
                for (int i = 0; i < _devices.Length; i++)
                {
                    config.dev[i] = new ComServer.CoreState.devType();
                    config.dev[i].name = _devices[i].PlaceInTree;

                    ComState[] _states = (_devices[i] as ComDevice).States.FindAll(x => x.IsActive).ToArray();

                    config.dev[i].state = new ComServer.CoreState.stateType[_states.Length];
                    for (int j = 0; j < _states.Length; j++)
                    {
                        config.dev[i].state[j] = new ComServer.CoreState.stateType();
                        config.dev[i].state[j].id = _states[j].Id;
                    }
                }
            }

            MemoryStream memoryStream = new MemoryStream();
            XmlSerializer serializer = new XmlSerializer(typeof(ComServer.CoreState.config));
            serializer.Serialize(memoryStream, config);
            memoryStream.Close();
            string message = Encoding.Default.GetString(memoryStream.ToArray());
            message = message.Replace(" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "");
            StateMessage = message;
        }

        public ComServer.CoreState.config GetCoreState()
        {
            OnGetState(null);
            return config;
        }

        string stateMessage;
        public string StateMessage
        {
            get { return stateMessage; }
            set
            {
                stateMessage = value;
                OnPropertyChanged("StateMessage");
            }
        }

        ObservableCollection<ComDevice> devices;
        public ObservableCollection<ComDevice> Devices
        {
            get { return devices; }
            set
            {
                devices = value;
                OnPropertyChanged("Devices");
            }
        }

        ComDevice selectedDevice;
        public ComDevice SelectedDevice
        {
            get { return selectedDevice; }
            set
            {
                selectedDevice = value;
                OnPropertyChanged("SelectedDevice");
            }
        }

        void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}

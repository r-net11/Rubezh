using System.Collections.ObjectModel;
using System.Windows;
using System.IO;
using System.Xml.Serialization;
using System.ComponentModel;

namespace DeviceEditor
{
    public class ViewModel : Window, INotifyPropertyChanged
    {
        public ViewModel()
        {
            DeviceManager deviceManager = new DeviceManager();
            Load(deviceManager);
        }
        public DeviceManager deviceManager;
        static public string deviceLibrary_xml = @"c:\Rubezh\Assad\Projects\ActivexDevices\Library\DeviceLibrary.xml";

        ObservableCollection<DeviceViewModel> deviceViewModels;
        public ObservableCollection<DeviceViewModel> DeviceViewModels
        {
            get { return deviceViewModels; }
            set
            {
                deviceViewModels = value;
                OnPropertyChanged("DeviceViewModels");
            }
        }

        public void Load(DeviceManager deviceManager)
        {
            this.deviceManager = deviceManager;
            FileStream filexml = new FileStream(deviceLibrary_xml, FileMode.Open, FileAccess.Read, FileShare.Read);
            XmlSerializer serializer = new XmlSerializer(typeof(DeviceManager));
            deviceManager = (DeviceManager)serializer.Deserialize(filexml);
            filexml.Close();

            DeviceViewModels = new ObservableCollection<DeviceViewModel>();
            foreach (Device device in deviceManager.Devices)
            {
                DeviceViewModel deviceViewModel = new DeviceViewModel();
                deviceViewModel.Id = device.Id;
                DeviceViewModels.Add(deviceViewModel);
                deviceViewModel.StateViewModels = new ObservableCollection<StateViewModel>();
                foreach (State state in device.States)
                {
                    StateViewModel stateViewModel = new StateViewModel();
                    stateViewModel.Id = state.Id;
                    deviceViewModel.StateViewModels.Add(stateViewModel);
                    stateViewModel.CadrViewModels = new ObservableCollection<CadrViewModel>();
                    foreach (Cadr cadr in state.Cadrs)
                    {
                        CadrViewModel cadrViewModel = new CadrViewModel();
                        cadrViewModel.Id = cadr.Id;
                        cadrViewModel.Image = cadr.Image;
                        stateViewModel.CadrViewModels.Add(cadrViewModel);
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

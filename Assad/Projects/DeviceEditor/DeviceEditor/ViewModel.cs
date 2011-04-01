using System.Collections.ObjectModel;
using System.Windows;
using System.IO;
using System.Xml.Serialization;
using System.ComponentModel;
using Common;
using System.Xml;
using System.Windows.Controls;
using RubezhDevices;
using System.Windows.Markup;
using System.Collections.Generic;
using System;

namespace DeviceEditor
{
    public class ViewModel : BaseViewModel
    {
        public ViewModel()
        {
            Current = this;
            DeviceManager deviceManager = new DeviceManager();
            Load(deviceManager);
            SaveCommand = new RelayCommand(OnSaveCommand);
            StopTimerCommand = new RelayCommand(OnStopTimerCommand);
        }

        public RelayCommand StopTimerCommand { get; private set; }
        void OnStopTimerCommand(object obj)
        {
            DeviceViewModel.Current.DispatcherTimerStop = true;
        }

        public RelayCommand SaveCommand { get; private set; }
        void OnSaveCommand(object obj)
        {
            var result = MessageBox.Show("Вы уверены что хотите сохранить все изменения на диск?", "Окно подтверждения", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.Cancel)
                return;
            deviceManager.Devices = new List<Device>();
            foreach (DeviceViewModel deviceViewModel in DeviceViewModels)
            {
                Device device = new Device();
                device.Id = deviceViewModel.Id;
                deviceManager.Devices.Add(device);
                device.States = new List<State>();
                foreach (StateViewModel stateViewModel in deviceViewModel.StateViewModels)
                {
                    State state = new State();
                    state.Id = stateViewModel.Id;
                    device.States.Add(state);
                    state.Cadrs = new List<Cadr>();
                    foreach (CadrViewModel cadrViewModel in stateViewModel.CadrViewModels)
                    {
                        Cadr cadr = new Cadr();
                        cadr.Id = cadrViewModel.Id;
                        cadr.Image = cadrViewModel.Image;
                        cadr.Duration = cadrViewModel.Duration;
                        state.Cadrs.Add(cadr);
                    }
                }
            }
            FileStream filexml = new FileStream(ViewModel.deviceLibrary_xml, FileMode.Create, FileAccess.Write, FileShare.Write);
            XmlSerializer serializer = new XmlSerializer(typeof(DeviceManager));
            serializer.Serialize(filexml, deviceManager);
            filexml.Close();
        }

        public static ViewModel Current { get; private set; }
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

        CadrViewModel selectedCadrViewModel;
        public CadrViewModel SelectedCadrViewModel
        {
            get { return selectedCadrViewModel; }
            set
            {
                selectedCadrViewModel = value;
                OnPropertyChanged("SelectedCadrViewModel");
            }
        }

        CadrViewModel selectedStateViewModel;
        public CadrViewModel SelectedStateViewModel
        {
            get { return selectedStateViewModel; }
            set
            {
                selectedStateViewModel = value;
                OnPropertyChanged("SelectedStateViewModel");
            }
        }

        Canvas readerLoadButton;
        public Canvas ReaderLoadButton
        {
            get { return readerLoadButton; }
            set
            {
                readerLoadButton = value;
                OnPropertyChanged("ReaderLoadButton");
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
                        cadrViewModel.Duration = cadr.Duration;
                        stateViewModel.CadrViewModels.Add(cadrViewModel);
                    }
                }
            }
        }
    }
}

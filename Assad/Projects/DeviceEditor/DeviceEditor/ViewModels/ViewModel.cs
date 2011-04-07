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
using System.Windows.Input;
using System.Windows.Threading;
using Firesec.Metadata;

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
            StartTimerCommand = new RelayCommand(OnStartTimerCommand);
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
        }

        public static ViewModel Current { get; private set; }
        public DeviceManager deviceManager;
        static public string deviceLibrary_xml = @"c:\Rubezh\Assad\Projects\ActivexDevices\Library\DeviceLibrary.xml";
        
        public RelayCommand StartTimerCommand { get; private set; }
        void OnStartTimerCommand(object obj)
        {
            if (SelectedStateViewModel == null)
                return;

            if (TimerButtonName == "Старт таймер")
            {
                if (SelectedStateViewModel.FrameViewModels.Count > 1)
                {
                    TimerButtonName = "Стоп таймер";
                    dispatcherTimer.Start();
                }
            }
            else
            {
                TimerButtonName = "Старт таймер";
                dispatcherTimer.Stop();
            }
        }

        string timerButtonName = "Старт таймер";
        public string TimerButtonName
        {
            get { return timerButtonName; }
            set
            {
                timerButtonName = value;
                OnPropertyChanged("TimerButtonName");
            }
        }

        public RelayCommand SaveCommand { get; private set; }
        public void OnSaveCommand(object obj)
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
                    state.Frames = new List<Frame>();
                    foreach (FrameViewModel frameViewModel in stateViewModel.FrameViewModels)
                    {
                        Frame frame = new Frame();
                        frame.Id = frameViewModel.Id;
                        frame.Image = frameViewModel.Image;
                        frame.Duration = frameViewModel.Duration;
                        state.Frames.Add(frame);
                    }
                }
            }
            FileStream file_xml = new FileStream(ViewModel.deviceLibrary_xml, FileMode.Create, FileAccess.Write, FileShare.Write);
            XmlSerializer serializer = new XmlSerializer(typeof(DeviceManager));
            serializer.Serialize(file_xml, deviceManager);
            file_xml.Close();
        }

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

        string frame1, frame2;
        int frameDuration1 = 100, frameDuration2 = 100;
        StateViewModel selectedStateViewModel;

        DeviceViewModel selectedDeviceViewModel;
        public DeviceViewModel SelectedDeviceViewModel
        {
            get { return selectedDeviceViewModel; }
            set
            {
                selectedDeviceViewModel = value;
                DeviceViewModel.Current.LoadStates();
                OnPropertyChanged("SelectedDeviceViewModel");
            }
        }

        public StateViewModel SelectedStateViewModel
        {
            get { return selectedStateViewModel; }
            set
            {
                selectedStateViewModel = value;
                if (selectedStateViewModel == null)
                    return;
                frame1 = Svg2Xaml.XSLT_Transform(selectedStateViewModel.FrameViewModels[0].Image, RubezhDevices.RubezhDevice.svg2xaml_xsl);
                frameDuration1 = selectedStateViewModel.FrameViewModels[0].Duration;
                SelectedStateViewModel.SelectedFrameViewModel = selectedStateViewModel.FrameViewModels[0];
                if (selectedStateViewModel.FrameViewModels.Count > 1)
                {
                    frame2 = Svg2Xaml.XSLT_Transform(selectedStateViewModel.FrameViewModels[1].Image, RubezhDevices.RubezhDevice.svg2xaml_xsl);
                    frameDuration2 = selectedStateViewModel.FrameViewModels[1].Duration;
                    TimerButtonName = "Старт таймер";
                    dispatcherTimer.Stop();
                }
                OnPropertyChanged("SelectedStateViewModel");
            }
        }

        public static DispatcherTimer dispatcherTimer = new DispatcherTimer();
        bool tick = false;
        /****************Таймер*****************/
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();
            tick = !tick;
            if (tick)
            {
                try
                {
                    dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, SelectedStateViewModel.FrameViewModels[0].Duration);
                    SelectedStateViewModel.SelectedFrameViewModel = SelectedStateViewModel.FrameViewModels[0];
                }
                catch { }
            }

            else
            {
                try
                {
                    dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, SelectedStateViewModel.FrameViewModels[1].Duration);
                    SelectedStateViewModel.SelectedFrameViewModel = SelectedStateViewModel.FrameViewModels[1];

                }
                catch { }
            }
        }

        public void Load(DeviceManager deviceManager)
        {
            this.deviceManager = deviceManager;
            FileStream file_xml = new FileStream(deviceLibrary_xml, FileMode.Open, FileAccess.Read, FileShare.Read);
            XmlSerializer serializer = new XmlSerializer(typeof(DeviceManager));
            deviceManager = (DeviceManager)serializer.Deserialize(file_xml);
            file_xml.Close();

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
                    stateViewModel.FrameViewModels = new ObservableCollection<FrameViewModel>();
                    foreach (Frame frame in state.Frames)
                    {
                        FrameViewModel frameViewModel = new FrameViewModel();
                        frameViewModel.Id = frame.Id;
                        frameViewModel.Image = frame.Image;
                        frameViewModel.Duration = frame.Duration;
                        stateViewModel.FrameViewModels.Add(frameViewModel);
                    }
                }
            }
        }
    }
}

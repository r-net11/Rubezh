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
            DispatcherTimerStop = true;
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

        public bool dispatcherTimerStop;
        public bool DispatcherTimerStop
        {
            get { return dispatcherTimerStop; }
            set
            {
                dispatcherTimerStop = value;
                dispatcherTimer.Stop();
                OnPropertyChanged("DispatcherTimerStop");
            }
        }


        public static DispatcherTimer dispatcherTimer = new DispatcherTimer();

        bool DispatcherTimerTickOn = false;
        string frame1, frame2;
        int frameDuration1 = 100, frameDuration2 = 100;
        StateViewModel selectedStateViewModel;
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
                dispatcherTimer.Stop();
                if (selectedStateViewModel.FrameViewModels.Count > 1)
                {
                    frame2 = Svg2Xaml.XSLT_Transform(selectedStateViewModel.FrameViewModels[1].Image, RubezhDevices.RubezhDevice.svg2xaml_xsl);
                    frameDuration2 = selectedStateViewModel.FrameViewModels[1].Duration;
                    if (!DispatcherTimerTickOn)
                    {
                        dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
                        DispatcherTimerTickOn = true;
                    }
                    dispatcherTimer.Start();
                }
                else
                {
                    StringReader stringReader = new StringReader(frame1);
                    XmlReader xmlReader = XmlReader.Create(stringReader);
                    FrameViewModel.Current.ReaderLoadButton = (Canvas)XamlReader.Load(xmlReader);
                }
                OnPropertyChanged("SelectedStateViewModel");
            }
        }

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
                    dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, frameDuration1);
                    frame1 = Svg2Xaml.XSLT_Transform(selectedStateViewModel.FrameViewModels[0].Image, RubezhDevices.RubezhDevice.svg2xaml_xsl);
                    StringReader stringReader = new StringReader(frame1);
                    XmlReader xmlReader = XmlReader.Create(stringReader);
                    FrameViewModel.Current.ReaderLoadButton = (Canvas)XamlReader.Load(xmlReader);
                }
                catch { }
            }

            else
            {
                try
                {
                    dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, frameDuration2);
                    frame2 = Svg2Xaml.XSLT_Transform(selectedStateViewModel.FrameViewModels[1].Image, RubezhDevices.RubezhDevice.svg2xaml_xsl);
                    StringReader stringReader = new StringReader(frame2);
                    XmlReader xmlReader = XmlReader.Create(stringReader);
                    FrameViewModel.Current.ReaderLoadButton = (Canvas)XamlReader.Load(xmlReader);

                }
                catch { }
            }
        }

        FrameViewModel selectedFrameViewModel;
        public FrameViewModel SelectedFrameViewModel
        {
            get { return selectedFrameViewModel; }
            set
            {
                selectedFrameViewModel = value;
                if (selectedFrameViewModel == null)
                    return;
                string frameImage = Svg2Xaml.XSLT_Transform(selectedFrameViewModel.Image, RubezhDevices.RubezhDevice.svg2xaml_xsl);
                StringReader stringReader = new StringReader(frameImage);
                XmlReader xmlReader = XmlReader.Create(stringReader);
                FrameViewModel.Current.ReaderLoadButton = (Canvas)XamlReader.Load(xmlReader);
                SelectedFrameViewModel = selectedFrameViewModel;
                OnPropertyChanged("SelectedFrameViewModel");
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

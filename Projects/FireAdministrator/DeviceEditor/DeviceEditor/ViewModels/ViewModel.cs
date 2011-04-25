using System.Collections.ObjectModel;
using System.Windows;
using System.IO;
using System.Xml.Serialization;
using Common;
using System.Xml;

using Resources;
using System.Windows.Markup;
using System.Collections.Generic;
using System;
using System.Windows.Input;
using System.Windows.Threading;
using Firesec.Metadata;
using System.Linq;

namespace DeviceEditor
{
    public class ViewModel : BaseViewModel
    {
        public ViewModel()
        {
            Current = this;
            LoadMetadata();
            Load();

            SaveCommand = new RelayCommand(OnSaveCommand);
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
        }

        public static ViewModel Current { get; private set; }
        /// <summary>
        /// Список всех устройств, полученный из файла metadata.xml
        /// </summary>
        public List<drvType> DevicesList;
        public void LoadMetadata()
        {
            FileStream file_xml = new FileStream(References.metadata_xml, FileMode.Open, FileAccess.Read, FileShare.Read);
            XmlSerializer serializer = new XmlSerializer(typeof(config));
            config metadata = (config)serializer.Deserialize(file_xml);
            file_xml.Close();

            DevicesList = new List<drvType>();
            foreach (drvType drivers in metadata.drv)
                try
                {
                    DevicesList.Add(drivers);

                }
                catch { }
        }

        /// <summary>
        /// Метод преобразующий svg-строку в Canvas c рисунком.
        /// </summary>
        /// <param name="svgString">svg-строка</param>
        /// <returns>Canvas, полученный из svg-строки</returns>
        static public System.Windows.Controls.Canvas Str2Canvas(string svgString, int layer)
        {
            string frameImage = Functions.Svg2Xaml(svgString, Resources.References.svg2xaml_xsl);
            StringReader stringReader = new StringReader(frameImage);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            System.Windows.Controls.Canvas Picture = (System.Windows.Controls.Canvas)XamlReader.Load(xmlReader);
            System.Windows.Controls.Canvas.SetZIndex(Picture, layer);
            return (Picture);
        }

        /// <summary>
        /// Комманда сохранения текущей конфигурации в файл.
        /// </summary>
        public RelayCommand SaveCommand { get; private set; }
        public void OnSaveCommand(object obj)
        {
            DeviceManager deviceManager = new DeviceManager();
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
                foreach (StateViewModel stateViewModel in deviceViewModel.StatesViewModel)
                {
                    State state = new State();
                    state.Id = stateViewModel.Id;
                    state.IsAdditional = stateViewModel.IsAdditional;
                    device.States.Add(state);
                    state.Frames = new List<Frame>();
                    foreach (FrameViewModel frameViewModel in stateViewModel.FrameViewModels)
                    {
                        Frame frame = new Frame();
                        frame.Id = frameViewModel.Id;
                        frame.Image = frameViewModel.Image;
                        frame.Duration = frameViewModel.Duration;
                        frame.Layer = frameViewModel.Layer;
                        state.Frames.Add(frame);
                    }
                }
            }
            FileStream file_xml = new FileStream(Resources.References.deviceLibrary_xml, FileMode.Create, FileAccess.Write, FileShare.Write);
            XmlSerializer serializer = new XmlSerializer(typeof(DeviceManager));
            serializer.Serialize(file_xml, deviceManager);
            file_xml.Close();
        }

        ObservableCollection<DeviceViewModel> deviceViewModels;
        /// <summary>
        /// Список всех устройств
        /// </summary>
        public ObservableCollection<DeviceViewModel> DeviceViewModels
        {
            get { return deviceViewModels; }
            set
            {
                deviceViewModels = value;
                OnPropertyChanged("DeviceViewModels");
            }
        }

        DeviceViewModel selectedDeviceViewModel;
        /// <summary>
        /// Выбранное устройство.
        /// </summary>
        public DeviceViewModel SelectedDeviceViewModel
        {
            get { return selectedDeviceViewModel; }
            set
            {
                selectedDeviceViewModel = value;
                OnPropertyChanged("SelectedDeviceViewModel");
            }
        }

        StateViewModel selectedStateViewModel;
        /// <summary>
        /// Выбранное состояние.
        /// </summary>
        public StateViewModel SelectedStateViewModel
        {
            get { return selectedStateViewModel; }
            set
            {
                selectedStateViewModel = value;
                SelectedStateViewModel.ParentDevice.StatesPicture.Clear();
                SelectedStateViewModel.SelectedFrameViewModel = selectedStateViewModel.FrameViewModels[0];
                SelectedStateViewModel.ParentDevice.StatesPicture.Add(Str2Canvas(SelectedStateViewModel.SelectedFrameViewModel.Image, selectedStateViewModel.FrameViewModels[0].Layer));

                if (selectedStateViewModel.FrameViewModels.Count > 1)
                {
                    SelectedStateViewModel.ParentDevice.StatesPicture.Clear();
                    dispatcherTimer.Start();
                }
                OnPropertyChanged("SelectedStateViewModel");
            }
        }

        ObservableCollection<StateViewModel> additionalStatesViewModel;
        public ObservableCollection<StateViewModel> AdditionalStatesViewModel
        {
            get { return additionalStatesViewModel; }
            set
            {
                additionalStatesViewModel = value;
                OnPropertyChanged("AdditionalStatesViewModel");
            }
        }

        public static DispatcherTimer dispatcherTimer = new DispatcherTimer();
        int tick = 0;

        public System.Windows.Controls.Canvas DynamicPicture;
        /****************Таймер*****************/
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();
            string frameImage;
            StringReader stringReader;
            XmlReader xmlReader;
            try
            {
                SelectedStateViewModel.ParentDevice.StatesPicture.Remove(DynamicPicture);
                dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, SelectedStateViewModel.FrameViewModels[tick].Duration);
                frameImage = Functions.Svg2Xaml(SelectedStateViewModel.FrameViewModels[tick].Image, Resources.References.svg2xaml_xsl);
                stringReader = new StringReader(frameImage);
                xmlReader = XmlReader.Create(stringReader);
                DynamicPicture = (System.Windows.Controls.Canvas)XamlReader.Load(xmlReader);
                System.Windows.Controls.Canvas.SetZIndex(DynamicPicture, SelectedStateViewModel.FrameViewModels[tick].Layer);
                SelectedStateViewModel.ParentDevice.StatesPicture.Add(DynamicPicture);
                tick = (tick + 1) % SelectedStateViewModel.FrameViewModels.Count;
            }
            catch { }
        }

        public void Load()
        {
            DeviceManager deviceManager = new DeviceManager();
            FileStream file_xml = new FileStream(References.deviceLibrary_xml, FileMode.Open, FileAccess.Read, FileShare.Read);
            XmlSerializer serializer = new XmlSerializer(typeof(DeviceManager));
            deviceManager = (DeviceManager)serializer.Deserialize(file_xml);
            file_xml.Close();

            DeviceViewModels = new ObservableCollection<DeviceViewModel>();
            AdditionalStatesViewModel = new ObservableCollection<StateViewModel>();
            foreach (Device device in deviceManager.Devices)
            {
                DeviceViewModel deviceViewModel = new DeviceViewModel();
                deviceViewModel.Id = device.Id;
                DeviceViewModels.Add(deviceViewModel);
                deviceViewModel.StatesViewModel = new ObservableCollection<StateViewModel>();
                try
                {
                    deviceViewModel.IconPath = @"C:/Program Files/Firesec/Icons/" + DevicesList.FirstOrDefault(x => x.name == deviceViewModel.Id).dev_icon + ".ico";
                }
                catch { }
                foreach (State state in device.States)
                {
                    StateViewModel stateViewModel = new StateViewModel();
                    stateViewModel.Id = state.Id;
                    stateViewModel.IsAdditional = state.IsAdditional;
                    deviceViewModel.StatesViewModel.Add(stateViewModel);
                    stateViewModel.FrameViewModels = new ObservableCollection<FrameViewModel>();
                    foreach (Frame frame in state.Frames)
                    {
                        FrameViewModel frameViewModel = new FrameViewModel();
                        frameViewModel.Id = frame.Id;
                        frameViewModel.Image = frame.Image;
                        frameViewModel.Duration = frame.Duration;
                        frameViewModel.Layer = frame.Layer;
                        stateViewModel.FrameViewModels.Add(frameViewModel);
                    }
                }
            }
        }
    }
}

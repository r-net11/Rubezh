using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Serialization;
using Common;
using Firesec.Metadata;
using Firesec;
using DeviceLibrary;
using Frame = DeviceLibrary.Frame;

namespace DeviceEditor
{
    public class ViewModel : BaseViewModel
    {
        public static DispatcherTimer dispatcherTimer = new DispatcherTimer();

        /// <summary>
        /// Список всех устройств, полученный из файла metadata.xml
        /// </summary>
        public List<drvType> DevicesList;

        public Canvas DynamicPicture;
        private ObservableCollection<StateViewModel> additionalStatesViewModel;
        private ObservableCollection<DeviceViewModel> deviceViewModels;
        private DeviceViewModel selectedDeviceViewModel;
        private StateViewModel selectedStateViewModel;
        private int tick;

        public ViewModel()
        {
            Current = this;
            LoadMetadata();
            Load();

            SaveCommand = new RelayCommand(OnSaveCommand);
            dispatcherTimer.Tick += dispatcherTimer_Tick;
        }

        public static ViewModel Current { get; private set; }

        /// <summary>
        /// Комманда сохранения текущей конфигурации в файл.
        /// </summary>
        public RelayCommand SaveCommand { get; private set; }

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
                SelectedStateViewModel.ParentDevice.StatesPicture.Add(
                    Str2Canvas(SelectedStateViewModel.SelectedFrameViewModel.Image,
                               selectedStateViewModel.FrameViewModels[0].Layer));

                if (selectedStateViewModel.FrameViewModels.Count > 1)
                {
                    SelectedStateViewModel.ParentDevice.StatesPicture.Clear();
                    dispatcherTimer.Start();
                }
                OnPropertyChanged("SelectedStateViewModel");
            }
        }

        public ObservableCollection<StateViewModel> AdditionalStatesViewModel
        {
            get { return additionalStatesViewModel; }
            set
            {
                additionalStatesViewModel = value;
                OnPropertyChanged("AdditionalStatesViewModel");
            }
        }

        public void LoadMetadata()
        {
            var file_xml = new FileStream(ResourceHelper.metadata_xml, FileMode.Open, FileAccess.Read, FileShare.Read);
            var serializer = new XmlSerializer(typeof(config));
            var metadata = (config)serializer.Deserialize(file_xml);
            file_xml.Close();

            DevicesList = new List<drvType>();
            foreach (drvType drivers in metadata.drv)
                try
                {
                    DevicesList.Add(drivers);
                }
                catch
                {
                }
        }

        /// <summary>
        /// Метод преобразующий svg-строку в Canvas c рисунком.
        /// </summary>
        /// <param name="svgString">svg-строка</param>
        /// <returns>Canvas, полученный из svg-строки</returns>
        public static Canvas Str2Canvas(string svgString, int layer)
        {
            string frameImage = SvgConverter.Svg2Xaml(svgString, ResourceHelper.svg2xaml_xsl);
            var stringReader = new StringReader(frameImage);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            var Picture = (Canvas)XamlReader.Load(xmlReader);
            Panel.SetZIndex(Picture, layer);
            return (Picture);
        }

        public void OnSaveCommand(object obj)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены что хотите сохранить все изменения на диск?",
                                                      "Окно подтверждения", MessageBoxButton.OKCancel,
                                                      MessageBoxImage.Question);
            if (result == MessageBoxResult.Cancel)
                return;
            LibraryManager.Devices = new List<Device>();
            foreach (DeviceViewModel deviceViewModel in DeviceViewModels)
            {
                var device = new Device();
                device.Id = deviceViewModel.Id;
                LibraryManager.Devices.Add(device);
                device.States = new List<State>();
                foreach (StateViewModel stateViewModel in deviceViewModel.StatesViewModel)
                {
                    var state = new State();
                    state.Id = stateViewModel.Id;
                    state.IsAdditional = stateViewModel.IsAdditional;
                    device.States.Add(state);
                    state.Frames = new List<Frame>();
                    foreach (FrameViewModel frameViewModel in stateViewModel.FrameViewModels)
                    {
                        var frame = new Frame();
                        frame.Id = frameViewModel.Id;
                        frame.Image = frameViewModel.Image;
                        frame.Duration = frameViewModel.Duration;
                        frame.Layer = frameViewModel.Layer;
                        state.Frames.Add(frame);
                    }
                }
            }
            LibraryManager.Save();
        }

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
                dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0,
                                                        SelectedStateViewModel.FrameViewModels[tick].Duration);
                frameImage = SvgConverter.Svg2Xaml(SelectedStateViewModel.FrameViewModels[tick].Image,
                                                ResourceHelper.svg2xaml_xsl);
                stringReader = new StringReader(frameImage);
                xmlReader = XmlReader.Create(stringReader);
                DynamicPicture = (Canvas)XamlReader.Load(xmlReader);
                Panel.SetZIndex(DynamicPicture, SelectedStateViewModel.FrameViewModels[tick].Layer);
                SelectedStateViewModel.ParentDevice.StatesPicture.Add(DynamicPicture);
                tick = (tick + 1) % SelectedStateViewModel.FrameViewModels.Count;
            }
            catch
            {
            }
        }

        public void Load()
        {
            DeviceViewModels = new ObservableCollection<DeviceViewModel>();
            AdditionalStatesViewModel = new ObservableCollection<StateViewModel>();
            foreach (Device device in LibraryManager.Devices)
            {
                var deviceViewModel = new DeviceViewModel();
                deviceViewModel.Id = device.Id;
                DeviceViewModels.Add(deviceViewModel);
                deviceViewModel.StatesViewModel = new ObservableCollection<StateViewModel>();
                try
                {
                    deviceViewModel.IconPath = @"C:/Program Files/Firesec/Icons/" +
                                               DevicesList.FirstOrDefault(x => x.name == DriversHelper.GetDriverNameById(deviceViewModel.Id)).dev_icon +
                                               ".ico";
                }
                catch
                {
                }
                foreach (State state in device.States)
                {
                    var stateViewModel = new StateViewModel();
                    stateViewModel.Id = state.Id;
                    stateViewModel.IsAdditional = state.IsAdditional;
                    deviceViewModel.StatesViewModel.Add(stateViewModel);
                    stateViewModel.FrameViewModels = new ObservableCollection<FrameViewModel>();
                    foreach (Frame frame in state.Frames)
                    {
                        var frameViewModel = new FrameViewModel();
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
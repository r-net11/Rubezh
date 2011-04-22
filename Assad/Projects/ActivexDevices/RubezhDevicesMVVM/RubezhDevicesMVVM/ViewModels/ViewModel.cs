using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using Common;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Input;
using System.Xml;
using System.Windows.Markup;
using RubezhDevices;

namespace RubezhDevicesMVVM
{
    public class ViewModel:BaseViewModel
    {
        public ViewModel()
        {
            Current = this;
            StatesPicture = new ObservableCollection<Canvas>();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            deviceSelectionViewModel = new DeviceSelectionViewModel();
            DeviceManager = new DeviceManager();
            DeviceManagerLoad();
            DeviceSelectionCommand = new RelayCommand(OnDeviceSelectionCommand);
        }
        public static string deviceLibrary_xml = @"c:\Rubezh\Assad\Projects\ActivexDevices\Library\DeviceLibrary.xml";
        public static string metadata_xml = @"c:\Rubezh\Assad\Projects\Assad\DeviceModelManager\metadata.xml";
        static public ViewModel Current { get; private set; }
        DeviceSelectionViewModel deviceSelectionViewModel;
        DeviceManager DeviceManager;

        public RelayCommand DeviceSelectionCommand{ get; private set; }
        public void OnDeviceSelectionCommand(object obj)
        {
            DeviceSelectionView deviceSelectionView = new DeviceSelectionView();
            deviceSelectionView.DataContext = deviceSelectionViewModel;
            deviceSelectionView.ShowDialog();
        }
        void DeviceManagerLoad()
        {
            FileStream file_xml = new FileStream(ViewModel.deviceLibrary_xml, FileMode.Open, FileAccess.Read, FileShare.Read);
            XmlSerializer serializer = new XmlSerializer(typeof(DeviceManager));
            DeviceManager = (DeviceManager)serializer.Deserialize(file_xml);
            file_xml.Close();

            Load();
        }


        ObservableCollection<DeviceViewModel> devicesViewModel;
        public ObservableCollection<DeviceViewModel> DevicesViewModel
        {
            get { return devicesViewModel; }
            set
            {
                devicesViewModel = value;
                DeviceSelectionViewModel.Current.DevicesViewModel = devicesViewModel;
                OnPropertyChanged("DevicesViewModel");
            }
        }

        DeviceViewModel selectedDeviceViewModel;
        public DeviceViewModel SelectedDeviceViewModel
        {
            get { return selectedDeviceViewModel; }
            set
            {
                selectedDeviceViewModel = value;
                DeviceSelectionViewModel.Current.SelectedDeviceViewModel = selectedDeviceViewModel;
                OnPropertyChanged("SelectedDeviceViewModel");
            }
        }

        /// <summary>
        /// Метод преобразующий svg-строку в Canvas c рисунком.
        /// </summary>
        /// <param name="svgString">svg-строка</param>
        /// <returns>Canvas, полученный из svg-строки</returns>
        static public Canvas Str2Canvas(string svgString, int layer)
        {
            string frameImage = Svg2Xaml.XSLT_Transform(svgString, RubezhDevices.RubezhDevice.svg2xaml_xsl);
            StringReader stringReader = new StringReader(frameImage);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            Canvas Picture = (Canvas)XamlReader.Load(xmlReader);
            Canvas.SetZIndex(Picture, layer);
            return (Picture);
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

        public void Load()
        {
            DevicesViewModel = new ObservableCollection<DeviceViewModel>();
            foreach (Device device in DeviceManager.Devices)
            {
                DeviceViewModel deviceViewModel = new DeviceViewModel();
                deviceViewModel.Id = device.Id;
                DevicesViewModel.Add(deviceViewModel);
                deviceViewModel.StatesViewModel = new ObservableCollection<StateViewModel>();
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

        public ObservableCollection<Canvas> statesPicture;
        public ObservableCollection<Canvas> StatesPicture
        {
            get { return statesPicture; }
            set
            {
                statesPicture = value;
                OnPropertyChanged("StatesPicture");
            }
        }

        public static DispatcherTimer dispatcherTimer = new DispatcherTimer();
        int tick = 0;

        public Canvas DynamicPicture;
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
                frameImage = RubezhDevices.Svg2Xaml.XSLT_Transform(SelectedStateViewModel.FrameViewModels[tick].Image, RubezhDevices.RubezhDevice.svg2xaml_xsl);
                stringReader = new StringReader(frameImage);
                xmlReader = XmlReader.Create(stringReader);
                DynamicPicture = (Canvas)XamlReader.Load(xmlReader);
                Canvas.SetZIndex(DynamicPicture, SelectedStateViewModel.FrameViewModels[tick].Layer);
                SelectedStateViewModel.ParentDevice.StatesPicture.Add(DynamicPicture);
                tick = (tick + 1) % SelectedStateViewModel.FrameViewModels.Count;
            }
            catch { }
        }
    }
}

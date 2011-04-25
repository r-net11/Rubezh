using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using Common;
using System.Collections.ObjectModel;

using System.Windows.Threading;
using System.Windows.Input;
using System.Xml;
using System.Windows.Markup;
using Resources;

namespace DevicesControl
{
    public class ViewModel:BaseViewModel
    {
        public ViewModel()
        {
            Current = this;
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            deviceSelectionViewModel = new DeviceSelectionViewModel();
            DeviceManager = new DeviceManager();
            DeviceManagerLoad();
        }

        static public ViewModel Current { get; private set; }
        DeviceSelectionViewModel deviceSelectionViewModel;
        DeviceManager DeviceManager;

        void DeviceManagerLoad()
        {
            FileStream file_xml = new FileStream(Resources.References.deviceLibrary_xml, FileMode.Open, FileAccess.Read, FileShare.Read);
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
        static public System.Windows.Controls.Canvas Str2Canvas(string svgString, int layer)
        {
            string frameImage = Functions.Svg2Xaml(svgString, Resources.References.svg2xaml_xsl);
            StringReader stringReader = new StringReader(frameImage);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            System.Windows.Controls.Canvas Picture = (System.Windows.Controls.Canvas)XamlReader.Load(xmlReader);
            System.Windows.Controls.Canvas.SetZIndex(Picture, layer);
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
                if (selectedStateViewModel == null)
                    return;
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
    }
}

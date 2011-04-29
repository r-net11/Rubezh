using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Serialization;
using Common;
using Resources;
using Frame = Resources.Frame;

namespace DevicesControl
{
    public class ViewModel : BaseViewModel
    {
        private static DeviceManager DeviceManager;
        private readonly DispatcherTimer dispatcherTimer = new DispatcherTimer();
        public Canvas DynamicPicture;
        public string Name;
        private List<StateViewModel> additionalStatesViewModel;
        private DeviceViewModel selectedDeviceViewModel;
        private StateViewModel selectedStateViewModel;
        public ObservableCollection<Canvas> statesPicture;
        private int tick;

        public ViewModel()
        {
            Current = this;
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            StatesPicture = new ObservableCollection<Canvas>();
        }

        public static ViewModel Current { get; private set; }

        public static ObservableCollection<DeviceViewModel> DevicesViewModel { get; set; }

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
                if (selectedStateViewModel == null)
                    return;
                StatesPicture.Clear();
                SelectedStateViewModel.SelectedFrameViewModel = selectedStateViewModel.FrameViewModels[0];
                StatesPicture.Add(Functions.Str2Canvas(SelectedStateViewModel.SelectedFrameViewModel.Image,
                                                       selectedStateViewModel.FrameViewModels[0].Layer));

                if (selectedStateViewModel.FrameViewModels.Count > 1)
                {
                    StatesPicture.Clear();
                    dispatcherTimer.Start();
                }
                OnPropertyChanged("SelectedStateViewModel");
            }
        }

        public List<StateViewModel> AdditionalStatesViewModel
        {
            get { return additionalStatesViewModel; }
            set
            {
                additionalStatesViewModel = value;
                for (int i = 0; i < additionalStatesViewModel.Count; i++)
                {
                    additionalStatesViewModel[i].ParentViewModel = this;
                    additionalStatesViewModel[i].IsChecked = true;
                }
            }
        }

        public ObservableCollection<Canvas> StatesPicture
        {
            get { return statesPicture; }
            set
            {
                statesPicture = value;
                OnPropertyChanged("StatesPicture");
            }
        }

        public static void DeviceManagerLoad()
        {
            var file_xml = new FileStream(References.deviceLibrary_xml, FileMode.Open, FileAccess.Read, FileShare.Read);
            var serializer = new XmlSerializer(typeof (DeviceManager));
            DeviceManager = (DeviceManager) serializer.Deserialize(file_xml);
            file_xml.Close();

            Load();
        }

        public static void Load()
        {
            DevicesViewModel = new ObservableCollection<DeviceViewModel>();
            foreach (Device device in DeviceManager.Devices)
            {
                var deviceViewModel = new DeviceViewModel();
                deviceViewModel.Id = device.Id;
                DevicesViewModel.Add(deviceViewModel);
                deviceViewModel.StatesViewModel = new ObservableCollection<StateViewModel>();
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

        /****************Таймер*****************/
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();
            string frameImage;
            StringReader stringReader;
            XmlReader xmlReader;
            try
            {
                StatesPicture.Remove(DynamicPicture);
                dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0,
                                                        SelectedStateViewModel.FrameViewModels[tick].Duration);
                frameImage = Functions.Svg2Xaml(SelectedStateViewModel.FrameViewModels[tick].Image,
                                                References.svg2xaml_xsl);
                stringReader = new StringReader(frameImage);
                xmlReader = XmlReader.Create(stringReader);
                DynamicPicture = (Canvas) XamlReader.Load(xmlReader);
                Panel.SetZIndex(DynamicPicture, SelectedStateViewModel.FrameViewModels[tick].Layer);
                StatesPicture.Add(DynamicPicture);
                tick = (tick + 1)%SelectedStateViewModel.FrameViewModels.Count;
            }
            catch
            {
            }
        }
    }
}
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
using DeviceControls;

namespace DeviceEditor
{
    public class ViewModel : BaseViewModel
    {
        private ObservableCollection<DeviceViewModel> deviceViewModels;
        private DeviceViewModel selectedDeviceViewModel;
        private StateViewModel selectedStateViewModel;

        public ViewModel()
        {
            Current = this;
            LoadMetadata();
            Load();
            SaveCommand = new RelayCommand(OnSaveCommand);

        }
        /// <summary>
        /// Список всех устройств, полученный из файла metadata.xml
        /// </summary>
        public List<drvType> DevicesList;
        public Canvas DynamicPicture;
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

                SelectedStateViewModel.ParentDevice.DeviceControl.DriverId = SelectedStateViewModel.ParentDevice.Id;
                SelectedStateViewModel.ParentDevice.DeviceControl.IsAdditional = SelectedStateViewModel.IsAdditional;
                SelectedStateViewModel.ParentDevice.DeviceControl.StateId = SelectedStateViewModel.Id;
                if (SelectedStateViewModel.IsAdditional)
                    SelectedStateViewModel.ParentDevice.DeviceControl.AdditionalStatesIds = null;
                else
                    SelectedStateViewModel.ParentDevice.DeviceControl.AdditionalStatesIds = SelectedStateViewModel.ParentDevice.AdditionalStatesViewModel;

                OnPropertyChanged("SelectedStateViewModel");
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
            LibraryManager.Save();
        }

        public void Load()
        {
            DeviceViewModels = new ObservableCollection<DeviceViewModel>();
            foreach (Device device in LibraryManager.Devices)
            {
                var deviceViewModel = new DeviceViewModel();
                deviceViewModel.Id = device.Id;
                DeviceViewModels.Add(deviceViewModel);
                deviceViewModel.StatesViewModel = new ObservableCollection<StateViewModel>();
                try
                {
                    deviceViewModel.IconPath = @"C:/Program Files/Firesec/Icons/" + DevicesList.FirstOrDefault(x => x.name == DriversHelper.GetDriverNameById(deviceViewModel.Id)).dev_icon + ".ico";
                }
                catch
                {
                }
                foreach (State state in device.States)
                {
                    var stateViewModel = new StateViewModel();
                    stateViewModel.IsAdditional = state.IsAdditional;
                    stateViewModel.Id = state.Id;
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
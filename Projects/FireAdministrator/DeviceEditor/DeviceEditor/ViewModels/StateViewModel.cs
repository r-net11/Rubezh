using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Xml;
using Common;
using DeviceLibrary;
using System.Linq;
using Firesec;
using Firesec.Metadata;

namespace DeviceEditor
{
    public class StateViewModel : BaseViewModel
    {
        private readonly DispatcherTimer dispatcherTimer;
        private Canvas framePicture;

        /// <summary>
        /// Список кадров состояния.
        /// </summary>
        public ObservableCollection<FrameViewModel> frameViewModels;

        /// <summary>
        /// Путь к иконке состояний.
        /// </summary>
        private string iconPath = @"C:\Rubezh\Assad\Projects\ActivexDevices\Library\Icons\3.png";

        /// <summary>
        /// Идентификатор состояния.
        /// </summary>
        public string id;

        private bool isAdditional;
        private bool isChecked;

        /// <summary>
        /// Выбранный кадр.
        /// </summary>
        private FrameViewModel selectedFrameViewModel;

        private int tick;

        public StateViewModel()
        {
            Current = this;
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            ParentDevice = DeviceViewModel.Current;
            RemoveStateCommand = new RelayCommand(OnRemoveStateCommand);
        }

        public static StateViewModel Current { get; private set; }
        public DeviceViewModel ParentDevice { get; set; }

        public FrameViewModel SelectedFrameViewModel
        {
            get { return selectedFrameViewModel; }
            set
            {
                selectedFrameViewModel = value;
                if (selectedFrameViewModel == null)
                    return;
                OnPropertyChanged("SelectedFrameViewModel");
            }
        }

        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;
                if ((IsAdditional) && (isChecked))
                {
                    dispatcherTimer.Start();
                }
                if (!isChecked)
                {
                    try
                    {
                        ParentDevice.StatesPicture.Remove(FramePicture);
                        dispatcherTimer.Stop();
                    }
                    catch
                    {
                    }
                }
                OnPropertyChanged("IsChecked");
            }
        }

        public string IconPath
        {
            get { return iconPath; }
            set
            {
                iconPath = value;
                OnPropertyChanged("IconPath");
            }
        }

        /// <summary>
        /// Команда удаления состояния.
        /// </summary>
        public RelayCommand RemoveStateCommand { get; private set; }

        public string Id
        {
            get { return id; }
            set
            {
                id = value;
                drvType driver = LibraryManager.Drivers.FirstOrDefault(x => x.id == ParentDevice.Id);
                if (IsAdditional)
                    Name = driver.state.FirstOrDefault(x => x.id == id).name;
                else
                    Name = LibraryManager.BaseStatesList[Convert.ToInt16(id)];

            }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        public ObservableCollection<FrameViewModel> FrameViewModels
        {
            get { return frameViewModels; }
            set
            {
                frameViewModels = value;
                OnPropertyChanged("FrameViewModels");
            }
        }

        public bool IsAdditional
        {
            get { return isAdditional; }
            set
            {
                isAdditional = value;
                OnPropertyChanged("IsAdditional");
            }
        }

        public Canvas FramePicture
        {
            get { return framePicture; }
            set
            {
                framePicture = value;
                OnPropertyChanged("FramePicture");
            }
        }

        private void OnRemoveStateCommand(object obj)
        {
            if (!IsAdditional)
            {
                MessageBox.Show("Невозможно удалить основное состояние");
                return;
            }
            else
            {
                IsChecked = false;
            }
            ParentDevice.StatesViewModel.Remove(this);
            var stateViewModel = new StateViewModel();
            stateViewModel.Id = Id;
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            ParentDevice.StatesPicture.Remove(FramePicture);
            if (ViewModel.Current.SelectedStateViewModel == null)
            {
                return;
            }
            if (!ViewModel.Current.SelectedStateViewModel.IsAdditional)
            {
                try
                {
                    string frameImage = SvgConverter.Svg2Xaml(FrameViewModels[tick].Image, ResourceHelper.svg2xaml_xsl);
                    var stringReader = new StringReader(frameImage);
                    XmlReader xmlReader = XmlReader.Create(stringReader);
                    FramePicture = (Canvas) XamlReader.Load(xmlReader);
                    Panel.SetZIndex(FramePicture, FrameViewModels[tick].Layer);
                    ParentDevice.StatesPicture.Add(FramePicture);

                    dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, FrameViewModels[tick].Duration);
                }
                catch
                {
                }
            }
            tick = (tick + 1)%FrameViewModels.Count;
        }
    }
}
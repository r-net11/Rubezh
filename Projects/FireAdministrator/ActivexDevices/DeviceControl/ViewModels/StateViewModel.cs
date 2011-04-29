using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Xml;
using Common;
using Resources;

namespace DevicesControl
{
    public class StateViewModel : BaseViewModel
    {
        private DispatcherTimer dispatcherTimer;
        private Canvas framePicture;

        /// <summary>
        /// Список кадров состояния.
        /// </summary>
        public ObservableCollection<FrameViewModel> frameViewModels;

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
            ParentDevice = DeviceViewModel.Current;
        }

        public ViewModel ParentViewModel { get; set; }
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
                    dispatcherTimer = new DispatcherTimer();
                    dispatcherTimer.Tick += dispatcherTimer_Tick;
                    dispatcherTimer.Start();
                }
                if (!isChecked)
                {
                    try
                    {
                        ParentViewModel.StatesPicture.Remove(FramePicture);
                        dispatcherTimer.Stop();
                    }
                    catch
                    {
                    }
                }
                OnPropertyChanged("IsChecked");
            }
        }

        public string Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("Id");
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

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {

            ParentViewModel.StatesPicture.Remove(FramePicture);
            if (ViewModel.Current.SelectedStateViewModel == null)
                return;
            if (!ViewModel.Current.SelectedStateViewModel.IsAdditional)
            {
                string frameImage = Functions.Svg2Xaml(FrameViewModels[tick].Image, References.svg2xaml_xsl);
                var stringReader = new StringReader(frameImage);
                XmlReader xmlReader = XmlReader.Create(stringReader);
                FramePicture = (Canvas)XamlReader.Load(xmlReader);
                Panel.SetZIndex(FramePicture, FrameViewModels[tick].Layer);
                ParentViewModel.StatesPicture.Add(FramePicture);

                dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, FrameViewModels[tick].Duration);
            }
            tick = (tick + 1) % FrameViewModels.Count;

        }
    }
}
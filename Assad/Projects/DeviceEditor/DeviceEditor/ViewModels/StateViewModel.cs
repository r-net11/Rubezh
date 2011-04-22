using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Common;
using System.Windows.Controls;
using RubezhDevices;
using System.IO;
using System.Xml;
using System.Windows.Markup;
using System.Windows.Input;
using Firesec.Metadata;
using System.Windows;
using System.Windows.Threading;

namespace DeviceEditor
{
    public class StateViewModel : BaseViewModel
    {

        public StateViewModel()
        {
            Current = this;
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            ParentDevice = DeviceViewModel.Current;
            RemoveStateCommand = new RelayCommand(OnRemoveStateCommand);
        }
        public static StateViewModel Current { get; private set; }
        public DeviceViewModel ParentDevice { get; set; }
        
        /// <summary>
        /// Выбранный кадр.
        /// </summary>
        FrameViewModel selectedFrameViewModel;
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

        bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;
                if ((this.IsAdditional)&&(this.isChecked))
                {
                    dispatcherTimer.Start();
                }
                if (!this.isChecked)
                {
                    try
                    {
                        this.ParentDevice.StatesPicture.Remove(FramePicture);
                        dispatcherTimer.Stop();
                    }
                    catch { }
                }
                OnPropertyChanged("IsChecked");
            }
        }

        /// <summary>
        /// Путь к иконке состояний.
        /// </summary>
        string iconPath = @"C:\Rubezh\Assad\Projects\ActivexDevices\Library\Icons\3.png";
        public string IconPath
        {
            get
            {
                return iconPath;
            }
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
        void OnRemoveStateCommand(object obj)
        {
            if (!this.IsAdditional)
            {
                MessageBox.Show("Невозможно удалить основное состояние");
                return;
            }
            else
            {
                this.IsChecked = false;
            }
            this.ParentDevice.StatesViewModel.Remove(this);
            StateViewModel stateViewModel = new StateViewModel();
            stateViewModel.Id = this.Id;
        }

        /// <summary>
        /// Идентификатор состояния.
        /// </summary>
        public string id;
        public string Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("Id");
            }
        }

        /// <summary>
        /// Список кадров состояния.
        /// </summary>
        public ObservableCollection<FrameViewModel> frameViewModels;
        public ObservableCollection<FrameViewModel> FrameViewModels
        {
            get { return frameViewModels; }
            set
            {
                frameViewModels = value;
                OnPropertyChanged("FrameViewModels");
            }
        }

        bool isAdditional;
        public bool IsAdditional
        {
            get { return isAdditional; }
            set
            {
                isAdditional = value;
                OnPropertyChanged("IsAdditional");
            }
        }

        DispatcherTimer dispatcherTimer;
        int tick;

        Canvas framePicture;
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
            this.ParentDevice.StatesPicture.Remove(FramePicture);
            if (ViewModel.Current.SelectedStateViewModel == null)
            {
                return;
            }
            if (!ViewModel.Current.SelectedStateViewModel.IsAdditional)
            {
                try
                {
                    string frameImage = Svg2Xaml.XSLT_Transform(FrameViewModels[tick].Image, RubezhDevices.RubezhDevice.svg2xaml_xsl);
                    StringReader stringReader = new StringReader(frameImage);
                    XmlReader xmlReader = XmlReader.Create(stringReader);
                    FramePicture = (Canvas)XamlReader.Load(xmlReader);
                    Canvas.SetZIndex(FramePicture, FrameViewModels[tick].Layer);
                    this.ParentDevice.StatesPicture.Add(FramePicture);

                    dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, FrameViewModels[tick].Duration);
                }
                catch { }
            }
            tick = (tick + 1) % FrameViewModels.Count;
        }

    }
}

﻿using System;
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
            Parent = DeviceViewModel.Current;
            RemoveStateCommand = new RelayCommand(OnRemoveStateCommand);
        }
        public static StateViewModel Current { get; private set; }
        public DeviceViewModel Parent { get; set; }
        
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
                    DynamicPicturesList = new ObservableCollection<Canvas>();
                    dispatcherTimer = new DispatcherTimer();
                    dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
                    dispatcherTimer.Start();
                }
                if (!this.isChecked)
                {
                    try
                    {
                        ViewModel.Current.SelectedStateViewModel.SelectedFrameViewModel.Picture.Remove(Picture);
                        DynamicPicturesList.Remove(DynamicPicture);
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
            if (this.Id == "Базовый рисунок")
            {
                MessageBox.Show("Невозможно удалить Базовый рисунок");
                return;
            }
            this.Parent.StateViewModels.Remove(this);
            StateViewModel stateViewModel = new StateViewModel();
            stateViewModel.Id = this.Id;
            DeviceViewModel.Current.StatesAvailableListViewModel.Add(stateViewModel);
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

        public Canvas Picture;

        Canvas dynamicPicture;
        public Canvas DynamicPicture
        {
            get { return dynamicPicture; }
            set
            {
                dynamicPicture = value;
                OnPropertyChanged("DynamicPicture");
            }
        }

        ObservableCollection<Canvas> dynamicPicturesList;
        public ObservableCollection<Canvas> DynamicPicturesList
        {
            get { return dynamicPicturesList; }
            set
            {
                dynamicPicturesList = value;
                OnPropertyChanged("DynamicPicturesList");
            }
        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {

            ViewModel.Current.SelectedStateViewModel.SelectedFrameViewModel.Picture.Remove(Picture);
            DynamicPicturesList.Remove(DynamicPicture);

            if (!ViewModel.Current.SelectedStateViewModel.IsAdditional)
            {
                string frameImage = Svg2Xaml.XSLT_Transform(FrameViewModels[tick].Image, RubezhDevices.RubezhDevice.svg2xaml_xsl);
                StringReader stringReader = new StringReader(frameImage);
                XmlReader xmlReader = XmlReader.Create(stringReader);
                DynamicPicture = (Canvas)XamlReader.Load(xmlReader);
                Canvas.SetZIndex(DynamicPicture, 10);
                DynamicPicturesList.Add(DynamicPicture);

                string _frameImage = Svg2Xaml.XSLT_Transform(FrameViewModels[tick].Image, RubezhDevices.RubezhDevice.svg2xaml_xsl);
                StringReader _stringReader = new StringReader(_frameImage);
                XmlReader _xmlReader = XmlReader.Create(_stringReader);
                Picture = (Canvas)XamlReader.Load(_xmlReader);
                Canvas.SetZIndex(Picture, 10);
                ViewModel.Current.SelectedStateViewModel.SelectedFrameViewModel.Picture.Add(Picture);

                if (FrameViewModels.Count < 2)
                {
                    dispatcherTimer.Stop();
                }
                dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, FrameViewModels[tick].Duration);
            }
            tick = (tick + 1) % FrameViewModels.Count;
        }

    }
}

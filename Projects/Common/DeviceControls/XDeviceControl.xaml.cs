using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Common;
using XFiresecAPI;

namespace DeviceControls
{
    public partial class XDeviceControl : INotifyPropertyChanged
    {
        public XDeviceControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        void XDeviceControl_Loaded(object sender, RoutedEventArgs e)
        {
            //_canvas.LayoutTransform = new ScaleTransform(ActualWidth / 500, ActualHeight / 500);
        }

        public Guid XDriverId { get; set; }
        public bool IsManualUpdate { get; set; }

        XStateClass _xstateClass;
        public XStateClass XStateClass
        {
            get { return _xstateClass; }
            set
            {
                _xstateClass = value;
            }
        }
        
        List<XStateViewModel> _xstateViewModelList;
        
        public void XUpdate()
        {
            var libraryXDevice = XManager.XDeviceLibraryConfiguration.XDevices.FirstOrDefault(x => x.XDriverId == XDriverId);
            if (libraryXDevice == null)
            {
                Logger.Error("DeviceControl.XUpdate libraryXDevice = null " + XDriverId.ToString());
                return;
            }

            var resultLibraryXStates = new List<LibraryXState>();

            if (_xstateViewModelList.IsNotNullOrEmpty())
                _xstateViewModelList.ForEach(x => x.Dispose());
            _xstateViewModelList = new List<XStateViewModel>();

            var libraryXState = libraryXDevice.XStates.FirstOrDefault(x => x.Code == null && x.XStateClass == XStateClass);
            if (libraryXState == null)
            {
                libraryXState = libraryXDevice.XStates.FirstOrDefault(x => x.Code == null && x.XStateClass == XStateClass.No);
                if (libraryXState == null)
                {
                    Logger.Error("DeviceControl.XUpdate libraryXState = null " + XDriverId.ToString());
                    return;
                }
            }

            if (libraryXState != null)
            {
                resultLibraryXStates.Add(libraryXState);
            }

            var sortedResultLibraryXStates = from LibraryXState xstate in resultLibraryXStates
                                             orderby xstate.Layer
                                             select xstate;
            var canvases = new List<Canvas>();
            foreach (var libraryXStates in sortedResultLibraryXStates)
            {
                _xstateViewModelList.Add(new XStateViewModel(libraryXStates, canvases));
            }

            _canvas.Children.Clear();
            foreach (var canvas in canvases)
                _canvas.Children.Add(new Viewbox() { Child = canvas });
        }

        public static FrameworkElement GetDefaultPicture(Guid DriverId)
        {
            UIElement content = null;

            var device = XManager.XDeviceLibraryConfiguration.XDevices.FirstOrDefault(x => x.XDriverId == DriverId);
            if (device != null)
            {
                var state = device.XStates.FirstOrDefault(x => x.Code == null && x.XStateClass == XStateClass.No);
                Canvas canvas = Helper.Xml2Canvas(state.XFrames[0].Image);
                canvas.Background = Brushes.Transparent;
                content = canvas;
            }
            else
                content = new TextBlock()
                {
                    Text = "?",
                    Background = Brushes.Transparent
                };
            return new Border()
            {
                BorderThickness = new Thickness(0),
                Background = Brushes.Transparent,
                Child = new Viewbox()
                {
                    Child = content
                }
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
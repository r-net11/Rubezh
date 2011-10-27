using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using PlansModule.Events;
using PlansModule.Views;
using System.Diagnostics;

namespace PlansModule.ViewModels
{
    public class PlanDetailsViewModel : RegionViewModel
    {
        Plan _plan;
        Canvas _canvas;

        public List<ElementSubPlanViewModel> SubPlans { get; set; }
        public List<ElementZoneViewModel> Zones { get; set; }
        public List<ElementDeviceViewModel> Devices { get; set; }

        public PlanDetailsViewModel(Canvas canvas)
        {
            _canvas = canvas;
            _canvas.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(_canvas_PreviewMouseLeftButtonDown);
            ServiceFactory.Events.GetEvent<PlanStateChangedEvent>().Subscribe(OnPlanStateChanged);
        }

        public void Initialize(Plan plan)
        {
            _plan = plan;
            DrawPlan();
            UpdateSubPlans();
            ResetView();
        }

        public void DrawPlan()
        {
            SubPlans = new List<ElementSubPlanViewModel>();
            Zones = new List<ElementZoneViewModel>();
            Devices = new List<ElementDeviceViewModel>();

            _canvas.Children.Clear();

            _canvas.Width = _plan.Width;
            _canvas.Height = _plan.Height;
            _canvas.Background = CreateBrush();

            foreach (var elementSubPlan in _plan.ElementSubPlans)
            {
                var subPlanViewModel = new ElementSubPlanViewModel();
                subPlanViewModel.Initialize(elementSubPlan, _canvas);
                SubPlans.Add(subPlanViewModel);
            }

            foreach (var rectangleBox in _plan.ElementRectangles)
            {
                var rectangle = new Rectangle()
                {
                    Width = rectangleBox.Width,
                    Height = rectangleBox.Height
                };
                Canvas.SetLeft(rectangle, rectangleBox.Left);
                Canvas.SetTop(rectangle, rectangleBox.Top);

                BitmapImage image = null;
                using (MemoryStream imageStream = new MemoryStream(rectangleBox.BackgroundPixels))
                {
                    image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = imageStream;
                    image.EndInit();
                }

                var imageBrushRect = new ImageBrush()
                {
                    ImageSource = image
                };
                rectangle.Fill = imageBrushRect;

                _canvas.Children.Add(rectangle);
            }

            foreach (var elementPolygonZone in _plan.ElementPolygonZones)
            {
                if (elementPolygonZone.ZoneNo != null)
                {
                    var zonePlanViewModel = new ElementZoneViewModel();
                    zonePlanViewModel.Initialize(elementPolygonZone, _canvas);
                    zonePlanViewModel.Selected += () => { SelectedZone = zonePlanViewModel; };
                    Zones.Add(zonePlanViewModel);
                }
            }

            foreach (var elementDevice in _plan.ElementDevices)
            {
                var planDeviceViewModel = new ElementDeviceViewModel();
                planDeviceViewModel.Initialize(elementDevice, _canvas);
                planDeviceViewModel.Selected += () => { SelectedDevice = planDeviceViewModel; };
                Devices.Add(planDeviceViewModel);
            }

            //if (_plan.Caption == "Строение 2 - Этаж 2")
            //{
            //    AddVideo();
            //}
            //if (_plan.Caption == "Строение 1 - Этаж 4")
            //{
            //    AddPhone();
            //}
            //if (_plan.Caption == "Строение 1 - Этаж 5")
            //{
            //    AddDoor();
            //}

            SelectedDevice = null;
            SelectedZone = null;
        }

        #region Fake Objects

        void AddVideo()
        {
            var cameraCanvas = new Canvas();
            var rectangle = new Rectangle();
            rectangle.Width = 60;
            rectangle.Height = 30;
            rectangle.Fill = Brushes.DarkGray;
            rectangle.Stroke = Brushes.Black;
            rectangle.StrokeThickness = 3;
            rectangle.ToolTip = "Камера 1";
            rectangle.MouseDown += new System.Windows.Input.MouseButtonEventHandler(camera_MouseDown);

            var polygon = new Polygon();
            polygon.Fill = Brushes.DarkGray;
            polygon.Stroke = Brushes.Black;
            polygon.StrokeThickness = 3;
            polygon.Points = new PointCollection();
            polygon.Points.Add(new System.Windows.Point(60, 15));
            polygon.Points.Add(new System.Windows.Point(75, 0));
            polygon.Points.Add(new System.Windows.Point(75, 30));

            cameraCanvas.Children.Add(rectangle);
            cameraCanvas.Children.Add(polygon);
            var scaleTransform = new ScaleTransform();
            scaleTransform.ScaleX = 0.5;
            scaleTransform.ScaleY = 0.5;
            cameraCanvas.RenderTransform = scaleTransform;

            Canvas.SetLeft(cameraCanvas, 170);
            Canvas.SetTop(cameraCanvas, 223);
            _canvas.Children.Add(cameraCanvas);
        }

        void camera_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var videoViewModel = new VideoViewModel();
            ServiceFactory.UserDialogs.ShowModalWindow(videoViewModel);
        }

        void AddPhone()
        {
            var image = new Image();
            image.Width = 30;
            image.Height = 30;
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri("D:/phone.png");
            bitmapImage.EndInit();
            image.Source = bitmapImage;
            image.MouseDown += new System.Windows.Input.MouseButtonEventHandler(phone_MouseDown);

            Canvas.SetLeft(image, 128);
            Canvas.SetTop(image, 75);
            _canvas.Children.Add(image);
            image.ToolTip = "Телефон 123";
        }

        void phone_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var phoneViewModel = new PhoneViewModel();
            ServiceFactory.UserDialogs.ShowModalWindow(phoneViewModel);
        }

        void AddDoor()
        {
            var image = new Image();
            image.Width = 50;
            image.Height = 52;
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri("D:/door.png");
            bitmapImage.EndInit();
            image.Source = bitmapImage;
            image.ToolTip = "Дверь 1";
            image.MouseDown += new System.Windows.Input.MouseButtonEventHandler(door_MouseDown);

            Canvas.SetLeft(image, 54);
            Canvas.SetTop(image, 424);
            _canvas.Children.Add(image);
        }

        void door_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var doorViewModel = new DoorViewModel();
            ServiceFactory.UserDialogs.ShowModalWindow(doorViewModel);
        }

        #endregion Fake Objects

        ElementDeviceViewModel _selectedDevice;
        public ElementDeviceViewModel SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                _selectedDevice = value;

                Devices.ForEach(x => x.IsSelected = false);
                if (value != null)
                {
                    value.IsSelected = true;
                }

                OnPropertyChanged("SelectedDevice");
            }
        }

        ElementZoneViewModel _selectedZone;
        public ElementZoneViewModel SelectedZone
        {
            get { return _selectedZone; }
            set
            {
                _selectedZone = value;

                Zones.ForEach(x => x.IsSelected = false);
                if (value != null)
                {
                    value.IsSelected = true;
                }

                OnPropertyChanged("SelectedZone");
            }
        }

        void _canvas_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SelectedDevice = null;
            SelectedZone = null;
        }

        public void SelectDevice(Guid deviceUID)
        {
            SelectedDevice = Devices.FirstOrDefault(x => x.DeviceUID == deviceUID);
        }

        public void SelectZone(ulong? zoneNo)
        {
            SelectedZone = Zones.FirstOrDefault(x => x.ZoneNo == zoneNo);
        }

        void OnPlanStateChanged(string planName)
        {
            if ((_plan != null) && (_plan.Name == planName))
            {
                UpdateSubPlans();
            }
        }

        void UpdateSubPlans()
        {
            foreach (var subPlan in SubPlans)
            {
                var planViewModel = PlansViewModel.Current.Plans.FirstOrDefault(x => x._plan.Name == subPlan.Name);
                if (planViewModel != null)
                {
                    subPlan.StateType = planViewModel.StateType;
                }
            }
        }

        void ResetView()
        {
            if (CanvasView.Current != null)
            {
                CanvasView.Current.Reset();
            }
        }

        ImageBrush CreateBrush()
        {
            var imageBrush = new ImageBrush();
            if (_plan.BackgroundPixels != null)
            {
                BitmapImage image;
                using (MemoryStream imageStream = new MemoryStream(_plan.BackgroundPixels))
                {
                    image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = imageStream;
                    image.EndInit();
                }

                imageBrush.ImageSource = image;
            }
            return imageBrush;
        }
    }
}
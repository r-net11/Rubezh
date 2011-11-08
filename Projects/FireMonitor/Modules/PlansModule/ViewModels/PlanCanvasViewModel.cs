using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using PlansModule.Events;
using PlansModule.Views;

namespace PlansModule.ViewModels
{
    public class PlanCanvasViewModel : RegionViewModel
    {
        Plan _plan;
        Canvas _canvas;

        public List<ElementSubPlanViewModel> SubPlans { get; set; }
        public List<ElementZoneViewModel> Zones { get; set; }
        public List<ElementDeviceViewModel> Devices { get; set; }

        public PlanCanvasViewModel(Canvas canvas)
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
            _canvas.Background = new SolidColorBrush(_plan.BackgroundColor);
            if (_plan.BackgroundPixels != null)
            {
                _canvas.Background = PlanElementsHelper.CreateBrush(_plan.BackgroundPixels);
            }

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

                if (rectangleBox.BackgroundPixels != null)
                {
                    rectangle.Fill = PlanElementsHelper.CreateBrush(rectangleBox.BackgroundPixels);
                }

                _canvas.Children.Add(rectangle);
            }

            foreach (var elementRectangleZone in _plan.ElementRectangleZones)
            {
                if (elementRectangleZone.ZoneNo != null)
                {
                    var elementPolygonZone = new ElementPolygonZone()
                    {
                        Left = elementRectangleZone.Left,
                        Top = elementRectangleZone.Top,
                        Width = elementRectangleZone.Width,
                        Height = elementRectangleZone.Height,
                        ZoneNo = elementRectangleZone.ZoneNo
                    };

                    elementPolygonZone.PolygonPoints = new PointCollection();
                    elementPolygonZone.PolygonPoints.Add(new Point(0, 0));
                    elementPolygonZone.PolygonPoints.Add(new Point(elementRectangleZone.Width, 0));
                    elementPolygonZone.PolygonPoints.Add(new Point(elementRectangleZone.Width, elementRectangleZone.Height));
                    elementPolygonZone.PolygonPoints.Add(new Point(0, elementRectangleZone.Height));

                    var elementZoneViewModel = new ElementZoneViewModel();
                    elementZoneViewModel.Initialize(elementPolygonZone, _canvas);
                    elementZoneViewModel.Selected += () => { SelectedZone = elementZoneViewModel; };
                    Zones.Add(elementZoneViewModel);
                }
            }

            foreach (var elementPolygonZone in _plan.ElementPolygonZones)
            {
                if (elementPolygonZone.ZoneNo != null)
                {
                    var elementZoneViewModel = new ElementZoneViewModel();
                    elementZoneViewModel.Initialize(elementPolygonZone, _canvas);
                    elementZoneViewModel.Selected += () => { SelectedZone = elementZoneViewModel; };
                    Zones.Add(elementZoneViewModel);
                }
            }

            foreach (var elementDevice in _plan.ElementDevices)
            {
                var elementDeviceViewModel = new ElementDeviceViewModel();
                elementDeviceViewModel.Initialize(elementDevice, _canvas);
                elementDeviceViewModel.Selected += () => { SelectedDevice = elementDeviceViewModel; };
                Devices.Add(elementDeviceViewModel);
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

        void OnPlanStateChanged(Guid planUID)
        {
            if ((_plan != null) && (_plan.UID == planUID))
            {
                UpdateSubPlans();
            }
        }

        void UpdateSubPlans()
        {
            foreach (var subPlan in SubPlans)
            {
                var planViewModel = PlansViewModel.Current.Plans.FirstOrDefault(x => x._plan.UID == subPlan.PlanUID);
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
    }
}
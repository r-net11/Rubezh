using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;
using System.Windows.Controls;
using PlansModule.Views;
using PlansModule.Models;
using System.Windows.Media;
using System.Reflection;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using PlansModule.Events;
using Infrastructure.Common;
using System.Windows.Shapes;
using System.Windows;

namespace PlansModule.ViewModels
{
    public class PlanDetailsViewModel :RegionViewModel
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
            this._plan = plan;
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

            string backgroundPath = PathHelper.Data + _plan.BackgroundSource;
            _canvas.Background = CreateBrush(backgroundPath);

            foreach (var elementSubPlan in _plan.ElementSubPlans)
            {
                ElementSubPlanViewModel subPlanViewModel = new ElementSubPlanViewModel();
                subPlanViewModel.Initialize(elementSubPlan, _canvas);
                SubPlans.Add(subPlanViewModel);
            }

            foreach (var elementZone in _plan.ElementZones)
            {
                ElementZoneViewModel zonePlanViewModel = new ElementZoneViewModel();
                zonePlanViewModel.Initialize(elementZone, _canvas);
                zonePlanViewModel.Selected += () => { SelectedZone = zonePlanViewModel; };
                Zones.Add(zonePlanViewModel);
            }

            foreach (var elementDevice in _plan.ElementDevices)
            {
                ElementDeviceViewModel planDeviceViewModel = new ElementDeviceViewModel();
                planDeviceViewModel.Initialize(elementDevice, _canvas);
                planDeviceViewModel.Selected += () => { SelectedDevice = planDeviceViewModel; };
                Devices.Add(planDeviceViewModel);
            }

            if (_plan.Caption == "Строение 2 - Этаж 2")
            {
                AddVideo();
            }
            if (_plan.Caption == "Строение 1 - Этаж 4")
            {
                AddPhone();
            }
            if (_plan.Caption == "Строение 1 - Этаж 5")
            {
                AddDoor();
            }

            SelectedDevice = null;
            SelectedZone = null;
        }

        void AddVideo()
        {
            Canvas cameraCanvas = new Canvas();
            Rectangle rectangle = new Rectangle();
            rectangle.Width = 60;
            rectangle.Height = 30;
            rectangle.Fill = Brushes.DarkGray;
            rectangle.Stroke = Brushes.Black;
            rectangle.StrokeThickness = 3;
            rectangle.ToolTip = "Камера 1";
            rectangle.MouseDown += new System.Windows.Input.MouseButtonEventHandler(camera_MouseDown);

            Polygon polygon = new Polygon();
            polygon.Fill = Brushes.DarkGray;
            polygon.Stroke = Brushes.Black;
            polygon.StrokeThickness = 3;
            polygon.Points = new PointCollection();
            polygon.Points.Add(new System.Windows.Point(60, 15));
            polygon.Points.Add(new System.Windows.Point(75, 0));
            polygon.Points.Add(new System.Windows.Point(75, 30));

            cameraCanvas.Children.Add(rectangle);
            cameraCanvas.Children.Add(polygon);
            ScaleTransform scaleTransform = new ScaleTransform();
            scaleTransform.ScaleX = 0.5;
            scaleTransform.ScaleY = 0.5;
            cameraCanvas.RenderTransform = scaleTransform;

            Canvas.SetLeft(cameraCanvas, 170);
            Canvas.SetTop(cameraCanvas, 223);
            _canvas.Children.Add(cameraCanvas);
            
        }

        void camera_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            VideoViewModel videoViewModel = new VideoViewModel();
            ServiceFactory.UserDialogs.ShowModalWindow(videoViewModel);
        }

        void AddPhone()
        {
            Image image = new Image();
            image.Width = 30;
            image.Height = 30;
            BitmapImage bitmapImage = new BitmapImage();
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
            PhoneViewModel phoneViewModel = new PhoneViewModel();
            ServiceFactory.UserDialogs.ShowModalWindow(phoneViewModel);
        }

        void AddDoor()
        {
            Image image = new Image();
            image.Width = 50;
            image.Height = 52;
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri("D:/door.png");
            bitmapImage.EndInit();
            image.Source = bitmapImage;
            image.ToolTip = "Дверь 1";
            image.MouseDown +=new System.Windows.Input.MouseButtonEventHandler(door_MouseDown);

            Canvas.SetLeft(image, 54);
            Canvas.SetTop(image, 424);
            _canvas.Children.Add(image);
        }

        void door_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DoorViewModel doorViewModel = new DoorViewModel();
            ServiceFactory.UserDialogs.ShowModalWindow(doorViewModel);
        }

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

        public void SelectDevice(string id)
        {
            SelectedDevice = Devices.FirstOrDefault(x => x.DeviceId == id);
        }

        public void SelectZone(string zoneNo)
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
                    subPlan.State = planViewModel.State;
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

        ImageBrush CreateBrush(string source)
        {
            ImageBrush imageBrush = new ImageBrush();
            Uri uri = new Uri(source);
            imageBrush.ImageSource = new BitmapImage(uri);
            return imageBrush;
        }
    }
}

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
        }

        public void Initialize(Plan plan)
        {
            this._plan = plan;
            DrawPlan();
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

            _canvas.Background = CreateBrush(_plan.BackgroundSource);

            foreach (ElementSubPlan elementSubPlan in _plan.ElementSubPlans)
            {
                ElementSubPlanViewModel subPlanViewModel = new ElementSubPlanViewModel();
                subPlanViewModel.Initialize(elementSubPlan, _canvas);
                SubPlans.Add(subPlanViewModel);
            }

            foreach (ElementZone elementZone in _plan.ElementZones)
            {
                ElementZoneViewModel zonePlanViewModel = new ElementZoneViewModel();
                zonePlanViewModel.Initialize(elementZone, _canvas);
                zonePlanViewModel.Selected += () => { SelectedZone = zonePlanViewModel; };
                Zones.Add(zonePlanViewModel);
            }

            foreach (ElementDevice elementDevice in _plan.ElementDevices)
            {
                ElementDeviceViewModel planDeviceViewModel = new ElementDeviceViewModel();
                planDeviceViewModel.Initialize(elementDevice, _canvas);
                planDeviceViewModel.Selected += () => { SelectedDevice = planDeviceViewModel; };
                Devices.Add(planDeviceViewModel);
            }

            SelectedDevice = null;
            SelectedZone = null;
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
                    HasSelectedDevice = true;
                    value.IsSelected = true;
                }
                else
                {
                    HasSelectedDevice = false;
                }

                OnPropertyChanged("SelectedDevice");
            }
        }

        bool _hasSelectedDevice;
        public bool HasSelectedDevice
        {
            get { return _hasSelectedDevice; }
            set
            {
                _hasSelectedDevice = value;
                OnPropertyChanged("HasSelectedDevice");
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
                    HasSelectedZone = true;
                    value.IsSelected = true;
                }
                else
                {
                    HasSelectedZone = false;
                }

                OnPropertyChanged("SelectedZone");
            }
        }

        bool _hasSelectedZone;
        public bool HasSelectedZone
        {
            get { return _hasSelectedZone; }
            set
            {
                _hasSelectedZone = value;
                OnPropertyChanged("HasSelectedZone");
            }
        }

        void _canvas_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SelectedDevice = null;
            SelectedZone = null;
        }

        public void SelectDevice(string path)
        {
            SelectedDevice = Devices.FirstOrDefault(x => x.elementDevice.Path == path);
        }

        public void SelectZone(string zoneNo)
        {
            SelectedZone = Zones.FirstOrDefault(x => x.elementZone.ZoneNo == zoneNo);
        }

        public override void Dispose()
        {
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
            string executablePath = Assembly.GetExecutingAssembly().Location;
            string relativePath = "../../../Data/" + source;
            Uri uri = new Uri(System.IO.Path.Combine(executablePath, relativePath));
            imageBrush.ImageSource = new BitmapImage(uri);
            return imageBrush;
        }
    }
}

using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PlansModule.ViewModels;

namespace PlansModule.Views
{
    public partial class PlanDesignerView : UserControl
    {
        public static PlanDesignerView Current { get; set; }
        Point? lastCenterPositionOnTarget;
        Point? lastMousePositionOnTarget;

        public static void Update()
        {
            if (Current != null)
                Current.slider.Value = 1;
        }

        public PlanDesignerView()
        {
            Current = this;
            InitializeComponent();

            scrollViewer.ScrollChanged += OnScrollViewerScrollChanged;
            scrollViewer.PreviewMouseWheel += OnPreviewMouseWheel;
            slider.ValueChanged += OnSliderValueChanged;

            this.Loaded += new RoutedEventHandler(CanvasView_Loaded);
        }

        void CanvasView_Loaded(object sender, RoutedEventArgs e)
        {
        }

        void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            lastMousePositionOnTarget = Mouse.GetPosition(grid);

            if (e.Delta > 0)
            {
                slider.Value += 1;
            }
            else if (e.Delta < 0)
            {
                slider.Value -= 1;
            }

            e.Handled = true;
        }

        private void OnZoomIn(object sender, RoutedEventArgs e)
        {
            slider.Value += 1;
        }

        private void OnZoomOut(object sender, RoutedEventArgs e)
        {
            slider.Value -= 1;
        }

        void OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            (DataContext as PlansViewModel).PlanDesignerViewModel.ChangeZoom(e.NewValue);
            Trace.WriteLine("Zooming: " + e.NewValue);

            var centerOfViewport = new Point(scrollViewer.ViewportWidth / 2, scrollViewer.ViewportHeight / 2);
            lastCenterPositionOnTarget = scrollViewer.TranslatePoint(centerOfViewport, _contentControl);
        }

        void OnScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.ExtentHeightChange == 0 && e.ExtentWidthChange == 0)
                return;

            Point? targetBefore = null;
            Point? targetNow = null;

            if (!lastMousePositionOnTarget.HasValue)
            {
                if (lastCenterPositionOnTarget.HasValue)
                {
                    var centerOfViewport = new Point(scrollViewer.ViewportWidth / 2, scrollViewer.ViewportHeight / 2);
                    Point centerOfTargetNow = scrollViewer.TranslatePoint(centerOfViewport, grid);

                    targetBefore = lastCenterPositionOnTarget;
                    targetNow = centerOfTargetNow;
                }
            }
            else
            {
                targetBefore = lastMousePositionOnTarget;
                targetNow = Mouse.GetPosition(grid);

                lastMousePositionOnTarget = null;
            }

            if (targetBefore.HasValue)
            {
                double dXInTargetPixels = targetNow.Value.X - targetBefore.Value.X;
                double dYInTargetPixels = targetNow.Value.Y - targetBefore.Value.Y;

                double multiplicatorX = e.ExtentWidth / grid.ActualWidth;
                double multiplicatorY = e.ExtentHeight / grid.ActualHeight;

                double newOffsetX = scrollViewer.HorizontalOffset - dXInTargetPixels * multiplicatorX;
                double newOffsetY = scrollViewer.VerticalOffset - dYInTargetPixels * multiplicatorY;

                if (double.IsNaN(newOffsetX) || double.IsNaN(newOffsetY))
                    return;

                scrollViewer.ScrollToHorizontalOffset(newOffsetX);
                scrollViewer.ScrollToVerticalOffset(newOffsetY);
            }
        }
    }
}

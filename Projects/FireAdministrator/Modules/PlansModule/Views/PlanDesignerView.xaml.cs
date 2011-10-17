using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PlansModule.Views
{
    public partial class PlanDesignerView : UserControl
    {
        public static PlanDesignerView Current { get; set; }

        Point? lastCenterPositionOnTarget;
        Point? lastMousePositionOnTarget;

        public void Reset()
        {
            FullSize();
            slider.Value = 1;
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
            Reset();
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

        void OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (e.NewValue == 0)
                return;

            scaleTransform.ScaleX = e.NewValue * initialScale;
            scaleTransform.ScaleY = e.NewValue * initialScale;

            var centerOfViewport = new Point(scrollViewer.ViewportWidth / 2, scrollViewer.ViewportHeight / 2);
            lastCenterPositionOnTarget = scrollViewer.TranslatePoint(centerOfViewport, grid);
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

        double initialScale = 1;

        void FullSize()
        {
            var canvas = _contentControl.Content as Canvas;
            if (canvas == null)
                return;

            var contentWidth = canvas.Width;
            var contentHeight = canvas.Height;

            //var contentWidth = scrollViewer.ActualWidth;
            //var contentHeight = scrollViewer.ActualHeight;

            double scaleX = (scrollViewer.ActualWidth - 30) / contentWidth;
            double scaleY = (scrollViewer.ActualHeight - 30) / contentHeight;
            double scale = Math.Min(scaleX, scaleY);
            initialScale = scale;

            scaleTransform.ScaleX = scale;
            scaleTransform.ScaleY = scale;
        }
    }
}

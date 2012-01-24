using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PlansModule.ViewModels;

namespace PlansModule.Views
{
    public partial class PlanDesignerView : UserControl
    {
        public static PlanDesignerView Current { get; set; }
        Point? lastMousePositionOnTarget;
        Point? lastCenterPositionOnTarget;
        double initialScale = 1;

        public static void Update()
        {
            if (Current != null)
            {
                Current.slider.Value = 1;
                Current._scrollViewer.ScrollToHorizontalOffset(0);
                Current._scrollViewer.ScrollToVerticalOffset(0);
                Current.slider.Value = 1;
            }
        }

        public PlanDesignerView()
        {
            Current = this;
            InitializeComponent();

            _scrollViewer.PreviewMouseWheel += OnPreviewMouseWheel;
            slider.ValueChanged += OnSliderValueChanged;

            _scrollViewer.PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
            _scrollViewer.MouseMove += OnMouseMove;
            _scrollViewer.PreviewMouseLeftButtonUp += OnMouseLeftButtonUp;
            _scrollViewer.ScrollChanged += OnScrollViewerScrollChanged;

            deviceSlider.ValueChanged +=new RoutedPropertyChangedEventHandler<double>(deviceSlider_ValueChanged);

            //FullSize();
        }

        void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            lastMousePositionOnTarget = Mouse.GetPosition(_grid);

            if (e.Delta > 0)
            {
                slider.Value += 0.2;
            }
            else if (e.Delta < 0)
            {
                slider.Value -= 0.2;
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
            double newZoom = e.NewValue;
            double oldZoom = e.OldValue;

            if (newZoom < 1)
            {
                newZoom = 1 / (-newZoom + 2);
            }

            if (oldZoom < 1)
            {
                oldZoom = 1 / (-oldZoom + 2);
            }

            double deltaZoom = newZoom / oldZoom;

            (DataContext as PlansViewModel).PlanDesignerViewModel.ChangeZoom(newZoom);

            scaleTransform.ScaleX = newZoom;
            scaleTransform.ScaleY = newZoom;

            var centerOfViewport = new Point(_scrollViewer.ViewportWidth / 2, _scrollViewer.ViewportHeight / 2);
            lastCenterPositionOnTarget = _scrollViewer.TranslatePoint(centerOfViewport, _grid);
        }

        private void OnSizeToFit(object sender, RoutedEventArgs e)
        {
            var canvas = _contentControl.Content as Canvas;
            if (canvas == null)
                return;

            double scaleX = (_scrollViewer.ActualWidth - 30) / canvas.Width;
            double scaleY = (_scrollViewer.ActualHeight - 30) / canvas.Height;
            double scale = Math.Min(scaleX, scaleY);
            initialScale = scale;

            if (scale >= 1)
            {
                slider.Value = scale;
            }
            if (scale < 1)
            {
                slider.Value = 2 - 1 / scale;
            }
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
                    var centerOfViewport = new Point(_scrollViewer.ViewportWidth / 2, _scrollViewer.ViewportHeight / 2);
                    Point centerOfTargetNow = _scrollViewer.TranslatePoint(centerOfViewport, _grid);

                    targetBefore = lastCenterPositionOnTarget;
                    targetNow = centerOfTargetNow;
                }
            }
            else
            {
                targetBefore = lastMousePositionOnTarget;
                targetNow = Mouse.GetPosition(_grid);

                lastMousePositionOnTarget = null;
            }

            if (targetBefore.HasValue)
            {
                double dXInTargetPixels = targetNow.Value.X - targetBefore.Value.X;
                double dYInTargetPixels = targetNow.Value.Y - targetBefore.Value.Y;

                double multiplicatorX = e.ExtentWidth / _grid.ActualWidth;
                double multiplicatorY = e.ExtentHeight / _grid.ActualHeight;

                double newOffsetX = _scrollViewer.HorizontalOffset - dXInTargetPixels * multiplicatorX;
                double newOffsetY = _scrollViewer.VerticalOffset - dYInTargetPixels * multiplicatorY;

                if (double.IsNaN(newOffsetX) || double.IsNaN(newOffsetY))
                    return;

                _scrollViewer.ScrollToHorizontalOffset(newOffsetX);
                _scrollViewer.ScrollToVerticalOffset(newOffsetY);
            }
        }

        public void FullSize()
        {
            var canvas = _contentControl.Content as Canvas;
            if (canvas == null)
                return;

            double scaleX = (_scrollViewer.ActualWidth - 30) / canvas.Width;
            double scaleY = (_scrollViewer.ActualHeight - 30) / canvas.Height;
            double scale = Math.Min(scaleX, scaleY);
            initialScale = scale;

            scaleTransform.ScaleX = scale;
            scaleTransform.ScaleY = scale;
        }

        #region Hand Moving
        Point? lastDragPoint;

        void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (lastDragPoint.HasValue)
            {
                Point posNow = e.GetPosition(_scrollViewer);

                double dX = posNow.X - lastDragPoint.Value.X;
                double dY = posNow.Y - lastDragPoint.Value.Y;

                lastDragPoint = posNow;

                _scrollViewer.ScrollToHorizontalOffset(_scrollViewer.HorizontalOffset - dX);
                _scrollViewer.ScrollToVerticalOffset(_scrollViewer.VerticalOffset - dY);
            }
        }

        void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                var mousePos = e.GetPosition(_scrollViewer);
                if (mousePos.X <= _scrollViewer.ViewportWidth && mousePos.Y < _scrollViewer.ViewportHeight)
                {
                    _scrollViewer.Cursor = Cursors.Hand;
                    lastDragPoint = mousePos;
                    Mouse.Capture(_scrollViewer);
                }
                e.Handled = true;
            }
        }

        void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _scrollViewer.Cursor = Cursors.Arrow;
            _scrollViewer.ReleaseMouseCapture();
            lastDragPoint = null;
        }
        #endregion

        private void OnDeviceZoomOut(object sender, RoutedEventArgs e)
        {
            deviceSlider.Value--;
        }

        private void OnDeviceZoomIn(object sender, RoutedEventArgs e)
        {
            deviceSlider.Value++;
        }

        private void deviceSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            (DataContext as PlansViewModel).PlanDesignerViewModel.ChangeDeviceZoom(e.NewValue);
        }
    }
}

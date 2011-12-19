using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PlansModule.ViewModels;
using System.Collections.Generic;

namespace PlansModule.Views
{
    public partial class CanvasView : UserControl
    {
        Point? lastCenterPositionOnTarget;
        Point? lastMousePositionOnTarget;
        Point? lastDragPoint;

        public static CanvasView Current { get; set; }

        public void Reset()
        {
            FullSize();
            slider.Value = 1;
        }

        public CanvasView()
        {
            Current = this;
            InitializeComponent();

            _scrollViewer.ScrollChanged += OnScrollViewerScrollChanged;
            _scrollViewer.MouseLeftButtonUp += OnMouseLeftButtonUp;
            _scrollViewer.PreviewMouseLeftButtonUp += OnMouseLeftButtonUp;
            _scrollViewer.PreviewMouseWheel += OnPreviewMouseWheel;

            _scrollViewer.PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
            _scrollViewer.MouseMove += OnMouseMove;

            slider.ValueChanged += OnSliderValueChanged;

            this.Loaded += new RoutedEventHandler(CanvasView_Loaded);
        }

        void CanvasView_Loaded(object sender, RoutedEventArgs e)
        {
            Reset();
        }

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
            var mousePos = e.GetPosition(_scrollViewer);
            if (mousePos.X <= _scrollViewer.ViewportWidth && mousePos.Y < _scrollViewer.ViewportHeight)
            {
                _scrollViewer.Cursor = Cursors.SizeAll;
                lastDragPoint = mousePos;
                Mouse.Capture(_scrollViewer);
            }
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

        void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _scrollViewer.Cursor = Cursors.Arrow;
            _scrollViewer.ReleaseMouseCapture();
            lastDragPoint = null;
        }

        void OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (e.NewValue == 0)
                return;

            scaleTransform.ScaleX = e.NewValue * initialScale;
            scaleTransform.ScaleY = e.NewValue * initialScale;

            var centerOfViewport = new Point(_scrollViewer.ViewportWidth / 2, _scrollViewer.ViewportHeight / 2);
            lastCenterPositionOnTarget = _scrollViewer.TranslatePoint(centerOfViewport, grid);
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
                    Point centerOfTargetNow = _scrollViewer.TranslatePoint(centerOfViewport, grid);

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

                double newOffsetX = _scrollViewer.HorizontalOffset - dXInTargetPixels * multiplicatorX;
                double newOffsetY = _scrollViewer.VerticalOffset - dYInTargetPixels * multiplicatorY;

                if (double.IsNaN(newOffsetX) || double.IsNaN(newOffsetY))
                    return;

                _scrollViewer.ScrollToHorizontalOffset(newOffsetX);
                _scrollViewer.ScrollToVerticalOffset(newOffsetY);
            }
        }

        double initialScale = 1;

        void FullSize()
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
    }
}
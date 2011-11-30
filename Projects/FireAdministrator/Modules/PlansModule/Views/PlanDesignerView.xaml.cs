using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PlansModule.ViewModels;
using System;

namespace PlansModule.Views
{
    public partial class PlanDesignerView : UserControl
    {
        public static PlanDesignerView Current { get; set; }

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
        }

        void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
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

            var position = Mouse.GetPosition(_grid);

            double newOffsetX = _scrollViewer.HorizontalOffset + (position.X - 25) * (deltaZoom - 1) / deltaZoom;
            double newOffsetY = _scrollViewer.VerticalOffset + (position.Y - 25) * (deltaZoom - 1) / deltaZoom;

            _scrollViewer.ScrollToHorizontalOffset(newOffsetX);
            _scrollViewer.ScrollToVerticalOffset(newOffsetY);
        }

        private void OnSizeToFit(object sender, RoutedEventArgs e)
        {
            double scaleX = (_scrollViewer.ActualWidth - 30) / _contentControl.ActualWidth;
            double scaleY = (_scrollViewer.ActualHeight - 30) / _contentControl.ActualHeight;
            double scale = Math.Max(scaleX, scaleY);

            //(DataContext as PlansViewModel).PlanDesignerViewModel.ChangeZoom(scale);
            if (scale >= 1)
            {
                slider.Value = scale;
            }
            if (scale < 1)
            {
                slider.Value = 2 - 1 / scale;
            }
            //(DataContext as PlansViewModel).PlanDesignerViewModel.ChangeZoom((DataContext as PlansViewModel).PlanDesignerViewModel.ZoomFactor / scale);
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
    }
}

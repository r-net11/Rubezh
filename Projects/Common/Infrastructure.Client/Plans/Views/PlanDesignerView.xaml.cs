using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Infrastructure.Common;

namespace Infrastructure.Client.Plans
{
	public partial class PlanDesignerView
	{
		private const string DeviceZoomSetting = "Plans.DeviceZoom";
		private const double WheelScrollSpeed = 1;
		private Point? _lastMousePositionOnTarget;
		private Point? _lastCenterPositionOnTarget;
		private double _initialScale = 1;
		private bool _requreRefresh;
		private bool _locked;
		private readonly DispatcherTimer _timer;

		public PlanDesignerView()
		{
			InitializeComponent();

			var deviceZoom = RegistrySettingsHelper.GetDouble(DeviceZoomSetting);
			if (deviceZoom == 0)
				deviceZoom = 30;
			deviceSlider.Value = deviceZoom;

			_scrollViewer.PreviewMouseDown += OnMouseMiddleDown;
			_scrollViewer.PreviewMouseUp += OnMouseMiddleUp;
			_scrollViewer.PreviewMouseMove += OnMiddleMouseMove;
			_scrollViewer.MouseLeave += OnMiddleMouseLeave;

			_scrollViewer.PreviewMouseWheel += OnPreviewMouseWheel;
			_scrollViewer.ScrollChanged += OnScrollViewerScrollChanged;

			slider.ValueChanged += OnSliderValueChanged;
			deviceSlider.ValueChanged += deviceSlider_ValueChanged;

			Loaded += OnLoaded;
			_scrollViewer.SizeChanged += OnSizeChanged;
			_scrollViewer.LayoutUpdated += OnLayoutUpdated;
			_timer = new DispatcherTimer
			{
				Interval = TimeSpan.FromMilliseconds(100),
				IsEnabled = false,
			};
			_timer.Tick += (s, e) => _locked = false;
			_requreRefresh = true;
			_locked = true;
			Dispatcher.ShutdownStarted += (s, e) => RegistrySettingsHelper.SetDouble(DeviceZoomSetting, deviceSlider.Value);
			ZoomInCommand = new RelayCommand(OnZoomIn, CanZoomIn);
			ZoomOutCommand = new RelayCommand(OnZoomOut, CanZoomOut);
			DeviceZoomInCommand = new RelayCommand(OnDeviceZoomIn, CanDeviceZoomIn);
			DeviceZoomOutCommand = new RelayCommand(OnDeviceZoomOut, CanDeviceZoomOut);
		}

		void OnLayoutUpdated(object sender, EventArgs e)
		{
			if (_requreRefresh)
				Reset();
		}
		private void OnSizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (e.NewSize != e.PreviousSize)
				_requreRefresh = true;
		}
		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			_locked = false;
			var viewModel = (IPlanDesignerViewModel)DataContext;
			viewModel.Updated += (s, ee) => Reset();
			Reset();
			_scrollViewer.VerticalScrollBarVisibility = viewModel.AlwaysShowScroll ? ScrollBarVisibility.Visible : ScrollBarVisibility.Auto;
			_scrollViewer.HorizontalScrollBarVisibility = viewModel.AlwaysShowScroll ? ScrollBarVisibility.Visible : ScrollBarVisibility.Auto;
		}

		public void Reset()
		{
			_requreRefresh = false;
			if (_locked) return;

			FullSize();
			slider.Value = 1;
			_timer.IsEnabled = false;
			_locked = false;
			UpdateScale();
			((IPlanDesignerViewModel)DataContext).ResetZoom(slider.Value * _initialScale, deviceSlider.Value);
		}

		private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (!((IPlanDesignerViewModel)DataContext).HasPermissionsToScale)
				return;
			_lastMousePositionOnTarget = Mouse.GetPosition(_grid);
			if (e.Delta != 0)
				slider.Value += e.Delta > 0 ? WheelScrollSpeed : -WheelScrollSpeed;
			e.Handled = true;
		}
		private void OnContentMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (!Equals(e.Source, _grid)) return;

			var viewModel = (IPlanDesignerViewModel)DataContext;
			if (viewModel != null && viewModel.Canvas != null)
				viewModel.Canvas.BackgroundMouseDown(e);
		}

		public RelayCommand ZoomInCommand { get; private set; }
		private void OnZoomIn()
		{
			slider.Value += 1;
		}
		private bool CanZoomIn()
		{
			return slider.Value < slider.Maximum;
		}
		public RelayCommand ZoomOutCommand { get; private set; }
		private void OnZoomOut()
		{
			slider.Value -= 1;
		}
		private bool CanZoomOut()
		{
			return slider.Value > slider.Minimum;
		}

		public RelayCommand DeviceZoomOutCommand { get; private set; }
		private void OnDeviceZoomOut()
		{
			deviceSlider.Value--;
		}
		private bool CanDeviceZoomOut()
		{
			return deviceSlider.Value > deviceSlider.Minimum;
		}
		public RelayCommand DeviceZoomInCommand { get; private set; }
		private void OnDeviceZoomIn()
		{
			deviceSlider.Value++;
		}
		private bool CanDeviceZoomIn()
		{
			return deviceSlider.Value < deviceSlider.Maximum;
		}

		private void deviceSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			((IPlanDesignerViewModel)DataContext).ChangeDeviceZoom(e.NewValue);
		}
		void OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (e.NewValue == 0)
				return;
			_locked = true;
			_timer.IsEnabled = true;
			_timer.Stop();
			_timer.Start();
			UpdateScale();
		}
		private void UpdateScale()
		{
			scaleTransform.ScaleX = slider.Value * _initialScale;
			scaleTransform.ScaleY = slider.Value * _initialScale;

			((IPlanDesignerViewModel)DataContext).ChangeZoom(slider.Value * _initialScale);

			var centerOfViewport = new Point(_scrollViewer.ViewportWidth / 2, _scrollViewer.ViewportHeight / 2);
			_lastCenterPositionOnTarget = _scrollViewer.TranslatePoint(centerOfViewport, _grid);
		}
		void OnScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			if (e.ExtentHeightChange == 0 && e.ExtentWidthChange == 0)
				return;

			Point? targetBefore = null;
			Point? targetNow = null;

			if (!_lastMousePositionOnTarget.HasValue)
			{
				if (_lastCenterPositionOnTarget.HasValue)
				{
					var centerOfViewport = new Point(_scrollViewer.ViewportWidth / 2, _scrollViewer.ViewportHeight / 2);
					Point centerOfTargetNow = _scrollViewer.TranslatePoint(centerOfViewport, _grid);

					targetBefore = _lastCenterPositionOnTarget;
					targetNow = centerOfTargetNow;
				}
			}
			else
			{
				targetBefore = _lastMousePositionOnTarget;
				targetNow = Mouse.GetPosition(_grid);

				_lastMousePositionOnTarget = null;
			}

			if (!targetBefore.HasValue) return;

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

		private void FullSize()
		{
			var viewModel = (IPlanDesignerViewModel)DataContext;
			if (viewModel == null || viewModel.Canvas == null)
				return;
			if (viewModel.FullScreenSize)
			{
				var margin = viewModel.AlwaysShowScroll ? 51 : 5;
				double scaleX = (_scrollViewer.ViewportWidth - margin) / viewModel.Canvas.CanvasWidth;
				double scaleY = (_scrollViewer.ViewportHeight - margin) / viewModel.Canvas.CanvasHeight;
				double scale = Math.Min(scaleX, scaleY);
				if (scale < 0)
					return;
				if (Double.IsNaN(scale))
					scale = 1;
				_initialScale = scale;
			}
			else
				_initialScale = 1;
		}

		private Point? _lastDragPoint;
		private void OnMouseMiddleDown(object sender, MouseButtonEventArgs e)
		{
			if (e.MiddleButton != MouseButtonState.Pressed) return;

			var mousePos = e.GetPosition(_scrollViewer);
			if (mousePos.X <= _scrollViewer.ViewportWidth && mousePos.Y < _scrollViewer.ViewportHeight)
			{
				_lastDragPoint = mousePos;
				_scrollViewer.Cursor = Cursors.SizeAll;
				_scrollViewer.CaptureMouse();
			}
			e.Handled = true;
		}
		private void OnMouseMiddleUp(object sender, MouseButtonEventArgs e)
		{
			if (e.MiddleButton != MouseButtonState.Released) return;

			_lastDragPoint = null;
			_scrollViewer.Cursor = Cursors.Arrow;
			_scrollViewer.ReleaseMouseCapture();
		}
		private void OnMiddleMouseMove(object sender, MouseEventArgs e)
		{
			if (e.MiddleButton != MouseButtonState.Pressed || !_lastDragPoint.HasValue) return;

			Point posNow = e.GetPosition(_scrollViewer);

			double dX = posNow.X - _lastDragPoint.Value.X;
			double dY = posNow.Y - _lastDragPoint.Value.Y;

			_lastDragPoint = posNow;

			_scrollViewer.ScrollToHorizontalOffset(_scrollViewer.HorizontalOffset - dX);
			_scrollViewer.ScrollToVerticalOffset(_scrollViewer.VerticalOffset - dY);
		}
		private void OnMiddleMouseLeave(object sender, MouseEventArgs e)
		{
			if (e.MiddleButton != MouseButtonState.Pressed) return;

			_lastDragPoint = null;
			_scrollViewer.Cursor = Cursors.Arrow;
			_scrollViewer.ReleaseMouseCapture();
		}
	}
}
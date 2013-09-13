using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Infrastructure.Common;

namespace Infrastructure.Client.Plans
{
	public partial class PlanDesignerView : UserControl
	{
		private const string DeviceZoomSetting = "Plans.DeviceZoom";
		private double WheelScrollSpeed = 1;
		private Point? lastMousePositionOnTarget;
		private Point? lastCenterPositionOnTarget;
		private double initialScale = 1;
		private bool _requreRefresh;
		private bool _locked;
		private DispatcherTimer _timer;

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
			deviceSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(deviceSlider_ValueChanged);

			Loaded += new RoutedEventHandler(OnLoaded);
			_scrollViewer.SizeChanged += new SizeChangedEventHandler(OnSizeChanged);
			_scrollViewer.LayoutUpdated += new EventHandler(OnLayoutUpdated);
			_timer = new DispatcherTimer()
			{
				Interval = TimeSpan.FromMilliseconds(100),
				IsEnabled = false,
			};
			_timer.Tick += (s, e) => _locked = false;
			_requreRefresh = true;
			_locked = true;
			Dispatcher.ShutdownStarted += (s, e) => RegistrySettingsHelper.SetDouble(DeviceZoomSetting, deviceSlider.Value);
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
			if (!_locked)
			{
				FullSize();
				slider.Value = 1;
				_timer.IsEnabled = false;
				_locked = false;
				UpdateScale();
				((IPlanDesignerViewModel)DataContext).ResetZoom(slider.Value * initialScale, deviceSlider.Value);
			}
		}

		private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (!((IPlanDesignerViewModel)DataContext).HasPermissionsToScale)
				return;
			lastMousePositionOnTarget = Mouse.GetPosition(_grid);
			if (e.Delta != 0)
				slider.Value += e.Delta > 0 ? WheelScrollSpeed : -WheelScrollSpeed;
			e.Handled = true;
		}
		private void OnContentMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.Source == _grid)
			{
				var viewModel = (IPlanDesignerViewModel)DataContext;
				if (viewModel != null && viewModel.Canvas != null)
					viewModel.Canvas.BackgroundMouseDown(e);
			}
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
			scaleTransform.ScaleX = slider.Value * initialScale;
			scaleTransform.ScaleY = slider.Value * initialScale;

			((IPlanDesignerViewModel)DataContext).ChangeZoom(slider.Value * initialScale);

			var centerOfViewport = new Point(_scrollViewer.ViewportWidth / 2, _scrollViewer.ViewportHeight / 2);
			lastCenterPositionOnTarget = _scrollViewer.TranslatePoint(centerOfViewport, _grid);
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

		private void FullSize()
		{
			var viewModel = (IPlanDesignerViewModel)DataContext;
			if (viewModel == null || viewModel.Canvas == null)
				return;
			var margin = viewModel.AlwaysShowScroll ? 51 : 5;

			double scaleX = (_scrollViewer.ViewportWidth - margin) / viewModel.Canvas.CanvasWidth;
			double scaleY = (_scrollViewer.ViewportHeight - margin) / viewModel.Canvas.CanvasHeight;
			double scale = Math.Min(scaleX, scaleY);
			if (scale < 0)
				return;
			if (Double.IsNaN(scale))
				scale = 1;
			initialScale = scale;

			scaleTransform.ScaleX = scale;
			scaleTransform.ScaleY = scale;
		}

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
			((IPlanDesignerViewModel)DataContext).ChangeDeviceZoom(e.NewValue);
		}

		private Point? lastDragPoint;
		private void OnMouseMiddleDown(object sender, MouseButtonEventArgs e)
		{
			if (e.MiddleButton == MouseButtonState.Pressed)
			{
				var mousePos = e.GetPosition(_scrollViewer);
				if (mousePos.X <= _scrollViewer.ViewportWidth && mousePos.Y < _scrollViewer.ViewportHeight)
				{
					lastDragPoint = mousePos;
					_scrollViewer.Cursor = Cursors.SizeAll;
					_scrollViewer.CaptureMouse();
				}
				e.Handled = true;
			}
		}
		private void OnMouseMiddleUp(object sender, MouseButtonEventArgs e)
		{
			if (e.MiddleButton == MouseButtonState.Released)
			{
				lastDragPoint = null;
				_scrollViewer.Cursor = Cursors.Arrow;
				_scrollViewer.ReleaseMouseCapture();
			}
		}
		private void OnMiddleMouseMove(object sender, MouseEventArgs e)
		{
			if (e.MiddleButton == MouseButtonState.Pressed && lastDragPoint.HasValue)
			{
				Point posNow = e.GetPosition(_scrollViewer);

				double dX = posNow.X - lastDragPoint.Value.X;
				double dY = posNow.Y - lastDragPoint.Value.Y;

				lastDragPoint = posNow;

				_scrollViewer.ScrollToHorizontalOffset(_scrollViewer.HorizontalOffset - dX);
				_scrollViewer.ScrollToVerticalOffset(_scrollViewer.VerticalOffset - dY);
			}
		}
		private void OnMiddleMouseLeave(object sender, MouseEventArgs e)
		{
			if (e.MiddleButton == MouseButtonState.Pressed)
			{
				lastDragPoint = null;
				_scrollViewer.Cursor = Cursors.Arrow;
				_scrollViewer.ReleaseMouseCapture();
			}
		}
	}
}
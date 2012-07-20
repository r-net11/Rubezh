using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Controls;
using PlansModule.ViewModels;

namespace PlansModule.Views
{
	public partial class CanvasView : UserControl
	{
		public static CanvasView Current { get; set; }
		Point? lastCenterPositionOnTarget;
		Point? lastMousePositionOnTarget;
		Point? lastDragPoint;
		double initialScale = 1;

		public CanvasView()
		{
			Current = this;
			InitializeComponent();

			_scrollViewer.PreviewMouseDown += OnMouseMiddleDown;
			_scrollViewer.PreviewMouseUp += OnMouseMiddleUp;
			_scrollViewer.PreviewMouseMove += OnMiddleMouseMove;
			_scrollViewer.MouseLeave += OnMiddleMouseLeave;

			_scrollViewer.PreviewMouseWheel += OnPreviewMouseWheel;
			_scrollViewer.PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
			_scrollViewer.PreviewMouseLeftButtonUp += OnMouseLeftButtonUp;
			_scrollViewer.MouseLeftButtonUp += OnMouseLeftButtonUp;
			_scrollViewer.MouseMove += OnMouseMove;
			_scrollViewer.ScrollChanged += OnScrollViewerScrollChanged;

			slider.ValueChanged += OnSliderValueChanged;
			deviceSlider.ValueChanged += deviceSlider_ValueChanged;

			this.Loaded += new RoutedEventHandler(OnLoaded);
			this.SizeChanged += new SizeChangedEventHandler(OnSizeChanged);
		}

		void OnSizeChanged(object sender, SizeChangedEventArgs e)
		{
			Reset();
		}

		void OnLoaded(object sender, RoutedEventArgs e)
		{
			Reset();
		}

		public void Reset()
		{
			FullSize();
			slider.Value = 1;
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
			UpdateSlider();
			UpdateDeviceSlider();
		}

		void UpdateSlider()
		{
			scaleTransform.ScaleX = slider.Value * initialScale;
			scaleTransform.ScaleY = slider.Value * initialScale;

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

		void FullSize()
		{
			var canvas = _contentControl.Content as Canvas;
			if (canvas == null)
				return;

			double scaleX = (_scrollViewer.ActualWidth - 30) / canvas.Width;
			double scaleY = (_scrollViewer.ActualHeight - 30) / canvas.Height;
			double scale = Math.Min(scaleX, scaleY);
			if (scale < 0)
				return;
			initialScale = scale;

			scaleTransform.ScaleX = scale;
			scaleTransform.ScaleY = scale;

			UpdateDeviceSlider();
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
			UpdateDeviceSlider();
		}

		void UpdateDeviceSlider()
		{
			var canvas = _contentControl.Content as Canvas;
			if (canvas == null)
				return;

			foreach (var child in canvas.Children)
				if (child is ElementDeviceView)
				{
					ElementDeviceView elementDeviceView = child as ElementDeviceView;
					ElementDeviceViewModel viewModel = elementDeviceView.DataContext as ElementDeviceViewModel;
					double k = deviceSlider.Value / slider.Value;
					elementDeviceView.Width = k;
					elementDeviceView.Height = k;
					Canvas.SetLeft(elementDeviceView, viewModel.Location.X - k / 2);
					Canvas.SetTop(elementDeviceView, viewModel.Location.Y - k / 2);
				}
		}

		void OnMouseMiddleDown(object sender, MouseButtonEventArgs e)
		{
			if (e.MiddleButton == MouseButtonState.Pressed)
			{
				MiddleButtonScrollHelper.StartScrolling(_scrollViewer, e);
			}
		}

		void OnMouseMiddleUp(object sender, MouseButtonEventArgs e)
		{
			if (e.MiddleButton == MouseButtonState.Released)
			{
				MiddleButtonScrollHelper.StopScrolling();
			}
		}

		void OnMiddleMouseMove(object sender, MouseEventArgs e)
		{
			if (e.MiddleButton == MouseButtonState.Pressed)
			{
				MiddleButtonScrollHelper.UpdateScrolling(e);
			}
		}

		void OnMiddleMouseLeave(object sender, MouseEventArgs e)
		{
			if (e.MiddleButton == MouseButtonState.Pressed)
			{
				MiddleButtonScrollHelper.StopScrolling();
			}
		}
	}
}
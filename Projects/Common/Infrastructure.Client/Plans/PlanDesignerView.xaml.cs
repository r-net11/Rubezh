using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Controls;

namespace Infrastructure.Client.Plans
{
	public partial class PlanDesignerView : UserControl
	{
		Point? lastMousePositionOnTarget;
		Point? lastCenterPositionOnTarget;
		double initialScale = 1;

		public PlanDesignerView()
		{
			InitializeComponent();

			_scrollViewer.PreviewMouseDown += OnMouseMiddleDown;
			_scrollViewer.PreviewMouseUp += OnMouseMiddleUp;
			_scrollViewer.PreviewMouseMove += OnMiddleMouseMove;
			_scrollViewer.MouseLeave += OnMiddleMouseLeave;


			_scrollViewer.PreviewMouseWheel += OnPreviewMouseWheel;
			_scrollViewer.PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
			_scrollViewer.PreviewMouseLeftButtonUp += OnMouseLeftButtonUp;
			_scrollViewer.MouseMove += OnMouseMove;
			_scrollViewer.ScrollChanged += OnScrollViewerScrollChanged;

			slider.ValueChanged += OnSliderValueChanged;
			deviceSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(deviceSlider_ValueChanged);

			Loaded += new RoutedEventHandler(OnLoaded);
			SizeChanged += new SizeChangedEventHandler(OnSizeChanged);
		}

		private void OnSizeChanged(object sender, SizeChangedEventArgs e)
		{
			Reset();
		}
		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			((IPlanDesignerViewModel)DataContext).Updated += (s, ee) => Reset();
			Reset();
		}

		public void Reset()
		{
			((IPlanDesignerViewModel)DataContext).ChangeDeviceZoom(deviceSlider.Value);
			FullSize();
			slider.Value = 1;
		}

		private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
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
			if (e.NewValue == 0)
				return;

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

			((IPlanDesignerViewModel)DataContext).ChangeZoom(slider.Value * initialScale);
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
			((IPlanDesignerViewModel)DataContext).ChangeDeviceZoom(e.NewValue);
		}

		private void OnMouseMiddleDown(object sender, MouseButtonEventArgs e)
		{
			if (e.MiddleButton == MouseButtonState.Pressed)
			{
				MiddleButtonScrollHelper.StartScrolling(_scrollViewer, e);
			}
		}
		private void OnMouseMiddleUp(object sender, MouseButtonEventArgs e)
		{
			if (e.MiddleButton == MouseButtonState.Released)
			{
				MiddleButtonScrollHelper.StopScrolling();
			}
		}
		private void OnMiddleMouseMove(object sender, MouseEventArgs e)
		{
			if (e.MiddleButton == MouseButtonState.Pressed)
			{
				MiddleButtonScrollHelper.UpdateScrolling(e);
			}
		}
		private void OnMiddleMouseLeave(object sender, MouseEventArgs e)
		{
			if (e.MiddleButton == MouseButtonState.Pressed)
			{
				MiddleButtonScrollHelper.StopScrolling();
			}
		}
	}
}
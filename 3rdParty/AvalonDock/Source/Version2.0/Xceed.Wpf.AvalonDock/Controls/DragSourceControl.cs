using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Xceed.Wpf.AvalonDock.Controls
{
	public class DragSourceControl : Decorator
	{
		private bool _isMouseDown = false;
		private Point _mouseDownPoint;

		public static readonly DependencyProperty DragCommandProperty = DependencyProperty.Register("DragCommand", typeof(ICommand), typeof(DragSourceControl), new UIPropertyMetadata(null));
		public ICommand DragCommand
		{
			get { return (ICommand)GetValue(DragCommandProperty); }
			set { SetValue(DragCommandProperty, value); }
		}
		public static readonly DependencyProperty ClickCommandProperty = DependencyProperty.Register("ClickCommand", typeof(ICommand), typeof(DragSourceControl), new UIPropertyMetadata(null));
		public ICommand ClickCommand
		{
			get { return (ICommand)GetValue(ClickCommandProperty); }
			set { SetValue(ClickCommandProperty, value); }
		}

		public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(DragSourceControl), new UIPropertyMetadata(null));
		public object CommandParameter
		{
			get { return (object)GetValue(CommandParameterProperty); }
			set { SetValue(CommandParameterProperty, value); }
		}

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonDown(e);
			if (e.ClickCount == 1 && ((DragCommand != null && DragCommand.CanExecute(CommandParameter)) || (ClickCommand != null && ClickCommand.CanExecute(CommandParameter))))
			{
				_mouseDownPoint = e.GetPosition(this);
				_isMouseDown = true;
				CaptureMouse();
			}
		}
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			if (_isMouseDown && ClickCommand != null)
				ClickCommand.Execute(CommandParameter);
			UpdateCursor();
			_isMouseDown = false;
			base.OnMouseLeftButtonUp(e);
			if (IsMouseCaptured)
				ReleaseMouseCapture();
			if (Mouse.Captured != null)
				Mouse.Captured.ReleaseMouseCapture();
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (_isMouseDown)
			{
				if (!IsMouseCaptured)
					CaptureMouse();
				Point ptMouseMove = e.GetPosition(this);
				if (Math.Abs(ptMouseMove.X - _mouseDownPoint.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(ptMouseMove.Y - _mouseDownPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
				{
					_isMouseDown = false;
					if (IsMouseCaptured)
						ReleaseMouseCapture();
					if (Mouse.Captured != null)
						Mouse.Captured.ReleaseMouseCapture();
					if (DragCommand != null)
						DragCommand.Execute(CommandParameter);
					UpdateCursor();
				}
			}
		}
		protected override void OnMouseEnter(MouseEventArgs e)
		{
			base.OnMouseEnter(e);
			_isMouseDown = false;
			UpdateCursor();
		}
		protected override void OnMouseLeave(MouseEventArgs e)
		{
			if (IsMouseCaptured)
				ReleaseMouseCapture();
			base.OnMouseLeave(e);
			_isMouseDown = false;
			UpdateCursor();
		}

		private void UpdateCursor()
		{
			Cursor = (DragCommand != null && DragCommand.CanExecute(CommandParameter)) || (ClickCommand != null && ClickCommand.CanExecute(CommandParameter)) ? Cursors.Hand : Cursors.Arrow;
		}
	}
}

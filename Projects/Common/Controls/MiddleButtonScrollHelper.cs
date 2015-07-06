using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Controls
{
	internal enum DirectionType
	{
		None,
		Left,
		Right,
		Up,
		Down
	}

	public static class VisualTreeFinder
	{
		public static childItem FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject
		{
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
			{
				DependencyObject child = VisualTreeHelper.GetChild(obj, i);
				if (child != null && child is childItem)
					return (childItem)child;
				else
				{
					childItem childOfChild = FindVisualChild<childItem>(child);
					if (childOfChild != null)
						return childOfChild;
				}
			}
			return null;
		}

		public static T FindParentControl<T>(DependencyObject outerDepObj) where T : DependencyObject
		{
			DependencyObject dObj = VisualTreeHelper.GetParent(outerDepObj);
			if (dObj == null)
				return null;
			if (dObj is T)
				return dObj as T;
			while ((dObj = VisualTreeHelper.GetParent(dObj)) != null)
				if (dObj is T)
					return dObj as T;
			return null;
		}
	}

	public static class MiddleButtonScrollHelper
	{
		private static bool _isScrolling = false;
		private static Point _previousMousePos;
		private static Point _currentMousePos;
		private static DirectionType _currentDirection;
		private static DirectionType _previousDirection;
		private static ScrollViewer _scrollViewer;

		private static DirectionType DirectionX
		{
			get
			{
				if ((_currentMousePos.X - _previousMousePos.X) > 0)
					return DirectionType.Right;
				if ((_currentMousePos.X - _previousMousePos.X) < 0)
					return DirectionType.Left;
				return DirectionType.None;
			}
		}

		private static DirectionType DirectionY
		{
			get
			{
				if ((_currentMousePos.Y - _previousMousePos.Y) > 0)
					return DirectionType.Down;
				if ((_currentMousePos.Y - _previousMousePos.Y) < 0)
					return DirectionType.Up;
				return DirectionType.None;
			}
		}

		private static void Scroll()
		{
			switch (_currentDirection)
			{
				case DirectionType.Left:
					_scrollViewer.LineLeft();
					break;

				case DirectionType.Right:
					_scrollViewer.LineRight();
					break;

				case DirectionType.Down:
					_scrollViewer.LineDown();
					break;

				case DirectionType.Up:
					_scrollViewer.LineUp();
					break;

				default:
					break;
			}
		}

		private static void SetDirection()
		{
			if (DirectionX == DirectionType.None && DirectionY == DirectionType.None)
			{
				_currentDirection = _previousDirection;
			}
			else
			{
				if (DirectionX == DirectionType.None)
				{
					_currentDirection = DirectionY;
				}
				else
					if (DirectionY == DirectionType.None)
					{
						_currentDirection = DirectionX;
					}
			}
		}

		private static void SetCursorDirection()
		{
			switch (_currentDirection)
			{
				case DirectionType.Left:
					_scrollViewer.Cursor = Cursors.ScrollW;
					break;

				case DirectionType.Right:
					_scrollViewer.Cursor = Cursors.ScrollE;
					break;

				case DirectionType.Down:
					_scrollViewer.Cursor = Cursors.ScrollS;
					break;

				case DirectionType.Up:
					_scrollViewer.Cursor = Cursors.ScrollN;
					break;

				default:
					break;
			}
		}

		public static void StartScrolling(ScrollViewer scrollViewer, MouseButtonEventArgs e)
		{
			if (scrollViewer == null)
				return;
			_isScrolling = true;
			_scrollViewer = scrollViewer;
			_previousMousePos = _currentMousePos = scrollViewer.PointToScreen(e.GetPosition(scrollViewer));
			_currentDirection = _previousDirection = DirectionType.None;
			_scrollViewer.Cursor = Cursors.ScrollAll;
		}

		public static void UpdateScrolling(MouseEventArgs e)
		{
			if (_scrollViewer == null || !_isScrolling)
				return;
			_currentMousePos = _scrollViewer.PointToScreen(e.GetPosition(_scrollViewer));
			SetDirection();
			SetCursorDirection();
			Scroll();
			_previousMousePos = _currentMousePos;
			_previousDirection = _currentDirection;
		}

		public static void StopScrolling()
		{
			if (_scrollViewer != null && _isScrolling)
			{
				_scrollViewer.Cursor = Cursors.Arrow;
				_isScrolling = false;
				_currentDirection = _previousDirection = DirectionType.None;
				_scrollViewer = null;
			}
		}
	}
}
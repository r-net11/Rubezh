using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Controls
{
    enum DirectionType
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
        static bool _isScrolling = false;
		static Point _previousMousePos;
		static Point _currentMousePos;
        static DirectionType _currentDirection;
        static DirectionType _previousDirection;
        static ScrollViewer _scrollViewer;

        static DirectionType DirectionX
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

        static DirectionType DirectionY
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

        static void Scroll()
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

        static void SetDirection()
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

        static void SetCursorDirection()
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

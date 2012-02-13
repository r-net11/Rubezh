using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Input;

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

    public static class MiddleButtonScrollHelper
    {
        static bool _isScrolling = false;
        static double _previousMouseXPos = 0;
        static double _previousMouseYPos = 0;
        static double _currentMouseXPos = 0;
        static double _currentMouseYPos = 0;
        static DirectionType _currentDirection;
        static DirectionType _previousDirection;
        static ScrollViewer _scrollViewer;

        static DirectionType DirectionX
        {
            get
            {
                if ((_currentMouseXPos - _previousMouseXPos) > 0)
                    return DirectionType.Right;
                if ((_currentMouseXPos - _previousMouseXPos) < 0)
                    return DirectionType.Left;
                return DirectionType.None;
            }
        }

        static DirectionType DirectionY
        {
            get
            {
                if ((_currentMouseYPos - _previousMouseYPos) > 0)
                    return DirectionType.Down;
                if ((_currentMouseYPos - _previousMouseYPos) < 0)
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
            _previousMouseXPos = _currentMouseXPos = scrollViewer.PointToScreen(e.GetPosition(scrollViewer)).X;
            _previousMouseYPos = _currentMouseYPos = scrollViewer.PointToScreen(e.GetPosition(scrollViewer)).Y;
            _currentDirection = _previousDirection = DirectionType.None;
            _scrollViewer.Cursor = Cursors.ScrollAll;
        }

        public static void UpdateScrolling(MouseEventArgs e)
        {
            if (_scrollViewer == null || !_isScrolling)
                return;
            _currentMouseXPos = _scrollViewer.PointToScreen(e.GetPosition(_scrollViewer)).X;
            _currentMouseYPos = _scrollViewer.PointToScreen(e.GetPosition(_scrollViewer)).Y;
            SetDirection();
            SetCursorDirection();
            Scroll();
            _previousMouseXPos = _currentMouseXPos;
            _previousMouseYPos = _currentMouseYPos;
            _previousDirection = _currentDirection;
        }

        public static void StopScrolling()
        {
            if (_scrollViewer != null && _isScrolling)
            {
                _scrollViewer.Cursor = Cursors.Arrow;
                _isScrolling = false;
                _previousMouseXPos = 0;
                _previousMouseYPos = 0;
                _currentMouseXPos = 0;
                _currentMouseYPos = 0;
                _currentDirection = _previousDirection = DirectionType.None;
                _scrollViewer = null;
            }
        }

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
    }
}

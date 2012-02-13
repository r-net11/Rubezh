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
        static double previousMouseXPos = 0;
        static double previousMouseYPos = 0;
        static double currentMouseXPos = 0;
        static double currentMouseYPos = 0;
        static bool isScrolling = false;
        static DirectionType currentDirection;
        static DirectionType previousDirection;

        static DirectionType DirectionX
        {
            get
            {
                if ((currentMouseXPos - previousMouseXPos) > 0)
                    return DirectionType.Right;
                if ((currentMouseXPos - previousMouseXPos) < 0)
                    return DirectionType.Left;
                return DirectionType.None;
            }
        }

        static DirectionType DirectionY
        {
            get
            {
                if ((currentMouseYPos - previousMouseYPos) > 0)
                    return DirectionType.Down;
                if ((currentMouseYPos - previousMouseYPos) < 0)
                    return DirectionType.Up;
                return DirectionType.None;
            }
        }

        static void _scroll(ScrollViewer scrollViewer)
        {
            switch (currentDirection)
            {
                case DirectionType.Left:
                    scrollViewer.LineLeft();
                    break;
                case DirectionType.Right:
                    scrollViewer.LineRight();
                    break;
                case DirectionType.Down:
                    scrollViewer.LineDown();
                    break;
                case DirectionType.Up:
                    scrollViewer.LineUp();
                    break;
                default:
                    break;
            }
        }

        static void SetDirection()
        {
            if (DirectionX == DirectionType.None && DirectionY == DirectionType.None)
            {
                currentDirection = previousDirection;
            }
            else
            {
                if (DirectionX == DirectionType.None)
                {
                    currentDirection = DirectionY;
                }
                else
                    if (DirectionY == DirectionType.None)
                    {
                        currentDirection = DirectionX;
                    }
            }
        }

        static void SetCursorDirection(Control control)
        {
            if (control == null)
                return;
            switch (currentDirection)
            {
                case DirectionType.Left:
                    control.Cursor = Cursors.ScrollW;
                    break;
                case DirectionType.Right:
                    control.Cursor = Cursors.ScrollE;
                    break;
                case DirectionType.Down:
                    control.Cursor = Cursors.ScrollS;
                    break;
                case DirectionType.Up:
                    control.Cursor = Cursors.ScrollN;
                    break;
                default:
                    break;
            }
        }

        public static void StartScrolling(object sender, MouseButtonEventArgs e, ScrollViewer scrollViewer)
        {
            var control = sender as Control;
            if (control == null)
                return;
            isScrolling = true;
            previousMouseXPos = currentMouseXPos = control.PointToScreen(e.GetPosition(control)).X;
            previousMouseYPos = currentMouseYPos = control.PointToScreen(e.GetPosition(control)).Y;
            currentDirection = previousDirection = DirectionType.None;
            control.Cursor = Cursors.ScrollAll;
        }

        public static void UpdateScrolling(object sender, MouseEventArgs e, ScrollViewer scrollViewer)
        {
            var control = sender as Control;
            if (control == null || scrollViewer == null || !isScrolling)
                return;
            currentMouseXPos = control.PointToScreen(e.GetPosition(control)).X;
            currentMouseYPos = control.PointToScreen(e.GetPosition(control)).Y;
            SetDirection();
            SetCursorDirection(control);
            _scroll(scrollViewer);
            previousMouseXPos = currentMouseXPos;
            previousMouseYPos = currentMouseYPos;
            previousDirection = currentDirection;
        }

        public static void StopScrolling(object sender)
        {
            var control = sender as Control;
            if (control != null && isScrolling)
            {
                isScrolling = false;
                control.Cursor = Cursors.Arrow;
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

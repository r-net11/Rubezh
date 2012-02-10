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
        static double startMouseXPos = 0;
        static double startMouseYPos = 0;
        static bool isScrolling = false;
        static DirectionType currentDirection;
        static DirectionType previousDirection;

        static double DiffXPosAbs { get { return Math.Abs(currentMouseXPos - startMouseXPos); } }
        static double DiffYPosAbs { get { return Math.Abs(currentMouseYPos - startMouseYPos); } }
        static double SpeedXPos
        {
            get
            {
                if (DiffXPosAbs > 10)
                    return 1;
                if (DiffXPosAbs > 100)
                    return 5;
                return 0;
            }
        }
        static double SpeedYPos
        {
            get
            {
                if (DiffYPosAbs > 10)
                    return 1;
                if (DiffYPosAbs > 100)
                    return 5;
                return 0;
            }
        }

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
                    for (int i = 0; i < SpeedXPos; i++) scrollViewer.LineLeft();
                    break;
                case DirectionType.Right:
                    for (int i = 0; i < SpeedXPos; i++) scrollViewer.LineRight();
                    break;
                case DirectionType.Down:
                    for (int i = 0; i < SpeedYPos; i++) scrollViewer.LineDown();
                    break;
                case DirectionType.Up:
                    for (int i = 0; i < SpeedYPos; i++) scrollViewer.LineUp();
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
            if (SpeedXPos == 0 && SpeedYPos == 0) control.Cursor = Cursors.ScrollAll;
        }

        public static void StartScrolling(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control == null)
                return;
            isScrolling = true;
            startMouseXPos = control.PointToScreen(e.GetPosition(control)).X;
            startMouseYPos = control.PointToScreen(e.GetPosition(control)).Y;
            previousMouseXPos = currentMouseXPos = startMouseXPos;
            previousMouseYPos = currentMouseYPos = startMouseYPos;
            currentDirection = previousDirection = DirectionType.None;
            control.Cursor = Cursors.ScrollAll;
        }

        public static void UpdateScrolling(object sender, MouseEventArgs e, ScrollViewer scrollViewer)
        {
            var control = sender as Control;
            if (control == null || scrollViewer == null)
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

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
    public static class MiddleButtonScrollHelper
    {
        static double mouseXPos = 0;
        static double mouseYPos = 0;
        static double startMouseXPos = 0;
        static double startMouseYPos = 0;
        static bool isScrolling = false;

        static double DiffXPosAbs { get { return Math.Abs(mouseXPos - startMouseXPos); } }
        static double DiffYPosAbs { get { return Math.Abs(mouseYPos - startMouseYPos); } }
        static double SpeedXPos
        {
            get
            {
                if (DiffXPosAbs > 5)
                    return 0.5 * DirectionX;
                if (DiffXPosAbs > 50)
                    return 1 * DirectionX;
                if (DiffXPosAbs > 200)
                    return 3 * DirectionX;
                return 1 * DirectionX;
            }
        }
        static double SpeedYPos
        {
            get
            {
                if (DiffYPosAbs > 5)
                    return 1 * DirectionY;
                if (DiffYPosAbs > 100)
                    return 5 * DirectionY;
                return 1 * DirectionY;
            }
        }
        static int DirectionX
        {
            get
            {
                if ((mouseXPos - startMouseXPos) > 0)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
        }
        static int DirectionY
        {
            get
            {
                if ((mouseYPos - startMouseYPos) > 0)
                    return 1;
                else
                    return -1;
            }
        }


        static void SetCursorDirection(Control control)
        {
            if (control == null)
                return;
            if (DirectionY > 0) control.Cursor = Cursors.ScrollS;
            if (DirectionY < 0) control.Cursor = Cursors.ScrollN;
        }

        public static void SetStartPosition(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control == null)
                return;
            isScrolling = true;
            mouseXPos = control.PointToScreen(e.GetPosition(control)).X;
            mouseYPos = control.PointToScreen(e.GetPosition(control)).Y;
            startMouseXPos = mouseXPos;
            startMouseYPos = mouseYPos;
            control.Cursor = Cursors.ScrollAll;
        }

        public static void Scrolling(object sender, MouseEventArgs e, ScrollViewer scrollViewer)
        {
            var control = sender as Control;
            if (control == null || scrollViewer == null)
                return;
            mouseXPos = control.PointToScreen(e.GetPosition(control)).X;
            mouseYPos = control.PointToScreen(e.GetPosition(control)).Y;
            SetCursorDirection(control);
            scrollViewer.ScrollToVerticalOffset(scrollViewer.ContentVerticalOffset + SpeedYPos);
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

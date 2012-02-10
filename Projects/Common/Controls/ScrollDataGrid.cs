using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;

namespace Controls
{
    public class ScrollDataGrid : DataGrid
    {
        static ScrollDataGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ScrollDataGrid), new FrameworkPropertyMetadata(typeof(ScrollDataGrid)));
        }

        public ScrollDataGrid()
        {
            MouseDown += new MouseButtonEventHandler(MouseMiddleDown);
            MouseUp += new MouseButtonEventHandler(MouseMiddleUp);
            MouseMove += new MouseEventHandler(MiddleMouseMove);
            Loaded += new RoutedEventHandler(ScrollDataGridLoaded);
        }

        ScrollViewer _scrollViewer;

        void MouseMiddleDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                MiddleButtonScrollHelper.StartScrolling(sender, e);
            }
        }

        void MouseMiddleUp(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Released)
            {
                MiddleButtonScrollHelper.StopScrolling(sender);
            }
        }

        void MiddleMouseMove(object sender, MouseEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                MiddleButtonScrollHelper.UpdateScrolling(sender, e, _scrollViewer);
            }
            else
            {
                MiddleButtonScrollHelper.StopScrolling(sender);
            }
        }

        void ScrollDataGridLoaded(object sender, RoutedEventArgs e)
        {
            _scrollViewer = MiddleButtonScrollHelper.FindVisualChild<ScrollViewer>(this);
        }
    }
}

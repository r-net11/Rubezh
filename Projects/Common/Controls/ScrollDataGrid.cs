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
            Loaded += new RoutedEventHandler(ScrollDataGridLoaded);
        }

        ScrollViewer _scrollViewer;

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

        void ScrollDataGridLoaded(object sender, RoutedEventArgs e)
        {

            _scrollViewer = MiddleButtonScrollHelper.FindVisualChild<ScrollViewer>(this);
            if (_scrollViewer == null)
                return;
            _scrollViewer.PreviewMouseDown += new MouseButtonEventHandler(OnMouseMiddleDown);
            _scrollViewer.PreviewMouseUp += new MouseButtonEventHandler(OnMouseMiddleUp);
            _scrollViewer.PreviewMouseMove += new MouseEventHandler(OnMiddleMouseMove);
            _scrollViewer.MouseLeave += new MouseEventHandler(OnMiddleMouseLeave);
        }
    }
}

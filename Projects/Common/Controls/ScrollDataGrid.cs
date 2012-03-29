using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System;
using System.Windows.Controls.Primitives;
using System.Reflection;
using System.Collections.Generic;
using System.Windows.Media;
using System.ComponentModel;

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
                MiddleButtonScrollHelper.StartScrolling(_scrollViewer, e);
        }

        void OnMouseMiddleUp(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Released)
                MiddleButtonScrollHelper.StopScrolling();
        }

        void OnMiddleMouseMove(object sender, MouseEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
                MiddleButtonScrollHelper.UpdateScrolling(e);
        }

        void OnMiddleMouseLeave(object sender, MouseEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
                MiddleButtonScrollHelper.StopScrolling();
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

    public class DataGridRollbackOnUnfocusedBehaviour
    {
        public static bool GetDataGridRollbackOnUnfocused(DataGrid datagrid)
        {
            return (bool)datagrid.GetValue(DataGridRollbackOnUnfocusedProperty);
        }

        public static void SetDataGridRollbackOnUnfocused(DataGrid datagrid, bool value)
        {
            datagrid.SetValue(DataGridRollbackOnUnfocusedProperty, value);
        }

        public static readonly DependencyProperty DataGridRollbackOnUnfocusedProperty =
            DependencyProperty.RegisterAttached("DataGridRollbackOnUnfocused",typeof(bool), typeof(DataGridRollbackOnUnfocusedBehaviour), 
            new UIPropertyMetadata(false, OnDataGridRollbackOnUnfocusedChanged));

        static void OnDataGridRollbackOnUnfocusedChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            DataGrid datagrid = depObj as DataGrid;
            if (datagrid == null)
                return;
            
            if (e.NewValue is bool == false)
                return;

            if ((bool)e.NewValue)
                datagrid.DataContextChanged += RollbackDataGridOnDataContextChanged;
            else
                datagrid.DataContextChanged -= RollbackDataGridOnDataContextChanged;
        }

        static void RollbackDataGridOnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            DataGrid senderDatagrid = sender as DataGrid;

            if (senderDatagrid == null)
                return;
            IEditableCollectionView collection = senderDatagrid.Items as IEditableCollectionView;

            if (collection.IsEditingItem)
            {
                collection.CommitEdit();
                collection.CancelEdit();
            }
            else if (collection.IsAddingNew)
            {
                collection.CancelNew();
            }
        }
    }

    class VisualTreeFinder
    {
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
}

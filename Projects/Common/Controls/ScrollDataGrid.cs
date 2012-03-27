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

    public class DataGridRollbackOnUnfocusedBehaviour
    {
        #region DataGridRollbackOnUnfocusedBehaviour

        public static bool GetDataGridRollbackOnUnfocused(DataGrid datagrid)
        {
            return (bool)datagrid.GetValue(DataGridRollbackOnUnfocusedProperty);
        }

        public static void SetDataGridRollbackOnUnfocused(
         DataGrid datagrid, bool value)
        {
            datagrid.SetValue(DataGridRollbackOnUnfocusedProperty, value);
        }

        public static readonly DependencyProperty DataGridRollbackOnUnfocusedProperty =
            DependencyProperty.RegisterAttached(
            "DataGridRollbackOnUnfocused",
            typeof(bool),
            typeof(DataGridRollbackOnUnfocusedBehaviour),
            new UIPropertyMetadata(false, OnDataGridRollbackOnUnfocusedChanged));

        static void OnDataGridRollbackOnUnfocusedChanged(
         DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            DataGrid datagrid = depObj as DataGrid;
            if (datagrid == null)
                return;

            if (e.NewValue is bool == false)
                return;

            if ((bool)e.NewValue)
            {
                //datagrid.LostKeyboardFocus += RollbackDataGridOnLostFocus;
                datagrid.DataContextChanged += RollbackDataGridOnDataContextChanged;
            }
            else
            {
                //datagrid.LostKeyboardFocus -= RollbackDataGridOnLostFocus;
                datagrid.DataContextChanged -= RollbackDataGridOnDataContextChanged;
            }
        }

        static void RollbackDataGridOnLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            DataGrid senderDatagrid = sender as DataGrid;

            if (senderDatagrid == null)
                return;

            UIElement focusedElement = Keyboard.FocusedElement as UIElement;
            if (focusedElement == null)
                return;

            DataGrid focusedDatagrid = GetParentDatagrid(focusedElement); //let's see if the new focused element is inside a datagrid
            if (focusedDatagrid == senderDatagrid)
            {
                return;
                //if the new focused element is inside the same datagrid, then we don't need to do anything;
                //this happens, for instance, when we enter in edit-mode: the DataGrid element loses keyboard-focus, which passes to the selected DataGridCell child
            }

            //otherwise, the focus went outside the datagrid; in order to avoid exceptions like ("DeferRefresh' is not allowed during an AddNew or EditItem transaction")
            //or ("CommitNew is not allowed for this view"), we undo the possible pending changes, if any
            IEditableCollectionView collection = senderDatagrid.Items as IEditableCollectionView;

            if (collection.IsEditingItem)
            {
                collection.CancelEdit();
            }
            else if (collection.IsAddingNew)
            {
                collection.CancelNew();
            }
        }

        private static DataGrid GetParentDatagrid(UIElement element)
        {
            UIElement childElement; //element from which to start the tree navigation, looking for a Datagrid parent
            var elem = element as ComboBoxItem;
            if (elem != null) //since ComboBoxItem.Parent is null, we must pass through ItemsPresenter in order to get the parent ComboBox
            {
                ItemsPresenter parentItemsPresenter = VisualTreeFinder.FindParentControl<ItemsPresenter>((elem));
                ComboBox combobox = parentItemsPresenter.TemplatedParent as ComboBox;
                childElement = combobox;
            }
            else
            {
                childElement = element;
            }

            DataGrid parentDatagrid = VisualTreeFinder.FindParentControl<DataGrid>(childElement); //let's see if the new focused element is inside a datagrid
            return parentDatagrid;
        }

        static void RollbackDataGridOnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            DataGrid senderDatagrid = sender as DataGrid;

            if (senderDatagrid == null)
                return;

            
            IEditableCollectionView collection = senderDatagrid.Items as IEditableCollectionView;

            if (collection.IsEditingItem && collection.CanCancelEdit)
            {
                collection.CancelEdit();
            }
            else if (collection.IsAddingNew)
            {
                collection.CancelNew();
            }
        }

        #endregion DataGridRollbackOnUnfocusedBehaviour
    }

    //public class DataGridCommitEditBehavior
    //{
    //    public static readonly DependencyProperty CommitOnLostFocusProperty =
    //        DependencyProperty.RegisterAttached(
    //            "CommitOnLostFocus",
    //            typeof(bool),
    //            typeof(DataGridCommitEditBehavior),
    //            new UIPropertyMetadata(false, OnCommitOnLostFocusChanged));

    //    /// <summary>
    //    ///   A hack to find the data grid in the event handler of the tab control.
    //    /// </summary>
    //    private static readonly Dictionary<TabPanel, DataGrid> ControlMap = new Dictionary<TabPanel, DataGrid>();

    //    public static bool GetCommitOnLostFocus(DataGrid datagrid)
    //    {
    //        return (bool)datagrid.GetValue(CommitOnLostFocusProperty);
    //    }

    //    public static void SetCommitOnLostFocus(DataGrid datagrid, bool value)
    //    {
    //        datagrid.SetValue(CommitOnLostFocusProperty, value);
    //    }

    //    private static void CommitEdit(DataGrid dataGrid)
    //    {
    //        dataGrid.CommitEdit(DataGridEditingUnit.Cell, true);
    //        dataGrid.CommitEdit(DataGridEditingUnit.Row, true);
    //    }

    //    private static DataGrid GetParentDatagrid(UIElement element)
    //    {
    //        UIElement childElement; // element from which to start the tree navigation, looking for a Datagrid parent

    //        if (element is ComboBoxItem)
    //        {
    //            // Since ComboBoxItem.Parent is null, we must pass through ItemsPresenter in order to get the parent ComboBox
    //            var parentItemsPresenter = VisualTreeFinder.FindParentControl<ItemsPresenter>(element as ComboBoxItem);
    //            var combobox = parentItemsPresenter.TemplatedParent as ComboBox;
    //            childElement = combobox;
    //        }
    //        else
    //        {
    //            childElement = element;
    //        }

    //        var parentDatagrid = VisualTreeFinder.FindParentControl<DataGrid>(childElement);
    //        return parentDatagrid;
    //    }

    //    private static TabPanel GetTabPanel(TabControl tabControl)
    //    {
    //        return
    //            (TabPanel)
    //                tabControl.GetType().InvokeMember(
    //                    "ItemsHost",
    //                    BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.Instance,
    //                    null,
    //                    tabControl,
    //                    null);
    //    }

    //    private static void OnCommitOnLostFocusChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
    //    {
    //        var dataGrid = depObj as DataGrid;
    //        if (dataGrid == null)
    //        {
    //            return;
    //        }

    //        if (e.NewValue is bool == false)
    //        {
    //            return;
    //        }

    //        var parentTabControl = VisualTreeFinder.FindParentControl<TabControl>(dataGrid);
    //        var tabPanel = GetTabPanel(parentTabControl);
    //        if (tabPanel != null)
    //        {
    //            ControlMap[tabPanel] = dataGrid;
    //        }

    //        if ((bool)e.NewValue)
    //        {
    //            // Attach event handlers
    //            if (parentTabControl != null)
    //            {
    //                tabPanel.PreviewMouseLeftButtonDown += OnParentTabControlPreviewMouseLeftButtonDown;
    //            }

    //            dataGrid.LostKeyboardFocus += OnDataGridLostFocus;
    //            dataGrid.DataContextChanged += OnDataGridDataContextChanged;
    //            dataGrid.IsVisibleChanged += OnDataGridIsVisibleChanged;
    //        }
    //        else
    //        {
    //            // Detach event handlers
    //            if (parentTabControl != null)
    //            {
    //                tabPanel.PreviewMouseLeftButtonDown -= OnParentTabControlPreviewMouseLeftButtonDown;
    //            }

    //            dataGrid.LostKeyboardFocus -= OnDataGridLostFocus;
    //            dataGrid.DataContextChanged -= OnDataGridDataContextChanged;
    //            dataGrid.IsVisibleChanged -= OnDataGridIsVisibleChanged;
    //        }
    //    }

    //    private static void OnDataGridDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    //    {
    //        var dataGrid = (DataGrid)sender;
    //        CommitEdit(dataGrid);
    //    }

    //    private static void OnDataGridIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    //    {
    //        var senderDatagrid = (DataGrid)sender;

    //        if ((bool)e.NewValue == false)
    //        {
    //            CommitEdit(senderDatagrid);
    //        }
    //    }

    //    private static void OnDataGridLostFocus(object sender, KeyboardFocusChangedEventArgs e)
    //    {
    //        var dataGrid = (DataGrid)sender;

    //        var focusedElement = Keyboard.FocusedElement as UIElement;
    //        if (focusedElement == null)
    //        {
    //            return;
    //        }

    //        var focusedDatagrid = GetParentDatagrid(focusedElement);

    //        // Let's see if the new focused element is inside a datagrid
    //        if (focusedDatagrid == dataGrid)
    //        {
    //            // If the new focused element is inside the same datagrid, then we don't need to do anything;
    //            // this happens, for instance, when we enter in edit-mode: the DataGrid element loses keyboard-focus, 
    //            // which passes to the selected DataGridCell child
    //            return;
    //        }

    //        CommitEdit(dataGrid);
    //    }

    //    private static void OnParentTabControlPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    //    {
    //        var dataGrid = ControlMap[(TabPanel)sender];
    //        CommitEdit(dataGrid);
    //    }
    //}

    //public static class VisualTreeFinder
    //{
    //    /// <summary>
    //    ///   Find a specific parent object type in the visual tree
    //    /// </summary>
    //    public static T FindParentControl<T>(DependencyObject outerDepObj) where T : DependencyObject
    //    {
    //        var dObj = VisualTreeHelper.GetParent(outerDepObj);
    //        if (dObj == null)
    //        {
    //            return null;
    //        }

    //        if (dObj is T)
    //        {
    //            return dObj as T;
    //        }

    //        while ((dObj = VisualTreeHelper.GetParent(dObj)) != null)
    //        {
    //            if (dObj is T)
    //            {
    //                return dObj as T;
    //            }
    //        }

    //        return null;
    //    }
    //}

    class VisualTreeFinder
    {

        /// <summary>
        /// Find a specific parent object type in the visual tree
        /// </summary>
        public static T FindParentControl<T>(DependencyObject outerDepObj) where T : DependencyObject
        {
            DependencyObject dObj = VisualTreeHelper.GetParent(outerDepObj);
            if (dObj == null)
                return null;

            if (dObj is T)
                return dObj as T;

            while ((dObj = VisualTreeHelper.GetParent(dObj)) != null)
            {
                if (dObj is T)
                    return dObj as T;
            }

            return null;
        }

    }

}

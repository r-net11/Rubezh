using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Input;

namespace Controls
{
    public class DataGridProperties
    {
        static DataGridProperties()
        {
            var metadata = new FrameworkPropertyMetadata((Brush)null);
            HeaderBrushProperty = DependencyProperty.RegisterAttached("HeaderBrush", typeof(Brush), typeof(DataGridProperties), metadata);

            DataGridRollbackOnUnfocusedProperty = DependencyProperty.RegisterAttached("DataGridRollbackOnUnfocused", typeof(bool), typeof(DataGridProperties),
            new UIPropertyMetadata(false, OnDataGridRollbackOnUnfocusedChanged));

            DataGridMiddleButtonScrollProperty = DependencyProperty.RegisterAttached("DataGridMiddleButtonScroll", typeof(bool), typeof(DataGridProperties),
                new UIPropertyMetadata(false, OnDataGridMiddleButtonScrollChanged));
        }

        public static readonly DependencyProperty HeaderBrushProperty;
        public static readonly DependencyProperty DataGridRollbackOnUnfocusedProperty;
        public static readonly DependencyProperty DataGridMiddleButtonScrollProperty;

        public static Brush GetHeaderBrush(DependencyObject obj)
        {
            return (Brush)obj.GetValue(HeaderBrushProperty);
        }

        public static void SetHeaderBrush(DependencyObject obj, Brush value)
        {
            obj.SetValue(HeaderBrushProperty, value);
        }

        public static bool GetDataGridRollbackOnUnfocused(DataGrid datagrid)
        {
            return (bool)datagrid.GetValue(DataGridRollbackOnUnfocusedProperty);
        }

        public static void SetDataGridRollbackOnUnfocused(DataGrid datagrid, bool value)
        {
            datagrid.SetValue(DataGridRollbackOnUnfocusedProperty, value);
        }

        public static bool GetDataGridMiddleButtonScroll(DataGrid datagrid)
        {
            return (bool)datagrid.GetValue(DataGridMiddleButtonScrollProperty);
        }

        public static void SetDataGridMiddleButtonScroll(DataGrid datagrid, bool value)
        {
            datagrid.SetValue(DataGridMiddleButtonScrollProperty, value);
        }

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

        static void OnDataGridMiddleButtonScrollChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            DataGrid datagrid = depObj as DataGrid;
            if (datagrid == null)
                return;

            if (e.NewValue is bool == false)
                return;

            if ((bool)e.NewValue)
                datagrid.Loaded += ScrollDataGridLoaded;
            else
                datagrid.Loaded -= ScrollDataGridLoaded;
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

        static void OnMouseMiddleDown(object sender, MouseButtonEventArgs e)
        {
            
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                var scrollViewer = sender as ScrollViewer;
                if (scrollViewer == null)
                    return;
                MiddleButtonScrollHelper.StartScrolling(scrollViewer, e);
            }
        }

        static void OnMouseMiddleUp(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Released)
                MiddleButtonScrollHelper.StopScrolling();
        }

        static void OnMiddleMouseMove(object sender, MouseEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
                MiddleButtonScrollHelper.UpdateScrolling(e);
        }

        static void OnMiddleMouseLeave(object sender, MouseEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
                MiddleButtonScrollHelper.StopScrolling();
        }

        static void ScrollDataGridLoaded(object sender, RoutedEventArgs e)
        {
            var dg = sender as DataGrid;
            if (dg == null)
                return;
            var scrollViewer = VisualTreeFinder.FindVisualChild<ScrollViewer>(dg);
            if (scrollViewer == null)
                return;
            scrollViewer.PreviewMouseDown += new MouseButtonEventHandler(OnMouseMiddleDown);
            scrollViewer.PreviewMouseUp += new MouseButtonEventHandler(OnMouseMiddleUp);
            scrollViewer.PreviewMouseMove += new MouseEventHandler(OnMiddleMouseMove);
            scrollViewer.MouseLeave += new MouseEventHandler(OnMiddleMouseLeave);
        }
    }
}

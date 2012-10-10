using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DevicesModule.Views
{
	public partial class DevicesView : UserControl
	{
		public DevicesView()
		{
			InitializeComponent();
			//Loaded += new RoutedEventHandler(DevicesView_Loaded);
			//_devicesDataGrid.SelectionChanged += new SelectionChangedEventHandler(DevicesView_SelectionChanged);
			//_devicesDataGrid.PreviewKeyDown += new KeyEventHandler(_devicesDataGrid_PreviewKeyDown);
		}

		//void DevicesView_Loaded(object sender, RoutedEventArgs e)
		//{
		//    if (_devicesDataGrid.SelectedItem != null)
		//        _devicesDataGrid.ScrollIntoView(_devicesDataGrid.SelectedItem);
		//    Focus();
		//}

		//void DevicesView_SelectionChanged(object sender, RoutedEventArgs e)
		//{
		//    if (_devicesDataGrid.SelectedItem != null)
		//        _devicesDataGrid.ScrollIntoView(_devicesDataGrid.SelectedItem);
		//}

		//private void _devicesDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
		//{
		//    if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Space)
		//    {
		//        e.Handled = true;
		//        var dataGrid = sender as DataGrid;
		//        if (dataGrid != null)
		//            dataGrid.BeginEdit();
		//    }
		//}

		//#region DataGrid Helper
		//public static DataGridCell GetCurrentCell(DataGrid SourceDataGrid)
		//{
		//    if (SourceDataGrid.CurrentCell == null)
		//        return null;

		//    var RowContainer = SourceDataGrid.ItemContainerGenerator.ContainerFromItem(SourceDataGrid.CurrentCell.Item);
		//    if (RowContainer == null)
		//        return null;

		//    var RowPresenter = GetVisualChild<System.Windows.Controls.Primitives.DataGridCellsPresenter>(RowContainer);
		//    if (RowPresenter == null)
		//        return null;

		//    var Container = RowPresenter.ItemContainerGenerator.ContainerFromItem(SourceDataGrid.CurrentCell.Item);
		//    var Cell = Container as DataGridCell;

		//    // Try to get the cell if null, because maybe the cell is virtualized
		//    if (Cell == null)
		//    {
		//        SourceDataGrid.ScrollIntoView(RowContainer, SourceDataGrid.CurrentCell.Column);
		//        Container = RowPresenter.ItemContainerGenerator.ContainerFromItem(SourceDataGrid.CurrentCell.Item);
		//        Cell = Container as DataGridCell;
		//    }

		//    return Cell;
		//}

		//public static TRet GetVisualChild<TRet>(DependencyObject Target) where TRet : DependencyObject
		//{
		//    if (Target == null)
		//        return null;

		//    for (int ChildIndex = 0; ChildIndex < VisualTreeHelper.GetChildrenCount(Target); ChildIndex++)
		//    {
		//        var Child = VisualTreeHelper.GetChild(Target, ChildIndex);

		//        if (Child != null && Child is TRet)
		//            return (TRet)Child;
		//        else
		//        {
		//            TRet childOfChild = GetVisualChild<TRet>(Child);

		//            if (childOfChild != null)
		//                return childOfChild;
		//        }
		//    }

		//    return null;
		//}
		//#endregion
	}
}
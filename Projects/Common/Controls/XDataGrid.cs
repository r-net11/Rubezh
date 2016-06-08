using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Controls
{
	public class XDataGrid : DataGrid
	{
		static XDataGrid()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(XDataGrid), new FrameworkPropertyMetadata(typeof(XDataGrid)));
		}

		private DataGridCell _previousDataGridCell;
		private DataGridCell _currentDataGridCell;

		public XDataGrid()
		{
			ClipboardCopyMode = DataGridClipboardCopyMode.None;
			SelectionChanged += DataGrid_SelectionChanged;
			PreviewMouseDown += DataGrid_PreviewMouseDown;
			MouseDoubleClick += DataGrid_MouseDoubleClick;
		}

		public static readonly DependencyProperty DoubleClickOffProperty = DependencyProperty.Register("IsDoubleClickOff", typeof(Boolean), typeof(XDataGrid), new PropertyMetadata(false));

		public Boolean IsDoubleClickOff
		{
			get { return (Boolean)GetValue(DoubleClickOffProperty); }
			set { SetValue(DoubleClickOffProperty, value); }
		}

		private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var dataGrid = sender as DataGrid;
			if (dataGrid == null || dataGrid.SelectedItem == null) return;

			dataGrid.ScrollIntoView(dataGrid.SelectedItem);

			var dgrow = (DataGridRow)ItemContainerGenerator.ContainerFromItem(dataGrid.SelectedItem);
			if (dgrow != null)
				dgrow.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
		}

		private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var depObj = (DependencyObject)e.Device.Target;

			if (CheckForHeader(depObj))
				return;

			if (IsDoubleClickOff)
				return;
			if (_previousDataGridCell != _currentDataGridCell)
				return;

			var dataGrid = sender as DataGrid;
			var viewModel = dataGrid.DataContext as IEditingBaseViewModel;
			if (viewModel != null)
			{
				if (viewModel.EditCommand.CanExecute(null))
					viewModel.EditCommand.Execute();
			}
		}

		private static bool CheckForHeader(DependencyObject depObj)
		{
			while (depObj != null && !(depObj is DataGridColumnHeader))
				depObj = VisualTreeHelper.GetParent(depObj);

			return depObj != null;
		}

		private void DataGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			var depObj = (DependencyObject)e.OriginalSource;

			var dataGrid = sender as DataGrid;
			if (dataGrid == null) return;

			if (CheckForHeader(depObj)) //Off context menu if click on DataGridColumnHeader
				dataGrid.ContextMenu = null;
			else
				dataGrid.ClearValue(ContextMenuProperty);

			_previousDataGridCell = _currentDataGridCell;
			var element = e.MouseDevice.DirectlyOver;

			if (element is FrameworkElement && (((FrameworkElement)element).Parent is DataGridCell || ((FrameworkElement)element).Parent == null))
			{
				_currentDataGridCell = ((FrameworkElement)element).Parent as DataGridCell;
			}
			else
			{
				_currentDataGridCell = null;
				dataGrid.SelectedItem = null;
			}
		}

		protected override void OnCanExecuteCopy(CanExecuteRoutedEventArgs args)
		{
			args.CanExecute = false;
			args.ContinueRouting = true;
		}
	}
}
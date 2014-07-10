using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Infrastructure.Common.Windows.ViewModels;

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
			SelectionChanged += new SelectionChangedEventHandler(DataGrid_SelectionChanged);
			PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(DataGrid_PreviewMouseDown);
			MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(DataGrid_MouseDoubleClick);
		}

		public static readonly DependencyProperty DoubleClickOffProperty = DependencyProperty.Register("IsDoubleClickOff", typeof(Boolean), typeof(XDataGrid), new PropertyMetadata(false));
		public Boolean IsDoubleClickOff
		{
			get { return (Boolean)this.GetValue(DoubleClickOffProperty); }
			set { this.SetValue(DoubleClickOffProperty, value); }
		}

		private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var dataGrid = sender as DataGrid;
			if (dataGrid != null && dataGrid.SelectedItem != null)
			{
				dataGrid.ScrollIntoView(dataGrid.SelectedItem);
				var dgrow = (DataGridRow)ItemContainerGenerator.ContainerFromItem(dataGrid.SelectedItem);
				if (dgrow != null)
					dgrow.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
			}
		}
		private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
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
		private void DataGrid_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			_previousDataGridCell = _currentDataGridCell;
			IInputElement element = e.MouseDevice.DirectlyOver;
			if (element != null && element is FrameworkElement && (((FrameworkElement)element).Parent is DataGridCell || ((FrameworkElement)element).Parent == null))
			{
				_currentDataGridCell = ((FrameworkElement)element).Parent as DataGridCell;
			}
			else
			{
				_currentDataGridCell = null;
				var dataGrid = sender as DataGrid;
				dataGrid.SelectedItem = null;
			}
		}
	}
}
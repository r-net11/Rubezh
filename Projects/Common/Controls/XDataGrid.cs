using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Infrastructure.Common.Windows.ViewModels;
using System.Diagnostics;

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
			var depObj = (DependencyObject)e.OriginalSource;
			while (depObj != null && !(depObj is DataGridCell) && !(depObj is DataGridColumnHeader))
			{
				depObj = VisualTreeHelper.GetParent(depObj);
			}
			if (!(depObj is DataGridCell))
			{
				return;
			}

			if (IsDoubleClickOff)
				return;
			if (_previousDataGridCell != _currentDataGridCell)
				return;
			if(e.LeftButton != MouseButtonState.Pressed)
				return;

			var dataGrid = sender as DataGrid;
			var viewModel = dataGrid.DataContext as IEditingBaseViewModel;
			if (viewModel != null)
			{
				if (viewModel.EditCommand.CanExecute(null))
					viewModel.EditCommand.Execute();
			}
		}

		private void DataGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			var dataGrid = sender as DataGrid;
			if (dataGrid == null)
				return;

			var depObj = (DependencyObject)e.OriginalSource;
			while (depObj != null && !(depObj is DataGridCell) && !(depObj is DataGridColumnHeader))
			{
				depObj = VisualTreeHelper.GetParent(depObj);
			}

			_previousDataGridCell = _currentDataGridCell;

			if (depObj == null || depObj is DataGridColumnHeader)
			{
				_currentDataGridCell = null;
				dataGrid.SelectedItem = null;
				return;
			}

			if(depObj is DataGridCell)
			{
				_currentDataGridCell = depObj as DataGridCell;
				return;
			}
		}

		protected override void OnCanExecuteCopy(CanExecuteRoutedEventArgs args)
		{
			args.CanExecute = false;
			args.ContinueRouting = true;
		}
	}
}
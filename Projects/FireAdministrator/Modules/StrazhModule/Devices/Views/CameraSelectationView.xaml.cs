﻿using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace StrazhModule.Views
{
	public partial class CameraSelectationView
	{
		public CameraSelectationView()
		{
			InitializeComponent();
		}

		public static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
		{
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
			{
				DependencyObject child = VisualTreeHelper.GetChild(obj, i);
				if (child != null && child is T)
					return (T)child;
				else
				{
					T childOfChild = FindVisualChild<T>(child);
					if (childOfChild != null)
						return childOfChild;
				}
			}
			return null;
		}

		public static DataGridCell GetCell(DataGrid dataGrid, DataGridRow rowContainer, int column)
		{
			if (rowContainer != null)
			{
				DataGridCellsPresenter presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);
				if (presenter == null)
				{
					/* if the row has been virtualized away, call its ApplyTemplate() method
					 * to build its visual tree in order for the DataGridCellsPresenter
					 * and the DataGridCells to be created */
					rowContainer.ApplyTemplate();
					presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);
				}
				if (presenter != null)
				{
					DataGridCell cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
					if (cell == null)
					{
						/* bring the column into view
						 * in case it has been virtualized away */
						dataGrid.ScrollIntoView(rowContainer, dataGrid.Columns[column]);
						cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
					}
					return cell;
				}
			}
			return null;
		}

		private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{

			if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)
				|| Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
				return;
			DataGrid dataGrid = sender as DataGrid;
			if (dataGrid != null && dataGrid.SelectedItem != null)
			{
				dataGrid.UpdateLayout();
				dataGrid.ScrollIntoView(dataGrid.SelectedItem);
				var row = dataGrid.ItemContainerGenerator.ContainerFromItem(dataGrid.SelectedItem) as DataGridRow;
				if (row != null)
				{
					DataGridCell cell = GetCell(dataGrid, row, 0);
					if (cell != null)
					{
						if (cell.IsFocused)
							return;
						var method = typeof(DataGrid).GetMethod("HandleSelectionForCellInput", BindingFlags.Instance | BindingFlags.NonPublic);
						method.Invoke(dataGrid, new object[] { cell, false, false, false });
						cell.Focus();
					}
				}
			}
		}
	}
}
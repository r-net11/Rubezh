using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Controls
{
	public static class DataGridSingleRowSelected
	{
		public static readonly DependencyProperty IsSelectionFixEnabledProperty = DependencyProperty.RegisterAttached
		(
			"IsSelectionFixEnabled",
			typeof(bool?),
			typeof(DataGridSingleRowSelected),
			new PropertyMetadata(null, IsSelectionFixEnabledChanged)
		);

		public static bool GetIsSelectionFixEnabled(DataGrid element)
		{
			return (bool)element.GetValue(IsSelectionFixEnabledProperty);
		}

		public static void SetIsSelectionFixEnabled(DataGrid element, bool value)
		{
			element.SetValue(IsSelectionFixEnabledProperty, value);
		}

		private static void IsSelectionFixEnabledChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			var dataGrid = sender as DataGrid;
			if (dataGrid != null)
			{
				if (args.OldValue == null)
				{
					dataGrid.ItemContainerGenerator.StatusChanged += (s, e) => ContainerStatusChanged(dataGrid, ((ItemContainerGenerator)s));
				}
			}
		}

		private static void ContainerStatusChanged(DataGrid dataGrid, ItemContainerGenerator generator)
		{
			if (generator != null && generator.Status == GeneratorStatus.ContainersGenerated && dataGrid.SelectedItems.Count == 1)
			{
				var row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromItem(dataGrid.SelectedItems[0]);
				if (row != null)
				{
					//var cell = dataGrid.GetCell(row, 0);
					var cell = DataGridHelper.GetCell(dataGrid, 0, 0);
					if (cell != null)
					{
						SelectCellMethod.Invoke(dataGrid, new object[] { cell, false, false, false });
					}
				}
			}
		}

		private static readonly MethodInfo SelectCellMethod = typeof(DataGrid).GetMethod("HandleSelectionForCellInput", BindingFlags.Instance | BindingFlags.NonPublic);
	}
}

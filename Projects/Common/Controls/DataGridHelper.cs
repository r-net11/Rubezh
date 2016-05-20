using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Controls
{
	static class DataGridHelper
	{
		static public DataGridCell GetCell(DataGrid dg, int row, int column)
		{
			DataGridRow rowContainer = GetRow(dg, row);

			if (rowContainer != null)
			{
				DataGridCellsPresenter presenter = VisualHelper.FindVisualChild<DataGridCellsPresenter>(rowContainer);

				// try to get the cell but it may possibly be virtualized
				DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
				if (cell == null)
				{
					// now try to bring into view and retreive the cell
					dg.ScrollIntoView(rowContainer, dg.Columns[column]);
					cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
				}
				return cell;
			}
			return null;
		}

		static public DataGridRow GetRow(DataGrid dg, int index)
		{
			DataGridRow row = (DataGridRow)dg.ItemContainerGenerator.ContainerFromIndex(index);
			if (row == null)
			{
				// may be virtualized, bring into view and try again
				dg.ScrollIntoView(dg.Items[index]);
				row = (DataGridRow)dg.ItemContainerGenerator.ContainerFromIndex(index);
			}
			return row;
		}
	}
}
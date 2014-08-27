using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeReason.Reports.Interfaces;
using System.Windows.Documents;
using CodeReason.Reports.Document;
using System.IO;
using System.Data;
using System.Windows.Media;

namespace CodeReason.Reports.Providers
{
	internal class SplitTableProvider
	{
		private TableProvider _tableProvider;
		private FlowDocument _flowDocument;
		private Section _lastSection;
		private DocumentWalker _walker;
		private ISplitTable _splitTable;
		private ReportData _data;
		public SplitTableProvider(ISplitTable splitTable, ReportData data)
		{
			_tableProvider = new TableProvider();
			_walker = new DocumentWalker();
			_splitTable = splitTable;
			_data = data;
		}

		public void FillTable()
		{
			var splitTableSection = _splitTable as SplitTable;
			if (splitTableSection == null)
				return;
			_flowDocument = splitTableSection.Parent as FlowDocument;
			if (_flowDocument == null)
				throw new InvalidDataException("SplitTable must have FlowDocument as parent");
			_lastSection = splitTableSection;
			if (_splitTable.PageBreakBefore)
				AddSection(new Section() { BreakPageBefore = true });
			if (_splitTable.Header != null)
				AddSection(_splitTable.Header);

			FillTableInternal();

			if (_splitTable.Footer != null)
				AddSection(_splitTable.Footer);
			if (_splitTable.PageBreakAfter)
				AddSection(new Section() { BreakPageBefore = true });
			_flowDocument.Blocks.Remove(splitTableSection);
			_lastSection = null;
			_flowDocument = null;
		}

		private void FillTableInternal()
		{
			var table = _data.GetDataTableByName(_splitTable.TableName);
			if (table == null)
			{
				if (_data.ShowUnknownValues)
					AddEmptyTable();
				return;
			}
			int pageNumber = 0;
			while (!GenerateTable(table, pageNumber))
				pageNumber++;
		}
		private void AddSection(Section section)
		{
			var splitTableSection = (SplitTable)_splitTable;
			section.FontSize = splitTableSection.FontSize;
			section.FontWeight = splitTableSection.FontWeight;
			section.TextAlignment = splitTableSection.TextAlignment;
			_flowDocument.Blocks.InsertAfter(_lastSection, section);
			_lastSection = section;
		}
		private void AddEmptyTable()
		{
			var emptyTable = CreateTable();
			if (_splitTable.HeaderRowGroup != null)
				emptyTable.RowGroups.Add(_splitTable.HeaderRowGroup);
			if (_splitTable.ContentRowGroup != null)
			{
				foreach (var tableRow in _splitTable.ContentRowGroup.Rows)
					foreach (TableCell cell in tableRow.Cells)
					{
						List<ITableCellValue> tableCells = _walker.TraverseBlockCollection<ITableCellValue>(cell.Blocks);
						foreach (ITableCellValue cv in tableCells)
						{
							IPropertyValue dv = cv as IPropertyValue;
							if (dv != null)
								dv.Value = "[" + dv.PropertyName + "]";
							else
							{
								IIndexValue iv = cv as IIndexValue;
								iv.Value = "[" + iv.Index + "]";
							}
						}
					}
				emptyTable.RowGroups.Add(_splitTable.ContentRowGroup);
			}
			if (_splitTable.FooterRowGroup != null)
				emptyTable.RowGroups.Add(_splitTable.FooterRowGroup);
			SetTableBorder(emptyTable);
			var emptySection = new Section();
			emptySection.Blocks.Add(emptyTable);
			AddSection(emptySection);
		}
		private Table CreateTable()
		{
			var table = new Table();
			if (_splitTable.Columns != null)
				foreach (var column in _splitTable.Columns)
					table.Columns.Add(new TableColumn() { Width = column.Width });
			return table;
		}
		private void SetTableBorder(Table table)
		{
			table.BorderThickness = _splitTable.BorderThickness;
			table.CellSpacing = 0;
			table.BorderBrush = Brushes.Black;
			if (_splitTable.BorderThickness != null)
				foreach (var rowGroup in table.RowGroups)
					foreach (var row in rowGroup.Rows)
						foreach (var cell in row.Cells)
						{
							cell.BorderThickness = _splitTable.BorderThickness;
							cell.BorderBrush = Brushes.Black;
						}
		}
		private bool GenerateTable(DataTable dataTable, int pageNumber)
		{
			var rowsOnFirstPage = _splitTable.RowsOnFirstPage == 0 ? _splitTable.RowsOnPage : _splitTable.RowsOnFirstPage;
			var rowsOnLastPage = _splitTable.RowsOnLatPage == 0 ? _splitTable.RowsOnPage : _splitTable.RowsOnLatPage;

			var startRow = rowsOnFirstPage + _splitTable.RowsOnPage * (pageNumber - 1);
			var isLastPage = false;
			var rowsOnPage = _splitTable.RowsOnPage;
			if (rowsOnPage == 0)
			{
				rowsOnPage = dataTable.Rows.Count;
				isLastPage = true;
			}
			else
			{
				if (pageNumber == 0)
				{
					rowsOnPage = rowsOnFirstPage;
					startRow = 0;
				}
				isLastPage = startRow + rowsOnPage >= dataTable.Rows.Count;
				if (isLastPage)
				{
					if (pageNumber > 1 && startRow >= dataTable.Rows.Count && dataTable.Rows.Count - startRow + rowsOnPage > rowsOnLastPage)
						startRow = startRow - rowsOnPage + rowsOnLastPage;
					if (dataTable.Rows.Count - startRow > rowsOnLastPage)
					{
						isLastPage = false;
						rowsOnPage = rowsOnLastPage;
					}
					else
						rowsOnPage = dataTable.Rows.Count - startRow;
				}
			}

			var table = CreateTable();
			if ((pageNumber == 0 || _splitTable.TableHeaderOnEachPage) && _splitTable.HeaderRowGroup != null)
				table.RowGroups.Add(_tableProvider.Clone(_splitTable.HeaderRowGroup, true));

			for (int i = startRow; i < startRow + rowsOnPage; i++)
			{
				var rowGroup = _tableProvider.Clone(_splitTable.ContentRowGroup, true);
				foreach (TableRow row in rowGroup.Rows)
					foreach (TableCell cell in row.Cells)
					{
						var newCells = _walker.TraverseBlockCollection<ITableCellValue>(cell.Blocks);
						foreach (ITableCellValue cv in newCells)
						{
							IPropertyValue dv = cv as IPropertyValue;
							IIndexValue iv = null;
							if (dv == null)
							{
								iv = cv as IIndexValue;
								if (iv == null)
									continue;
							}
							try
							{
								object obj = dv == null ? dataTable.Rows[i][iv.Index] : dataTable.Rows[i][dv.PropertyName];
								if (obj == DBNull.Value)
									obj = null;
								cv.Value = obj;
							}
							catch
							{
								if (_data.ShowUnknownValues)
									cv.Value = "[" + (dv == null ? iv.Index.ToString() : dv.PropertyName) + "]";
								else
									cv.Value = "";
							}
						}
					}
				table.RowGroups.Add(rowGroup);
			}

			if ((isLastPage || _splitTable.TableFooterOnEachPage) && _splitTable.FooterRowGroup != null)
				table.RowGroups.Add(_tableProvider.Clone(_splitTable.FooterRowGroup, true));

			SetTableBorder(table);
			var tableSection = new Section();
			if (pageNumber != 0)
				tableSection.BreakPageBefore = true;
			tableSection.Blocks.Add(table);
			AddSection(tableSection);
			return isLastPage;
		}
	}
}

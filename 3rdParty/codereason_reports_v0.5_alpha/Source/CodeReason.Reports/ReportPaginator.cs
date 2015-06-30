﻿/************************************************************************
 * Copyright: Hans Wolff
 *
 * License:  This software abides by the LGPL license terms. For further
 *           licensing information please see the top level LICENSE.txt 
 *           file found in the root directory of CodeReason Reports.
 *
 * Author:   Hans Wolff
 *
 ************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CodeReason.Reports.Document;
using CodeReason.Reports.Interfaces;
using System.Windows.Markup;
using CodeReason.Reports.Providers;
//using System.Windows.Markup;

namespace CodeReason.Reports
{
	/// <summary>
	/// Creates all pages of a report
	/// </summary>
	public class ReportPaginator : DocumentPaginator
	{
		/// <summary>
		/// Reference to a original flowdoc paginator
		/// </summary>
		protected DocumentPaginator _paginator = null;

		protected FlowDocument _flowDocument = null;
		protected ReportDocument _report = null;
		protected ReportData _data = null;
		protected Block _blockPageHeader = null;
		protected Block _blockPageFooter = null;
		protected ArrayList _reportContextValues = null;
		protected ReportPaginatorDynamicCache _dynamicCache = null;
		protected DateTime _reportDate;
		protected Dictionary<string, List<object>> _aggregateValues;
		protected Hint _hints;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="report">report document</param>
		/// <param name="data">report data</param>
		/// <exception cref="ArgumentException">Flow document must have a specified page height</exception>
		/// <exception cref="ArgumentException">Flow document must have a specified page width</exception>
		/// <exception cref="ArgumentException">Flow document can have only one report header section</exception>
		/// <exception cref="ArgumentException">Flow document can have only one report footer section</exception>
		public ReportPaginator(ReportDocument report, ReportData data)
		{
			TotalPageCount = -1;
			PageShift = 0;
			_report = report;
			_data = data;
			_reportDate = DateTime.Now;

			using (new TimeCounter("\t\tCreateFlowDocument	{0}"))
				_flowDocument = report.CreateFlowDocument(out _hints);
			_pageSize = new Size(_flowDocument.PageWidth, _flowDocument.PageHeight);

			if (_flowDocument.PageHeight == double.NaN)
				throw new ArgumentException("Flow document must have a specified page height");
			if (_flowDocument.PageWidth == double.NaN)
				throw new ArgumentException("Flow document must have a specified page width");

			_dynamicCache = new ReportPaginatorDynamicCache(_flowDocument);
			ArrayList listPageHeaders = _dynamicCache.GetFlowDocumentVisualListByType(typeof(SectionReportHeader));
			if (listPageHeaders.Count > 1)
				throw new ArgumentException("Flow document can have only one report header section");
			if (listPageHeaders.Count == 1)
				_blockPageHeader = (SectionReportHeader)listPageHeaders[0];
			ArrayList listPageFooters = _dynamicCache.GetFlowDocumentVisualListByType(typeof(SectionReportFooter));
			if (listPageFooters.Count > 1)
				throw new ArgumentException("Flow document can have only one report footer section");
			if (listPageFooters.Count == 1)
				_blockPageFooter = (SectionReportFooter)listPageFooters[0];

			_paginator = ((IDocumentPaginatorSource)_flowDocument).DocumentPaginator;

			// remove header and footer in our working copy
			Block block = _flowDocument.Blocks.FirstBlock;
			while (block != null)
			{
				Block thisBlock = block;
				block = block.NextBlock;
				if ((thisBlock == _blockPageHeader) || (thisBlock == _blockPageFooter))
					_flowDocument.Blocks.Remove(thisBlock);
			}

			HandleDataGroups();

			// get report context values
			_reportContextValues = _dynamicCache.GetFlowDocumentVisualListByInterface(typeof(IInlineContextValue));

			using (new TimeCounter("\t\tFillData	{0}"))
				FillData();
		}

		protected void RememberAggregateValue(Dictionary<string, List<object>> aggregateValues, string aggregateGroups, object value)
		{
			if (String.IsNullOrEmpty(aggregateGroups))
				return;

			string[] aggregateGroupParts = aggregateGroups.Split(',', ';');

			// remember value for aggregate functions
			List<object> aggregateValueList = null;
			foreach (string aggregateGroup in aggregateGroupParts)
			{
				if (String.IsNullOrEmpty(aggregateGroup))
					continue;
				string trimmedGroup = aggregateGroup.Trim();
				if (String.IsNullOrEmpty(trimmedGroup))
					continue;
				if (!aggregateValues.TryGetValue(trimmedGroup, out aggregateValueList))
				{
					aggregateValueList = new List<object>();
					aggregateValues[trimmedGroup] = aggregateValueList;
				}
				aggregateValueList.Add(value);
			}
		}

		ContainerVisual CloneVisualBlock(Block block, int pageNumber)
		{
			FlowDocument tmpDoc = new FlowDocument();
			tmpDoc.ColumnWidth = double.PositiveInfinity;
			tmpDoc.PageHeight = _report.PageHeight;
			tmpDoc.PageWidth = _report.PageWidth;
			tmpDoc.PagePadding = new Thickness(0);

			Block newBlock = XamlHelper.Clone<Block>(block);
			tmpDoc.Blocks.Add(newBlock);

			DocumentWalker walkerBlock = new DocumentWalker();
			ArrayList blockValues = new ArrayList();
			blockValues.AddRange(walkerBlock.Walk<IInlineContextValue>(tmpDoc));

			// fill context values
			FillContextValues(blockValues, pageNumber);

			DocumentPage dp = ((IDocumentPaginatorSource)tmpDoc).DocumentPaginator.GetPage(0);
			return (ContainerVisual)dp.Visual;
		}

		protected virtual void FillContextValues(ArrayList list, int pageNumber)
		{
			// fill context values
			foreach (IInlineContextValue cv in list)
			{
				if (cv == null)
					continue;
				switch (cv.Type)
				{
					case ReportContextValueType.PageNumber:
						cv.Value = pageNumber + PageShift;
						break;
					case ReportContextValueType.PageCount:
						cv.Value = TotalPageCount == -1 ? PageCount : TotalPageCount;
						Console.WriteLine(cv.Value);
						break;
					case ReportContextValueType.ReportName:
						cv.Value = _report.ReportName;
						break;
					case ReportContextValueType.ReportTitle:
						cv.Value = _report.ReportTitle;
						break;
					case ReportContextValueType.ReportDate:
						cv.Value = _reportDate;
						break;
				}
			}
		}

		public int PageShift { get; set; }
		public int TotalPageCount { get; set; }

		/// <summary>
		/// This is most important method, modifies the original 
		/// </summary>
		/// <param name="pageNumber">page number</param>
		/// <returns></returns>
		public override DocumentPage GetPage(int pageNumber)
		{
			for (int i = 0; i < 2; i++) // do it twice because filling context values could change the page count
			{
				// compute page count
				if (pageNumber == 0)
				{
					_paginator.ComputePageCount();
					_pageCount = _paginator.PageCount;
				}

				// fill context values
				FillContextValues(_reportContextValues, pageNumber + 1);
			}

			DocumentPage page = _paginator.GetPage(pageNumber);
			if (page == DocumentPage.Missing)
				return DocumentPage.Missing; // page missing

			_pageSize = page.Size;
			ContainerVisual newPage = null;
			DependencyObject parent = VisualTreeHelper.GetParent(page.Visual);
			if (parent != null)
				newPage = VisualTreeHelper.GetParent(parent) as ContainerVisual;
			if (newPage == null)
			{
				// add header block
				newPage = new ContainerVisual();

				if (_blockPageHeader != null)
				{
					ContainerVisual v = CloneVisualBlock(_blockPageHeader, pageNumber + 1);
					v.Offset = new Vector(0, 0);
					newPage.Children.Add(v);
				}

				// TODO: process ReportContextValues

				// add content page
				ContainerVisual smallerPage = new ContainerVisual();
				smallerPage.Offset = new Vector(0, _report.PageHeaderHeight / 100d * _report.PageHeight);
				smallerPage.Children.Add(page.Visual);
				newPage.Children.Add(smallerPage);

				// add footer block
				if (_blockPageFooter != null)
				{
					ContainerVisual v = CloneVisualBlock(_blockPageFooter, pageNumber + 1);
					v.Offset = new Vector(0, _report.PageHeight - _report.PageFooterHeight / 100d * _report.PageHeight);
					newPage.Children.Add(v);
				}
			}

			// create modified BleedBox
			Rect bleedBox = new Rect(page.BleedBox.Left, page.BleedBox.Top, page.BleedBox.Width,
				_report.PageHeight - (page.Size.Height - page.BleedBox.Size.Height));

			// create modified ContentBox
			Rect contentBox = new Rect(page.ContentBox.Left, page.ContentBox.Top, page.ContentBox.Width,
				_report.PageHeight - (page.Size.Height - page.ContentBox.Size.Height));

			DocumentPage dp = new DocumentPage(newPage, new Size(_report.PageWidth, _report.PageHeight), bleedBox, contentBox);
			_report.FireEventGetPageCompleted(new GetPageCompletedEventArgs(page, pageNumber, null, false, null));
			return dp;
		}

		/// <summary>
		/// Determines if the current page count is valid
		/// </summary>
		public override bool IsPageCountValid
		{
			get { return _paginator.IsPageCountValid; }
		}

		private int _pageCount = 0;
		/// <summary>
		/// Gets the total page count
		/// </summary>
		public override int PageCount
		{
			get { return _pageCount; }
		}

		private Size _pageSize = Size.Empty;
		/// <summary>
		/// Gets or sets the page size
		/// </summary>
		public override Size PageSize
		{
			get { return _pageSize; }
			set { _pageSize = value; }
		}

		/// <summary>
		/// Gets the paginator source
		/// </summary>
		public override IDocumentPaginatorSource Source
		{
			get { return _paginator.Source; }
		}

		/// <summary>
		/// Fills document with data
		/// </summary>
		/// <exception cref="InvalidDataException">ReportTableRow must have a TableRowGroup as parent</exception>
		protected virtual void FillData()
		{
			ArrayList blockDocumentValues = _dynamicCache.GetFlowDocumentVisualListByInterface(typeof(IInlineDocumentValue)); // walker.Walk<IInlineDocumentValue>(_flowDocument);
			ArrayList blockTableRows = _dynamicCache.GetFlowDocumentVisualListByInterface(typeof(ITableRowForDataTable)); // walker.Walk<TableRowForDataTable>(_flowDocument);
			ArrayList blockTableRowGroups = _dynamicCache.GetFlowDocumentVisualListByInterface(typeof(ITableRowGroupForDataTable));
			ArrayList blockAggregateValues = _dynamicCache.GetFlowDocumentVisualListByType(typeof(InlineAggregateValue)); // walker.Walk<InlineAggregateValue>(_flowDocument);
			ArrayList charts = _dynamicCache.GetFlowDocumentVisualListByInterface(typeof(IChart)); // walker.Walk<IChart>(_flowDocument);
			ArrayList dynamicHeaderTableRows = _dynamicCache.GetFlowDocumentVisualListByInterface(typeof(ITableRowForDynamicHeader));
			ArrayList dynamicDataTableRows = _dynamicCache.GetFlowDocumentVisualListByInterface(typeof(ITableRowForDynamicDataTable));
			ArrayList splitTables = _dynamicCache.GetFlowDocumentVisualListByInterface(typeof(ISplitTable));

			List<Block> blocks = new List<Block>();
			if (_blockPageHeader != null)
				blocks.Add(_blockPageHeader);
			if (_blockPageFooter != null)
				blocks.Add(_blockPageFooter);

			DocumentWalker walker = new DocumentWalker();
			blockDocumentValues.AddRange(walker.TraverseBlockCollection<IInlineDocumentValue>(blocks));
			foreach (ISplitTable splitTable in splitTables)
			{
				if (splitTable.ContentRowGroup != null)
					blockDocumentValues.AddRange(walker.TraverseRowGroup<IInlineDocumentValue>(splitTable.ContentRowGroup));
				if (splitTable.HeaderRowGroup != null)
					blockDocumentValues.AddRange(walker.TraverseRowGroup<IInlineDocumentValue>(splitTable.HeaderRowGroup));
				if (splitTable.FooterRowGroup != null)
					blockDocumentValues.AddRange(walker.TraverseRowGroup<IInlineDocumentValue>(splitTable.FooterRowGroup));
				if (splitTable.Header != null)
					blockDocumentValues.AddRange(walker.TraverseBlockCollection<IInlineDocumentValue>(splitTable.Header.Blocks));
				if (splitTable.Footer != null)
					blockDocumentValues.AddRange(walker.TraverseBlockCollection<IInlineDocumentValue>(splitTable.Footer.Blocks));
			}

			_aggregateValues = new Dictionary<string, List<object>>();

			FillCharts(charts);
			FillDocumentValues(blockDocumentValues);
			FillSplitTables(splitTables);
			FillDynamicDataTableRows(dynamicDataTableRows);
			FillDynamicHeaderTableRows(dynamicHeaderTableRows);
			FillTableRows(blockTableRows);
			FillTableRowGroups(blockTableRowGroups);
			FillAggregateValues(blockAggregateValues);
		}

		protected virtual void FillDocumentValues(ArrayList blockDocumentValues)
		{

			// fill report values
			foreach (IInlineDocumentValue dv in blockDocumentValues)
			{
				if (dv == null)
					continue;
				object obj = null;
				if ((dv.PropertyName != null) && (_data.ReportDocumentValues.TryGetValue(dv.PropertyName, out obj)))
				{
					dv.Value = obj;
					RememberAggregateValue(_aggregateValues, dv.AggregateGroup, obj);
				}
				else
				{
					if ((_data.ShowUnknownValues) && (dv.Value == null))
						dv.Value = "[" + ((dv.PropertyName != null) ? dv.PropertyName : "NULL") + "]";
					RememberAggregateValue(_aggregateValues, dv.AggregateGroup, null);
				}
			}
		}
		protected virtual void FillSplitTables(ArrayList splitTables)
		{
			foreach (ISplitTable table in splitTables)
			{
				var provider = new SplitTableProvider(table, _data);
				provider.FillTable();
			}
		}
		protected virtual void FillDynamicDataTableRows(ArrayList dynamicDataTableRows)
		{
			// fill dynamic tables
			foreach (ITableRowForDynamicDataTable iTableRow in dynamicDataTableRows)
			{
				TableRow tableRow = iTableRow as TableRow;
				if (tableRow == null)
					continue;

				TableRowGroup tableGroup = tableRow.Parent as TableRowGroup;
				if (tableGroup == null)
					continue;

				TableRow currentRow = null;

				DataTable table = _data.GetDataTableByName(iTableRow.TableName);

				for (int i = 0; i < table.Rows.Count; i++)
				{
					currentRow = new TableRow();

					DataRow dataRow = table.Rows[i];
					for (int j = 0; j < table.Columns.Count; j++)
					{
						string value = dataRow[j].ToString();
						currentRow.Cells.Add(new TableCell(new Paragraph(new Run(value))));
					}
					tableGroup.Rows.Add(currentRow);
				}
			}
		}
		protected virtual void FillDynamicHeaderTableRows(ArrayList dynamicHeaderTableRows)
		{
			foreach (ITableRowForDynamicHeader iTableRow in dynamicHeaderTableRows)
			{
				TableRow tableRow = iTableRow as TableRow;
				if (tableRow == null)
					continue;

				DataTable table = _data.GetDataTableByName(iTableRow.TableName);

				foreach (DataRow row in table.Rows)
				{
					string value = row[0].ToString();
					TableCell tableCell = new TableCell(new Paragraph(new Run(value)));
					tableRow.Cells.Add(tableCell);
				}

			}
		}
		protected virtual void FillTableRows(ArrayList blockTableRows)
		{
			// fill tables
			foreach (ITableRowForDataTable iTableRow in blockTableRows)
			{
				TableRow tableRow = iTableRow as TableRow;
				if (tableRow == null)
					continue;

				DataTable table = _data.GetDataTableByName(iTableRow.TableName);
				if (table == null)
				{
					if (_data.ShowUnknownValues)
					{
						// show unknown values
						foreach (TableCell cell in tableRow.Cells)
						{
							DocumentWalker localWalker = new DocumentWalker();
							List<ITableCellValue> tableCells = localWalker.TraverseBlockCollection<ITableCellValue>(cell.Blocks);
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
								IAggregateValue av = cv as IAggregateValue;
								if (av != null)
									RememberAggregateValue(_aggregateValues, av.AggregateGroup, null);
							}
						}
					}
					else
						continue;
				}
				else
				{
					TableRowGroup rowGroup = tableRow.Parent as TableRowGroup;
					if (rowGroup == null)
						throw new InvalidDataException("ReportTableRow must have a TableRowGroup as parent");

					List<TableRow> listNewRows = new List<TableRow>();
					foreach (TableRow row in rowGroup.Rows)
					{
						TableRowForDataTable reportTableRow = row as TableRowForDataTable;
						if (reportTableRow == null)
						{
							// clone regular row
							if ((_hints & Hint.MoveRegular) == Hint.MoveRegular)
								listNewRows.Add(row);
							else
								listNewRows.Add(XamlHelper.Clone(row));
						}
						else
						{
							string reportTableRowXaml = null;
							if ((_hints & Hint.SimpleClone) == Hint.None)
								reportTableRowXaml = XamlWriter.Save(reportTableRow);

							// clone ReportTableRows
							List<TableRow> clonedTableRows = new List<TableRow>();
							for (int i = 0; i < table.Rows.Count; i++)
							{
								var clonedRow = (_hints & Hint.SimpleClone) == Hint.SimpleClone ? XamlHelper.SimpleClone(reportTableRow) : XamlHelper.LoadXamlFromString<TableRow>(reportTableRowXaml);
								clonedTableRows.Add(clonedRow);
							}

							foreach (DataRow dataRow in table.Rows)
							{
								// get cloned ReportTableRow
								TableRow newTableRow = clonedTableRows[0];
								clonedTableRows.RemoveAt(0);

								FillTableRow(newTableRow, dataRow);
								listNewRows.Add(newTableRow);

								// fire event
								_report.FireEventDataRowBoundEventArgs(new DataRowBoundEventArgs(_report, dataRow) { TableName = dataRow.Table.TableName, TableRow = newTableRow });
							}
						}
					}
					rowGroup.Rows.Clear();
					foreach (TableRow row in listNewRows)
						rowGroup.Rows.Add(row);
				}
			}
		}
		protected virtual void FillTableRowGroups(ArrayList blockTableRowGroups)
		{
			foreach (ITableRowGroupForDataTable iTableRowGroup in blockTableRowGroups)
			{
				TableRowGroup tableRowGroup = iTableRowGroup as TableRowGroup;
				if (tableRowGroup == null)
					continue;

				DataTable dataTable = _data.GetDataTableByName(iTableRowGroup.TableName);
				if (dataTable == null)
				{
					if (_data.ShowUnknownValues)
					{
						// show unknown values
						foreach (var tableRow in tableRowGroup.Rows)
							foreach (TableCell cell in tableRow.Cells)
							{
								DocumentWalker localWalker = new DocumentWalker();
								List<ITableCellValue> tableCells = localWalker.TraverseBlockCollection<ITableCellValue>(cell.Blocks);
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
									IAggregateValue av = cv as IAggregateValue;
									if (av != null)
										RememberAggregateValue(_aggregateValues, av.AggregateGroup, null);
								}
							}
					}
					else
						continue;
				}
				else
				{
					Table table = tableRowGroup.Parent as Table;
					if (table == null)
						throw new InvalidDataException("ReportTableRow must have a TableRowGroup as parent");

					List<TableRowGroup> listNewRowGroups = new List<TableRowGroup>();
					foreach (TableRowGroup rowGroup in table.RowGroups)
					{
						TableRowGroupForDataTable reportTableRowGroup = rowGroup as TableRowGroupForDataTable;
						if (reportTableRowGroup == null)
						{
							// clone regular row group
							if ((_hints & Hint.MoveRegular) == Hint.MoveRegular)
								listNewRowGroups.Add(rowGroup);
							else
								listNewRowGroups.Add(XamlHelper.Clone(rowGroup));
						}
						else
						{
							string reportTableRowGroupXaml = null;
							using (new TimeCounter("\t\tSave	{0}"))
								if ((_hints & Hint.SimpleClone) == Hint.None)
									reportTableRowGroupXaml = XamlWriter.Save(reportTableRowGroup);

							// clone ReportTableRows
							List<TableRowGroup> clonedTableRowGroups = new List<TableRowGroup>();
							using (new TimeCounter("\t\tLoad	{0}"))
								for (int i = 0; i < dataTable.Rows.Count; i++)
								{
									var clonedRowGroup = (_hints & Hint.SimpleClone) == Hint.SimpleClone ? XamlHelper.SimpleClone(reportTableRowGroup) : XamlHelper.LoadXamlFromString<TableRowGroup>(reportTableRowGroupXaml);
									clonedTableRowGroups.Add(clonedRowGroup);
								}

							using (new TimeCounter("\t\tSet	{0}"))
								foreach (DataRow dataRow in dataTable.Rows)
								{
									// get cloned ReportTableRow
									TableRowGroup newTableRowGroup = clonedTableRowGroups[0];
									clonedTableRowGroups.RemoveAt(0);

									foreach (TableRow row in newTableRowGroup.Rows)
										FillTableRow(row, dataRow);
									listNewRowGroups.Add(newTableRowGroup);

									// fire event
									_report.FireEventDataRowBoundEventArgs(new DataRowBoundEventArgs(_report, dataRow) { TableName = dataRow.Table.TableName, TableRowGroup = newTableRowGroup });
								}
						}
					}
					table.RowGroups.Clear();
					foreach (TableRowGroup group in listNewRowGroups)
						table.RowGroups.Add(group);
				}
			}
		}
		protected virtual void FillAggregateValues(ArrayList blockAggregateValues)
		{
			// fill aggregate values
			foreach (InlineAggregateValue av in blockAggregateValues)
			{
				if (String.IsNullOrEmpty(av.AggregateGroup))
					continue;
				if (!_aggregateValues.ContainsKey(av.AggregateGroup))
				{
					av.Text = av.EmptyValue;
				}
				else
				{
					av.Text = av.ComputeAndFormat(_aggregateValues);
				}
			}
		}

		/// <summary>
		/// Fill charts with data
		/// </summary>
		/// <param name="charts">list of charts</param>
		/// <exception cref="TimeoutException">Thread for drawing charts timed out</exception>
		protected virtual void FillCharts(ArrayList charts)
		{
			Window window = null;

			// fill charts
			foreach (IChart chart in charts)
			{
				if (chart == null)
					continue;
				Canvas chartCanvas = chart as Canvas;
				if (String.IsNullOrEmpty(chart.TableName))
					continue;
				if (String.IsNullOrEmpty(chart.TableColumns))
					continue;

				DataTable table = _data.GetDataTableByName(chart.TableName);
				if (table == null)
					continue;

				if (chartCanvas != null)
				{
					// HACK: this here is REALLY dirty!!!
					IChart newChart = (IChart)chart.Clone();
					if (window == null)
					{
						window = new Window();
						window.WindowStyle = WindowStyle.None;
						window.BorderThickness = new Thickness(0);
						window.ShowInTaskbar = false;
						window.Left = 30000;
						window.Top = 30000;
						window.Show();
					}
					window.Width = chartCanvas.Width + 2 * SystemParameters.BorderWidth;
					window.Height = chartCanvas.Height + 2 * SystemParameters.BorderWidth;
					window.Content = newChart;

					newChart.DataColumns = null;

					newChart.DataView = table.DefaultView;
					newChart.DataColumns = chart.TableColumns.Split(',', ';');
					newChart.UpdateChart();

					RenderTargetBitmap bitmap = new RenderTargetBitmap((int)((window.Content as FrameworkElement).RenderSize.Width * 600d / 96d), (int)((window.Content as FrameworkElement).RenderSize.Height * 600d / 96d), 600d, 600d, PixelFormats.Pbgra32);
					bitmap.Render(window);
					chartCanvas.Children.Add(new Image() { Source = bitmap });
				}
				else
				{
					chart.DataColumns = null;

					chart.DataView = table.DefaultView;
					chart.DataColumns = chart.TableColumns.Split(',', ';');
					chart.UpdateChart();
				}
			}

			if (window != null)
				window.Close();
		}

		protected virtual void FillTableRow(TableRow row, DataRow dataRow)
		{
			DocumentWalker localWalker = new DocumentWalker();
			foreach (TableCell cell in row.Cells)
			{
				List<ITableCellValue> newCells = localWalker.TraverseBlockCollection<ITableCellValue>(cell.Blocks);
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
					IAggregateValue av = cv as IAggregateValue;
					try
					{
						object obj = dv == null ? dataRow[iv.Index] : dataRow[dv.PropertyName];
						if (obj == DBNull.Value)
							obj = null;
						cv.Value = obj;

						if (av != null)
							RememberAggregateValue(_aggregateValues, av.AggregateGroup, obj);
					}
					catch
					{
						if (_data.ShowUnknownValues)
							cv.Value = "[" + (dv == null ? iv.Index.ToString() : dv.PropertyName) + "]";
						else
							cv.Value = "";
						if (av != null)
							RememberAggregateValue(_aggregateValues, av.AggregateGroup, null);
					}
				}
			}
		}

		protected virtual void HandleDataGroups()
		{
			var groups = _dynamicCache.GetFlowDocumentVisualListByType(typeof(SectionDataGroup));
			foreach (SectionDataGroup group in groups)
				if (!string.IsNullOrEmpty(group.DataGroupName) && !_data.Groups.Contains(group.DataGroupName))
				{
					// remove DataGroup section from FlowDocument
					DependencyObject parent = group.Parent;
					if (parent is FlowDocument) { ((FlowDocument)parent).Blocks.Remove(group); parent = null; }
					if (parent is Section) { ((Section)parent).Blocks.Remove(group); parent = null; }
				}
		}
	}
}

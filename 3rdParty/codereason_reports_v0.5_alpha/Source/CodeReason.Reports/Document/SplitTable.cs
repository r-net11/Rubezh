using System.Windows.Documents;
using CodeReason.Reports.Interfaces;
using System.Windows;
using System.Collections.Generic;

namespace CodeReason.Reports.Document
{
	public class SplitTable : Section, ISplitTable
	{
		public bool PageBreakBefore { get; set; }
		public bool PageBreakAfter { get; set; }
		public int RowsOnPage { get; set; }
		public int RowsOnFirstPage { get; set; }
		public int RowsOnLatPage { get; set; }
		public string TableName { get; set; }
		public bool TableHeaderOnEachPage { get; set; }
		public bool TableFooterOnEachPage { get; set; }

		public Section Header { get; set; }
		public TableRowGroup ContentRowGroup { get; set; }
		public TableRowGroup HeaderRowGroup { get; set; }
		public TableRowGroup FooterRowGroup { get; set; }
		public Section Footer { get; set; }
		public List<TableColumn> Columns { get; set; }

		public SplitTable()
		{
			Columns = new List<TableColumn>();
		}
	}
}

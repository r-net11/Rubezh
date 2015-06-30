using System.Windows;
using System.Windows.Documents;
using System.Collections.Generic;

namespace CodeReason.Reports.Interfaces
{
	public interface ISplitTable
	{
		bool PageBreakBefore { get; set; }
		bool PageBreakAfter { get; set; }
		int RowsOnPage { get; set; }
		int RowsOnFirstPage { get; set; }
		int RowsOnLatPage { get; set; }
		string TableName { get; set; }
		bool TableHeaderOnEachPage { get; set; }
		bool TableFooterOnEachPage { get; set; }


		Section Header { get; set; }
		TableRowGroup ContentRowGroup { get; set; }
		TableRowGroup HeaderRowGroup { get; set; }
		TableRowGroup FooterRowGroup { get; set; }
		Section Footer { get; set; }
		Thickness BorderThickness { get; set; }
		List<TableColumn> Columns { get; set; }
	}
}

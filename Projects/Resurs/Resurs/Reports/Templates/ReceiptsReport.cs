using DevExpress.XtraReports.UI;
using Resurs.Reports.DataSources;
using System;

namespace Resurs.Reports.Templates
{
	public partial class ReceiptsReport : XtraReport
	{
		public ReceiptsReport()
		{
			InitializeComponent();
			var dataSet = new ReceiptsDataSet();
			for (int i = 0; i < 10; i++)
			{
				var dataRow = dataSet.Data.NewDataRow();
				dataRow.DateTime = DateTime.Now;
				dataRow.Number = i;
				dataRow.Price = i * 14;
				dataRow.Paid = i * 14;
				dataRow.Condition = "оплачена";
				dataRow.AbonentName = "Петорв ПетрПетрович";
				dataSet.Data.Rows.Add(dataRow);
			}
			DataSource = dataSet;
		}
	}
}
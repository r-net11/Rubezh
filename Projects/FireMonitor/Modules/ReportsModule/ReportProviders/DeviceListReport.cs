using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using CodeReason.Reports;
using System.Data;

namespace ReportsModule.ReportProviders
{
	internal class DeviceListReport : BaseReport
	{
		public override ReportData GetData()
		{
			DataTable tableHeader = null;
			DataTable tableData = null;
			object[] obj = null;
			var data = new ReportData();

			// REPORT 1 DATA
			tableHeader = new DataTable("Header");
			tableData = new DataTable("Data");


			tableHeader.Columns.Add();
			tableHeader.Rows.Add(new object[] { "Service" });
			tableHeader.Rows.Add(new object[] { "Amount" });
			tableHeader.Rows.Add(new object[] { "Price" });
			tableData.Columns.Add();
			tableData.Columns.Add();
			tableData.Columns.Add();
			obj = new object[3];
			for (int i = 0; i < 15; i++)
			{
				obj[0] = String.Format("Service oferted. Nº{0}", i);
				obj[1] = i * 2;
				obj[2] = String.Format("{0} €", i);
				tableData.Rows.Add(obj);
			}

			data.DataTables.Add(tableData);
			data.DataTables.Add(tableHeader);


			// REPORT 2 DATA
			tableHeader = new DataTable("Header2");
			tableData = new DataTable("Data2");

			// sample table "Ean"
			tableHeader.Columns.Add();
			tableHeader.Rows.Add(new object[] { "Position" });
			tableHeader.Rows.Add(new object[] { "Item" });
			tableHeader.Rows.Add(new object[] { "EAN" });
			tableHeader.Rows.Add(new object[] { "Count" });
			tableData.Columns.Add("Position", typeof(string));
			tableData.Columns.Add("Item", typeof(string));
			tableData.Columns.Add("EAN", typeof(string));
			tableData.Columns.Add("Count", typeof(int));
			Random rnd = new Random(1234);
			for (int i = 1; i <= 10000; i++)
			{
				// randomly create some articles
				tableData.Rows.Add(new object[] { i, "Item " + i.ToString("0000"), "123456790123", rnd.Next(9) + 1 });
			}

			data.DataTables.Add(tableData);
			data.DataTables.Add(tableHeader);
			return data;
		}
	}
}

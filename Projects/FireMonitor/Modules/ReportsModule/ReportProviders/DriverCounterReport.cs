using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using CodeReason.Reports;
using System.Data;
using FiresecAPI.Models;
using System.Windows;
using FiresecClient;
using FiresecAPI;

namespace ReportsModule.ReportProviders
{
	internal class DriverCounterReport : BaseReport
	{
		public DriverCounterReport()
			: base(ReportType.ReportDriverCounter)
		{
		}

		public override ReportData GetData()
		{
			var data = new ReportData();
			data.ReportDocumentValues.Add("PrintDate", DateTime.Now);

			DataTable table = new DataTable("Devices");
			table.Columns.Add("Driver");
			table.Columns.Add("Count");
			//DataList = new List<ReportDriverCounterModel>();
			foreach (var driver in FiresecManager.Drivers)
				if (driver.IsPlaceable && driver.DriverType != DriverType.Computer)
					AddDrivers(driver, table);
				else
					switch (driver.DriverType)
					{
						case DriverType.IndicationBlock:
							AddDrivers(driver, table);
							break;
						case DriverType.MS_1:
							AddDrivers(driver, table);
							break;
						case DriverType.MS_2:
							AddDrivers(driver, table);
							break;
						case DriverType.MS_3:
							AddDrivers(driver, table);
							break;
						case DriverType.MS_4:
							AddDrivers(driver, table);
							break;
						default:
							break;
					}
			//DataList.Add(new ReportDriverCounterModel()
			//{
			//    DriverName = "Всего устройств",
			//    Count = CountDrivers()
			//});

			data.DataTables.Add(table);
			return data;
		}

		public override bool IsFilterable
		{
			get { return false; }
		}

		private void AddDrivers(Driver driver, DataTable table)
		{
			var devices = FiresecManager.Devices.FindAll(x => x.Driver.UID == driver.UID);
			if (devices.IsNotNullOrEmpty())
				table.Rows.Add(driver.ShortName, devices.Count);
			//{
			//    DataList.Add(new ReportDriverCounterModel()
			//    {
			//        DriverName = driver.ShortName,
			//        Count = devices.Count
			//    });
			//}
		}
		private int CountDrivers(DataTable table)
		{
			int count = 0;
			//DataList.ForEach(x => count += x.Count);
			return count;
		}
	}
}

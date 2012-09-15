using System.Data;
using CodeReason.Reports;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Reports;

namespace DevicesModule.Reports
{
	internal class DriverCounterReport : ISingleReportProvider
	{

		#region ISingleReportProvider Members

		public ReportData GetData()
		{
			var data = new ReportData();

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

		#endregion

		#region IReportProvider Members

		public string Template
		{
			get { return "Reports/DriverCounterReport.xaml"; }
		}

		public string Title
		{
			get { return "Количество устройств по типам"; }
		}

		public bool IsEnabled
		{
			get { return true; }
		}

		#endregion

		private void AddDrivers(Driver driver, DataTable table)
		{
			var devices = FiresecManager.Devices.FindAll(x => x.Driver.UID == driver.UID);
			if (devices.IsNotNullOrEmpty())
				table.Rows.Add(driver.ShortName, devices.Count);
		}
	}
}

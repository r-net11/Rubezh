using System.Data;
using CodeReason.Reports;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure.Common.Reports;
using RubezhAPI;

namespace GKModule.Reports
{
	internal class DriverCounterReport : ISingleReportProvider
	{
		public DriverCounterReport()
		{
			PdfProvider = new DriverCounterReportPdf();
		}

		#region ISingleReportProvider Members
		public ReportData GetData()
		{
			var data = new ReportData();

			var table = new DataTable("Devices");
			table.Columns.Add("Driver", typeof(string));
			table.Columns.Add("Count", typeof(int));
			foreach (var driver in  DriversRepository.Drivers)
			{
				if (driver.IsAutoCreate || driver.DriverType == GKDriverType.System)
					continue;
				AddDrivers(driver, table);
			}
			data.DataTables.Add(table);
			PdfProvider.ReportData = data;
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

		public IReportPdfProvider PdfProvider { get; private set; }
		#endregion

		private void AddDrivers(GKDriver driver, DataTable table)
		{
			var count = 0;
			foreach (var device in GKManager.Devices)
			{
				if (device.DriverType == driver.DriverType)
				{
					if (device.Parent.Driver.IsGroupDevice)
						continue;
					count++;
				}
			}
			if (count > 0)
				table.Rows.Add(driver.ShortName, count);
		}
	}
}
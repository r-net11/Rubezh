﻿using System.Data;
using CodeReason.Reports;
using FiresecClient;
using Infrastructure.Common.Reports;
using XFiresecAPI;

namespace GKModule.Reports
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
			foreach (var driver in XManager.DriversConfiguration.XDrivers)
			{
				if (driver.IsAutoCreate || driver.DriverType == XDriverType.System)
					continue;
				AddDrivers(driver, table);
			}
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

		private void AddDrivers(XDriver driver, DataTable table)
		{
			var count = 0;
			foreach (var device in XManager.DeviceConfiguration.Devices)
			{
				if (device.Driver.DriverType == driver.DriverType)
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
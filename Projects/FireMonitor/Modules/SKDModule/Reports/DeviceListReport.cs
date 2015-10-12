﻿using System.Data;
using CodeReason.Reports;
using FiresecAPI;
using FiresecAPI.GK;
using Infrastructure.Common.Reports;
using FiresecAPI.SKD;
using RubezhClient;

namespace SKDModule.Reports
{
	internal class DeviceListReport : ISingleReportProvider
	{
		public DeviceListReport()
		{
			PdfProvider = new DeviceListReportPdf();
		}

		#region ISingleReportProvider Members
		public ReportData GetData()
		{
			var data = new ReportData();
			var table = new DataTable("Devices");
			table.Columns.Add("Name");
			table.Columns.Add("Address");
			table.Columns.Add("DoorType");

			foreach (var device in GKManager.Devices)
			{
				if (device.DriverType == GKDriverType.RSR2_CardReader || device.DriverType == GKDriverType.RSR2_CodeReader)
				{
					var name = device.Name;
					var address = device.Address;
					var doorType = "";
					table.Rows.Add(name, address, doorType);
				}
			}
			data.DataTables.Add(table);
			PdfProvider.ReportData = data;
			return data;
		}
		#endregion

		#region IReportProvider Members
		public string Template
		{
			get { return "Reports/DeviceListReport.xaml"; }
		}

		public string Title
		{
			get { return "Список устройств СКД"; }
		}

		public bool IsEnabled
		{
			get { return true; }
		}

		public IReportPdfProvider PdfProvider { get; private set; }

		#endregion
	}
}
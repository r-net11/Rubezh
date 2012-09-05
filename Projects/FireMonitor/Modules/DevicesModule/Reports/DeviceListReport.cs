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
using Infrastructure.Common.Reports;

namespace DevicesModule.Reports
{
	internal class DeviceListReport : ISingleReportProvider
	{
		#region ISingleReportProvider Members

		public ReportData GetData()
		{
			var data = new ReportData();

			DataTable table = new DataTable("Devices");
			table.Columns.Add("Type");
			table.Columns.Add("Address");
			table.Columns.Add("Zone");

			if (FiresecManager.Devices.IsNotNullOrEmpty())
			{
				string type = "";
				string address = "";
				string zonePresentationName = "";
				foreach (var device in FiresecManager.Devices)
				{
					zonePresentationName = "";
					type = device.Driver.ShortName;
					address = device.DottedAddress;
					if (device.Driver.IsZoneDevice)
					{
						if (FiresecManager.Zones.IsNotNullOrEmpty())
						{
							var zone = FiresecManager.Zones.FirstOrDefault(x => x.No == device.ZoneNo);
							if (zone != null)
								zonePresentationName = zone.PresentationName;
						}
					}

					if (device.Driver.DriverType == DriverType.Indicator)
					{
						if (device.IndicatorLogic.Zones.Count == 1)
						{
							var zone = FiresecManager.Zones.FirstOrDefault(x => x.No == device.IndicatorLogic.Zones[0]);
							zonePresentationName = "Зоны: " + zone == null ? zone.PresentationName : "";
						}
						else
						{
							zonePresentationName = device.IndicatorLogic.ToString();
						}
					}

					if (device.Driver.DriverType == DriverType.Page)
						address = device.IntAddress.ToString();
					if (device.Driver.DriverType == DriverType.PumpStation)
					{
					}
					table.Rows.Add(type, address, zonePresentationName);
				}
			}
			data.DataTables.Add(table);
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
			get { return "Список устройств"; }
		}

		public bool IsEnabled
		{
			get { return true; }
		}

		#endregion
	}
}

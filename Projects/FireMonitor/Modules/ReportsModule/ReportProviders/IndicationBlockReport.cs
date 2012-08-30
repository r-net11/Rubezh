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
	internal class IndicationBlockReport : BaseReport
	{
		public IndicationBlockReport()
			: base(ReportType.ReportIndicationBlock)
		{
		}

		public override ReportData GetData()
		{
			var data = new ReportData();
			data.ReportDocumentValues.Add("PrintDate", DateTime.Now);

			DataTable table = new DataTable("Devices");
			table.Columns.Add("Type");
			table.Columns.Add("Address");
			table.Columns.Add("Zone");

			//DataList = new List<ReportDeviceListModel>();
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
					//DataList.Add(new ReportDeviceListModel()
					//{
					//    Type = type,
					//    Address = address,
					//    ZoneName = zonePresentationName
					//});
					table.Rows.Add(type, address, zonePresentationName);
				}
			}
			data.DataTables.Add(table);
			return data;
		}

		public override bool IsFilterable
		{
			get { return false; }
		}
	}
}

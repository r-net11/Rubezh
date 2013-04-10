using System.Data;
using System.Linq;
using CodeReason.Reports;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Reports;

namespace DevicesModule.Reports
{
	internal class DeviceParamsReport : ISingleReportProvider
	{
		#region ISingleReportProvider Members

		public ReportData GetData()
		{
			var data = new ReportData();

			DataTable table = new DataTable("Devices");
			table.Columns.Add("Type");
			table.Columns.Add("Address");
			table.Columns.Add("Zone");
			table.Columns.Add("Dustiness");
			table.Columns.Add("FailureType");

			//DataList = new List<ReportDeviceParamsModel>();
			if (FiresecManager.Devices.IsNotNullOrEmpty())
			{
				string type = "";
				string address = "";
				string zonePresentationName = "";
				string dustiness = "";
				string failureType = "";
				foreach (var device in FiresecManager.Devices)
				{
					if (device.Driver.Category == DeviceCategoryType.Other || device.Driver.Category == DeviceCategoryType.Communication)
						continue;

					type = device.Driver.ShortName;
					address = device.DottedAddress;
					zonePresentationName = "";
					dustiness = "";
					failureType = "";

					if (device.Driver.IsZoneDevice)
					{
						if (FiresecManager.Zones.IsNotNullOrEmpty())
						{
							var zone = FiresecManager.Zones.FirstOrDefault(x => x.UID == device.ZoneUID);
							if (zone != null)
								zonePresentationName = zone.PresentationName;
						}
					}

					var deviceState = device.DeviceState;
					var parameter = deviceState.ThreadSafeParameters.FirstOrDefault(x => (x.Name == "Dustiness" && x.Visible));
					if (parameter != null)
					{
						if (!parameter.IsIgnore)
							dustiness = parameter.Value;
					}
					parameter = deviceState.ThreadSafeParameters.FirstOrDefault(x => (x.Name == "FailureType" && x.Visible));
					if (parameter != null)
					{
						if (!parameter.IsIgnore)
							failureType = parameter.Value;
					}
					//DataList.Add(new ReportDeviceParamsModel()
					//{
					//    Type = type,
					//    Address = address,
					//    Zone = zonePresentationName,
					//    Dustiness = dustiness,
					//    FailureType = failureType
					//});
					table.Rows.Add(type, address, zonePresentationName, dustiness, failureType);
				}
			}
			data.DataTables.Add(table);
			return data;
		}

		#endregion

		#region IReportProvider Members

		public string Template
		{
			get { return "Reports/DeviceParamsReport.xaml"; }
		}

		public string Title
		{
			get { return "Параметры устройств"; }
		}

		public bool IsEnabled
		{
			get { return true; }
		}

		#endregion
	}
}
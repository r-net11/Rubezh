using System.Data;
using CodeReason.Reports;
using FiresecAPI;
using FiresecClient;
using Infrastructure.Common.Windows.Reports;

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
					zonePresentationName = FiresecManager.FiresecConfiguration.GetPresentationZone(device);
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
			get { return "Список устройств RSR1"; }
		}

		public bool IsEnabled
		{
			get { return true; }
		}

		public IReportPdfProvider PdfProvider
		{
			get { return null; }
		}

		#endregion
	}
}

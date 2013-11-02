using System.Data;
using CodeReason.Reports;
using Common.PDF;
using FiresecAPI;
using FiresecClient;
using Infrastructure.Common.Reports;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace GKModule.Reports
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
			table.Columns.Add("Type");
			table.Columns.Add("Address");
			table.Columns.Add("Zone");

			if (FiresecManager.Devices.IsNotNullOrEmpty())
			{
				foreach (var device in XManager.Devices)
				{
					if (device.DriverType == XFiresecAPI.XDriverType.System)
						continue;
					if (device.Driver.IsGroupDevice)
						continue;

					var type = device.ShortName;
					var address = device.DottedPresentationAddress;
					var zonePresentationName = XManager.GetPresentationZone(device);
					table.Rows.Add(type, address, zonePresentationName);
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
			get { return "Список устройств"; }
		}

		public bool IsEnabled
		{
			get { return true; }
		}

		public IReportPdfProvider PdfProvider { get; private set; }

		#endregion
	}
}
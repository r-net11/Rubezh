using System.Data;
using System.Linq;
using CodeReason.Reports;
using RubezhAPI;
using RubezhClient;
using Infrastructure.Common.Windows.Reports;

namespace GKModule.Reports
{
	internal class DeviceParametersReport : ISingleReportProvider
	{
		public DeviceParametersReport()
		{
			PdfProvider = new DeviceParametersReportPdf();
		}

		#region ISingleReportProvider Members
		public ReportData GetData()
		{
			var data = new ReportData();

			var table = new DataTable("Devices");
			table.Columns.Add("Type");
			table.Columns.Add("Address");
			table.Columns.Add("Zone");
			table.Columns.Add("Dustiness");

			if (GKManager.Devices.IsNotNullOrEmpty())
			{
				string type = "";
				string address = "";
				string zonePresentationName = "";
				string dustiness = "";
				foreach (var device in GKManager.Devices)
				{
					type = device.Driver.ShortName;
					address = device.DottedAddress;
					zonePresentationName = "";
					dustiness = "";

					if (device.Driver.HasZone)
					{
						zonePresentationName = GKManager.GetPresentationZoneOrLogic(device);
					}

					var deviceState = device.State;
					var parameter = deviceState.XMeasureParameterValues.FirstOrDefault(x => x.Name == "Dustiness");
					if (parameter != null)
					{
						dustiness = parameter.StringValue;
					}
					table.Rows.Add(type, address, zonePresentationName, dustiness);
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
			get { return "Reports/DeviceParametersReport.xaml"; }
		}

		public string Title
		{
			get { return "Параметры устройств"; }
		}

		public bool IsEnabled
		{
			get { return true; }
		}

		public IReportPdfProvider PdfProvider { get; private set; }

		#endregion
	}
}
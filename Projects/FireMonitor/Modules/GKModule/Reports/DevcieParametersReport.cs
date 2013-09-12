using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Reports;
using CodeReason.Reports;
using System.Data;
using FiresecClient;
using FiresecAPI;
using iTextSharp.text.pdf;
using Common.PDF;

namespace GKModule.Reports
{
	internal class DeviceParametersReport : ISingleReportProvider
	{
		private DataTable _table;

		#region ISingleReportProvider Members
		public ReportData GetData()
		{
			var data = new ReportData();

			_table = new DataTable("Devices");
			_table.Columns.Add("Type");
			_table.Columns.Add("Address");
			_table.Columns.Add("Zone");
			_table.Columns.Add("Dustiness");

			if (XManager.Devices.IsNotNullOrEmpty())
			{
				string type = "";
				string address = "";
				string zonePresentationName = "";
				string dustiness = "";
				foreach (var device in XManager.Devices)
				{
					type = device.Driver.ShortName;
					address = device.DottedAddress;
					zonePresentationName = "";
					dustiness = "";

					if (device.Driver.HasZone)
					{
						zonePresentationName = XManager.GetPresentationZone(device); ;
					}

					var deviceState = device.DeviceState;
					var parameter = deviceState.MeasureParameter.Dustiness;
					if (parameter != null)
					{
						dustiness = parameter;
					}
					_table.Rows.Add(type, address, zonePresentationName, dustiness);
				}
			}
			data.DataTables.Add(_table);
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

		public bool CanPdfPrint
		{
			get { return true; }
		}
		public void PdfPrint(iTextSharp.text.Document document)
		{
			var table = new PdfPTable(_table.Columns.Count);
			PDFHelper.PrintTable(table, _table);
			document.Add(table);
		}
		
		#endregion
	}
}
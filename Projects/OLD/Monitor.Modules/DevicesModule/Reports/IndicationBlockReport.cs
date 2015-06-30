using System.Collections.Generic;
using System.Data;
using System.Linq;
using CodeReason.Reports;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Reports;

namespace DevicesModule.Reports
{
	internal class IndicationBlockReport : IMultiReportProvider
	{
		#region IMultiReportProvider Members

		public IEnumerable<ReportData> GetData()
		{
			var fullData = new List<ReportData>();

			var IndicationBlockList = new List<IndicationBlocksList>();
			if (FiresecManager.Devices.IsNotNullOrEmpty())
			{
				var devices = FiresecManager.Devices.FindAll(x => (x.Driver.DriverType == DriverType.IndicationBlock));
				foreach (var device in devices)
					IndicationBlockList.Add(new IndicationBlocksList(device));
			}

			foreach (var block in IndicationBlockList)
				foreach (var page in block.Pages)
				{
					var data = new ReportData();
					data.ReportDocumentValues.Add("IndicationBlockNumber", block.IndicationBlockNumber);
					data.ReportDocumentValues.Add("PageNumber", page.PageNumber.ToString());

					DataTable table = new DataTable("Records");
					table.Columns.Add("NumberFirstColumn");
					table.Columns.Add("PresentationNameFirstColumn");
					table.Columns.Add("NumberSecondColumn");
					table.Columns.Add("PresentationNameSecondColumn");
					table.Columns.Add("NumberThirdColumn");
					table.Columns.Add("PresentationNameThirdColumn");
					table.Columns.Add("NumberFourthColumn");
					table.Columns.Add("PresentationNameFourthColumn");
					table.Columns.Add("NumberFifthColumn");
					table.Columns.Add("PresentationNameFifthColumn");
					for (int i = 0; i < 10; i++)
					{
						table.Rows.Add(
							page.ElementsPage[i].No.ToString(),
							page.ElementsPage[i].PresentationName.ToString(),
							page.ElementsPage[i + 10].No.ToString(),
							page.ElementsPage[i + 10].PresentationName.ToString(),
							page.ElementsPage[i + 20].No.ToString(),
							page.ElementsPage[i + 20].PresentationName.ToString(),
							page.ElementsPage[i + 30].No.ToString(),
							page.ElementsPage[i + 30].PresentationName.ToString(),
							page.ElementsPage[i + 40].No.ToString(),
							page.ElementsPage[i + 40].PresentationName.ToString());
					}
					data.DataTables.Add(table);
					fullData.Add(data);
				}
			return fullData;
		}

		#endregion

		#region IReportProvider Members

		public string Template
		{
			get { return "Reports/IndicationBlockReport.xaml"; }
		}

		public string Title
		{
			get { return "Блоки индикации RSR1"; }
		}

		public bool IsEnabled
		{
			get { return FiresecManager.Devices.Any(x => (x.Driver.DriverType == DriverType.IndicationBlock)); }
		}

		public IReportPdfProvider PdfProvider
		{
			get { return null; }
		}

		#endregion
	}
}

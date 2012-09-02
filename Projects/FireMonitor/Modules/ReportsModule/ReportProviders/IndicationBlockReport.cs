using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CodeReason.Reports;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using System.Windows.Documents;
using ReportsModule.Models;

namespace ReportsModule.ReportProviders
{
	internal class IndicationBlockReport : BaseReport
	{
		public IndicationBlockReport()
			: base(ReportType.ReportIndicationBlock)
		{
		}

		public override bool IsMultiReport
		{
			get { return true; }
		}

		public override IEnumerable<ReportData> GetMultiData()
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
					data.ReportDocumentValues.Add("PrintDate", DateTime.Now);
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

		public override bool IsFilterable
		{
			get { return false; }
		}
		public override bool IsEnabled
		{
			get { return FiresecManager.Devices.IsNotNullOrEmpty(); }
		}
	}
}

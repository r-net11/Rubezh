using System.Collections.Generic;
using System.Text;
using FiresecAPI.Models;
using FiresecClient;
using ReportsModule2.Models;
using Microsoft.Reporting.WinForms;
using System;

namespace ReportsModule2.Reports
{
	public class ReportIndicationBlock : BaseReportGeneric<ReportIndicationBlockModel>
	{
		public ReportIndicationBlock()
		{
			ReportFileName = "IndicationBlockRdlc.rdlc";
			DataSourceFileName = "IndicationBlockData";
			BlockIndicationNumbers = new List<string>();
			PageNumbers = new List<string>();
		}

		public List<string> PageNumbers { get; set; }
		public List<string> BlockIndicationNumbers { get; set; }

		public override void LoadData()
		{
			var IndicationBlockList = new List<IndicationBlocksList>();
			if (FiresecManager.Devices.IsNotNullOrEmpty())
			{
				var devices = FiresecManager.Devices.FindAll(x => (x.Driver.DriverType == DriverType.IndicationBlock));
				foreach (var device in devices)
				{
					IndicationBlockList.Add(new IndicationBlocksList(device));
				}
			}

			foreach (var block in IndicationBlockList)
			{
				foreach (var page in block.Pages)
				{
					BlockIndicationNumbers.Add(block.IndicationBlockNumber);
					PageNumbers.Add(page.PageNumber.ToString());
					for (int i = 0; i < 10; i++)
					{
						DataList.Add(new ReportIndicationBlockModel()
						{
							NumberFirstColumn = page.ElementsPage[i].No.ToString(),
							NumberSecondColumn = page.ElementsPage[i + 10].No.ToString(),
							NumberThirdColumn = page.ElementsPage[i + 20].No.ToString(),
							NumberFourthColumn = page.ElementsPage[i + 30].No.ToString(),
							NumberFifthColumn = page.ElementsPage[i + 40].No.ToString(),
							PresentationNameFirstColumn = page.ElementsPage[i].PresentationName.ToString(),
							PresentationNameSecondColumn = page.ElementsPage[i + 10].PresentationName.ToString(),
							PresentationNameThirdColumn = page.ElementsPage[i + 20].PresentationName.ToString(),
							PresentationNameFourthColumn = page.ElementsPage[i + 30].PresentationName.ToString(),
							PresentationNameFifthColumn = page.ElementsPage[i + 40].PresentationName.ToString(),
						});
					}
				}
			}
		}

		public override void LoadReportViewer(ReportViewer reportViewer)
		{
			base.LoadReportViewer(reportViewer);
			var pageNumbersParameter = new ReportParameter("PageNumbers", PageNumbers.ToArray());
			var blockIndicationNumbersParameter = new ReportParameter("BlockIndicationNumbers", BlockIndicationNumbers.ToArray());
			reportViewer.LocalReport.SetParameters(new ReportParameter[] { pageNumbersParameter, blockIndicationNumbersParameter });
		}
	}
}
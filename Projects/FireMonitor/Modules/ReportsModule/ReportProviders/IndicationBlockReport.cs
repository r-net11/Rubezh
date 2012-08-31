using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CodeReason.Reports;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;

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

		public override IEnumerable<ReportData>  GetMultiData()
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
	}


	public class IndicationBlocksList
	{
		protected IndicationBlocksList() { }

		public IndicationBlocksList(Device device)
		{
			if (device.Driver.DriverType != DriverType.IndicationBlock)
				return;

			IndicationBlockNumber = device.DottedAddress;
			Pages = new List<Page>(
				device.Children.Select(x => new Page(x))
			);
		}

		public string IndicationBlockNumber { get; set; }
		public List<Page> Pages { get; set; }
	}

	public class Page
	{
		public Page(Device device)
		{
			PageNumber = device.IntAddress;
			ElementsPage = new List<ElementPage>();
			foreach (var elementPage in device.Children)
			{
				ElementsPage.Add(new ElementPage(
					elementPage.IntAddress,
					elementPage.IndicatorLogic.Zones,
					elementPage.IndicatorLogic.ToString()));
			}
		}

		public int PageNumber { get; set; }
		public List<ElementPage> ElementsPage { get; set; }
	}

	public class ElementPage
	{
		private ElementPage() { }

		public ElementPage(int number, List<int> zonesNo, string presentationName)
		{
			No = number;
			ZonesNo = zonesNo;
			PresentationName = presentationName;
		}

		public int No { get; set; }
		public List<int> ZonesNo { get; set; }

		string _presentationName;
		public string PresentationName
		{
			get
			{
				if (ZonesNo.Count == 1)
				{
					var zone = FiresecManager.Zones.FirstOrDefault(x => x.No == ZonesNo[0]);
					string presentationName = "";
					if (zone != null)
						presentationName = zone.PresentationName;
					return ("Зоны: " + presentationName);
				}
				else
				{
					return _presentationName;
				}
			}
			set { _presentationName = value; }
		}
	}
}

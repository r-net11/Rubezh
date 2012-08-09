using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;

namespace ReportsModule2.Models
{
	public class ReportIndicationBlockModel
	{
        public string NumberFirstColumn { get; set; }
		public string NumberSecondColumn { get; set; }
		public string NumberThirdColumn { get; set; }
		public string NumberFourthColumn { get; set; }
		public string NumberFifthColumn { get; set; }
        public string PresentationNameFirstColumn { get; set; }
		public string PresentationNameSecondColumn { get; set; }
		public string PresentationNameThirdColumn { get; set; }
		public string PresentationNameFourthColumn { get; set; }
		public string PresentationNameFifthColumn { get; set; }
    }

	public class ReportIndicationBlockOldModel
	{
		public string Number { get; set; }
		public string PresentationName { get; set; }
		public string BlockIndicationNumber { get; set; }
		public string PageNumber { get; set; }
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
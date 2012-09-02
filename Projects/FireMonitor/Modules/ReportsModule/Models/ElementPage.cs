using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecClient;

namespace ReportsModule.Models
{
	internal class ElementPage
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace DevicesModule.Reports
{
	internal class Page
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

}

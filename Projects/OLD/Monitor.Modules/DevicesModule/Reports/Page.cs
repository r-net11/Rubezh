﻿using System.Collections.Generic;
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
				ElementsPage.Add(new ElementPage(elementPage.IntAddress, elementPage));
			}
		}

		public int PageNumber { get; set; }
		public List<ElementPage> ElementsPage { get; set; }
	}
}
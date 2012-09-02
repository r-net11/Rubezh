using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace ReportsModule.Models
{
	internal class IndicationBlocksList
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

}

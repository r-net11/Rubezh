using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecClient;
using FiresecAPI.Models;

namespace DevicesModule.Reports
{
	internal class ElementPage
	{
		private ElementPage() { }

		public ElementPage(int number, Device device)
		{
			No = number;
			_device = device;
		}

		Device _device;
		public int No { get; set; }

		public string PresentationName
		{
			get
			{
				return FiresecManager.FiresecConfiguration.GetIndicatorString(_device);
			}
		}
	}
}
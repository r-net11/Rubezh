using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChinaSKDDriverAPI
{
	public class SKDJournalItem
	{
		public SKDJournalItem()
		{

		}

		public DateTime SystemDateTime { get; set;}
		public DateTime DeviceDateTime { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
	}
}
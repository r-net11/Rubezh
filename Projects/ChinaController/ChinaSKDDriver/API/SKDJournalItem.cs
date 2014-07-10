using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Journal;

namespace ChinaSKDDriverAPI
{
	public class SKDJournalItem
	{
		public SKDJournalItem()
		{

		}

		public DateTime SystemDateTime { get; set;}
		public DateTime DeviceDateTime { get; set; }
		public JournalEventNameType JournalEventNameType { get; set; }
		public string Description { get; set; }
	}
}
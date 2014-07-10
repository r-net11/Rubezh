using System;
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
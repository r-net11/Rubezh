using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class SKDSystemConfiguration
	{
		public SKDSystemConfiguration()
		{
			JournalFilters = new List<SKDJournalFilter>();
		}

		[DataMember]
		public List<SKDJournalFilter> JournalFilters { get; set; }
	}
}
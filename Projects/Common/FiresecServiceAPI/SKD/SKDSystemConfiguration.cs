using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
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
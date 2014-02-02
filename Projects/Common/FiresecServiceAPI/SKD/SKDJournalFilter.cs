using System.Collections.Generic;
using System.Runtime.Serialization;
using System;

namespace FiresecAPI
{
	[DataContract]
	public class SKDJournalFilter
	{
		public SKDJournalFilter()
		{
			Devices = new List<Guid>();
            EventNames = new List<string>();
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public List<Guid> Devices { get; set; }

		[DataMember]
        public List<string> EventNames { get; set; }
	}
}
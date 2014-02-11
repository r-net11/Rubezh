using System;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class Day
	{
		public Day()
		{
			Uid = Guid.NewGuid();
		}

		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public NamedInterval NamedInterval { get; set; }

		[DataMember]
		public int? Number { get; set; }
	}
}
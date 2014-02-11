using System;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class Phone
	{
		public Phone()
		{
			Uid = Guid.NewGuid();
		}

		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string NumberString { get; set; }
	}
}
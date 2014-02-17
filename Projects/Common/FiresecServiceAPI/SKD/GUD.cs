using System;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class GUD
	{
		public GUD()
		{
			UID = Guid.NewGuid();
		}

		[DataMember]
		public Guid UID;

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }
	}
}
using System;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class ShortPosition
	{
		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public Guid? OrganisationUID { get; set; }
	}
}

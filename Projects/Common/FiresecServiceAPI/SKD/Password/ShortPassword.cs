using System.Runtime.Serialization;
using System;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class ShortPassword
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
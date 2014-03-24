using System;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class CardZoneLink : SKDIsDeletedModel
	{
		[DataMember]
		public Guid? CardUid { get; set; }

		[DataMember]
		public Guid? ZoneUid { get; set; }

		[DataMember]
		public bool? IsWithEscort { get; set; }

		[DataMember]
		public IntervalType IntervalType { get; set; }

		[DataMember]
		public Guid? IntervalUid { get; set; }
	}
}
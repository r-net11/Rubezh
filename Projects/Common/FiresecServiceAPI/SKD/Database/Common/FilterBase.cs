using System.Runtime.Serialization;
using System;
using System.Collections.Generic;

namespace FiresecAPI
{
	[DataContract]
	public abstract class FilterBase
	{
		[DataMember]
		public DeletedType WithDeleted { get; set; }

		[DataMember]
		public List<Guid> Uids { get; set; }

		[DataMember]
		public DateTimePeriod RemovalDates { get; set; }

		public FilterBase()
		{
			Uids = new List<Guid>();
			RemovalDates = new DateTimePeriod();
			WithDeleted = DeletedType.Not;
		}
	}

	[DataContract]
	public enum DeletedType
	{
		[EnumMember]
		Deleted,

		[EnumMember]
		Not,

		[EnumMember]
		All
	}
}
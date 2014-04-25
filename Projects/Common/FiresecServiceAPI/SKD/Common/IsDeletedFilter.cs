using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public abstract class IsDeletedFilter : FilterBase
	{
		[DataMember]
		public DeletedType WithDeleted { get; set; }

		[DataMember]
		public DateTimePeriod RemovalDates { get; set; }

		public IsDeletedFilter()
			: base()
		{
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
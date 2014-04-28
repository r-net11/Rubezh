using System.Runtime.Serialization;
using System.ComponentModel;

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
		[Description("Только неудаленные")]
		Not,

		[EnumMember]
		[Description("Только удаленный")]
		Deleted,

		[EnumMember]
		[Description("Все")]
		All
	}
}
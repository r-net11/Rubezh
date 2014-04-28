using System.Runtime.Serialization;
using System.ComponentModel;

namespace FiresecAPI
{
	[DataContract]
	public abstract class IsDeletedFilter : FilterBase
	{
		[DataMember]
		public LogicalDeletationType WithDeleted { get; set; }

		[DataMember]
		public DateTimePeriod RemovalDates { get; set; }

		public IsDeletedFilter()
			: base()
		{
			RemovalDates = new DateTimePeriod();
			WithDeleted = LogicalDeletationType.Active;
		}
	}

	[DataContract]
	public enum LogicalDeletationType
	{
		[EnumMember]
		[Description("Только активные")]
		Active,

		[EnumMember]
		[Description("Только удаленные")]
		Deleted,

		[EnumMember]
		[Description("Все")]
		All
	}
}
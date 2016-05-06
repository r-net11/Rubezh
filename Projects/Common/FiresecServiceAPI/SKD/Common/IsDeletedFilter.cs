using System.ComponentModel;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
{
	[DataContract]
	public abstract class IsDeletedFilter : FilterBase
	{
		[DataMember]
		public LogicalDeletationType LogicalDeletationType { get; set; }

		[DataMember]
		public DateTimePeriod RemovalDates { get; set; }

		public IsDeletedFilter()
			: base()
		{
			RemovalDates = new DateTimePeriod();
			LogicalDeletationType = LogicalDeletationType.Active;
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
using System.ComponentModel;
using System.Runtime.Serialization;
using Localization;

namespace FiresecAPI.SKD
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
		//[Description("Только активные")]
        [LocalizedDescription(typeof(Resources.Language.SKD.Common.IsDeletedFilter), "Active")]
		Active,

		[EnumMember]
        //[Description("Только удаленные")]
        [LocalizedDescription(typeof(Resources.Language.SKD.Common.IsDeletedFilter), "Deleted")]
		Deleted,

		[EnumMember]
        //[Description("Все")]
        [LocalizedDescription(typeof(Resources.Language.SKD.Common.IsDeletedFilter), "All")]
		All
	}
}
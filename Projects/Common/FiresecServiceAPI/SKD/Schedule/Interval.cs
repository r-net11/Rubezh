using System;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class Interval : SKDModelBase
	{
		[DataMember]
		public DateTime? BeginDate { get; set; }

		[DataMember]
		public DateTime? EndDate { get; set; }

		[DataMember]
		public TransitionType TransitionType { get; set; }

		[DataMember]
		public Guid? NamedIntervalUid { get; set; }
	}

	[DataContract]
	public enum TransitionType
	{
		[EnumMember]
		Day,
		[EnumMember]
		Night,
		[EnumMember]
		DayNight
	}
}
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public enum ScheduleSchemeType
	{
		[EnumMember]
		Week,
		[EnumMember]
		Shift,
		[EnumMember]
		Month
	}
}
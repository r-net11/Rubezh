using System.Runtime.Serialization;

namespace FiresecAPI.SKD
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
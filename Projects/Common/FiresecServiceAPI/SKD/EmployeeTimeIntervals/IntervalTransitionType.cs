using System.ComponentModel;
using System.Runtime.Serialization;

namespace FiresecAPI.EmployeeTimeIntervals
{
	[DataContract]
	public enum IntervalTransitionType
	{
		[DescriptionAttribute("Нет")]
		[EnumMember]
		Day = 0,

		[DescriptionAttribute("Переход")]
		[EnumMember]
		Night = 1,
	}
}
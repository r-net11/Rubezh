using System.ComponentModel;
using System.Runtime.Serialization;

namespace FiresecAPI.EmployeeTimeIntervals
{
	[DataContract]
	public enum IntervalTransitionType
	{
		[DescriptionAttribute("Нет")]
		[EnumMember]
		Day,

		[DescriptionAttribute("Переход")]
		[EnumMember]
		Night,

		[DescriptionAttribute("Следующий день")]
		[EnumMember]
		NextDay
	}
}
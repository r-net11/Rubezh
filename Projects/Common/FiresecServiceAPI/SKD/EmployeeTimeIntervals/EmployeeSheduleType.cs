using System.ComponentModel;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public enum EmployeeSheduleType
	{
		[DescriptionAttribute("Недельная")]
		Week,

		[DescriptionAttribute("Сменная")]
		SlideDay,

		[DescriptionAttribute("Месячная")]
		Month
	}
}
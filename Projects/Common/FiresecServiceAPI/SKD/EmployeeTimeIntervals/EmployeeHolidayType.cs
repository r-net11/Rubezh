using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace FiresecAPI
{
	[DataContract]
	public enum EmployeeHolidayType
	{
		[DescriptionAttribute("Праздник")]
		Holiday,

		[DescriptionAttribute("Предпраздничный день")]
		BeforeHoliday,

		[DescriptionAttribute("Рабочий выходной")]
		WorkingHoliday
	}
}
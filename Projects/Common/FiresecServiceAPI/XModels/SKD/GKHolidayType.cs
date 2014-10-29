using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Тип праздника
	/// </summary>
	/// [DataContract]
	public enum GKHolidayType
	{
		[DescriptionAttribute("Праздник")]
		[EnumMember]
		Holiday = 0,

		[DescriptionAttribute("Предпраздничный день")]
		[EnumMember]
		BeforeHoliday = 1,

		[DescriptionAttribute("Рабочий выходной")]
		[EnumMember]
		WorkingHoliday = 2,
	}
}
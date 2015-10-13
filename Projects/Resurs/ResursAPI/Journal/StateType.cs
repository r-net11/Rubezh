using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ResursAPI
{
	public enum StateType
	{
		[DescriptionAttribute("за указанное число последних часов")]
		LastHours,

		[DescriptionAttribute("за указанное число последних дней")]
		LastDays,

		[DescriptionAttribute("начиная с указанной даты")]
		FromDate,

		[DescriptionAttribute("согласно указанному диапазону дат")]
		RangeDate
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace SKDModule.Models
{
	public enum EmployeeReportPeriod
	{
		[DescriptionAttribute("День")]
		Day,
		[DescriptionAttribute("Неделя")]
		Week,
		[DescriptionAttribute("Месяц")]
		Month,
		[DescriptionAttribute("Период")]
		Period,
	}
}
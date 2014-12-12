using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Infrastructure.Common.SKDReports
{
	public enum SKDReportGroup
	{
		[DescriptionAttribute("Пустышка")]
		Empty = 1,
		[DescriptionAttribute("Учет рабочего времени")]
		WorkingTime = 100,
		[DescriptionAttribute("Картотека")]
		HR = 200,
	}
}

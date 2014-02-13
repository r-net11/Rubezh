using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace SKDModule.Models
{
	public enum EmployeeReportType
	{
		[DescriptionAttribute("Опоздания")]
		Опоздания,

		[DescriptionAttribute("Уходы раньше")]
		Уходы_раньше,

		[DescriptionAttribute("Отсутствующие")]
		Отсутствующие,

		[DescriptionAttribute("Все нарушители")]
		Все_нарушители,

		[DescriptionAttribute("Отсутствующие на текущий момент")]
		Отсутствующие_на_текущий_момент,

		[DescriptionAttribute("Время отсутствия")]
		Время_отсутствия,

		[DescriptionAttribute("Присутствующие на текущий момент")]
		Присутствующие_на_текущий_момент,

		[DescriptionAttribute("Время после работы")]
		Время_после_работы,

		[DescriptionAttribute("Время до начала работы")]
		Время_до_начала_работы,

		[DescriptionAttribute("Нарушение дисциплины в течение рабочего дня")]
		Нарушение_дисциплины_в_течение_рабочего_дня,

		[DescriptionAttribute("Время присутствия")]
		Время_присутствия,

		[DescriptionAttribute("Отработанное время")]
		Отработанное_время
	}
}
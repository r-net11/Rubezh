using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace GKModule
{
	public enum XAlarmType
	{
		[DescriptionAttribute("НПТ")]
		NPT = 0,

		[DescriptionAttribute("Пожар 1")]
		Fire1 = 1,

		[DescriptionAttribute("Пожар 2")]
		Fire2 = 2,

		[DescriptionAttribute("Внимание")]
		Attention = 3,

		[DescriptionAttribute("Неисправность")]
		Failure = 4,

		[DescriptionAttribute("Отключенное оборудование")]
		Ignore = 5,

		[DescriptionAttribute("Автоматика отключена")]
		Auto = 6,

		[DescriptionAttribute("Требуется обслуживание")]
		Service = 7,

		[DescriptionAttribute("Информация")]
		Info = 8
	}
}
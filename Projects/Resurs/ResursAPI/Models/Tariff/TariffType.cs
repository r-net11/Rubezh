using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ResursAPI
{
	public enum TariffType
	{
		[DescriptionAttribute("Холодная вода")]
		ColdWater = 0,
		[DescriptionAttribute("Горячая вода")]
		HotWater = 1,
		[DescriptionAttribute("Газ")]
		Gas = 2,
		[DescriptionAttribute("Электричество")]
		Electricity = 3,
		[DescriptionAttribute("Тепло")]
		Heat = 4,
	}
}

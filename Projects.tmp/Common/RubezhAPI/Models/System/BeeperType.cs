﻿using System.ComponentModel;

namespace RubezhAPI.Models
{
	public enum BeeperType
	{
		[DescriptionAttribute("<нет>")]
		None = 0,

		[DescriptionAttribute("Тревога")]
		Alarm = 200,

		[DescriptionAttribute("Внимание")]
		Attention = 50
	}
}
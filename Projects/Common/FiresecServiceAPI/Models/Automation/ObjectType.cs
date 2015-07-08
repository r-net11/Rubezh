﻿using System.ComponentModel;

namespace FiresecAPI.Automation
{
	public enum ObjectType
	{
		[DescriptionAttribute("ГК-устройство")]
		Device,

		[DescriptionAttribute("ГК-зона")]
		Zone,

		[DescriptionAttribute("ГК-направление")]
		Direction,

		[DescriptionAttribute("Задержка")]
		Delay,

		[DescriptionAttribute("Охранная зона")]
		GuardZone,
		
		[DescriptionAttribute("Видеоустройство")]
		VideoDevice,

		[DescriptionAttribute("Точка доступа ГК")]
		GKDoor,

		[DescriptionAttribute("Организация")]
		Organisation
	}
}

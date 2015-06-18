using System.ComponentModel;

namespace FiresecAPI.Automation
{
	public enum ObjectType
	{
		[DescriptionAttribute("ГК-устройство")]
		Device,

		[DescriptionAttribute("ГК-направление")]
		Direction,

		[DescriptionAttribute("Задержка")]
		Delay,

		[DescriptionAttribute("СКД-устройство")]
		SKDDevice,

		[DescriptionAttribute("СКД-зона")]
		SKDZone,

		[DescriptionAttribute("Видеоустройство")]
		VideoDevice,

		[DescriptionAttribute("Точка доступа")]
		Door,

		[DescriptionAttribute("Точка доступа ГК")]
		GKDoor,

		[DescriptionAttribute("Организация")]
		Organisation
	}
}

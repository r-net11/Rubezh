using System.ComponentModel;

namespace RubezhAPI.Automation
{
	public enum ObjectType
	{
		[DescriptionAttribute("Устройство")]
		Device,

		[DescriptionAttribute("Пожарная зона")]
		Zone,

		[DescriptionAttribute("Направление")]
		Direction,

		[DescriptionAttribute("Задержка")]
		Delay,

		[DescriptionAttribute("Охранная зона")]
		GuardZone,

		[DescriptionAttribute("Насосная станция")]
		PumpStation,

		[DescriptionAttribute("МПТ")]
		MPT,

		[DescriptionAttribute("Видеоустройство")]
		VideoDevice,

		[DescriptionAttribute("Точка доступа")]
		GKDoor,

		[DescriptionAttribute("Организация")]
		Organisation
	}
}
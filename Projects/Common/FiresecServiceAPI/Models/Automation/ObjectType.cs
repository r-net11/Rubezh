using System.ComponentModel;

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

		[DescriptionAttribute("СКД-устройство")]
		SKDDevice,

		[DescriptionAttribute("СКД-зона")]
		SKDZone,

		[DescriptionAttribute("Охранная зона")]
		GuardZone,
		
		[DescriptionAttribute("Карта")]
		Card,

		[DescriptionAttribute("Человек")]
		Person,

		[DescriptionAttribute("Графический план")]
		Plan,

		[DescriptionAttribute("Видеоустройство")]
		VideoDevice,
	}
}

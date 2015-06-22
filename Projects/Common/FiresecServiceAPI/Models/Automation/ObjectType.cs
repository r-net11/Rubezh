using System.ComponentModel;

namespace FiresecAPI.Automation
{
	public enum ObjectType
	{
		[DescriptionAttribute("ГК-устройство")]
		Device,

		[DescriptionAttribute("СКД-устройство")]
		SKDDevice,

		[DescriptionAttribute("СКД-зона")]
		SKDZone,

		[DescriptionAttribute("Видеоустройство")]
		VideoDevice,

		[DescriptionAttribute("Точка доступа")]
		Door,

		[DescriptionAttribute("Организация")]
		Organisation
	}
}

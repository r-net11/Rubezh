using System.ComponentModel;

namespace FiresecAPI.Automation
{
	public enum ObjectType
	{
		[DescriptionAttribute("СКД-устройство")]
		SKDDevice,

		[DescriptionAttribute("СКД-зона")]
		SKDZone,

		[DescriptionAttribute("Видеоустройство")]
		VideoDevice,

		[DescriptionAttribute("Точка доступа")]
		Door,

		[DescriptionAttribute("Организация")]
		Organisation,

		[DescriptionAttribute("Пользователь")]
		User,

		[DescriptionAttribute("Сотрудник")]
		Employee,
	}
}
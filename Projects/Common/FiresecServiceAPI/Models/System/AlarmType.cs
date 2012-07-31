using System.ComponentModel;

namespace FiresecAPI.Models
{
	public enum AlarmType
	{
		[DescriptionAttribute("Тревога")]
		Guard = 0,

		[DescriptionAttribute("Пожар")]
		Fire = 1,

		[DescriptionAttribute("Внимание")]
		Attention = 2,

		[DescriptionAttribute("Неисправность")]
		Failure = 3,

		[DescriptionAttribute("Требуется обслуживание")]
		Service = 4,

		[DescriptionAttribute("Отключенное оборудование")]
		Off = 5,

		[DescriptionAttribute("Автоматика отключена")]
		Auto = 6,

		[DescriptionAttribute("Информация")]
		Info = 7
	}
}
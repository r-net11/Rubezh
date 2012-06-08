using System.ComponentModel;

namespace FiresecAPI.Models
{
	public enum AlarmType
	{
		[DescriptionAttribute("Пожар")]
		Fire = 0,

		[DescriptionAttribute("Внимание")]
		Attention = 1,

		[DescriptionAttribute("Неисправность")]
		Failure = 2,

		[DescriptionAttribute("Требуется обслуживание")]
		Service = 3,

		[DescriptionAttribute("Отключенное оборудование")]
		Off = 4,

		[DescriptionAttribute("Автоматика отключена")]
		Auto = 5,

		[DescriptionAttribute("Информация")]
		Info = 6
	}
}
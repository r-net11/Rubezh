using System.ComponentModel;

namespace FiresecAPI
{
	public enum StateType
	{
		[DescriptionAttribute("Тревога")]
		Fire = 0,

		[DescriptionAttribute("Внимание")]
		Attention = 1,

		[DescriptionAttribute("Неисправность")]
		Failure = 2,

		[DescriptionAttribute("Требуется обслуживание")]
		Service = 3,

		[DescriptionAttribute("Отключено")]
		Off = 4,

		[DescriptionAttribute("Неизвестно")]
		Unknown = 5,

		[DescriptionAttribute("Норма*")]
		Info = 6,

		[DescriptionAttribute("Норма")]
		Norm = 7,

		[DescriptionAttribute("Нет")]
		No = 8
	}
}
using System.ComponentModel;

namespace RubezhAPI.Models
{
	public enum SoundType
	{
		[DescriptionAttribute("Пожар-1")]
		Fire1 = 0,

		[DescriptionAttribute("Пожар-2")]
		Fire2 = 1,

		[DescriptionAttribute("Внимание")]
		Attention = 2,

		[DescriptionAttribute("Тревога")]
		Alarm = 3,

		[DescriptionAttribute("Неисправность")]
		Failure = 4,

		[DescriptionAttribute("Отключено")]
		Off = 5,

		[DescriptionAttribute("Пуск")]
		TurningOn = 6,

		[DescriptionAttribute("Останов пуска")]
		StopStart = 7,

		[DescriptionAttribute("Автоматика отключена")]
		AutoOff = 8,

	}
}
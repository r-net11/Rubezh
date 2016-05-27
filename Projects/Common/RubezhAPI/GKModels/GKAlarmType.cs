using System.ComponentModel;

namespace RubezhAPI.GK
{
	/// <summary>
	/// Тип тревоги
	/// </summary>
	public enum GKAlarmType
	{
		[DescriptionAttribute("Пожаротушение")]
		NPTOn = 0,

		[DescriptionAttribute("Тревога")]
		GuardAlarm = 1,

		[DescriptionAttribute("Пожар 2")]
		Fire2 = 2,

		[DescriptionAttribute("Пожар 1")]
		Fire1 = 3,

		[DescriptionAttribute("Внимание")]
		Attention = 4,

		[DescriptionAttribute("Неисправность")]
		Failure = 5,

		[DescriptionAttribute("Отключенное оборудование")]
		Ignore = 6,

		[DescriptionAttribute("Автоматика отключена")]
		AutoOff = 7,

		[DescriptionAttribute("Останов пуска")]
		StopStart = 8,

		[DescriptionAttribute("Требуется обслуживание")]
		Service = 9,

		[DescriptionAttribute("Включается/Включено")]
		Turning = 10
	}
}
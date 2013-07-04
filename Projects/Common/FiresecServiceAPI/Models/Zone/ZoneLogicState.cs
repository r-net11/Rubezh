using System.ComponentModel;
namespace FiresecAPI.Models
{
	public enum ZoneLogicState
	{
		[DescriptionAttribute("Пожар")]
		Fire = 2,

		[DescriptionAttribute("Внимание")]
		Attention = 5,

		[DescriptionAttribute("Включение автоматики МПТ")]
		MPTAutomaticOn = 0,

		[DescriptionAttribute("Включение модуля пожаротушения")]
		MPTOn = 6,

		[DescriptionAttribute("Тревога")]
		Alarm = 1,

		[DescriptionAttribute("Поставлен на охрану")]
		GuardSet = 7,

		[DescriptionAttribute("Снят с охраны")]
		GuardUnSet = 8,

		[DescriptionAttribute("ПЦН")]
		PCN = 9,

		[DescriptionAttribute("Лампа")]
		Lamp = 10,

		[DescriptionAttribute("Неисправность прибора")]
		Failure = 3,

		[DescriptionAttribute("Сработка Устройства")]
		AM1TOn = 11,

		[DescriptionAttribute("Тушение")]
		Firefighting = 12,

		[DescriptionAttribute("Включение НС")]
		PumpStationOn = 13,

		[DescriptionAttribute("Выключение автоматики НС")]
		PumpStationAutomaticOff = 14,

		[DescriptionAttribute("Сработка ШУЗ")]
		ShuzOn = 18,

		[DescriptionAttribute("Включение без задержки по пожару двух зонах")]
		DoubleFire = 17
	}
}
using System.ComponentModel;

namespace RubezhAPI.GK
{
	public enum GKStateBit
	{
		[DescriptionAttribute("Автоматика включена")]
		Norm = 0,

		[DescriptionAttribute("Внимание")]
		Attention = 1,

		[DescriptionAttribute("Сработка 1")]
		Fire1 = 2,

		[DescriptionAttribute("Сработка 2")]
		Fire2 = 3,

		[DescriptionAttribute("Тест")]
		Test = 4,

		[DescriptionAttribute("Неисправность")]
		Failure = 5,

		[DescriptionAttribute("Отключено")]
		Ignore = 6,

		[DescriptionAttribute("Включено")]
		On = 7,

		[DescriptionAttribute("Выключено")]
		Off = 8,

		[DescriptionAttribute("Включается")]
		TurningOn = 9,

		[DescriptionAttribute("Выключается")]
		TurningOff = 10,

		[DescriptionAttribute("Включить для режима Автоматика")]
		TurnOn_InAutomatic = 11,

		[DescriptionAttribute("Резерв")]
		Reserve1 = 12,

		[DescriptionAttribute("Выключить для режима Автоматика")]
		TurnOff_InAutomatic = 13,

		[DescriptionAttribute("Остановить")]
		Stop_InManual = 14,

		[DescriptionAttribute("Запретить пуск")]
		ForbidStart_InManual = 15,

		[DescriptionAttribute("Включить немедленно для режима Автоматика")]
		TurnOnNow_InAutomatic = 16,

		[DescriptionAttribute("Выключить немедленно для режима Автоматика")]
		TurnOffNow_InAutomatic = 17,

		[DescriptionAttribute("Перевести в режим Автоматика")]
		SetRegime_Automatic = 18,

		[DescriptionAttribute("Перевести в режим Ручное")]
		SetRegime_Manual = 19,

		[DescriptionAttribute("Включить")]
		TurnOn_InManual = 20,

		[DescriptionAttribute("Выключить")]
		TurnOff_InManual = 21,

		[DescriptionAttribute("Включить немедленно")]
		TurnOnNow_InManual = 22,

		[DescriptionAttribute("Выключить немедленно")]
		TurnOffNow_InManual = 23,

		[DescriptionAttribute("Перевести в режим Отключение")]
		SetRegime_Off = 24,

		[DescriptionAttribute("Сброс")]
		Reset = 25,

		[DescriptionAttribute("Резерв")]
		Reserve2 = 26,

		[DescriptionAttribute("Сохранение состояния")]
		Save = 31,

		[DescriptionAttribute("Нет")]
		No = 32,

		[DescriptionAttribute("Включить 2")]
		TurnOn2_InManual = 33
	}
}
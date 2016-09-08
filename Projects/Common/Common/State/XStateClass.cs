using System.ComponentModel;
using Localization.Common;
using Localization.Converters;

namespace StrazhAPI.GK
{
	/// <summary>
	/// Класс состояния
	/// </summary>
	public enum XStateClass
	{
		//[Description("Лицензия отсутствует")]
		[LocalizedDescription(typeof(CommonResources), "HasNoLicense")]
		HasNoLicense = 0,

		//[Description("База данных устройства не соответствует базе данных ПК")]
		[LocalizedDescription(typeof(CommonResources), "DBMissmatch")]
		DBMissmatch = 1,

		//[Description("Контроллер в технологическом режиме")]
		[LocalizedDescription(typeof(CommonResources), "TechnologicalRegime")]
		TechnologicalRegime = 2,

		//[Description("Потеря связи")]
		[LocalizedDescription(typeof(CommonResources), "ConnectionLost")]
		ConnectionLost = 3,

		//[Description("Пожар 2")]
		[LocalizedDescription(typeof(CommonResources), "Fire2")]
		Fire2 = 4,

		//[Description("Пожар 1")]
		[LocalizedDescription(typeof(CommonResources), "Fire1")]
		Fire1 = 5,

		//[Description("Тревога")]
		[LocalizedDescription(typeof(CommonResources), "Alarm")]
		Attention = 6,

		//[Description("Отключено")]
		[LocalizedDescription(typeof(CommonResources), "Ignore")]
		Ignore = 7,

		//[Description("Неисправность")]
		[LocalizedDescription(typeof(CommonResources), "Failure")]
		Failure = 8,

		//[Description("Включено")]
		[LocalizedDescription(typeof(CommonResources), "On")]
		On = 9,

		//[Description("Включается")]
		[LocalizedDescription(typeof(CommonResources), "TurningOn")]
		TurningOn = 10,

		//[Description("Выключается")]
		[LocalizedDescription(typeof(CommonResources), "TurningOff")]
		TurningOff = 11,

		//[Description("Автоматика отключена")]
		[LocalizedDescription(typeof(CommonResources), "AutoOff")]
		AutoOff = 12,

		//[Description("Требуется обслуживание")]
		[LocalizedDescription(typeof(CommonResources), "Service")]
		Service = 13,

		//[Description("Тест")]
		[LocalizedDescription(typeof(CommonResources), "Test")]
		Test = 14,

		//[Description("Информация")]
		[LocalizedDescription(typeof(CommonResources), "Info")]
		Info = 15,

		//[Description("Выключено")]
		[LocalizedDescription(typeof(CommonResources), "Off")]
		Off = 16,

		//[Description("Неизвестно")]
		[LocalizedDescription(typeof(CommonResources), "Unknown")]
		Unknown = 17,

		//[Description("Норма")]
		[LocalizedDescription(typeof(CommonResources), "Norm")]
		Norm = 18,

		//[Description("Нет")]
		[LocalizedDescription(typeof(CommonResources), "No")]
		No = 19,

		//[Description("Лицензия обнаружена")]
		[LocalizedDescription(typeof(CommonResources), "HasLicense")]
		HasLicense = 20,
	}
}
using System.ComponentModel;
using Common.Properties;
using Org.BouncyCastle.Asn1.Crmf;
using Localization;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Класс состояния
	/// </summary>
	public enum XStateClass
	{
		//[DescriptionAttribute("Лицензия отсутствует")]
        [LocalizedDescription(typeof(Common.Resources.Language.XStateClass), "HasNoLicense")]
		HasNoLicense = 0,

        //[DescriptionAttribute("База данных устройства не соответствует базе данных ПК")]
        [LocalizedDescription(typeof(Common.Resources.Language.XStateClass), "DBMissmatch")]
        DBMissmatch = 1,

        //[DescriptionAttribute("Контроллер в технологическом режиме")]
        [LocalizedDescription(typeof(Common.Resources.Language.XStateClass), "TechnologicalRegime")]
		TechnologicalRegime = 2,

        //[DescriptionAttribute("Потеря связи")]
        [LocalizedDescription(typeof(Common.Resources.Language.XStateClass), "ConnectionLost")]
		ConnectionLost = 3,

        //[DescriptionAttribute("Пожар 2")]
        [LocalizedDescription(typeof(Common.Resources.Language.XStateClass), "Fire2")]
		Fire2 = 4,

        //[DescriptionAttribute("Пожар 1")]
        [LocalizedDescription(typeof(Common.Resources.Language.XStateClass), "Fire1")]
		Fire1 = 5,

        //[DescriptionAttribute("Тревога")]
        [LocalizedDescription(typeof(Common.Resources.Language.XStateClass), "Attention")]
		Attention = 6,

        //[DescriptionAttribute("Отключено")]
        [LocalizedDescription(typeof(Common.Resources.Language.XStateClass), "Ignore")]
		Ignore = 7,

        //[DescriptionAttribute("Неисправность")]
        [LocalizedDescription(typeof(Common.Resources.Language.XStateClass), "Failure")]
		Failure = 8,

        //[DescriptionAttribute("Включено")]
        [LocalizedDescription(typeof(Common.Resources.Language.XStateClass), "On")]
		On = 9,

        //[DescriptionAttribute("Включается")]
        [LocalizedDescription(typeof(Common.Resources.Language.XStateClass), "TurningOn")]
		TurningOn = 10,

        //[DescriptionAttribute("Выключается")]
        [LocalizedDescription(typeof(Common.Resources.Language.XStateClass), "TurningOff")]
		TurningOff = 11,

        //[DescriptionAttribute("Автоматика отключена")]
        [LocalizedDescription(typeof(Common.Resources.Language.XStateClass), "AutoOff")]
		AutoOff = 12,

        //[DescriptionAttribute("Требуется обслуживание")]
        [LocalizedDescription(typeof(Common.Resources.Language.XStateClass), "Service")]
		Service = 13,

        //[DescriptionAttribute("Тест")]
        [LocalizedDescription(typeof(Common.Resources.Language.XStateClass), "Test")]
		Test = 14,

        //[DescriptionAttribute("Информация")]
        [LocalizedDescription(typeof(Common.Resources.Language.XStateClass), "Info")]
		Info = 15,

        //[DescriptionAttribute("Выключено")]
        [LocalizedDescription(typeof(Common.Resources.Language.XStateClass), "Off")]
		Off = 16,

        //[DescriptionAttribute("Неизвестно")]
        [LocalizedDescription(typeof(Common.Resources.Language.XStateClass), "Unknown")]
		Unknown = 17,

        //[DescriptionAttribute("Норма")]
        [LocalizedDescription(typeof(Common.Resources.Language.XStateClass), "Norm")]
		Norm = 18,

        //[DescriptionAttribute("Нет")]
        [LocalizedDescription(typeof(Common.Resources.Language.XStateClass), "No")]
		No = 19,

		//[DescriptionAttribute("Лицензия обнаружена")]
        [LocalizedDescription(typeof(Common.Resources.Language.XStateClass), "HasLicense")]
		HasLicense = 20,
	}
}
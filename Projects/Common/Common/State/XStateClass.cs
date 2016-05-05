using System.ComponentModel;
using Common.Properties;
using Org.BouncyCastle.Asn1.Crmf;
using LocalizationConveters;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Класс состояния
	/// </summary>
	public enum XStateClass
	{
		//[DescriptionAttribute("Лицензия отсутствует")]
        [LocalizedDescription(typeof(Common.Resources.Language.State.XStateClass), "HasNoLicense")]
		HasNoLicense = 0,

        //[DescriptionAttribute("База данных устройства не соответствует базе данных ПК")]
        [LocalizedDescription(typeof(Common.Resources.Language.State.XStateClass), "DBMissmatch")]
        DBMissmatch = 1,

        //[DescriptionAttribute("Контроллер в технологическом режиме")]
        [LocalizedDescription(typeof(Common.Resources.Language.State.XStateClass), "TechnologicalRegime")]
		TechnologicalRegime = 2,

        //[DescriptionAttribute("Потеря связи")]
        [LocalizedDescription(typeof(Common.Resources.Language.State.XStateClass), "ConnectionLost")]
		ConnectionLost = 3,

        //[DescriptionAttribute("Пожар 2")]
        [LocalizedDescription(typeof(Common.Resources.Language.State.XStateClass), "Fire2")]
		Fire2 = 4,

        //[DescriptionAttribute("Пожар 1")]
        [LocalizedDescription(typeof(Common.Resources.Language.State.XStateClass), "Fire1")]
		Fire1 = 5,

        //[DescriptionAttribute("Тревога")]
        [LocalizedDescription(typeof(Common.Resources.Language.State.XStateClass), "Attention")]
		Attention = 6,

        //[DescriptionAttribute("Отключено")]
        [LocalizedDescription(typeof(Common.Resources.Language.State.XStateClass), "Ignore")]
		Ignore = 7,

        //[DescriptionAttribute("Неисправность")]
        [LocalizedDescription(typeof(Common.Resources.Language.State.XStateClass), "Failure")]
		Failure = 8,

        //[DescriptionAttribute("Включено")]
        [LocalizedDescription(typeof(Common.Resources.Language.State.XStateClass), "On")]
		On = 9,

        //[DescriptionAttribute("Включается")]
        [LocalizedDescription(typeof(Common.Resources.Language.State.XStateClass), "TurningOn")]
		TurningOn = 10,

        //[DescriptionAttribute("Выключается")]
        [LocalizedDescription(typeof(Common.Resources.Language.State.XStateClass), "TurningOff")]
		TurningOff = 11,

        //[DescriptionAttribute("Автоматика отключена")]
        [LocalizedDescription(typeof(Common.Resources.Language.State.XStateClass), "AutoOff")]
		AutoOff = 12,

        //[DescriptionAttribute("Требуется обслуживание")]
        [LocalizedDescription(typeof(Common.Resources.Language.State.XStateClass), "Service")]
		Service = 13,

        //[DescriptionAttribute("Тест")]
        [LocalizedDescription(typeof(Common.Resources.Language.State.XStateClass), "Test")]
		Test = 14,

        //[DescriptionAttribute("Информация")]
        [LocalizedDescription(typeof(Common.Resources.Language.State.XStateClass), "Info")]
		Info = 15,

        //[DescriptionAttribute("Выключено")]
        [LocalizedDescription(typeof(Common.Resources.Language.State.XStateClass), "Off")]
		Off = 16,

        //[DescriptionAttribute("Неизвестно")]
        [LocalizedDescription(typeof(Common.Resources.Language.State.XStateClass), "Unknown")]
		Unknown = 17,

        //[DescriptionAttribute("Норма")]
        [LocalizedDescription(typeof(Common.Resources.Language.State.XStateClass), "Norm")]
		Norm = 18,

        //[DescriptionAttribute("Нет")]
        [LocalizedDescription(typeof(Common.Resources.Language.State.XStateClass), "No")]
		No = 19,

		//[DescriptionAttribute("Лицензия обнаружена")]
        [LocalizedDescription(typeof(Common.Resources.Language.State.XStateClass), "HasLicense")]
		HasLicense = 20,
	}
}
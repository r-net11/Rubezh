using System.ComponentModel;

namespace XFiresecAPI
{
	public enum XStateClass
	{
        [DescriptionAttribute("Пожар 2")]
        Fire2 = 0,

		[DescriptionAttribute("Пожар 1")]
		Fire1 = 1,

        [DescriptionAttribute("Внимание")]
        Attention = 2,

        [DescriptionAttribute("Неисправность")]
        Failure = 3,

        [DescriptionAttribute("Отключение")]
        Ignore = 4,

        [DescriptionAttribute("Автоматика отключена")]
        AutoOff = 5,

        [DescriptionAttribute("Требуется обслуживание")]
        Service = 6,

        [DescriptionAttribute("Включено")]
        On = 7,

        [DescriptionAttribute("Информация")]
        Info = 8,

        [DescriptionAttribute("Неизвестно")]
        Unknown = 9,

        [DescriptionAttribute("Норма")]
        Norm = 10,

        [DescriptionAttribute("Нет")]
        No = 11
	}
}
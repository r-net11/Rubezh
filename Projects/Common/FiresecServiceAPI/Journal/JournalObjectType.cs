using System.ComponentModel;

namespace FiresecAPI.Journal
{
	public enum JournalObjectType
	{
		[DescriptionAttribute("Нет")]
		None = 0,

		[DescriptionAttribute("Устройства Страж")]
		SKDDevice = 10,

		[DescriptionAttribute("Зоны Страж")]
		SKDZone = 11,

		[DescriptionAttribute("Точки доступа Страж")]
		SKDDoor = 12,

		[DescriptionAttribute("Видеоустройства")]
		VideoDevice = 13
	}
}
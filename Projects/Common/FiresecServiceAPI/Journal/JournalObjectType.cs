using System.ComponentModel;

namespace FiresecAPI.Journal
{
	public enum JournalObjectType
	{
		[DescriptionAttribute("Нет")]
		None,

		[DescriptionAttribute("Устройства Страж")]
		SKDDevice,

		[DescriptionAttribute("Зоны Страж")]
		SKDZone,

		[DescriptionAttribute("Точки доступа Страж")]
		SKDDoor,

		[DescriptionAttribute("Видеоустройства")]
		VideoDevice,

		[DescriptionAttribute("Зоны СКД ГК")]
		GKSKDZone,
	}
}
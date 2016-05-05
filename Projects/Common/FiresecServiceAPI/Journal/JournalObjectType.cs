using System.ComponentModel;

namespace StrazhAPI.Journal
{
	public enum JournalObjectType
	{
		[Description("Нет")]
		None = 0,

		[Description("Устройства Страж")]
		SKDDevice = 10,

		[Description("Зоны Страж")]
		SKDZone = 11,

		[Description("Точки доступа Страж")]
		SKDDoor = 12,

		[Description("Видеоустройства")]
		VideoDevice = 13
	}
}
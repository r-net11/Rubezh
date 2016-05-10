using System.ComponentModel;
using System.Windows.Data;
using LocalizationConveters;

namespace StrazhAPI.Journal
{
	public enum JournalObjectType
	{
		//[DescriptionAttribute("Нет")]
        [LocalizedDescription(typeof(Resources.Language.Journal.JournalObjectType), "None")]
		None = 0,

		//[DescriptionAttribute("Устройства Страж")]
        [LocalizedDescription(typeof(Resources.Language.Journal.JournalObjectType), "SKDDevice")]
        SKDDevice = 10,

		//[DescriptionAttribute("Зоны Страж")]
        [LocalizedDescription(typeof(Resources.Language.Journal.JournalObjectType), "SKDZone")]
        SKDZone = 11,

		//[DescriptionAttribute("Точки доступа Страж")]
        [LocalizedDescription(typeof(Resources.Language.Journal.JournalObjectType), "SKDDoor")]
        SKDDoor = 12,

		//[DescriptionAttribute("Видеоустройства")]
        [LocalizedDescription(typeof(Resources.Language.Journal.JournalObjectType), "VideoDevice")]
        VideoDevice = 13
	}
}
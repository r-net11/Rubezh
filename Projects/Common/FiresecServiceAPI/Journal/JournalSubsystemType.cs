using System.ComponentModel;
using LocalizationConveters;

namespace StrazhAPI.Journal
{
	public enum JournalSubsystemType
	{
		//[Description("Система")]
        [LocalizedDescription(typeof(Resources.Language.Journal.JournalSubSystemType), "System")]
        System = 0,

		//[Description("Страж")]
        [LocalizedDescription(typeof(Resources.Language.Journal.JournalSubSystemType), "SKD")]
        SKD = 2,

		//[Description("Видео")]
        [LocalizedDescription(typeof(Resources.Language.Journal.JournalSubSystemType), "Video")]
		Video = 3
	}
}
using System.ComponentModel;

namespace StrazhAPI.Journal
{
	public enum JournalSubsystemType
	{
		[Description("Система")]
		System = 0,

		[Description("Страж")]
		SKD = 2,

		[Description("Видео")]
		Video = 3
	}
}
using System.ComponentModel;

namespace FiresecAPI.Journal
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
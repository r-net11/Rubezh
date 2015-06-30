using System.ComponentModel;

namespace FiresecAPI.Journal
{
	public enum JournalSubsystemType
	{
		[Description("Система")]
		System,

		[Description("ГК")]
		GK,

		[Description("Страж")]
		SKD,

		[Description("Видео")]
		Video
	}
}
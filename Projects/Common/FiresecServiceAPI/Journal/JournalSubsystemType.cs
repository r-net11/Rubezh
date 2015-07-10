using System.ComponentModel;

namespace FiresecAPI.Journal
{
	public enum JournalSubsystemType
	{
		[Description("Система")]
		System,

		[Description("ГК")]
		GK,

		[Description("СКД")]
		SKD,

		[Description("Видео")]
		Video
	}
}
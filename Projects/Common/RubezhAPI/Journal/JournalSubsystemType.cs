using System.ComponentModel;

namespace RubezhAPI.Journal
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
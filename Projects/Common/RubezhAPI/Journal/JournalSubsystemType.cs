using System.ComponentModel;

namespace RubezhAPI.Journal
{
	/// <summary>
	/// Тип подсистемы
	/// </summary>
	public enum JournalSubsystemType
	{
		[Description("Система")]
		System = 0,

		[Description("ГК")]
		GK = 1,

		[Description("СКД")]
		SKD = 2,

		[Description("Видео")]
		Video = 3
	}
}
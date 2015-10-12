using System.ComponentModel;

namespace RubezhAPI.GK
{
	/// <summary>
	/// Тип периодичности графика
	/// </summary>
	public enum GKSchedulePeriodType
	{
		[Description("Недельный")]
		Weekly,

		[Description("Суточный")]
		Dayly,

		[Description("Произвольный")]
		Custom,

		[Description("Непериодичный")]
		NonPeriodic
	}
}
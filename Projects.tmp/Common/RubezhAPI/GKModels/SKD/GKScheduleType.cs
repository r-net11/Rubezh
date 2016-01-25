using System.ComponentModel;

namespace RubezhAPI.GK
{
	/// <summary>
	/// Тип графика
	/// </summary>
	public enum GKScheduleType
	{
		[Description("График доступа")]
		Access,

		[Description("Праздничные дни")]
		Holiday,

		[Description("Рабочие выходные")]
		WorkHoliday,
	}
}
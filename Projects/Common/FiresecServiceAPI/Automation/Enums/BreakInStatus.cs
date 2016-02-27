using System.ComponentModel;

namespace FiresecAPI.Automation.Enums
{
	/// <summary>
	/// Описывает статус по взому в автоматизации
	/// </summary>
	public enum BreakInStatus
	{
		/// <summary>
		/// Норма
		/// </summary>
		[Description("Норма")]
		Normal,
		
		/// <summary>
		/// Взлом
		/// </summary>
		[Description("Взлом")]
		BreakIn
	}
}
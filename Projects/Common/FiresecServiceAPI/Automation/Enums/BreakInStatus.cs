using System.ComponentModel;
using Localization;

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
		//[Description("Норма")]
        [LocalizedDescription(typeof(Resources.Language.Automation.Enums.BreakInStatus), "Normal")]
		Normal,
		
		/// <summary>
		/// Взлом
		/// </summary>
		//[Description("Взлом")]
        [LocalizedDescription(typeof(Resources.Language.Automation.Enums.BreakInStatus), "BreakIn")]
        BreakIn
	}
}
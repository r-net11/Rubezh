using System.ComponentModel;
using LocalizationConveters;

namespace StrazhAPI.Automation.Enums
{
	/// <summary>
	/// Описывает состояние двери в автоматизации
	/// </summary>
	public enum DoorStatus
	{
		/// <summary>
		/// Дверь открыта
		/// </summary>
		//[Description("Открыто")]
        [LocalizedDescription(typeof(Resources.Language.Automation.Enums.DoorStatus), "Opened")]
		Opened,
		
		/// <summary>
		/// Дверь закрыта
		/// </summary>
        //[Description("Закрыто")]
        [LocalizedDescription(typeof(Resources.Language.Automation.Enums.DoorStatus), "Closed")]
		Closed
	}
}
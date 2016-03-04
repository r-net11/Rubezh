using System.ComponentModel;

namespace FiresecAPI.Automation.Enums
{
	/// <summary>
	/// Описывает состояние двери в автоматизации
	/// </summary>
	public enum DoorStatus
	{
		/// <summary>
		/// Дверь открыта
		/// </summary>
		[Description("Открыто")]
		Opened,
		
		/// <summary>
		/// Дверь закрыта
		/// </summary>
		[Description("Закрыто")]
		Closed
	}
}
using System.ComponentModel;

namespace FiresecAPI.Automation.Enums
{
	/// <summary>
	/// Описывает режим доступа для замка в автоматизации
	/// </summary>
	public enum AccessState
	{
		/// <summary>
		/// Режим ОТКРЫТО
		/// </summary>
		[Description("Режим ОТКРЫТО")]
		Opened = FiresecAPI.SKD.AccessState.OpenAlways,
		
		/// <summary>
		/// Режим НОРМА
		/// </summary>
		[Description("Режим НОРМА")]
		Normal = FiresecAPI.SKD.AccessState.Normal,
		
		/// <summary>
		/// Режим ЗАКРЫТО
		/// </summary>
		[Description("Режим ЗАКРЫТО")]
		Closed
	}
}
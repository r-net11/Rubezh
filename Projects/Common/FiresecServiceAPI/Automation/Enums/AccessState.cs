using System.ComponentModel;

namespace StrazhAPI.Automation.Enums
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
		Opened = SKD.AccessState.OpenAlways,
		
		/// <summary>
		/// Режим НОРМА
		/// </summary>
		[Description("Режим НОРМА")]
		Normal = SKD.AccessState.Normal,
		
		/// <summary>
		/// Режим ЗАКРЫТО
		/// </summary>
		[Description("Режим ЗАКРЫТО")]
		Closed
	}
}
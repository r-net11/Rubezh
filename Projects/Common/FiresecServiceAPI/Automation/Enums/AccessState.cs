using System.ComponentModel;
using LocalizationConveters;

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
		//[Description("Режим ОТКРЫТО")]
        [LocalizedDescription(typeof(Resources.Language.Automation.Enums.AccessState), "Opened")]
		Opened = FiresecAPI.SKD.AccessState.OpenAlways,
		
		/// <summary>
		/// Режим НОРМА
		/// </summary>
		//[Description("Режим НОРМА")]
        [LocalizedDescription(typeof(Resources.Language.Automation.Enums.AccessState), "Normal")]
        Normal = FiresecAPI.SKD.AccessState.Normal,
		
		/// <summary>
		/// Режим ЗАКРЫТО
		/// </summary>
		//[Description("Режим ЗАКРЫТО")]
        [LocalizedDescription(typeof(Resources.Language.Automation.Enums.AccessState), "Closed")]
        Closed
	}
}
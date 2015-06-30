using System.ComponentModel;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Тип условия
	/// </summary>
	public enum ClauseConditionType
	{
		[DescriptionAttribute("Если")]
		If,

		[DescriptionAttribute("Если НЕ")]
		IfNot
	}
}
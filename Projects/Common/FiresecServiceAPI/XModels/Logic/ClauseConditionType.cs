using System.ComponentModel;

namespace FiresecAPI.GK
{
	public enum ClauseConditionType
	{
		[DescriptionAttribute("Если")]
		If,

		[DescriptionAttribute("Если НЕ")]
		IfNot
	}
}
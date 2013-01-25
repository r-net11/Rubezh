using System.ComponentModel;

namespace XFiresecAPI
{
	public enum ClauseConditionType
	{
		[DescriptionAttribute("Если")]
		If,

		[DescriptionAttribute("Если НЕ")]
		IfNot
	}
}
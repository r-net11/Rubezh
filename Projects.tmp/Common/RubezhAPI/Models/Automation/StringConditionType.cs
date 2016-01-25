using System.ComponentModel;

namespace FiresecAPI.Automation
{
	public enum StringConditionType
	{
		[Description("Начинается с")]
		StartsWith,

		[Description("Заканчивается на")]
		EndsWith,

		[Description("Содержит")]
		Contains
	}
}

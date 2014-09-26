using System.ComponentModel;

namespace FiresecAPI.Automation
{
	public enum JoinOperator
	{
		[Description("И")]
		And,
		[DescriptionAttribute("Или")]
		Or
	}
}
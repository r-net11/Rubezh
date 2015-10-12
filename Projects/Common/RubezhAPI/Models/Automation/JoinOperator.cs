using System.ComponentModel;

namespace RubezhAPI.Automation
{
	public enum JoinOperator
	{
		[Description("И")]
		And,
		[DescriptionAttribute("Или")]
		Or
	}
}
using System.ComponentModel;
using LocalizationConveters;

namespace FiresecAPI.Automation
{
	public enum JoinOperator
	{
		//[Description("И")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.JoinOperator),"And")]
		And,

		//[DescriptionAttribute("Или")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.JoinOperator), "Or")]
        Or
	}
}
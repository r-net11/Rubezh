using System.ComponentModel;
using Localization;

namespace FiresecAPI.Automation
{
	public enum VariableScope
	{
		//[Description("Локальная переменная")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.VariableScope), "LocalVariable")]
		LocalVariable,

		//[Description("Глобальная переменная")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.VariableScope), "GlobalVariable")]
        GlobalVariable,

		//[Description("Явное значение")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.VariableScope), "ExplicitValue")]
        ExplicitValue
	}
}
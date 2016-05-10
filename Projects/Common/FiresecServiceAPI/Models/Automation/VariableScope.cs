using System.ComponentModel;
using LocalizationConveters;

namespace StrazhAPI.Automation
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
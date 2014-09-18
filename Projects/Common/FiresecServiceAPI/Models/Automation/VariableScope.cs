using System.ComponentModel;

namespace FiresecAPI.Automation
{
	public enum VariableScope
	{
		[Description("Локальная переменная")]
		LocalVariable,

		[Description("Глобальная переменная")]
		GlobalVariable,

		[Description("Явное значение")]
		ExplicitValue
	}
}
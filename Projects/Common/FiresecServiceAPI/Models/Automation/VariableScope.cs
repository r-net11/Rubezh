using System.ComponentModel;

namespace FiresecAPI.Automation
{
	public enum VariableScope
	{
		[Description("Глобальная переменная")]
		GlobalVariable,

		[Description("Локальная переменная")]
		LocalVariable,

		[Description("Явное значение")]
		ExplicitValue
	}
}
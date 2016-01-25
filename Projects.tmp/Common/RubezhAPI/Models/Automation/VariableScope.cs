using System.ComponentModel;

namespace RubezhAPI.Automation
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
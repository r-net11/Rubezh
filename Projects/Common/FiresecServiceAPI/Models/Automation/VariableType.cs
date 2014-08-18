using System.ComponentModel;

namespace FiresecAPI.Automation
{
	public enum VariableType
	{
		[Description("Глобальная переменная")]
		IsGlobalVariable,

		[Description("Локальная переменнаяя")]
		IsLocalVariable,
		
		[Description("Явное значение")]
		IsValue
	}
}

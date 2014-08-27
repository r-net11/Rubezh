using System.ComponentModel;

namespace FiresecAPI.Automation
{
	public enum VariableType
	{
		[Description("Глобальная переменная")]
		IsGlobalVariable,

		[Description("Локальная переменная")]
		IsLocalVariable,
		
		[Description("Явное значение")]
		IsValue
	}
}

using System.ComponentModel;

namespace FiresecAPI.Automation
{
	public enum ValueType
	{
		[Description("Явное значение")]
		IsValue,

		[Description("Глобальная переменная")]
		IsGlobalVariable,

		[Description("Локальная переменнаяя")]
		IsLocalVariable
	}
}

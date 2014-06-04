using System.ComponentModel;

namespace FiresecAPI.XModels.Automation
{
	public enum ProcedureStepType
	{
		[DescriptionAttribute("Проигрывание звуков")]
		PlaySound = 0,

		[DescriptionAttribute("Делать действие (тест)")]
		DoAction = 1
	}
}

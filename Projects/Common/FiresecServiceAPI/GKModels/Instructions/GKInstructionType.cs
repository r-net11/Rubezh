using System.ComponentModel;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Тип инструкции ГК
	/// </summary>
	public enum GKInstructionType
	{
		[DescriptionAttribute("Общая инструкция")]
		General,

		[DescriptionAttribute("Инструкция для устройств, зон и направлений")]
		Details
	}
}
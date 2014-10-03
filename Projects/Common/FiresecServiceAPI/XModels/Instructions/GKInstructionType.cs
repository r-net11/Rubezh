using System.ComponentModel;

namespace FiresecAPI.GK
{
	public enum GKInstructionType
	{
		[DescriptionAttribute("Общая инструкция")]
		General,

		[DescriptionAttribute("Инструкция для устройств, зон и направлений")]
		Details
	}
}
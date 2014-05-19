using System.ComponentModel;

namespace FiresecAPI.GK
{
	public enum XInstructionType
	{
		[DescriptionAttribute("Общая инструкция")]
		General,

		[DescriptionAttribute("Инструкция для устройств, зон и направлений")]
		Details
	}
}
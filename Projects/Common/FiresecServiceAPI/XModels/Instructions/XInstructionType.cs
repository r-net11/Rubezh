using System.ComponentModel;

namespace XFiresecAPI
{
	public enum XInstructionType
	{
		[DescriptionAttribute("Общая инструкция")]
		General,

		[DescriptionAttribute("Инструкция для устройств, зон и направлений")]
		Details
	}
}
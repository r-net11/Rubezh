using System.ComponentModel;

namespace XFiresecAPI
{
	public enum XInstructionType
	{
		[DescriptionAttribute("Общая инструкция")]
		General,

		[DescriptionAttribute("Инструкция для зон и приборов")]
		Details
	}
}
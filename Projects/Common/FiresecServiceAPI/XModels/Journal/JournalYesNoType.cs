using System.ComponentModel;

namespace XFiresecAPI
{
	public enum JournalYesNoType
	{
		[DescriptionAttribute("")]
		Unknown = 0,

		[DescriptionAttribute("Есть")]
		Yes = 1,

		[DescriptionAttribute("Нет")]
		No = 2,
	}
}
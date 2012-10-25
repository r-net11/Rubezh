using System.ComponentModel;

namespace Common.GK
{
	public enum JournalItemType
	{
		[DescriptionAttribute("Система")]
		System = 0,

		[DescriptionAttribute("ГК")]
		GK = 1,

		[DescriptionAttribute("Устройство")]
		Device = 2,

		[DescriptionAttribute("Зона")]
		Zone = 3,

		[DescriptionAttribute("Направление")]
		Direction = 4
	}
}
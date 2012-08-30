using System.ComponentModel;

namespace Common.GK
{
	public enum JournalItemType
	{
		[DescriptionAttribute("Система")]
		System,

		[DescriptionAttribute("ГК")]
		GK,

		[DescriptionAttribute("Устройство")]
		Device,

		[DescriptionAttribute("Зона")]
		Zone,

		[DescriptionAttribute("Направление")]
		Direction
	}
}
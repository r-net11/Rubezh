using System.ComponentModel;

namespace Infrastructure.Models
{
	public enum ArchiveDefaultStateType
	{
		[DescriptionAttribute("за указанное число последних часов")]
		LastHours,

		[DescriptionAttribute("за указанное число последних дней")]
		LastDays,

		[DescriptionAttribute("начиная с указанной даты")]
		FromDate,

		[DescriptionAttribute("согласно указанному диапазону дат")]
		RangeDate
	}
}
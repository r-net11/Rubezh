using System.ComponentModel;
using Localization.Converters;
using Localization.Infrastructure.Common;

namespace Infrastructure.Models
{
	public enum ArchiveDefaultStateType
	{
        [LocalizedDescription(typeof(CommonResources),"LastHours")]
		//[DescriptionAttribute("за указанное число последних часов")]
		LastHours,

        [LocalizedDescription(typeof(CommonResources), "LastDays")]
		//[DescriptionAttribute("за указанное число последних дней")]
		LastDays,

        [LocalizedDescription(typeof(CommonResources), "FromDate")]
		//[DescriptionAttribute("начиная с указанной даты")]
		FromDate,

        [LocalizedDescription(typeof(CommonResources), "RangeDate")]
		//[DescriptionAttribute("согласно указанному диапазону дат")]
		RangeDate
	}
}
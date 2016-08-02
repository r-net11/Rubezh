using System.ComponentModel;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
{
	[DataContract]
	public enum HolidayType
	{
		[Description("Праздник")]
		[EnumMember]
		Holiday = 0,

		[Description("Предпраздничный день")]
		[EnumMember]
		BeforeHoliday = 1,

		[Description("Рабочий выходной")]
		[EnumMember]
		WorkingHoliday = 2,
	}
}
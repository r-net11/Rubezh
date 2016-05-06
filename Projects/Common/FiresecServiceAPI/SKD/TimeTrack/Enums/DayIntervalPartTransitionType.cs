using System.ComponentModel;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
{
	[DataContract]
	public enum DayIntervalPartTransitionType
	{
		[Description("Нет")]
		[EnumMember]
		Day = 0,

		[Description("Переход")]
		[EnumMember]
		Night = 1,
	}
}
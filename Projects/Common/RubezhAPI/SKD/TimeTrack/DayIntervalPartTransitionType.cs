using System.ComponentModel;
using System.Runtime.Serialization;

namespace RubezhAPI.SKD
{
	[DataContract]
	public enum DayIntervalPartTransitionType
	{
		[DescriptionAttribute("Нет")]
		[EnumMember]
		Day = 0,

		[DescriptionAttribute("Переход")]
		[EnumMember]
		Night = 1,
	}
}
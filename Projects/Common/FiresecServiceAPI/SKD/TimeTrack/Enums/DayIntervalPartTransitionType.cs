using System.ComponentModel;
using System.Runtime.Serialization;
using Localization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public enum DayIntervalPartTransitionType
	{
		//[DescriptionAttribute("Нет")]
        [LocalizedDescription(typeof(Resources.Language.SKD.TimeTrack.Enums.DayIntervalPartTransitionType),"Day")]
		[EnumMember]
		Day = 0,

        //[DescriptionAttribute("Переход")]
        [LocalizedDescription(typeof(Resources.Language.SKD.TimeTrack.Enums.DayIntervalPartTransitionType), "Night")]
		[EnumMember]
		Night = 1,
	}
}
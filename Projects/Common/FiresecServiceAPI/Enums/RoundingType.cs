using System.ComponentModel;
using LocalizationConveters;
using StrazhAPI.Properties;

namespace StrazhAPI.Enums
{
	public enum RoundingType
	{
		//[Description("Не используется")]
		[LocalizedDescription(typeof(Resources.Language.Enums.RoundingType), "None")]
		None,

		//[Description("До минут")]
		[LocalizedDescription(typeof(Resources.Language.Enums.RoundingType), "RoundToMin")]
		RoundToMin,

		//[Description("До часов")]
		[LocalizedDescription(typeof(Resources.Language.Enums.RoundingType), "RoundToHour")]
		RoundToHour
	}
}

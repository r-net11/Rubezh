using System.ComponentModel;

namespace StrazhAPI.Enums
{
	public enum RoundingType
	{
		[Description("Не используется")]
		None,

		[Description("До минут")]
		RoundToMin,

		[Description("До часов")]
		RoundToHour
	}
}

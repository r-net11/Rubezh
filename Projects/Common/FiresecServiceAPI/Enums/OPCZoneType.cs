using System.ComponentModel;

namespace StrazhAPI.Enums
{
	public enum OPCZoneType
	{
		[Description("Пожарная")]
		Fire = 0,
		[Description("Охранная")]
		Guard = 1,
		[Description("СКУД")]
		ASC = 2
	}
}

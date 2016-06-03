using System.ComponentModel;

namespace StrazhAPI.Enums
{
	public enum GuardZoneCommand
	{
		[Description("Поставить на охрану")]
		SetGuard,

		[Description("Снять с охраны")]
		UnsetGuard
	}
}

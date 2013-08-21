using System.ComponentModel;

namespace XFiresecAPI
{
	public enum ClauseOperationType
	{
		[DescriptionAttribute("в любой зоне из")]
		AnyZone,

		[DescriptionAttribute("во всех зонах из")]
		AllZones,

		[DescriptionAttribute("в любом устройстве из")]
		AnyDevice,

		[DescriptionAttribute("во всех устройствах из")]
		AllDevices,

		[DescriptionAttribute("в любом направлении из")]
		AnyDirection,

		[DescriptionAttribute("во всех направлениях из")]
		AllDirections
	}
}
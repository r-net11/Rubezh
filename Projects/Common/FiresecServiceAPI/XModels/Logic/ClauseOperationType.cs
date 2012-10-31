using System.ComponentModel;

namespace XFiresecAPI
{
    public enum ClauseOperationType
    {
		[DescriptionAttribute("во всех зонах из")]
		AllZones,

		[DescriptionAttribute("в любой зоне из")]
		AnyZone,

        [DescriptionAttribute("во всех устройствах из")]
        AllDevices,

        [DescriptionAttribute("в любом устройстве из")]
        AnyDevice,

		[DescriptionAttribute("во всех направлениях из")]
		AllDirections,

		[DescriptionAttribute("в любом направлении из")]
		AnyDirection
    }
}
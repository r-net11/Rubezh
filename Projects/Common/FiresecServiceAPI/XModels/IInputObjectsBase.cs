using System.Collections.Generic;

namespace XFiresecAPI
{
	public interface IInputObjectsBase
	{
		List<XDevice> InputDevices { get; set; }
		List<XZone> InputZones { get; set; }
		List<XDirection> InputDirections { get; set; }
		List<XMPT> InputMPTs { get; set; }
		List<XDelay> InputDelays { get; set; }
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;

namespace XFiresecAPI
{
	public interface IInputObjectsBase
	{
		List<XDevice> InputDevices { get; set; }
		List<XZone> InputZones { get; set; }
		List<XDirection> InputDirections { get; set; }
	}
}
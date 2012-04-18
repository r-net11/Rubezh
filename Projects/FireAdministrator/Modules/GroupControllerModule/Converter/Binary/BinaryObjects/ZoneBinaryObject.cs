using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;

namespace GKModule.Converter.Binary
{
	public class ZoneBinaryObject : BinaryObjectBase
	{
		public ZoneBinaryObject(XZone zone)
		{
			DeviceType = ToBytes((short)0x100);
		}
	}
}
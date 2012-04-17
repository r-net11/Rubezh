using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;

namespace GKModule.Converter
{
	public class GKBinaryObject : BinaryFormatterBase
	{
		XDevice Device;

		public void Initialize(XDevice device)
		{
			Device = device;

			DeviceType = ToBytes(device.Driver.DriverTypeNo);
		}
	}
}
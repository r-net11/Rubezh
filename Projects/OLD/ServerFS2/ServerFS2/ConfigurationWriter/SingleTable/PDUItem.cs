using System.Collections.Generic;
using FiresecAPI.Models;

namespace ServerFS2.ConfigurationWriter
{
	public class PDUItem
	{
		public PDUItem(Device pduDurectionDevice, Device device)
		{
			ParentPanel = device;
			PduDurectionDevice = pduDurectionDevice;
			DevicePDUDirections = new List<DevicePDUDirection>();
		}

		public Device ParentPanel { get; set; }
		public Device PduDurectionDevice { get; set; }
		public List<DevicePDUDirection> DevicePDUDirections { get; set; }
	}

	public class DevicePDUDirection
	{
		public Device Device { get; set; }
		public PDUGroupDevice PDUGroupDevice { get; set; }
		public ByteDescription ReferenceToByteDescriptions { get; set; }
	}
}
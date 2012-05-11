using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class XDeviceLogic
	{
		public XDeviceLogic()
		{
			StateLogics = new List<StateLogic>();
			DependentDevices = new List<XDevice>();
			DependentZones = new List<XZone>();
		}

		public List<XDevice> DependentDevices { get; set; }
		public List<XZone> DependentZones { get; set; }

		[DataMember]
		public List<StateLogic> StateLogics { get; set; }
	}
}
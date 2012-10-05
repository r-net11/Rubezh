using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class XDeviceLogic
	{
		public XDeviceLogic()
		{
            Clauses = new List<XClause>();
			DependentDevices = new List<XDevice>();
			DependentZones = new List<XZone>();
		}

		public List<XDevice> DependentDevices { get; set; }
		public List<XZone> DependentZones { get; set; }

        [DataMember]
        public List<XClause> Clauses { get; set; }
	}
}
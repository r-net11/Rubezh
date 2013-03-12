using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.Models;

namespace XFiresecAPI
{
	[DataContract]
	public class XDeviceLogic
	{
		public XDeviceLogic()
		{
            Clauses = new List<XClause>();
			DependentZones = new List<XZone>();
			DependentDevices = new List<XDevice>();
			DependentDirections = new List<XDirection>();
		}

		public List<XZone> DependentZones { get; set; }
		public List<XDevice> DependentDevices { get; set; }
		public List<XDirection> DependentDirections { get; set; }

        [DataMember]
        public List<XClause> Clauses { get; set; }

		[DataMember]
		public ZoneLogicMROMessageNo ZoneLogicMROMessageNo { get; set; }

		[DataMember]
		public ZoneLogicMROMessageType ZoneLogicMROMessageType { get; set; }
	}
}
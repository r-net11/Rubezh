using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
	[DataContract]
	public class GKReflectionItem
	{
		public GKReflectionItem()
		{
			ZoneUIDs = new List<Guid>();
			GuardZoneUIDs = new List<Guid>();
			DeviceUIDs = new List<Guid>();
			DelayUIDs = new List<Guid>();
			DiretionUIDs = new List<Guid>();
			NSUIDs = new List<Guid>();
			MPTUIDs = new List<Guid>();
			Zones = new List<GKZone>();
			Diretions = new List<GKDirection>();
			GuardZones = new List<GKGuardZone>();
			Devices = new List<GKDevice>();
			Delays = new List<GKDelay>();
			NSs = new List<GKPumpStation>();
			MPTs = new List<GKMPT>();
		}

		[XmlIgnore]
		public List<GKZone> Zones { get; set; }

		[XmlIgnore]
		public List<GKDirection> Diretions { get; set; }

		[XmlIgnore]
		public List<GKGuardZone> GuardZones { get; set; }

		[XmlIgnore]
		public List<GKDevice> Devices { get; set; }

		[XmlIgnore]
		public List<GKDelay> Delays { get; set; }

		[XmlIgnore]
		public List<GKPumpStation> NSs { get; set; }

		[XmlIgnore]
		public List<GKMPT> MPTs { get; set; }

		[DataMember]
		public List<Guid> ZoneUIDs { get; set; }

		[DataMember]
		public List<Guid> GuardZoneUIDs { get; set; }

	    [DataMember]
		public List<Guid> DeviceUIDs { get; set; }

		[DataMember]
		public List<Guid> DelayUIDs { get; set; }

		[DataMember]
		public List<Guid> DiretionUIDs { get; set; }

		[DataMember]
		public List<Guid> NSUIDs { get; set; }

		[DataMember]
		public List<Guid> MPTUIDs { get; set; }

		
	}
}
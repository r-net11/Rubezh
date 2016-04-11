using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

namespace RubezhAPI.GK
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
			GuardZones = new List<GKGuardZone>();
			Devices = new List<GKDevice>();
			Delays = new List<GKDelay>();
			Diretions = new List<GKDirection>();
			NSs = new List<GKPumpStation>();
			MPTs = new List<GKMPT>();
			MirrorUsers = new List<MirrorUser>();
		}

		[XmlIgnore]
		public List<GKBase> GKBases
		{
			get
			{
				var gkBases = new List<GKBase>();
				gkBases.AddRange(Zones);
				gkBases.AddRange(GuardZones);
				gkBases.AddRange(Devices);
				gkBases.AddRange(Delays);
				gkBases.AddRange(Diretions);
				gkBases.AddRange(NSs);
				gkBases.AddRange(MPTs);
				return gkBases;
			}
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
		public List<MirrorUser> MirrorUsers { get; set; }

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
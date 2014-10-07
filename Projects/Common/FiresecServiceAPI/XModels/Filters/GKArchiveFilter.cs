using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	[DataContract]
	public class GKArchiveFilter
	{
		public GKArchiveFilter()
		{
			StartDate = DateTime.Now.AddDays(-1);
			EndDate = DateTime.Now;
			XJournalObjectTypes = new List<GKJournalObjectType>();
			StateClasses = new List<XStateClass>();
			EventNames = new List<string>();
			Descriptions = new List<string>();
			UseDeviceDateTime = false;

			SubsystemTypes = new List<GKSubsystemType>();
			DeviceUIDs = new List<Guid>();
			ZoneUIDs = new List<Guid>();
			DirectionUIDs = new List<Guid>();
			DelayUIDs = new List<Guid>();
			PimUIDs = new List<Guid>();
			PumpStationUIDs = new List<Guid>();
			MPTUIDs = new List<Guid>();
			GuardZoneUIDs = new List<Guid>();
		}

		[DataMember]
		public DateTime StartDate { get; set; }

		[DataMember]
		public DateTime EndDate { get; set; }

		[DataMember]
		public bool UseDeviceDateTime { get; set; }

		[DataMember]
		public List<GKJournalObjectType> XJournalObjectTypes { get; set; }

		[DataMember]
		public List<XStateClass> StateClasses { get; set; }

		[DataMember]
		public List<string> EventNames { get; set; }

		[DataMember]
		public List<string> Descriptions { get; set; }

		[DataMember]
		public List<Guid> DeviceUIDs { get; set; }

		[DataMember]
		public List<Guid> ZoneUIDs { get; set; }

		[DataMember]
		public List<Guid> DirectionUIDs { get; set; }

		[DataMember]
		public List<Guid> PumpStationUIDs { get; set; }

		[DataMember]
		public List<Guid> MPTUIDs { get; set; }

		[DataMember]
		public List<Guid> DelayUIDs { get; set; }

		[DataMember]
		public List<Guid> PimUIDs { get; set; }

		[DataMember]
		public List<Guid> GuardZoneUIDs { get; set; }

		[DataMember]
		public List<GKSubsystemType> SubsystemTypes { get; set; }

		[DataMember]
		public int PageSize { get; set; }
	}
}
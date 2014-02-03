using System;
using System.Collections.Generic;
using XFiresecAPI;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class XArchiveFilter
	{
		public XArchiveFilter()
		{
			StartDate = DateTime.Now.AddDays(-1);
			EndDate = DateTime.Now;
			JournalItemTypes = new List<JournalItemType>();
			StateClasses = new List<XStateClass>();
			EventNames = new List<string>();
			Descriptions = new List<string>();
			UseDeviceDateTime = false;

			SubsystemTypes = new List<XSubsystemType>();
			DeviceUIDs = new List<Guid>();
			ZoneUIDs = new List<Guid>();
			DirectionUIDs = new List<Guid>();
			DelayUIDs = new List<Guid>();
			PimUIDs = new List<Guid>();
			PumpStationUIDs = new List<Guid>();
		}

		[DataMember]
		public DateTime StartDate { get; set; }

		[DataMember]
		public DateTime EndDate { get; set; }

		[DataMember]
		public bool UseDeviceDateTime { get; set; }

		[DataMember]
		public List<JournalItemType> JournalItemTypes { get; set; }

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
		public List<Guid> DelayUIDs { get; set; }

		[DataMember]
		public List<Guid> PimUIDs { get; set; }

		[DataMember]
		public List<XSubsystemType> SubsystemTypes { get; set; }
	}
}
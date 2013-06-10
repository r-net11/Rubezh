using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.Models;

namespace FS2Api
{
	[DataContract]
	public class FS2Callbac
	{
		public FS2Callbac()
		{
			JournalItems = new List<FS2JournalItem>();
			ArchiveJournalItems = new List<FS2JournalItem>();
			ChangedDeviceStates = new List<DeviceState>();
			ChangedDeviceParameters = new List<DeviceState>();
			ChangedZoneStates = new List<ZoneState>();
		}

		[DataMember]
		public List<FS2JournalItem> JournalItems { get; set; }

		[DataMember]
		public List<FS2JournalItem> ArchiveJournalItems { get; set; }

		[DataMember]
		public List<DeviceState> ChangedDeviceStates { get; set; }

		[DataMember]
		public List<DeviceState> ChangedDeviceParameters { get; set; }

		[DataMember]
		public List<ZoneState> ChangedZoneStates { get; set; }

		[DataMember]
		public FS2ProgressInfo FS2ProgressInfo { get; set; }
	}
}
using System;
using System.Runtime.Serialization;
using FiresecAPI;
using FiresecAPI.Models;

namespace FS2Api
{
	[DataContract]
	public class FS2JournalItem
	{
		public FS2JournalItem()
		{
			SubsystemType = SubsystemType.Other;
		}

		[DataMember]
		public int No { get; set; }

		[DataMember]
		public DateTime DeviceTime { get; set; }

		[DataMember]
		public DateTime SystemTime { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public string Detalization { get; set; }

		[DataMember]
		public string DeviceName { get; set; }

		[DataMember]
		public string PanelName { get; set; }

		[DataMember]
		public Guid DeviceUID { get; set; }

		[DataMember]
		public Guid PanelUID { get; set; }

		[DataMember]
		public Guid ZoneUID { get; set; }

		[DataMember]
		public string ZoneName { get; set; }

		[DataMember]
		public int DeviceCategory { get; set; }

		[DataMember]
		public StateType StateType { get; set; }

		[DataMember]
		public SubsystemType SubsystemType { get; set; }

        [DataMember]
		public string UserName { get; set; }

		public string BytesString { get; set; }
		public int EventCode { get; set; }
		public int AdditionalEventCode { get; set; }
		public Device PanelDevice { get; set; }
		public Zone Zone { get; set; }
		public int ZoneNo { get; set; }
		public Device Device { get; set; }
        public GuardUser GuardUser { get; set; }
		public bool HasZone { get; set; }
    }
}
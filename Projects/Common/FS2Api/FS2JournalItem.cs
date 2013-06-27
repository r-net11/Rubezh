using System;
using System.Runtime.Serialization;
using FiresecAPI;
using FiresecAPI.Models;

namespace FS2Api
{
	[DataContract]
	public class FS2JournalItem
	{
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
		public string ZoneName { get; set; }

		[DataMember]
		public int DeviceCategory { get; set; }

		[DataMember]
		public StateType StateType { get; set; }

		[DataMember]
		public SubsystemType SubsystemType { get; set; }

		[DataMember]
		public string UserName { get; set; }



		public int DeviceAddress { get; set; }
		public int ZoneNo { get; set; }
		public int EventClass { get; set; }
		public uint IntDate { get; set; }
		public Device PanelDevice { get; set; }
		public Device Device { get; set; }
		public int StateByte { get; set; }
		public int EventCode { get; set; }
		public int EventChoiceNo { get; set; }
		public string BytesString { get; set; }
	}
}
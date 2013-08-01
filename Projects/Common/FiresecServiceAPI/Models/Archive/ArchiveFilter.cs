using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ArchiveFilter
	{
		public ArchiveFilter()
		{
			Descriptions = new List<string>();
			Subsystems = new List<SubsystemType>();
			DeviceNames = new List<string>();
		}

		public List<DescriptionAndStateInfo> DescriptionAndStateInfos = new List<DescriptionAndStateInfo>();

		[DataMember]
		public bool UseSystemDate { get; set; }

		[DataMember]
		public DateTime StartDate { get; set; }

		[DataMember]
		public DateTime EndDate { get; set; }

		[DataMember]
		public List<string> Descriptions { get; set; }

		[DataMember]
		public List<string> DeviceNames { get; set; }

		[DataMember]
		public List<SubsystemType> Subsystems { get; set; }
	}

	public class DescriptionAndStateInfo
	{
		public string Description { get; set; }
		public StateType StateType { get; set; }
	}
}
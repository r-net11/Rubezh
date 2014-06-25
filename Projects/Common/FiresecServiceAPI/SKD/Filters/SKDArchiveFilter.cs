using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.GK;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class SKDArchiveFilter
	{
		public SKDArchiveFilter()
		{
			StartDate = DateTime.Now.AddDays(-1);
			EndDate = DateTime.Now;
			JournalItemTypes = new List<SKDJournalItemType>();
			StateClasses = new List<XStateClass>();
			EventNames = new List<string>();
			Descriptions = new List<string>();
			UseDeviceDateTime = false;

			SubsystemTypes = new List<SubsystemType>();
			DeviceUIDs = new List<Guid>();
		}

		[DataMember]
		public DateTime StartDate { get; set; }

		[DataMember]
		public DateTime EndDate { get; set; }

		[DataMember]
		public bool UseDeviceDateTime { get; set; }

		[DataMember]
		public List<SKDJournalItemType> JournalItemTypes { get; set; }

		[DataMember]
		public List<XStateClass> StateClasses { get; set; }

		[DataMember]
		public List<string> EventNames { get; set; }

		[DataMember]
		public List<string> Descriptions { get; set; }

		[DataMember]
		public List<Guid> DeviceUIDs { get; set; }

		[DataMember]
		public List<SubsystemType> SubsystemTypes { get; set; }

		[DataMember]
		public int PageSize { get; set; }
	}
}
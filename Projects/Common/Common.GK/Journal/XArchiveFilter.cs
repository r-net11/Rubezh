using System;
using System.Collections.Generic;
using XFiresecAPI;

namespace Common.GK
{
	public class XArchiveFilter
	{
		public XArchiveFilter()
		{
			StartDate = DateTime.Now.AddDays(-1);
			EndDate = DateTime.Now;
			JournalItemTypes = new List<JournalItemType>();
			StateClasses = new List<XStateClass>();
			GKAddresses = new List<string>();
            JournalDescriptionState = new List<JournalDescriptionState>();
			Descriptions = new List<string>();
			UseDeviceDateTime = false;

			DeviceUIDs = new List<Guid>();
			ZoneUIDs = new List<Guid>();
			DirectionUIDs = new List<Guid>();
		}

		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public bool UseDeviceDateTime { get; set; }
		public List<JournalItemType> JournalItemTypes { get; set; }
		public List<XStateClass> StateClasses { get; set; }
		public List<string> GKAddresses { get; set; }
        public List<JournalDescriptionState> JournalDescriptionState { get; set; }
		public List<string> Descriptions { get; set; }
		public List<Guid> DeviceUIDs { get; set; }
		public List<Guid> ZoneUIDs { get; set; }
		public List<Guid> DirectionUIDs { get; set; }
	}
}
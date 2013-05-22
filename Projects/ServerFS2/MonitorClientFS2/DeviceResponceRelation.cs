using System.Collections.Generic;
using FiresecAPI.Models;

namespace MonitorClientFS2
{
	public class DeviceResponceRelation
	{
		public DeviceResponceRelation()
		{
			;
		}
		public DeviceResponceRelation(Device device)
		{
			Device = device;
			Requests = new List<Request>();
			LastDisplayedRecord = XmlJournalHelper.GetLastId(device);
			FirstDisplayedRecord = -1;
		}
		public Device Device { get; set; }
		public List<Request> Requests { get; set; }
		public bool UnAnswered { get { return Requests.Count > 0; } }
		public int FirstDisplayedRecord { get; set; }
		int lastDisplayedRecord;
		public int LastDisplayedRecord
		{
			get { return lastDisplayedRecord; }
			set
			{
				lastDisplayedRecord = value;
				XmlJournalHelper.SetLastId(Device, value);
			}
		}
	}

	public class SecDeviceResponceRelation : DeviceResponceRelation
	{
		public SecDeviceResponceRelation(Device device)
		{
			Device = device;
			Requests = new List<Request>();
			LastDisplayedRecord = XmlJournalHelper.GetLastId(device);
			LastDisplayedSecRecord = XmlJournalHelper.GetLastSecId(device);
			FirstDisplayedRecord = -1;
		}
		int lastDisplayedSecRecord;
		public int LastDisplayedSecRecord
		{
			get { return lastDisplayedSecRecord; }
			set
			{
				lastDisplayedSecRecord = value;
				XmlJournalHelper.SetLastId(Device, value);
			}
		}
	}
}
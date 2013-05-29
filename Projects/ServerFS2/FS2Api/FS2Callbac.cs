using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FS2Api
{
	[DataContract]
	public class FS2Callbac
	{
		public FS2Callbac()
		{
			JournalRecords = new List<FS2JournalItem>();
		}

		[DataMember]
		public List<FS2JournalItem> JournalRecords { get; set; }

		[DataMember]
		public string CoreCongig { get; set; }

		[DataMember]
		public string CoreDeviceParams { get; set; }

		[DataMember]
		public FS2ProgressInfo FS2ProgressInfo { get; set; }

		[DataMember]
		public bool IsConnectionLost { get; set; }
	}
}
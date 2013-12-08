using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class GKCallbackResult
	{
		[DataMember]
		public List<JournalItem> JournalItems { get; set; }

		[DataMember]
		public GKStates GKStates { get; set; }

		public GKCallbackResult()
		{
			JournalItems = new List<JournalItem>();
			GKStates = new GKStates();
		}
	}
}
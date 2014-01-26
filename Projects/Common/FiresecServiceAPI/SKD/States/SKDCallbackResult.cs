using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class SKDCallbackResult
	{
		[DataMember]
		public List<SKDJournalItem> JournalItems { get; set; }

		[DataMember]
		public SKDStates GKStates { get; set; }

		public SKDCallbackResult()
		{
			JournalItems = new List<SKDJournalItem>();
			GKStates = new SKDStates();
		}
	}
}
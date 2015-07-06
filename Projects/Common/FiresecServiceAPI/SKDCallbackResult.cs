using FiresecAPI.Journal;
using FiresecAPI.SKD;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	[DataContract]
	public class SKDCallbackResult
	{
		[DataMember]
		public List<JournalItem> JournalItems { get; set; }

		[DataMember]
		public SKDStates SKDStates { get; set; }

		public SKDCallbackResult()
		{
			JournalItems = new List<JournalItem>();
			SKDStates = new SKDStates();
		}
	}
}
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class SKDCallbackResult
	{
		[DataMember]
		public List<SKDJournalItem> JournalItems { get; set; }

		[DataMember]
		public SKDStates SKDStates { get; set; }

		public SKDCallbackResult()
		{
			JournalItems = new List<SKDJournalItem>();
			SKDStates = new SKDStates();
		}
	}
}
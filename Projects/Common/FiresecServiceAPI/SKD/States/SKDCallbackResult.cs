using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class SKDCallbackResult
	{
		[DataMember]
		public SKDStates SKDStates { get; set; }

		public List<JournalItem> JournalItems { get; set; }

		public SKDCallbackResult()
		{
			SKDStates = new SKDStates();
			JournalItems = new List<JournalItem>();
		}
	}
}
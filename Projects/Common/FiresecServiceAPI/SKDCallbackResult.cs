using System.Collections.Generic;
using System.Runtime.Serialization;
using StrazhAPI.Journal;
using StrazhAPI.SKD;

namespace StrazhAPI.GK
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
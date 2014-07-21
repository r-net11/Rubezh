using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.Journal;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class SKDCallbackResult
	{
		public SKDCallbackResult()
		{
			SKDStates = new SKDStates();
		}

		[DataMember]
		public SKDStates SKDStates { get; set; }
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class SKDReaderProperty
	{
		public SKDReaderProperty()
		{
			DUControl = new SKDReaderDUProperty();
			DUConversation = new SKDReaderDUProperty();
			VerificationTime = 5;
		}

		[DataMember]
		public SKDReaderDUProperty DUControl { get; set; }

		[DataMember]
		public SKDReaderDUProperty DUConversation { get; set; }

		[DataMember]
		public int VerificationTime { get; set; }
	}
}
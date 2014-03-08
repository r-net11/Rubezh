using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace FiresecAPI
{
	[DataContract]
	public class Photo : SKDModelBase
	{
		[DataMember]
		public byte[] Data { get; set; }
	}
}

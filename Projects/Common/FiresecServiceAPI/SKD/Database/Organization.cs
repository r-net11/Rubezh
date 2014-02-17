using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class Organization : SKDModelBase
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }
	}
}

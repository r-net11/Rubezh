﻿using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	[DataContract]
	public class GKAdditionalState
	{
		[DataMember]
		public XStateClass StateClass { get; set; }

		[DataMember]
		public string Name { get; set; }
	}
}
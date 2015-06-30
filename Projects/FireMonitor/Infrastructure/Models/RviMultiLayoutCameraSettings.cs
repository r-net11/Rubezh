using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Infrastructure.Models
{
	[DataContract]
	public class RviMultiLayoutCameraSettings
	{
		public RviMultiLayoutCameraSettings()
		{
			Dictionary = new Dictionary<string, Guid>();
		}

		[DataMember]
		public Dictionary<string, Guid> Dictionary;

		[DataMember]
		public MultiGridType MultiGridType;
	}
}
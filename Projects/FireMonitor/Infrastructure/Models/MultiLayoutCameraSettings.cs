using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Infrastructure.Models
{
	[DataContract]
	public class MultiLayoutCameraSettings
	{
		public MultiLayoutCameraSettings()
		{
			Dictionary = new Dictionary<string, Guid>();
		}

		[DataMember]
		public Dictionary<string, Guid> Dictionary;
	}
}
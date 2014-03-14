using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

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

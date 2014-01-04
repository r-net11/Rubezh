using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class SKDDevice
	{
		public SKDDevice()
		{
			UID = Guid.NewGuid();
		}

		[DataMember]
		public Guid UID { get; set; }
	}
}
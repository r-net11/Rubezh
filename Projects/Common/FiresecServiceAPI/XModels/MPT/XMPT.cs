using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class XMPT
	{
		public XMPT()
		{
			BaseUID = Guid.NewGuid();
		}

		[DataMember]
		public Guid BaseUID { get; set; }

		[DataMember]
		public string Name { get; set; }
	}
}
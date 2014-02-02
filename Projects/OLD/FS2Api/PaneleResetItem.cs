using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FS2Api
{
	[DataContract]
	public class PanelResetItem
	{
		public PanelResetItem()
		{
			Ids = new HashSet<string>();
		}

		[DataMember]
		public Guid PanelUID { get; set; }

		[DataMember]
		public HashSet<string> Ids { get; set; }
	}
}
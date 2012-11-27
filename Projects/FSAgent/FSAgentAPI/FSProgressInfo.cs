using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FSAgentAPI
{
	[DataContract]
	public class FSProgressInfo
	{
		[DataMember]
		public int Stage { get; set; }

		[DataMember]
		public string Comment { get; set; }

		[DataMember]
		public int PercentComplete { get; set; }

		[DataMember]
		public int BytesRW { get; set; }
	}
}
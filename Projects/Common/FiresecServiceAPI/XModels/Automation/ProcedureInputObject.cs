using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using FiresecAPI.GK;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ProcedureInputObject
	{
		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public ProcedureObjectType ProcedureObjectType { get; set; }

		[DataMember]
		public XStateClass StateClass { get; set; }
	}
}
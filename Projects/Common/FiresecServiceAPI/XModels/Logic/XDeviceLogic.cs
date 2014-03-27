using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.Models;

namespace XFiresecAPI
{
	[DataContract]
	public class XDeviceLogic
	{
		public XDeviceLogic()
		{
			Clauses = new List<XClause>();
			OffClauses = new List<XClause>();
		}

		[DataMember]
		public List<XClause> Clauses { get; set; }

		[DataMember]
		public List<XClause> OffClauses { get; set; }

		[DataMember]
		public ZoneLogicMROMessageNo ZoneLogicMROMessageNo { get; set; }

		[DataMember]
		public ZoneLogicMROMessageType ZoneLogicMROMessageType { get; set; }
	}
}
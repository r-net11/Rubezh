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
			ClausesGroup = new XClauseGroup();
			OffClausesGroup = new XClauseGroup();
		}

		[DataMember]
		public XClauseGroup ClausesGroup { get; set; }

		[DataMember]
		public XClauseGroup OffClausesGroup { get; set; }

		[DataMember]
		public ZoneLogicMROMessageNo ZoneLogicMROMessageNo { get; set; }

		[DataMember]
		public ZoneLogicMROMessageType ZoneLogicMROMessageType { get; set; }
	}
}
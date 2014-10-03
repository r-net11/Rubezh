using System.Runtime.Serialization;
using FiresecAPI.Models;

namespace FiresecAPI.GK
{
	[DataContract]
	public class GKDeviceLogic
	{
		public GKDeviceLogic()
		{
			ClausesGroup = new GKClauseGroup();
			OffClausesGroup = new GKClauseGroup();
		}

		[DataMember]
		public GKClauseGroup ClausesGroup { get; set; }

		[DataMember]
		public GKClauseGroup OffClausesGroup { get; set; }

		[DataMember]
		public ZoneLogicMROMessageNo ZoneLogicMROMessageNo { get; set; }

		[DataMember]
		public ZoneLogicMROMessageType ZoneLogicMROMessageType { get; set; }
	}
}
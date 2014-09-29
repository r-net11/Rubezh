using System.Runtime.Serialization;
using FiresecAPI.Models;

namespace FiresecAPI.GK
{
	public class XDeviceLogic
	{
		public XDeviceLogic()
		{
			ClausesGroup = new XClauseGroup();
			OffClausesGroup = new XClauseGroup();
		}

		public XClauseGroup ClausesGroup { get; set; }
		public XClauseGroup OffClausesGroup { get; set; }
		public ZoneLogicMROMessageNo ZoneLogicMROMessageNo { get; set; }
		public ZoneLogicMROMessageType ZoneLogicMROMessageType { get; set; }
	}
}
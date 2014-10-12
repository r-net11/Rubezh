using System.Runtime.Serialization;
using FiresecAPI.Models;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Логика включения объектов ГК
	/// </summary>
	[DataContract]
	public class GKDeviceLogic
	{
		public GKDeviceLogic()
		{
			ClausesGroup = new GKClauseGroup();
			OffClausesGroup = new GKClauseGroup();
		}

		/// <summary>
		/// Группа условий для включения
		/// </summary>
		[DataMember]
		public GKClauseGroup ClausesGroup { get; set; }

		/// <summary>
		/// Группа условий для выключения
		/// </summary>
		[DataMember]
		public GKClauseGroup OffClausesGroup { get; set; }

		[DataMember]
		public ZoneLogicMROMessageNo ZoneLogicMROMessageNo { get; set; }

		[DataMember]
		public ZoneLogicMROMessageType ZoneLogicMROMessageType { get; set; }
	}
}
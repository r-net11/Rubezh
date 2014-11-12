using System.Runtime.Serialization;
using FiresecAPI.Models;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Логика включения объектов ГК
	/// </summary>
	[DataContract]
	public class GKLogic
	{
		public GKLogic()
		{
			OnClausesGroup = new GKClauseGroup();
			OffClausesGroup = new GKClauseGroup();
			StopClausesGroup = new GKClauseGroup();
		}

		/// <summary>
		/// Группа условий для включения
		/// </summary>
		[DataMember]
		public GKClauseGroup OnClausesGroup { get; set; }

		/// <summary>
		/// Группа условий для выключения
		/// </summary>
		[DataMember]
		public GKClauseGroup OffClausesGroup { get; set; }

		/// <summary>
		/// Группа условий для остановки
		/// </summary>
		[DataMember]
		public GKClauseGroup StopClausesGroup { get; set; }

		[DataMember]
		public ZoneLogicMROMessageNo ZoneLogicMROMessageNo { get; set; }

		[DataMember]
		public ZoneLogicMROMessageType ZoneLogicMROMessageType { get; set; }
	}
}
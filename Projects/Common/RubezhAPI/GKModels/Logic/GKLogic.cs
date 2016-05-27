using RubezhAPI.GK;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RubezhAPI.GK
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
			On2ClausesGroup = new GKClauseGroup();
			OffClausesGroup = new GKClauseGroup();
			OnNowClausesGroup = new GKClauseGroup();
			OffNowClausesGroup = new GKClauseGroup();
			StopClausesGroup = new GKClauseGroup();
			UseOffCounterLogic = true;
			RedIndicatorLogic = new ICIndicatorLogic();
			GreenIndicatorLogic = new ICIndicatorLogic();
			YellowIndicatorLogic = new ICIndicatorLogic();
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
		/// Группа условий для включения 2
		/// </summary>
		[DataMember]
		public GKClauseGroup On2ClausesGroup { get; set; }

		/// <summary>
		/// Группа условий для включения немедленно
		/// </summary>
		[DataMember]
		public GKClauseGroup OnNowClausesGroup { get; set; }

		/// <summary>
		/// Группа условий для выключения немедленно
		/// </summary>
		[DataMember]
		public GKClauseGroup OffNowClausesGroup { get; set; }

		/// <summary>
		/// Группа условий для остановки
		/// </summary>
		[DataMember]
		public GKClauseGroup StopClausesGroup { get; set; }

		/// <summary>
		/// Использовать условие включения в качестве противоположного для условия выключения
		/// </summary>
		[DataMember]
		public bool UseOffCounterLogic { get; set; }

		#region IC
		public ICIndicatorLogic RedIndicatorLogic { get; set; }
		public ICIndicatorLogic GreenIndicatorLogic { get; set; }
		public ICIndicatorLogic YellowIndicatorLogic { get; set; }
		#endregion

		public List<GKBase> GetObjects()
		{
			var result = new List<GKBase>();
			result.AddRange(OnClausesGroup.GetObjects());
			result.AddRange(On2ClausesGroup.GetObjects());
			if (!UseOffCounterLogic)
				result.AddRange(OffClausesGroup.GetObjects());
			result.AddRange(OnNowClausesGroup.GetObjects());
			result.AddRange(OffNowClausesGroup.GetObjects());
			result.AddRange(StopClausesGroup.GetObjects());
			result.AddRange(RedIndicatorLogic.GetObjects());
			result.AddRange(GreenIndicatorLogic.GetObjects());
			result.AddRange(YellowIndicatorLogic.GetObjects());
			return result;
		}
	}
}
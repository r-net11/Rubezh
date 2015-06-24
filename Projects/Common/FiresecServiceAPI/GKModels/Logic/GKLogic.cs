using System.Collections.Generic;
using System.Runtime.Serialization;

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
			OnNowClausesGroup = new GKClauseGroup();
			OffNowClausesGroup = new GKClauseGroup();
			StopClausesGroup = new GKClauseGroup();
			UseOffCounterLogic = true;
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

		public List<GKBase> GetObjects()
		{
			var result = new List<GKBase>();
			result.AddRange(OnClausesGroup.GetObjects());
			result.AddRange(OffClausesGroup.GetObjects());
			result.AddRange(OnNowClausesGroup.GetObjects());
			result.AddRange(OffNowClausesGroup.GetObjects());
			result.AddRange(StopClausesGroup.GetObjects());
			return result;
		}

		public List<GKClause> GetAllClauses()
		{
			var allClauses = new List<GKClause>();
			allClauses.AddRange(GetAllClausesOfGroup(OffClausesGroup));
			allClauses.AddRange(GetAllClausesOfGroup(OffNowClausesGroup));
			allClauses.AddRange(GetAllClausesOfGroup(OnClausesGroup));
			allClauses.AddRange(GetAllClausesOfGroup(OnNowClausesGroup));
			allClauses.AddRange(GetAllClausesOfGroup(StopClausesGroup));
			return allClauses;
		}

		List<GKClause> GetAllClausesOfGroup(GKClauseGroup gkClauseGroup)
		{
			var clauses = new List<GKClause>(gkClauseGroup.Clauses);
			gkClauseGroup.ClauseGroups.ForEach(x => clauses.AddRange(GetAllClausesOfGroup(x)));
			return clauses;
		}

	}
}
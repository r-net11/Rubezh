using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Группа условий
	/// </summary>
	[DataContract]
	public class GKClauseGroup
	{
		public GKClauseGroup()
		{
			ClauseGroups = new List<GKClauseGroup>();
			Clauses = new List<GKClause>();
			ClauseJounOperationType = ClauseJounOperationType.Or;
		}

		/// <summary>
		/// Дочерняя группа условий
		/// </summary>
		[DataMember]
		public List<GKClauseGroup> ClauseGroups { get; set; }

		/// <summary>
		/// Условия
		/// </summary>
		[DataMember]
		public List<GKClause> Clauses { get; set; }

		/// <summary>
		/// Тип объединения условий
		/// </summary>
		[DataMember]
		public ClauseJounOperationType ClauseJounOperationType { get; set; }

		public GKClauseGroup Clone()
		{
			var result = new GKClauseGroup();
			result.ClauseJounOperationType = ClauseJounOperationType;

			foreach (var clause in Clauses)
			{
				var clonedClause = new GKClause()
				{
					ClauseConditionType = clause.ClauseConditionType,
					ClauseOperationType = clause.ClauseOperationType,
					StateType = clause.StateType,
					DeviceUIDs = clause.DeviceUIDs,
					ZoneUIDs = clause.ZoneUIDs,
					GuardZoneUIDs = clause.GuardZoneUIDs,
					DirectionUIDs = clause.DirectionUIDs,
					Devices = clause.Devices,
					Zones = clause.Zones,
					GuardZones = clause.GuardZones,
					Directions = clause.Directions,
				};
				result.Clauses.Add(clonedClause);
			}

			foreach (var clauseGroup in ClauseGroups)
			{
				result.ClauseGroups.Add(clauseGroup.Clone());
			}

			return result;
		}

		public List<GKBase> GetObjects()
		{
			var result = new List<GKBase>();
			foreach (var clause in Clauses)
			{
				clause.Devices.ForEach(x => result.Add(x));
				clause.Zones.ForEach(x => result.Add(x));
				clause.GuardZones.ForEach(x => result.Add(x));
				clause.Directions.ForEach(x => result.Add(x));
				clause.Delays.ForEach(x => result.Add(x));
				clause.MPTs.ForEach(x => result.Add(x));
				clause.Devices.ForEach(x => result.Add(x));
			}
			foreach (var clauseGroup in ClauseGroups)
			{
				result.AddRange(clauseGroup.GetObjects());
			}
			return result;
		}
	}
}
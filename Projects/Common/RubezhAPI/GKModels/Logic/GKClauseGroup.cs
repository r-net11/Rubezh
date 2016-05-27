using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RubezhAPI.GK
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

		public bool IsNotEmpty() { return ClauseGroups.Count + GetObjects().Count > 0; }

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
					DelayUIDs = clause.DelayUIDs,
					DoorUIDs = clause.DoorUIDs,
					MPTUIDs = clause.MPTUIDs,
					PumpStationsUIDs = clause.PumpStationsUIDs,
					Devices = clause.Devices,
					Zones = clause.Zones,
					GuardZones = clause.GuardZones,
					Directions = clause.Directions,
					Delays = clause.Delays,
					Doors = clause.Doors,
					MPTs = clause.MPTs,
					PumpStations = clause.PumpStations,
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
				clause.Devices.ForEach(result.Add);
				clause.Zones.ForEach(result.Add);
				clause.GuardZones.ForEach(result.Add);
				clause.Directions.ForEach(result.Add);
				clause.Delays.ForEach(result.Add);
				clause.Doors.ForEach(result.Add);
				clause.MPTs.ForEach(result.Add);
				clause.PumpStations.ForEach(result.Add);
			}
			foreach (var clauseGroup in ClauseGroups)
			{
				result.AddRange(clauseGroup.GetObjects());
			}
			return result;
		}
	}
}
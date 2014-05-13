using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class XClauseGroup
	{
		public XClauseGroup()
		{
			ClauseGroups = new List<XClauseGroup>();
			Clauses = new List<XClause>();
			ClauseJounOperationType = ClauseJounOperationType.Or;
		}

		[DataMember]
		public List<XClauseGroup> ClauseGroups { get; set; }

		[DataMember]
		public List<XClause> Clauses { get; set; }

		[DataMember]
		public ClauseJounOperationType ClauseJounOperationType { get; set; }

		public XClauseGroup Clone()
		{
			var result = new XClauseGroup();
			result.ClauseJounOperationType = ClauseJounOperationType;

			foreach (var clause in Clauses)
			{
				var clonedClause = new XClause()
				{
					ClauseConditionType = clause.ClauseConditionType,
					ClauseOperationType = clause.ClauseOperationType,
					StateType = clause.StateType,
					DeviceUIDs = clause.DeviceUIDs,
					ZoneUIDs = clause.ZoneUIDs,
					DirectionUIDs = clause.DirectionUIDs,
					Devices = clause.Devices,
					Zones = clause.Zones,
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
	}
}
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;
using FiresecAPI.GK;

namespace FiresecClient
{
	public partial class GKManager
	{
		public static string GetPresentationLogic(GKLogic logic)
		{
			return GetPresentationLogic(logic.OnClausesGroup);
		}

		static string GetPresentationLogic(GKClauseGroup clauseGroup)
		{
			var stringBuilder = new StringBuilder();
			var index = 0;
			foreach (var clause in clauseGroup.Clauses)
			{
				if (index > 0)
					stringBuilder.Append(" " + clauseGroup.ClauseJounOperationType.ToDescription() + " ");

				if (clause.ClauseConditionType == ClauseConditionType.IfNot)
					stringBuilder.Append("Если НЕ ");
				stringBuilder.Append(GKClause.ClauseToString(clause.ClauseOperationType, clause.StateType) + " ");
				stringBuilder.Append(clause.ClauseOperationType.ToDescription() + " ");
				stringBuilder.Append(GetCommaSeparatedDevices(clause.Devices));
				stringBuilder.Append(GetCommaSeparatedObjects(new List<ModelBase>(clause.MPTs), new List<ModelBase>(MPTs)));
				stringBuilder.Append(GetCommaSeparatedObjects(new List<ModelBase>(clause.Doors), new List<ModelBase>(Doors)));
				index++;
			}

			foreach (var group in clauseGroup.ClauseGroups)
			{
				if (index > 0)
					stringBuilder.Append(" " + clauseGroup.ClauseJounOperationType.ToDescription() + " ");

				var groupString = GetPresentationLogic(group);

				stringBuilder.Append("(" + groupString + ")");
				index++;
			}
			return stringBuilder.ToString();
		}

		public static string GetCommaSeparatedDevices(IEnumerable<GKDevice> devices)
		{
			var stringBuilder = new StringBuilder();
			var deviceCount = 0;
			foreach (var device in devices)
			{
				if (deviceCount > 0)
					stringBuilder.Append(", ");
				stringBuilder.Append(device.PresentationName);
				deviceCount++;
			}
			return stringBuilder.ToString();
		}

		public static string GetCommaSeparatedObjects(List<ModelBase> baseObjects, List<ModelBase> allBaseObjects)
		{
			if (baseObjects.Count == 1)
			{
				return baseObjects[0].PresentationName;
			}
			if (baseObjects.Count() > 0)
			{
				var orderedDirections = baseObjects.OrderBy(x => x.No).Select(x => x.No).ToList();
				int prevDirectionNo = orderedDirections[0];
				List<List<int>> groupOfDirections = new List<List<int>>();

				for (int i = 0; i < orderedDirections.Count; i++)
				{
					var directionNo = orderedDirections[i];
					var haveObjectsBetween = allBaseObjects.Any(x => (x.No > prevDirectionNo) && (x.No < directionNo));
					if (haveObjectsBetween)
					{
						groupOfDirections.Add(new List<int> { directionNo });
					}
					else
					{
						if (groupOfDirections.Count == 0)
						{
							groupOfDirections.Add(new List<int> { directionNo });
						}
						else
						{
							groupOfDirections.Last().Add(directionNo);
						}
					}
					prevDirectionNo = directionNo;
				}

				var presenrationDirections = new StringBuilder();
				for (int i = 0; i < groupOfDirections.Count; i++)
				{
					var directionGroup = groupOfDirections[i];

					if (i > 0)
						presenrationDirections.Append(", ");

					presenrationDirections.Append(directionGroup.First().ToString());
					if (directionGroup.Count > 1)
					{
						if (directionGroup.Count > 2)
							presenrationDirections.Append(" - " + directionGroup.Last());
						else
							presenrationDirections.Append(", " + directionGroup.Last());
					}
				}

				return presenrationDirections.ToString();
			}
			return "";
		}
	}
}
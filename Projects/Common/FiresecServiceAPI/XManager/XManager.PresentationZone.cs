using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;
using FiresecAPI.GK;

namespace FiresecClient
{
	public partial class XManager
	{
		public static string GetPresentationZone(XDevice device)
		{
			if (device.Driver.HasZone)
			{
				var stringBuilder = new StringBuilder();
				if (device.Zones == null)
					device.Zones = new List<XZone>();
				if (device.Zones.Count == 1)
				{
					stringBuilder.Append(device.Zones[0].PresentationName);
				}
				else if (device.Zones.Count > 1)
				{
					stringBuilder.Append("зоны: ");
					stringBuilder.Append(GetCommaSeparatedObjects(new List<INamedBase>(device.Zones)));
				}
				return stringBuilder.ToString();
			}

			if (device.Driver.HasLogic && device.DeviceLogic != null)
				return GetPresentationZone(device.DeviceLogic);

			return "";
		}

		public static string GetPresentationZone(XDeviceLogic DeviceLogic)
		{
			return GetPresentationZone(DeviceLogic.ClausesGroup);
		}

		static string GetPresentationZone(XClauseGroup clauseGroup)
		{
			var stringBuilder = new StringBuilder();
			var index = 0;
			foreach (var clause in clauseGroup.Clauses)
			{
				if (index > 0)
					stringBuilder.Append(" " + clauseGroup.ClauseJounOperationType.ToDescription() + " ");

				if (clause.ClauseConditionType == ClauseConditionType.IfNot)
					stringBuilder.Append("Если НЕ ");
				stringBuilder.Append(XClause.ClauseToString(clause.ClauseOperationType, clause.StateType) + " ");
				stringBuilder.Append(clause.ClauseOperationType.ToDescription() + " ");
				stringBuilder.Append(GetCommaSeparatedDevices(clause.Devices));
				stringBuilder.Append(GetCommaSeparatedObjects(new List<INamedBase>(clause.Zones)));
				stringBuilder.Append(GetCommaSeparatedObjects(new List<INamedBase>(clause.GuardZones)));
				stringBuilder.Append(GetCommaSeparatedObjects(new List<INamedBase>(clause.Directions)));
				stringBuilder.Append(GetCommaSeparatedObjects(new List<INamedBase>(clause.MPTs)));
				stringBuilder.Append(GetCommaSeparatedObjects(new List<INamedBase>(clause.Delays)));
				index++;
			}

			foreach (var group in clauseGroup.ClauseGroups)
			{
				if (index > 0)
					stringBuilder.Append(" " + clauseGroup.ClauseJounOperationType.ToDescription() + " ");

				var groupString = GetPresentationZone(group);

				stringBuilder.Append("(" + groupString + ")");
				index++;
			}
			return stringBuilder.ToString();
		}

		public static string GetCommaSeparatedDevices(IEnumerable<XDevice> devices)
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

		public static string GetCommaSeparatedObjects(List<INamedBase> baseObject)
		{
			if (baseObject.Count == 1)
			{
				return baseObject[0].PresentationName;
			}
			if (baseObject.Count() > 0)
			{
				var orderedDirections = baseObject.OrderBy(x => x.No).Select(x => x.No).ToList();
				int prevDirectionNo = orderedDirections[0];
				List<List<int>> groupOfDirections = new List<List<int>>();

				for (int i = 0; i < orderedDirections.Count; i++)
				{
					var directionNo = orderedDirections[i];
					var haveDirectionsBetween = Directions.Any(x => (x.No > prevDirectionNo) && (x.No < directionNo));
					if (haveDirectionsBetween)
					{
						groupOfDirections.Add(new List<int>() { directionNo });
					}
					else
					{
						if (groupOfDirections.Count == 0)
						{
							groupOfDirections.Add(new List<int>() { directionNo });
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
						presenrationDirections.Append(" - " + directionGroup.Last().ToString());
					}
				}

				return presenrationDirections.ToString();
			}
			return "";
		}
	}
}
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;
using FiresecAPI.GK;

namespace FiresecClient
{
	public partial class GKManager
	{
		public static string GetPresentationZoneAndGuardZoneOrLogic(GKDevice device)
		{
			if (device.Driver.HasMirror)
			{
				var stringBuilder = new StringBuilder();
				if (device.GKReflectionItem.Zones != null)
				{
					if (device.GKReflectionItem.Zones.Count == 1)
						stringBuilder.Append(device.GKReflectionItem.Zones[0].PresentationName);
					if (device.GKReflectionItem.Zones.Count > 1)
					{
						var zones = new List<ModelBase>(device.GKReflectionItem.Zones);
						stringBuilder.Append("пожарные зоны: ");
						stringBuilder.Append(GetCommaSeparatedObjects(new List<ModelBase>(device.GKReflectionItem.Zones), new List<ModelBase>(device.GKReflectionItem.Zones)));
					}
				}
				if (device.GKReflectionItem.GuardZones != null)
				{
					if (device.GKReflectionItem.GuardZones.Count == 1)
					{
						stringBuilder.Append(device.GKReflectionItem.GuardZones[0].PresentationName);
					}
					if (device.GKReflectionItem.GuardZones.Count > 1)
					{
						stringBuilder.Append("охранные зоны: ");
						stringBuilder.Append(GetCommaSeparatedObjects(new List<ModelBase>(device.GKReflectionItem.GuardZones), new List<ModelBase>(device.GKReflectionItem.GuardZones)));
					}
				}
				if (device.GKReflectionItem.Devices != null && device.GKReflectionItem.Devices.Count>0)
				{				
					stringBuilder.Append("устройсва: ");
					foreach (var item in device.GKReflectionItem.Devices)
	                   {
		                 stringBuilder.Append(item.ShortName +" ");
	                   }				
				}
				if (device.GKReflectionItem.Delays != null)
				{
					if (device.GKReflectionItem.Delays.Count == 1)
					{
						stringBuilder.Append(device.GKReflectionItem.Delays[0].PresentationName);
					}
					if (device.GKReflectionItem.Delays.Count > 1)
					{
						stringBuilder.Append("задержки: ");
						stringBuilder.Append(GetCommaSeparatedObjects(new List<ModelBase>(device.GKReflectionItem.Delays), new List<ModelBase>(device.GKReflectionItem.Delays)));
					}
				}

				if (device.GKReflectionItem.Diretions != null)
				{
					if (device.GKReflectionItem.Diretions.Count == 1)
					{
						stringBuilder.Append(device.GKReflectionItem.Diretions[0].PresentationName);
					}
					if (device.GKReflectionItem.Diretions.Count > 1)
					{
						stringBuilder.Append("направления: ");
						stringBuilder.Append(GetCommaSeparatedObjects(new List<ModelBase>(device.GKReflectionItem.Diretions), new List<ModelBase>(device.GKReflectionItem.Diretions)));
					}
				}
				if (device.GKReflectionItem.MPTs != null)
				{
					if (device.GKReflectionItem.MPTs.Count == 1)
					{
						stringBuilder.Append(device.GKReflectionItem.MPTs[0].PresentationName);
					}
					if (device.GKReflectionItem.MPTs.Count > 1)
					{
						stringBuilder.Append("МПТ: ");
						stringBuilder.Append(GetCommaSeparatedObjects(new List<ModelBase>(device.GKReflectionItem.MPTs), new List<ModelBase>(device.GKReflectionItem.MPTs)));
					}
				}

				if (device.GKReflectionItem.NSs != null)
				{
					if (device.GKReflectionItem.NSs.Count == 1)
					{
						stringBuilder.Append(device.GKReflectionItem.MPTs[0].PresentationName);
					}
					if (device.GKReflectionItem.NSs.Count > 1)
					{
						stringBuilder.Append("НС: ");
						stringBuilder.Append(GetCommaSeparatedObjects(new List<ModelBase>(device.GKReflectionItem.NSs), new List<ModelBase>(device.GKReflectionItem.NSs)));
					}
				}
					
				return stringBuilder.ToString();
			}

			if (device.Driver.HasZone || device.Driver.HasGuardZone )
			{
				var stringBuilder = new StringBuilder();
				if (device.Zones == null)
					device.Zones = new List<GKZone>();
				if (device.GuardZones == null)
					device.GuardZones = new List<GKGuardZone>();
				if (device.Zones.Count + device.GuardZones.Count == 1)
				{
					stringBuilder.Append(device.Zones.Count == 1 ? device.Zones[0].PresentationName : device.GuardZones[0].PresentationName);
				}

				else if (device.Zones.Count + device.GuardZones.Count > 1)
				{
					var allZones = new List<ModelBase>(device.Zones);
					allZones.AddRange(new List<ModelBase>(device.GuardZones));
					stringBuilder.Append("зоны: ");
					if (device.Zones.Count > 0)
						stringBuilder.Append(GetCommaSeparatedObjects(new List<ModelBase>(device.Zones), new List<ModelBase>(Zones)));
					if (device.GuardZones.Count > 0)
					{
						if (device.Zones.Count > 0)
							stringBuilder.Append("; ");
						stringBuilder.Append(GetCommaSeparatedObjects(new List<ModelBase>(device.GuardZones), new List<ModelBase>(GuardZones)));
					}
				}
				return stringBuilder.ToString();
			}

			if (device.Driver.HasLogic && device.Logic != null)
				return GetPresentationLogic(device.Logic);

			return "";
		}

		public static string GetPresentationZoneOrLogic(GKDevice device)
		{			
			if (device.Driver.HasZone)
			{
				var stringBuilder = new StringBuilder();
				if (device.Zones == null)
					device.Zones = new List<GKZone>();
				if (device.Zones.Count == 1)
				{
					stringBuilder.Append(device.Zones[0].PresentationName);
				}
				else if (device.Zones.Count > 1)
				{
					stringBuilder.Append("зоны: ");
					stringBuilder.Append(GetCommaSeparatedObjects(new List<ModelBase>(device.Zones), new List<ModelBase>(Zones)));
				}
				return stringBuilder.ToString();
			}
			if (device.Driver.HasLogic && device.Logic != null)
				return GetPresentationLogic(device.Logic);

			return "";
		}

		public static string GetPresentationGuardZone(GKDevice device)
		{
			var stringBuilder = new StringBuilder();
			if (device.GuardZones == null)
				device.GuardZones = new List<GKGuardZone>();
			if (device.GuardZones.Count == 1)
			{
				stringBuilder.Append(device.GuardZones[0].PresentationName);
			}
			else if (device.GuardZones.Count > 1)
			{
				stringBuilder.Append("зоны: ");
				stringBuilder.Append(GetCommaSeparatedObjects(new List<ModelBase>(device.GuardZones), new List<ModelBase>(GuardZones)));
			}
			return stringBuilder.ToString();
		}

		public static string GetPresentationLogic(GKLogic logic)
		{
			return GetPresentationLogic(logic.OnClausesGroup);
		}

		public static string GetPresentationLogic(GKClauseGroup clauseGroup)
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
				stringBuilder.Append(GetCommaSeparatedObjects(new List<ModelBase>(clause.Zones), new List<ModelBase>(Zones)));
				stringBuilder.Append(GetCommaSeparatedObjects(new List<ModelBase>(clause.GuardZones), new List<ModelBase>(GuardZones)));
				stringBuilder.Append(GetCommaSeparatedObjects(new List<ModelBase>(clause.Directions), new List<ModelBase>(Directions)));
				stringBuilder.Append(GetCommaSeparatedObjects(new List<ModelBase>(clause.MPTs), new List<ModelBase>(MPTs)));
				stringBuilder.Append(GetCommaSeparatedObjects(new List<ModelBase>(clause.Delays), new List<ModelBase>(Delays)));
				stringBuilder.Append(GetCommaSeparatedObjects(new List<ModelBase>(clause.Doors), new List<ModelBase>(Doors)));
				stringBuilder.Append(GetCommaSeparatedObjects(new List<ModelBase>(clause.PumpStations), new List<ModelBase>(PumpStations)));
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
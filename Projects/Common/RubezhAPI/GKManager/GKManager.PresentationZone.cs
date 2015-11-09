﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using RubezhAPI;
using RubezhAPI.GK;
using System.IO;

namespace RubezhClient
{
	public partial class GKManager
	{
		public static string GetPresentationZoneAndGuardZoneOrLogic(GKDevice device)
		{
			if (device.Driver.HasMirror)
			{
				List<StringBuilder> list = new List<StringBuilder>();
				if (device.GKMirrorItem.Zones != null && device.GKMirrorItem.Zones.Count>0)
				{
					StringBuilder stringbuilderZones = new StringBuilder();
					if (device.GKMirrorItem.Zones.Count == 1)
						stringbuilderZones.Append("пожарные зоны: " + device.GKMirrorItem.Zones[0].PresentationName);
					if (device.GKMirrorItem.Zones.Count > 1)
					{
						var zones = new List<ModelBase>(device.GKMirrorItem.Zones);
						stringbuilderZones.Append("пожарные зоны: ");
						stringbuilderZones.Append(GetCommaSeparatedObjects(new List<ModelBase>(device.GKMirrorItem.Zones), new List<ModelBase>(device.GKMirrorItem.Zones)));
					}
					list.Add(stringbuilderZones);
				}
				if (device.GKMirrorItem.GuardZones != null && device.GKMirrorItem.GuardZones.Count>0)
				{
					StringBuilder stringbiulderGuardZones = new StringBuilder();
					if (device.GKMirrorItem.GuardZones.Count == 1)
					{
						stringbiulderGuardZones.Append("охранные зоны: " + device.GKMirrorItem.GuardZones[0].PresentationName);
					}
					if (device.GKMirrorItem.GuardZones.Count > 1)
					{
						stringbiulderGuardZones.Append("охранные зоны: ");
						stringbiulderGuardZones.Append(GetCommaSeparatedObjects(new List<ModelBase>(device.GKMirrorItem.GuardZones), new List<ModelBase>(device.GKMirrorItem.GuardZones)));
					}
					list.Add(stringbiulderGuardZones);
				}
				if (device.GKMirrorItem.Devices != null && device.GKMirrorItem.Devices.Count>0)
				{
					StringBuilder stringbuilderDevice = new StringBuilder();
					stringbuilderDevice.Append("устройсва: ");
					foreach (var item in device.GKMirrorItem.Devices)
					   {
						   stringbuilderDevice.Append(item.ShortName + ", "); 
	                   }
					list.Add(stringbuilderDevice);
				}
				if (device.GKMirrorItem.Delays != null && device.GKMirrorItem.Delays.Count>0)
				{
					StringBuilder stringbuilderDalays = new StringBuilder();
					if (device.GKMirrorItem.Delays.Count == 1)
					{
						stringbuilderDalays.Append("задержки: "+ device.GKMirrorItem.Delays[0].PresentationName);
					}
					if (device.GKMirrorItem.Delays.Count > 1)
					{
						stringbuilderDalays.Append("задержки: ");
						stringbuilderDalays.Append(GetCommaSeparatedObjects(new List<ModelBase>(device.GKMirrorItem.Delays), new List<ModelBase>(device.GKMirrorItem.Delays)));
					}
					list.Add(stringbuilderDalays);
				}

				if (device.GKMirrorItem.Diretions != null && device.GKMirrorItem.Diretions.Count > 0)
				{
					StringBuilder stringbuilderDerictions = new StringBuilder();
					if (device.GKMirrorItem.Diretions.Count == 1)
					{
						stringbuilderDerictions.Append("направления: "+ device.GKMirrorItem.Diretions[0].PresentationName);
					}
					if (device.GKMirrorItem.Diretions.Count > 1)
					{
						stringbuilderDerictions.Append("направления: ");
						stringbuilderDerictions.Append(GetCommaSeparatedObjects(new List<ModelBase>(device.GKMirrorItem.Diretions), new List<ModelBase>(device.GKMirrorItem.Diretions)));
					}
					list.Add(stringbuilderDerictions);
				}
				if (device.GKMirrorItem.MPTs != null && device.GKMirrorItem.MPTs.Count>0)
				{
					StringBuilder stringboilderMPTs = new StringBuilder();
					if (device.GKMirrorItem.MPTs.Count == 1)
					{
						stringboilderMPTs.Append("МПТ: "+device.GKMirrorItem.MPTs[0].PresentationName);
					}
					if (device.GKMirrorItem.MPTs.Count > 1)
					{
						stringboilderMPTs.Append("МПТ: ");
						stringboilderMPTs.Append(GetCommaSeparatedObjects(new List<ModelBase>(device.GKMirrorItem.MPTs), new List<ModelBase>(device.GKMirrorItem.MPTs)));
					}
					list.Add(stringboilderMPTs);
				}

				if (device.GKMirrorItem.NSs != null && device.GKMirrorItem.NSs.Count>0)
				{
					StringBuilder stringbuilderNSs = new StringBuilder();
					if (device.GKMirrorItem.NSs.Count == 1)
					{
						stringbuilderNSs.Append("НС: " + device.GKMirrorItem.NSs[0].PresentationName);	
					}
					if (device.GKMirrorItem.NSs.Count > 1)
					{
						stringbuilderNSs.Append("НС: ");
						stringbuilderNSs.Append(GetCommaSeparatedObjects(new List<ModelBase>(device.GKMirrorItem.NSs), new List<ModelBase>(device.GKMirrorItem.NSs)));					
					}
					list.Add(stringbuilderNSs);					
				}	
		
				return string.Join("\n",list);
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
			List<StringBuilder> list = new List<StringBuilder>();
			if (logic.OnClausesGroup.ClauseGroups.Count > 0 || logic.OnClausesGroup.Clauses.Count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("Условие включения: ");
				stringBuilder.Append(GetPresentationLogic(logic.OnClausesGroup));
				list.Add(stringBuilder);
			}

			if (logic.OnNowClausesGroup.ClauseGroups.Count > 0 || logic.OnNowClausesGroup.Clauses.Count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("Условие включения немедленно: ");
				stringBuilder.Append(GetPresentationLogic(logic.OnNowClausesGroup));
				list.Add(stringBuilder);
			}

			if (logic.OffClausesGroup.ClauseGroups.Count > 0 || logic.OffClausesGroup.Clauses.Count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("Условие выключения: ");
				stringBuilder.Append(GetPresentationLogic(logic.OffClausesGroup));
				list.Add(stringBuilder);
			}

			if (logic.OffNowClausesGroup.ClauseGroups.Count > 0 || logic.OffNowClausesGroup.Clauses.Count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("Условие выключения немедленно: ");
				stringBuilder.Append(GetPresentationLogic(logic.OffNowClausesGroup));
				list.Add(stringBuilder);
			}

			if (logic.StopClausesGroup.ClauseGroups.Count > 0 || logic.StopClausesGroup.Clauses.Count > 0 )
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("Условие остановки: ");
				stringBuilder.Append(GetPresentationLogic(logic.StopClausesGroup));
				list.Add(stringBuilder);
			}
			return string.Join("\n", list);
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
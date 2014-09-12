using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Automation;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using FiresecClient;
using FiresecService.Service;
using Infrastructure.Common.Video.RVI_VSS;
using Property = FiresecAPI.Automation.Property;

namespace FiresecService.Processor
{
	public static class ProcedureHelper
	{
		static ProcedureHelper()
		{
			WinFormsPlayers = new List<WinFormsPlayer>();
		}

		static public Procedure Procedure { get; set; }

		public static bool Compare(ProcedureStep procedureStep, Procedure procedure, List<Argument> arguments)
		{
			var conditionArguments = procedureStep.ConditionArguments;
			var result = conditionArguments.JoinOperator == JoinOperator.And;
			foreach (var condition in conditionArguments.Conditions)
			{
				int variable1 = GetValue<int>(condition.Variable1);
				int variable2 = GetValue<int>(condition.Variable2);
				switch (condition.ConditionType)
				{
					case ConditionType.IsEqual:
						result = conditionArguments.JoinOperator == JoinOperator.And ? result & (variable1 == variable2) : result | (variable1 == variable2);
						break;
					case ConditionType.IsLess:
						result = conditionArguments.JoinOperator == JoinOperator.And ? result & (variable1 < variable2) : result | (variable1 < variable2);
						break;
					case ConditionType.IsMore:
						result = conditionArguments.JoinOperator == JoinOperator.And ? result & (variable1 > variable2) : result | (variable1 > variable2);
						break;
					case ConditionType.IsNotEqual:
						result = conditionArguments.JoinOperator == JoinOperator.And ? result & (variable1 != variable2) : result | (variable1 != variable2);
						break;
					case ConditionType.IsNotLess:
						result = conditionArguments.JoinOperator == JoinOperator.And ? result & (variable1 >= variable2) : result | (variable1 >= variable2);
						break;
					case ConditionType.IsNotMore:
						result = conditionArguments.JoinOperator == JoinOperator.And ? result & (variable1 <= variable2) : result | (variable1 <= variable2);
						break;
				}
			}
			return result;
		}

		public static AutomationCallbackResult ShowMessage(ProcedureStep procedureStep, Procedure procedure)
		{
			var automationCallbackResult = new AutomationCallbackResult();
			//var sendMessageArguments = procedureStep.SendMessageArguments;
			//if (sendMessageArguments.VariableScope == VariableScope.IsValue)
			//    automationCallbackResult.Message = procedureStep.SendMessageArguments.Message;
			//if (sendMessageArguments.VariableScope == VariableScope.IsLocalVariable)
			//{
			//    var localVariable = procedure.Variables.FirstOrDefault(x => x.Uid == sendMessageArguments.VariableUid) ??
			//        procedure.Arguments.FirstOrDefault(x => x.Uid == sendMessageArguments.VariableUid);
			//    if (localVariable != null)
			//        automationCallbackResult.Message = localVariable.CurrentValue;
			//}
			//if (sendMessageArguments.VariableScope == VariableScope.IsGlobalVariable)
			//{
			//    var globalVariable = ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.GlobalVariables.FirstOrDefault(x => x.Uid == sendMessageArguments.GlobalVariableUid);
			//    if (globalVariable != null)
			//        automationCallbackResult.Message = globalVariable.CurrentValue;
			//}
			return automationCallbackResult;
		}

		public static void Calculate(ProcedureStep procedureStep, Procedure procedure, List<Argument> arguments)
		{
			var arithmeticArguments = procedureStep.ArithmeticArguments;
			object variable1;
			object variable2;
			var resultVariable = GetAllVariables(procedure).FirstOrDefault(x => x.Uid == arithmeticArguments.Result.VariableUid);
			switch (arithmeticArguments.ExplicitType)
			{
				case ExplicitType.Boolean:
					{
						variable1 = GetValue<bool>(arithmeticArguments.Variable1, procedure, arguments);
						variable2 = GetValue<bool>(arithmeticArguments.Variable2, procedure, arguments);
						bool result = false;
						if (arithmeticArguments.ArithmeticOperationType == ArithmeticOperationType.And)
							result = (bool)variable1 & (bool)variable2;
						if (arithmeticArguments.ArithmeticOperationType == ArithmeticOperationType.Or)
							result = (bool)variable1 || (bool)variable2;
						resultVariable.VariableItem.BoolValue = result;
						break;
					}

				case ExplicitType.Integer:
					{
						variable1 = GetValue<int>(arithmeticArguments.Variable1, procedure, arguments);
						variable2 = GetValue<int>(arithmeticArguments.Variable2, procedure, arguments);
						int result = 0;
						if (arithmeticArguments.ArithmeticOperationType == ArithmeticOperationType.Add)
							result = (int)variable1 + (int)variable2;
						if (arithmeticArguments.ArithmeticOperationType == ArithmeticOperationType.Sub)
							result = (int)variable1 - (int)variable2;
						if (arithmeticArguments.ArithmeticOperationType == ArithmeticOperationType.Multi)
							result = (int)variable1 * (int)variable2;
						if ((arithmeticArguments.ArithmeticOperationType == ArithmeticOperationType.Div) && ((int)variable2 != 0))
							result = (int)variable1 / (int)variable2;
						resultVariable.VariableItem.IntValue = result;
						break;
					}

				case ExplicitType.DateTime:
					{
						variable1 = GetValue<DateTime>(arithmeticArguments.Variable1, procedure, arguments);
						variable2 = GetValue<int>(arithmeticArguments.Variable2, procedure, arguments);
						var result = new DateTime();
						switch (arithmeticArguments.TimeType)
						{
							case TimeType.Sec:
								result = (DateTime)variable1 + TimeSpan.FromSeconds((int)variable2);
								break;
							case TimeType.Min:
								result = (DateTime)variable1 + TimeSpan.FromMinutes((int)variable2);
								break;
							case TimeType.Hour:
								result = (DateTime)variable1 + TimeSpan.FromHours((int)variable2);
								break;
							case TimeType.Day:
								result = (DateTime)variable1 + TimeSpan.FromDays((int)variable2);
								break;
						}
						resultVariable.VariableItem.DateTimeValue = result;
						break;
					}
			}
		}

		static T GetValue<T>(ArithmeticParameter arithmeticParameter, Procedure procedure, List<Argument> arguments)
		{
			T result = default(T);
			var variable = GetAllVariables(procedure).FirstOrDefault(x => x.Uid == arithmeticParameter.VariableUid);
			if (typeof(T) == typeof(bool))
			{
				result = (T) Convert.ChangeType(variable.VariableItem.BoolValue, typeof(T));
			}
			if (typeof(T) == typeof(int))
			{
				result = (T)Convert.ChangeType(variable.VariableItem.IntValue, typeof(T));
			}
			if (typeof(T) == typeof(DateTime))
			{
				result = (T)Convert.ChangeType(variable.VariableItem.DateTimeValue, typeof(T));
			}
			if (typeof(T) == typeof(string))
			{
				result = (T)Convert.ChangeType(variable.VariableItem.StringValue, typeof(T));
			}
			return result;
		}

		static List<Variable> GetAllVariables(Procedure procedure)
		{
			var allVariables = new List<Variable>(ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.GlobalVariables);
			allVariables.AddRange(procedure.Variables);
			allVariables.AddRange(procedure.Arguments);
			return allVariables;
		}

		public static void FindObjects(ProcedureStep procedureStep, Procedure procedure)
		{
			var findObjectArguments = procedureStep.FindObjectArguments;
			var variable = procedure.Variables.FirstOrDefault(x => x.Uid == findObjectArguments.ResultUid);
			if (findObjectArguments.JoinOperator == JoinOperator.Or)
				FindObjectsOr(variable, findObjectArguments.FindObjectConditions);
			else
				FindObjectsAnd(variable, findObjectArguments.FindObjectConditions);
		}

		static object GetPropertyValue(ref Guid itemUid, Property property, object item)
		{
			var propertyValue = new object();
			if (item is XDevice)
			{
				switch (property)
				{
					case Property.ShleifNo:
						propertyValue = (item as XDevice).ShleifNo;
						break;
					case Property.IntAddress:
						propertyValue = (item as XDevice).IntAddress;
						break;
					case Property.State:
						propertyValue = (int)(item as XDevice).State.StateClass;
						break;
					case Property.Type:
						propertyValue = (item as XDevice).Driver.Name.Trim();
						break;
					case Property.Description:
						propertyValue = (item as XDevice).Description.Trim();
						break;
				}
				itemUid = (item as XDevice).UID;
			}

			if (item is XZone)
			{
				switch (property)
				{
					case Property.No:
						propertyValue = (item as XZone).No;
						break;
					case Property.Type:
						propertyValue = (int)(item as XZone).ObjectType;
						break;
					case Property.State:
						propertyValue = (int)(item as XZone).State.StateClass;
						break;
					case Property.Description:
						propertyValue = (item as XZone).Description.Trim();
						break;
				}
				itemUid = (item as XZone).UID;
			}

			if (item is XDirection)
			{
				switch (property)
				{
					case Property.No:
						propertyValue = (item as XDirection).No;
						break;
					case Property.Delay:
						propertyValue = (int)(item as XDirection).Delay;
						break;
					case Property.Hold:
						propertyValue = (int)(item as XDirection).Hold;
						break;
					case Property.DelayRegime:
						propertyValue = (int)(item as XDirection).DelayRegime;
						break;
					case Property.Description:
						propertyValue = (item as XDirection).Description.Trim();
						break;
				}
				itemUid = (item as XDirection).UID;
			}

			return propertyValue;
		}

		static void InitializeItems(ref IEnumerable<object> items, ref Variable result)
		{
			var variableItems = new List<VariableItem>();
			if (result.ObjectType == ObjectType.Device)
			{
				items = new List<XDevice>(XManager.DeviceConfiguration.Devices);
				foreach (var objectUid in new List<Guid>(XManager.DeviceConfiguration.Devices.Select(x => x.UID)))
					variableItems.Add(new VariableItem { UidValue = objectUid });
			}
			if (result.ObjectType == ObjectType.Zone)
			{
				items = new List<XZone>(XManager.Zones);
				foreach (var objectUid in new List<Guid>(XManager.Zones.Select(x => x.UID)))
					variableItems.Add(new VariableItem { UidValue = objectUid });
			}
			if (result.ObjectType == ObjectType.Direction)
			{
				items = new List<XDirection>(XManager.Directions);
				foreach (var objectUid in new List<Guid>(XManager.Directions.Select(x => x.UID)))
					variableItems.Add(new VariableItem { UidValue = objectUid });
			}
			result.VariableItems = variableItems;
		}

		static void InitializeItem(ref object item, Guid itemUid, ObjectType objectType)
		{
			if (objectType == ObjectType.Device)
				item = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == itemUid);
			if (objectType == ObjectType.Zone)
				item = XManager.Zones.FirstOrDefault(x => x.UID == itemUid);
			if (objectType == ObjectType.Direction)
				item = XManager.Directions.FirstOrDefault(x => x.UID == itemUid);
		}

		static void FindObjectsOr(Variable result, IEnumerable<FindObjectCondition> findObjectConditions)
		{
			IEnumerable<object> items = new List<object>();
			InitializeItems(ref items, ref result);
			var resultObjects = new List<object>();
			var itemUid = new Guid();

			foreach (var findObjectCondition in findObjectConditions)
			{
				foreach (var item in items)
				{
					if (resultObjects.Contains(item))
						continue;
					var propertyValue = GetPropertyValue(ref itemUid, findObjectCondition.Property, item);
					var conditionValue = GetValue<object>(findObjectCondition.Variable2);
					var comparer = Compare(propertyValue, conditionValue, findObjectCondition.ConditionType);
					if ((comparer != null) && (comparer.Value))
					{
						resultObjects.Add(item);
						result.VariableItems.Add(new VariableItem { UidValue = itemUid });
					}
				}
			}
		}

		static void FindObjectsAnd(Variable result, IEnumerable<FindObjectCondition> findObjectConditions)
		{
			IEnumerable<object> items = new List<object>();
			InitializeItems(ref items, ref result);
			var resultObjects = new List<object>(items);
			var tempObjects = new List<object>(resultObjects);
			var itemUid = new Guid();

			foreach (var findObjectCondition in findObjectConditions)
			{
				foreach (var item in resultObjects)
				{
					var propertyValue = GetPropertyValue(ref itemUid, findObjectCondition.Property, item);
					var conditionValue = GetValue<object>(findObjectCondition.Variable2);
					var comparer = Compare(propertyValue, conditionValue, findObjectCondition.ConditionType);
					if ((comparer != null) && (!comparer.Value))
					{
						tempObjects.Remove(item);
						result.VariableItems.RemoveAll(x => x.UidValue == itemUid);
					}
				}
				resultObjects = new List<object>(tempObjects);
			}
		}

		static bool? Compare(object param1, object param2, ConditionType conditionType)
		{
			if (param1.GetType() == typeof(Enum) || param1.GetType() == typeof(int))
			{
				return (((conditionType == ConditionType.IsEqual) && ((int)param1 == (int)param2))
					|| ((conditionType == ConditionType.IsNotEqual) && ((int)param1 != (int)param2))
					|| ((conditionType == ConditionType.IsMore) && ((int)param1 > (int)param2))
					|| ((conditionType == ConditionType.IsNotMore) && ((int)param1 <= (int)param2))
					|| ((conditionType == ConditionType.IsLess) && ((int)param1 < (int)param2))
					|| ((conditionType == ConditionType.IsNotLess) && ((int)param1 >= (int)param2)));
			}
			if (param1.GetType() == typeof(string))
			{
				return (((conditionType == ConditionType.StartsWith) && (((string)param1).StartsWith((string)param2)))
					|| ((conditionType == ConditionType.EndsWith) && (((string)param1).EndsWith((string)param2)))
					|| ((conditionType == ConditionType.Contains) && (((string)param1).Contains((string)param2))));
			}
			return null;
		}

		public static void ControlGKDevice(ProcedureStep procedureStep)
		{
			var device = XManager.Devices.FirstOrDefault(x => x.UID == procedureStep.ControlGKDeviceArguments.Variable1.VariableItem.UidValue);
			if (device == null)
				return;
			FiresecServiceManager.SafeFiresecService.GKExecuteDeviceCommand(device.BaseUID, procedureStep.ControlGKDeviceArguments.Command);
		}

		public static List<WinFormsPlayer> WinFormsPlayers { get; private set; }
		public static void ControlCamera(ProcedureStep procedureStep)
		{
			var camera = ConfigurationCashHelper.SystemConfiguration.AllCameras.FirstOrDefault(x => x.UID == procedureStep.ControlCameraArguments.Variable1.VariableItem.UidValue);
			if (camera == null)
				return;
			if (procedureStep.ControlCameraArguments.CameraCommandType == CameraCommandType.StartRecord)
			{
				if (WinFormsPlayers.Any(x => x.Camera == camera))
					return;
				var winFormsPlayer = new WinFormsPlayer();
				winFormsPlayer.Camera = camera;
				winFormsPlayer.Connect(camera);
				if (winFormsPlayer.StartRecord(camera, camera.ChannelNumber))
					WinFormsPlayers.Add(winFormsPlayer);
			}
			else
			{
				var winFormsPlayer = WinFormsPlayers.FirstOrDefault(x => x.Camera == camera);
				if (winFormsPlayer != null)
				{
					winFormsPlayer.StopRecord();
					WinFormsPlayers.Remove(winFormsPlayer);
				}
			}
		}

		public static void ControlFireZone(ProcedureStep procedureStep)
		{
			var zoneUid = GetValue<Guid>(procedureStep.ControlGKFireZoneArguments.Variable1);
			var zoneCommandType = procedureStep.ControlGKFireZoneArguments.ZoneCommandType;
			if (zoneCommandType == ZoneCommandType.Ignore)
				FiresecServiceManager.SafeFiresecService.GKSetIgnoreRegime(zoneUid, XBaseObjectType.Zone);
			if (zoneCommandType == ZoneCommandType.ResetIgnore)
				FiresecServiceManager.SafeFiresecService.GKSetAutomaticRegime(zoneUid, XBaseObjectType.Zone);
			if (zoneCommandType == ZoneCommandType.Reset)
				FiresecServiceManager.SafeFiresecService.GKReset(zoneUid, XBaseObjectType.Zone);
		}

		public static void ControlGuardZone(ProcedureStep procedureStep)
		{
			var zoneUid = GetValue<Guid>(procedureStep.ControlGKGuardZoneArguments.Variable1);
			var guardZoneCommandType = procedureStep.ControlGKGuardZoneArguments.GuardZoneCommandType;
			if (guardZoneCommandType == GuardZoneCommandType.Automatic)
				FiresecServiceManager.SafeFiresecService.GKSetAutomaticRegime(zoneUid, XBaseObjectType.GuardZone);
			if (guardZoneCommandType == GuardZoneCommandType.Ignore)
				FiresecServiceManager.SafeFiresecService.GKSetIgnoreRegime(zoneUid, XBaseObjectType.GuardZone);
			if (guardZoneCommandType == GuardZoneCommandType.Manual)
				FiresecServiceManager.SafeFiresecService.GKSetManualRegime(zoneUid, XBaseObjectType.GuardZone);
			if (guardZoneCommandType == GuardZoneCommandType.Reset)
				FiresecServiceManager.SafeFiresecService.GKReset(zoneUid, XBaseObjectType.GuardZone);
			if (guardZoneCommandType == GuardZoneCommandType.TurnOff)
				FiresecServiceManager.SafeFiresecService.GKTurnOff(zoneUid, XBaseObjectType.GuardZone);
			if (guardZoneCommandType == GuardZoneCommandType.TurnOn)
				FiresecServiceManager.SafeFiresecService.GKTurnOn(zoneUid, XBaseObjectType.GuardZone);
			if (guardZoneCommandType == GuardZoneCommandType.TurnOnNow)
				FiresecServiceManager.SafeFiresecService.GKTurnOnNow(zoneUid, XBaseObjectType.GuardZone);
		}

		public static void ControlDirection(ProcedureStep procedureStep)
		{
			var directionUid = GetValue<Guid>(procedureStep.ControlDirectionArguments.Variable1);
			var directionCommandType = procedureStep.ControlDirectionArguments.DirectionCommandType;
			if (directionCommandType == DirectionCommandType.Automatic)
				FiresecServiceManager.SafeFiresecService.GKSetAutomaticRegime(directionUid, XBaseObjectType.Direction);
			if (directionCommandType == DirectionCommandType.ForbidStart)
				FiresecServiceManager.SafeFiresecService.GKStop(directionUid, XBaseObjectType.Direction);
			if (directionCommandType == DirectionCommandType.Ignore)
				FiresecServiceManager.SafeFiresecService.GKSetIgnoreRegime(directionUid, XBaseObjectType.Direction);
			if (directionCommandType == DirectionCommandType.Manual)
				FiresecServiceManager.SafeFiresecService.GKSetManualRegime(directionUid, XBaseObjectType.Direction);
			if (directionCommandType == DirectionCommandType.TurnOff)
				FiresecServiceManager.SafeFiresecService.GKTurnOff(directionUid, XBaseObjectType.Direction);
			if (directionCommandType == DirectionCommandType.TurnOn)
				FiresecServiceManager.SafeFiresecService.GKTurnOn(directionUid, XBaseObjectType.Direction);
			if (directionCommandType == DirectionCommandType.TurnOnNow)
				FiresecServiceManager.SafeFiresecService.GKTurnOnNow(directionUid, XBaseObjectType.Direction);
		}

		public static void ControlDoor(ProcedureStep procedureStep)
		{
			var door = SKDManager.Doors.FirstOrDefault(x => x.UID == procedureStep.ControlDoorArguments.Variable1.VariableItem.UidValue);
			if (door == null)
				return;
			if (procedureStep.ControlDoorArguments.DoorCommandType == DoorCommandType.Open)
				FiresecServiceManager.SafeFiresecService.SKDOpenDoor(door.UID);
			if (procedureStep.ControlDoorArguments.DoorCommandType == DoorCommandType.Close)
				FiresecServiceManager.SafeFiresecService.SKDCloseDoor(door.UID);
			if (procedureStep.ControlDoorArguments.DoorCommandType == DoorCommandType.OpenForever)
				FiresecServiceManager.SafeFiresecService.SKDOpenDoorForever(door.UID);
			if (procedureStep.ControlDoorArguments.DoorCommandType == DoorCommandType.CloseForever)
				FiresecServiceManager.SafeFiresecService.SKDCloseDoorForever(door.UID);
		}

		public static void ControlSKDZone(ProcedureStep procedureStep)
		{
			var sKDZone = SKDManager.Zones.FirstOrDefault(x => x.UID == procedureStep.ControlSKDZoneArguments.Variable1.VariableItem.UidValue);
			if (sKDZone == null)
				return;
			if (procedureStep.ControlSKDZoneArguments.SKDZoneCommandType == SKDZoneCommandType.Open)
				FiresecServiceManager.SafeFiresecService.SKDOpenZone(sKDZone.UID);
			if (procedureStep.ControlSKDZoneArguments.SKDZoneCommandType == SKDZoneCommandType.Close)
				FiresecServiceManager.SafeFiresecService.SKDCloseZone(sKDZone.UID);
			if (procedureStep.ControlSKDZoneArguments.SKDZoneCommandType == SKDZoneCommandType.OpenForever)
				FiresecServiceManager.SafeFiresecService.SKDOpenZoneForever(sKDZone.UID);
			if (procedureStep.ControlSKDZoneArguments.SKDZoneCommandType == SKDZoneCommandType.CloseForever)
				FiresecServiceManager.SafeFiresecService.SKDCloseZoneForever(sKDZone.UID);
			if (procedureStep.ControlSKDZoneArguments.SKDZoneCommandType == SKDZoneCommandType.DetectEmployees)
				return; // TODO
		}

		public static void IncrementValue(ProcedureStep procedureStep)
		{
			var incrementValueArguments = procedureStep.IncrementValueArguments;
			var globalVariable = ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.GlobalVariables.FirstOrDefault
				(x => x.Uid == incrementValueArguments.Variable1.VariableUid);
			if (globalVariable == null)
				return;
			globalVariable.VariableItem.IntValue = incrementValueArguments.IncrementType == IncrementType.Inc ? globalVariable.VariableItem.IntValue + 1 : globalVariable.VariableItem.IntValue - 1;
		}

		public static void SetValue(ProcedureStep procedureStep)
		{
			var setValueArguments = procedureStep.SetValueArguments;
			var globalVariable = ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.GlobalVariables.FirstOrDefault
				(x => x.Uid == setValueArguments.Result.VariableUid);
			if (globalVariable == null)
				return;
			//globalVariable.IntValue = setValueArguments.Value;
		}

		static T GetValue<T>(ArithmeticParameter arithmeticParameter)
		{
			var result = new object();
			var variableScope = arithmeticParameter.VariableScope;
			var explicitType = arithmeticParameter.ExplicitType;
			var enumType = arithmeticParameter.EnumType;
			var variableItem = arithmeticParameter.VariableItem;
			if (variableScope != VariableScope.ExplicitValue)
				variableItem = GetAllVariables(Procedure).FirstOrDefault(x => x.Uid == arithmeticParameter.VariableUid).VariableItem;
			if (explicitType == ExplicitType.Boolean)
				result = variableItem.BoolValue;
			if (explicitType == ExplicitType.DateTime)
				result = variableItem.DateTimeValue;
			if (explicitType == ExplicitType.Integer)
				result = variableItem.IntValue;
			if (explicitType == ExplicitType.String)
				result = variableItem.StringValue;
			if (explicitType == ExplicitType.Object)
				result = variableItem.UidValue;
			if (explicitType == ExplicitType.Enum)
			{
				if (enumType == EnumType.DriverType)
					result = variableItem.DriverTypeValue;
				if (enumType == EnumType.StateType)
					result = variableItem.StateTypeValue;
			}
			return (T)result;
		}
	}
}
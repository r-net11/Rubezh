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
using ValueType = FiresecAPI.Automation.ValueType;

namespace FiresecService.Processor
{
	public static class ProcedureHelper
	{
		static ProcedureHelper()
		{
			WinFormsPlayers = new List<WinFormsPlayer>();
		}

		public static bool Compare(ProcedureStep procedureStep, Procedure procedure, List<Argument> arguments)
		{
			var conditionArguments = procedureStep.ConditionArguments;
			var result = conditionArguments.JoinOperator == JoinOperator.And;
			foreach (var condition in conditionArguments.Conditions)
			{
				int variable1 = GetValue<int>(condition.Variable1, procedure, arguments);
				int variable2 = GetValue<int>(condition.Variable2, procedure, arguments);
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

		public static void GetString(ProcedureStep procedureStep, Procedure procedure)
		{
			var getStringArguments = procedureStep.GetStringArguments;
			var resultVariable = procedure.Variables.FirstOrDefault(x => x.Uid == getStringArguments.ResultVariableUid) ??
					procedure.Arguments.FirstOrDefault(x => x.Uid == getStringArguments.ResultVariableUid);
			var variable = procedure.Variables.FirstOrDefault(x => x.Uid == getStringArguments.VariableUid) ??
				procedure.Arguments.FirstOrDefault(x => x.Uid == getStringArguments.VariableUid);
			if ((resultVariable == null) || (variable == null))
				return;
			int intPropertyValue = 0;
			string stringPropertyValue = "";
			var item = new object();
			var itemUid = Guid.Empty;
			InitializeItem(ref item, variable.UidValue, variable.ObjectType);
			InitializeProperties(ref intPropertyValue, ref stringPropertyValue, ref itemUid, getStringArguments.Property, item);
			if (getStringArguments.StringOperation == StringOperation.Is)
				resultVariable.VariableItems = new List<VariableItem>();
			if (getStringArguments.Property != Property.Description)
				resultVariable.VariableItems.Add(new VariableItem { StringValue = intPropertyValue.ToString() });
			else
				resultVariable.VariableItems.Add(new VariableItem { StringValue = stringPropertyValue });
		}

		public static AutomationCallbackResult ShowMessage(ProcedureStep procedureStep, Procedure procedure)
		{
			var automationCallbackResult = new AutomationCallbackResult();
			//var sendMessageArguments = procedureStep.SendMessageArguments;
			//if (sendMessageArguments.VariableType == VariableType.IsValue)
			//    automationCallbackResult.Message = procedureStep.SendMessageArguments.Message;
			//if (sendMessageArguments.VariableType == VariableType.IsLocalVariable)
			//{
			//    var localVariable = procedure.Variables.FirstOrDefault(x => x.Uid == sendMessageArguments.VariableUid) ??
			//        procedure.Arguments.FirstOrDefault(x => x.Uid == sendMessageArguments.VariableUid);
			//    if (localVariable != null)
			//        automationCallbackResult.Message = localVariable.CurrentValue;
			//}
			//if (sendMessageArguments.VariableType == VariableType.IsGlobalVariable)
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
			switch (arithmeticArguments.ArithmeticValueType)
			{
				case ValueType.Boolean:
					{
						variable1 = GetValue<bool>(arithmeticArguments.Variable1, procedure, arguments);
						variable2 = GetValue<bool>(arithmeticArguments.Variable2, procedure, arguments);
						bool result = false;
						if (arithmeticArguments.ArithmeticOperationType == ArithmeticOperationType.And)
							result = (bool)variable1 & (bool)variable2;
						if (arithmeticArguments.ArithmeticOperationType == ArithmeticOperationType.Or)
							result = (bool)variable1 || (bool)variable2;
						resultVariable.BoolValue = result;
						break;
					}

				case ValueType.Integer:
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
						resultVariable.IntValue = result;
						break;
					}

				case ValueType.DateTime:
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
						resultVariable.DateTimeValue = result;
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
				result = (T) Convert.ChangeType(variable.BoolValue, typeof(T));
			}
			if (typeof(T) == typeof(int))
			{
				result = (T)Convert.ChangeType(variable.IntValue, typeof(T));
			}
			if (typeof(T) == typeof(DateTime))
			{
				result = (T)Convert.ChangeType(variable.DateTimeValue, typeof(T));
			}
			if (typeof(T) == typeof(string))
			{
				result = (T)Convert.ChangeType(variable.StringValue, typeof(T));
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

		static void InitializeProperties(ref int intPropertyValue, ref string stringPropertyValue, ref Guid itemUid, Property property, object item)
		{
			if (item is XDevice)
			{
				switch (property)
				{
					case Property.ShleifNo:
						intPropertyValue = (item as XDevice).ShleifNo;
						break;
					case Property.IntAddress:
						intPropertyValue = (item as XDevice).IntAddress;
						break;
					case Property.DeviceState:
						intPropertyValue = (int)(item as XDevice).State.StateClass;
						break;
					case Property.Type:
						stringPropertyValue = (item as XDevice).Driver.Name.Trim();
						break;
					case Property.Description:
						stringPropertyValue = (item as XDevice).Description.Trim();
						break;
				}
				itemUid = (item as XDevice).UID;
			}

			if (item is XZone)
			{
				switch (property)
				{
					case Property.No:
						intPropertyValue = (item as XZone).No;
						break;
					case Property.Type:
						intPropertyValue = (int)(item as XZone).ObjectType;
						break;
					case Property.Description:
						stringPropertyValue = (item as XZone).Description.Trim();
						break;
				}
				itemUid = (item as XZone).UID;
			}

			if (item is XDirection)
			{
				switch (property)
				{
					case Property.No:
						intPropertyValue = (item as XDirection).No;
						break;
					case Property.Delay:
						intPropertyValue = (int)(item as XDirection).Delay;
						break;
					case Property.Hold:
						intPropertyValue = (int)(item as XDirection).Hold;
						break;
					case Property.DelayRegime:
						intPropertyValue = (int)(item as XDirection).DelayRegime;
						break;
					case Property.Description:
						stringPropertyValue = (item as XDirection).Description.Trim();
						break;
				}
				itemUid = (item as XDirection).UID;
			}
		}

		static void InitializeItems(ref IEnumerable<object> items, ref Variable result)
		{
			var variableItems = new List<VariableItem>();
			if (result.ObjectType == ObjectType.Device)
			{
				items = new List<XDevice>(XManager.DeviceConfiguration.Devices);
				foreach (var objectUid in new List<Guid>(XManager.DeviceConfiguration.Devices.Select(x => x.UID)))
					variableItems.Add(new VariableItem { ObjectUid = objectUid });
			}
			if (result.ObjectType == ObjectType.Zone)
			{
				items = new List<XZone>(XManager.Zones);
				foreach (var objectUid in new List<Guid>(XManager.Zones.Select(x => x.UID)))
					variableItems.Add(new VariableItem { ObjectUid = objectUid });
			}
			if (result.ObjectType == ObjectType.Direction)
			{
				items = new List<XDirection>(XManager.Directions);
				foreach (var objectUid in new List<Guid>(XManager.Directions.Select(x => x.UID)))
					variableItems.Add(new VariableItem { ObjectUid = objectUid });
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
			int intPropertyValue = 0;
			string stringPropertyValue = "";
			var itemUid = new Guid();

			foreach (var findObjectCondition in findObjectConditions)
			{
				foreach (var item in items)
				{
					InitializeProperties(ref intPropertyValue, ref stringPropertyValue, ref itemUid, findObjectCondition.Property, item);
					if (resultObjects.Contains(item))
						continue;
					if (((findObjectCondition.Property != Property.Description) &&
						(((findObjectCondition.ConditionType == ConditionType.IsEqual) && (intPropertyValue == findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsNotEqual) && (intPropertyValue != findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsMore) && (intPropertyValue > findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsNotMore) && (intPropertyValue <= findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsLess) && (intPropertyValue < findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsNotLess) && (intPropertyValue >= findObjectCondition.IntValue)))) ||
						((findObjectCondition.Property == Property.Description) &&
						(((findObjectCondition.ConditionType == ConditionType.StartsWith) && (stringPropertyValue.StartsWith(findObjectCondition.StringValue))) ||
						((findObjectCondition.ConditionType == ConditionType.EndsWith) && (stringPropertyValue.EndsWith(findObjectCondition.StringValue))) ||
						((findObjectCondition.ConditionType == ConditionType.Contains) && (stringPropertyValue.Contains(findObjectCondition.StringValue))))))
					{ resultObjects.Add(item); result.VariableItems.Add(new VariableItem { ObjectUid = itemUid }); }
				}
			}
		}

		static void FindObjectsAnd(Variable result, IEnumerable<FindObjectCondition> findObjectConditions)
		{
			IEnumerable<object> items = new List<object>();
			InitializeItems(ref items, ref result);
			var resultObjects = new List<object>(items);
			var tempObjects = new List<object>(resultObjects);
			int intPropertyValue = 0;
			string stringPropertyValue = "";
			var itemUid = new Guid();

			foreach (var findObjectCondition in findObjectConditions)
			{
				foreach (var item in resultObjects)
				{
					InitializeProperties(ref intPropertyValue, ref stringPropertyValue, ref itemUid, findObjectCondition.Property, item);
					if (((findObjectCondition.Property != Property.Description) &&
						(((findObjectCondition.ConditionType == ConditionType.IsEqual) && (intPropertyValue != findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsNotEqual) && (intPropertyValue == findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsMore) && (intPropertyValue <= findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsNotMore) && (intPropertyValue > findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsLess) && (intPropertyValue >= findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsNotLess) && (intPropertyValue < findObjectCondition.IntValue)))) ||
						((findObjectCondition.Property == Property.Description) &&
						(((findObjectCondition.ConditionType == ConditionType.StartsWith) && (!stringPropertyValue.StartsWith(findObjectCondition.StringValue))) ||
						((findObjectCondition.ConditionType == ConditionType.EndsWith) && (!stringPropertyValue.EndsWith(findObjectCondition.StringValue))) ||
						((findObjectCondition.ConditionType == ConditionType.Contains) && (!stringPropertyValue.Contains(findObjectCondition.StringValue))))))
					{ tempObjects.Remove(item); result.VariableItems.RemoveAll(x => x.ObjectUid == itemUid); }
				}
				resultObjects = new List<object>(tempObjects);
			}
		}

		public static void ControlGKDevice(ProcedureStep procedureStep)
		{
			var device = XManager.Devices.FirstOrDefault(x => x.UID == procedureStep.ControlGKDeviceArguments.Variable1.UidValue);
			if (device == null)
				return;
			FiresecServiceManager.SafeFiresecService.GKExecuteDeviceCommand(device.BaseUID, procedureStep.ControlGKDeviceArguments.Command);
		}

		public static List<WinFormsPlayer> WinFormsPlayers { get; private set; }
		public static void ControlCamera(ProcedureStep procedureStep)
		{
			var camera = ConfigurationCashHelper.SystemConfiguration.AllCameras.FirstOrDefault(x => x.UID == procedureStep.ControlCameraArguments.Variable1.UidValue);
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
			var zone = XManager.Zones.FirstOrDefault(x => x.UID == procedureStep.ControlGKFireZoneArguments.Variable1.UidValue);
			if (zone == null)
				return;
			if (procedureStep.ControlGKFireZoneArguments.ZoneCommandType == ZoneCommandType.Ignore)
				FiresecServiceManager.SafeFiresecService.GKSetIgnoreRegime(zone.UID, zone.ObjectType);
			if (procedureStep.ControlGKFireZoneArguments.ZoneCommandType == ZoneCommandType.ResetIgnore)
				FiresecServiceManager.SafeFiresecService.GKSetAutomaticRegime(zone.UID, zone.ObjectType);
			if (procedureStep.ControlGKFireZoneArguments.ZoneCommandType == ZoneCommandType.Reset)
				FiresecServiceManager.SafeFiresecService.GKReset(zone.UID, zone.ObjectType);
		}

		public static void ControlGuardZone(ProcedureStep procedureStep)
		{
			var zone = XManager.GuardZones.FirstOrDefault(x => x.UID == procedureStep.ControlGKGuardZoneArguments.Variable1.UidValue);
			if (zone == null)
				return;
			if (procedureStep.ControlGKGuardZoneArguments.GuardZoneCommandType == GuardZoneCommandType.Automatic)
				FiresecServiceManager.SafeFiresecService.GKSetAutomaticRegime(zone.UID, zone.ObjectType);
			if (procedureStep.ControlGKGuardZoneArguments.GuardZoneCommandType == GuardZoneCommandType.Ignore)
				FiresecServiceManager.SafeFiresecService.GKSetIgnoreRegime(zone.UID, zone.ObjectType);
			if (procedureStep.ControlGKGuardZoneArguments.GuardZoneCommandType == GuardZoneCommandType.Manual)
				FiresecServiceManager.SafeFiresecService.GKSetManualRegime(zone.UID, zone.ObjectType);
			if (procedureStep.ControlGKGuardZoneArguments.GuardZoneCommandType == GuardZoneCommandType.Reset)
				FiresecServiceManager.SafeFiresecService.GKReset(zone.UID, zone.ObjectType);
			if (procedureStep.ControlGKGuardZoneArguments.GuardZoneCommandType == GuardZoneCommandType.TurnOff)
				FiresecServiceManager.SafeFiresecService.GKTurnOff(zone.UID, zone.ObjectType);
			if (procedureStep.ControlGKGuardZoneArguments.GuardZoneCommandType == GuardZoneCommandType.TurnOn)
				FiresecServiceManager.SafeFiresecService.GKTurnOn(zone.UID, zone.ObjectType);
			if (procedureStep.ControlGKGuardZoneArguments.GuardZoneCommandType == GuardZoneCommandType.TurnOnNow)
				FiresecServiceManager.SafeFiresecService.GKTurnOnNow(zone.UID, zone.ObjectType);
		}

		public static void ControlDirection(ProcedureStep procedureStep)
		{
			var direction = XManager.Directions.FirstOrDefault(x => x.UID == procedureStep.ControlDirectionArguments.Variable1.UidValue);
			if (direction == null)
				return;
			if (procedureStep.ControlDirectionArguments.DirectionCommandType == DirectionCommandType.Automatic)
				FiresecServiceManager.SafeFiresecService.GKSetAutomaticRegime(direction.UID, direction.ObjectType);
			if (procedureStep.ControlDirectionArguments.DirectionCommandType == DirectionCommandType.ForbidStart)
				FiresecServiceManager.SafeFiresecService.GKStop(direction.UID, direction.ObjectType);
			if (procedureStep.ControlDirectionArguments.DirectionCommandType == DirectionCommandType.Ignore)
				FiresecServiceManager.SafeFiresecService.GKSetIgnoreRegime(direction.UID, direction.ObjectType);
			if (procedureStep.ControlDirectionArguments.DirectionCommandType == DirectionCommandType.Manual)
				FiresecServiceManager.SafeFiresecService.GKSetManualRegime(direction.UID, direction.ObjectType);
			if (procedureStep.ControlDirectionArguments.DirectionCommandType == DirectionCommandType.TurnOff)
				FiresecServiceManager.SafeFiresecService.GKTurnOff(direction.UID, direction.ObjectType);
			if (procedureStep.ControlDirectionArguments.DirectionCommandType == DirectionCommandType.TurnOn)
				FiresecServiceManager.SafeFiresecService.GKTurnOn(direction.UID, direction.ObjectType);
			if (procedureStep.ControlDirectionArguments.DirectionCommandType == DirectionCommandType.TurnOnNow)
				FiresecServiceManager.SafeFiresecService.GKTurnOnNow(direction.UID, direction.ObjectType);
		}

		public static void ControlDoor(ProcedureStep procedureStep)
		{
			var door = SKDManager.Doors.FirstOrDefault(x => x.UID == procedureStep.ControlDoorArguments.Variable1.UidValue);
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
			var sKDZone = SKDManager.Zones.FirstOrDefault(x => x.UID == procedureStep.ControlSKDZoneArguments.Variable1.UidValue);
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
			globalVariable.IntValue = incrementValueArguments.IncrementType == IncrementType.Inc ? globalVariable.IntValue + 1 : globalVariable.IntValue - 1;
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
	}
}
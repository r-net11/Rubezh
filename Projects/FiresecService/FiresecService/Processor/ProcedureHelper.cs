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
				var variable1 = GetValue(condition.Variable1, procedure, arguments);
				var variable2 = GetValue(condition.Variable2, procedure, arguments);
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
			var itemUid = new Guid();
			InitializeItem(ref item, variable.ObjectUid, variable.ObjectType);
			InitializeProperties(ref intPropertyValue, ref stringPropertyValue, ref itemUid, getStringArguments.Property, item);
			if (getStringArguments.StringOperation == StringOperation.Is)
				resultVariable.StringValues = new List<string>();
			if (getStringArguments.Property != Property.Name)
				resultVariable.StringValues.Add(intPropertyValue.ToString());
			else
				resultVariable.StringValues.Add(stringPropertyValue);
		}

		public static AutomationCallbackResult SendMessage(ProcedureStep procedureStep, Procedure procedure)
		{
			var automationCallbackResult = new AutomationCallbackResult();
			var sendMessageArguments = procedureStep.SendMessageArguments;
			if (sendMessageArguments.ValueType == ValueType.IsValue)
				automationCallbackResult.Message = procedureStep.SendMessageArguments.Message;
			else
			{
				var localVariable = procedure.Variables.FirstOrDefault(x => x.Uid == sendMessageArguments.VariableUid) ??
					procedure.Arguments.FirstOrDefault(x => x.Uid == sendMessageArguments.VariableUid);
				if (localVariable != null)
				{
					foreach (var str in localVariable.StringValues)
					{
						automationCallbackResult.Message += str + "\n";
					}
				}
			}
			return automationCallbackResult;
		}

		public static void Calculate(ProcedureStep procedureStep, Procedure procedure, List<Argument> arguments)
		{
			var arithmeticArguments = procedureStep.ArithmeticArguments;
			var variable1 = GetValue(arithmeticArguments.Variable1, procedure, arguments);
			var variable2 = GetValue(arithmeticArguments.Variable2, procedure, arguments);
			int result = 0;
			if (arithmeticArguments.ArithmeticType == ArithmeticType.Add)
				result = variable1 + variable2;
			if (arithmeticArguments.ArithmeticType == ArithmeticType.Sub)
				result = variable1 - variable2;
			if (arithmeticArguments.ArithmeticType == ArithmeticType.Multi)
				result = variable1 * variable2;
			if ((arithmeticArguments.ArithmeticType == ArithmeticType.Div) && (variable2 != 0))
				result = variable1 / variable2;

			if (arithmeticArguments.Result.ValueType == ValueType.IsGlobalVariable)
			{
				var globalVariable = ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.GlobalVariables
					.FirstOrDefault(x => x.Uid == arithmeticArguments.Result.GlobalVariableUid);
				if (globalVariable != null)
					globalVariable.Value = result;
			}

			if (arithmeticArguments.Result.ValueType == ValueType.IsLocalVariable)
			{
				var localVariable = procedure.Variables.FirstOrDefault(x => x.Uid == arithmeticArguments.Result.VariableUid) ??
					procedure.Arguments.FirstOrDefault(x => x.Uid == arithmeticArguments.Result.VariableUid);
				if (localVariable != null)
					localVariable.IntValue = result;
			}
		}

		static int GetValue(ArithmeticParameter arithmeticParameter, Procedure procedure, List<Argument> arguments)
		{
			if (arithmeticParameter.ValueType == ValueType.IsGlobalVariable)
			{
				var globalVariable = ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.GlobalVariables
					.FirstOrDefault(
						x => x.Uid == arithmeticParameter.GlobalVariableUid);
				if (globalVariable != null)
					return globalVariable.Value;
			}
			if (arithmeticParameter.ValueType == ValueType.IsLocalVariable)
			{
				var localVariable = procedure.Variables.FirstOrDefault(x => x.Uid == arithmeticParameter.VariableUid) ??
					procedure.Arguments.FirstOrDefault(x => x.Uid == arithmeticParameter.VariableUid);
				if (localVariable != null)
				{
					var argument = arguments.FirstOrDefault(x => x.ArgumentUid == localVariable.Uid);
					if (argument != null)
						return argument.IntValue;
					return localVariable.IntValue;
				}
			}
			return arithmeticParameter.Value;
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
					case Property.Name:
						stringPropertyValue = (item as XDevice).PresentationName.Trim();
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
					case Property.ZoneType:
						intPropertyValue = (int)(item as XZone).ObjectType;
						break;
					case Property.Name:
						stringPropertyValue = (item as XZone).Name.Trim();
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
					case Property.Name:
						stringPropertyValue = (item as XDirection).Name.Trim();
						break;
				}
				itemUid = (item as XDirection).UID;
			}
		}

		static void InitializeItems(ref IEnumerable<object> items, ref Variable result)
		{
			result.ObjectsUids = new List<Guid>();
			if (result.ObjectType == ObjectType.Device)
			{
				items = new List<XDevice>(XManager.DeviceConfiguration.Devices);
				result.ObjectsUids = new List<Guid>(XManager.DeviceConfiguration.Devices.Select(x => x.UID));
			}
			if (result.ObjectType == ObjectType.Zone)
			{
				items = new List<XZone>(XManager.Zones);
				result.ObjectsUids = new List<Guid>(XManager.Zones.Select(x => x.UID));
			}
			if (result.ObjectType == ObjectType.Direction)
			{
				items = new List<XDirection>(XManager.Directions);
				result.ObjectsUids = new List<Guid>(XManager.Directions.Select(x => x.UID));
			}
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
					if (((findObjectCondition.Property != Property.Name) &&
						(((findObjectCondition.ConditionType == ConditionType.IsEqual) && (intPropertyValue == findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsNotEqual) && (intPropertyValue != findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsMore) && (intPropertyValue > findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsNotMore) && (intPropertyValue <= findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsLess) && (intPropertyValue < findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsNotLess) && (intPropertyValue >= findObjectCondition.IntValue)))) ||
						((findObjectCondition.Property == Property.Name) &&
						(((findObjectCondition.StringConditionType == StringConditionType.StartsWith) && (stringPropertyValue.StartsWith(findObjectCondition.StringValue))) ||
						((findObjectCondition.StringConditionType == StringConditionType.EndsWith) && (stringPropertyValue.EndsWith(findObjectCondition.StringValue))) ||
						((findObjectCondition.StringConditionType == StringConditionType.Contains) && (stringPropertyValue.Contains(findObjectCondition.StringValue))))))
					{ resultObjects.Add(item); result.ObjectsUids.Add(itemUid); }
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
					if (((findObjectCondition.Property != Property.Name) &&
						(((findObjectCondition.ConditionType == ConditionType.IsEqual) && (intPropertyValue != findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsNotEqual) && (intPropertyValue == findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsMore) && (intPropertyValue <= findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsNotMore) && (intPropertyValue > findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsLess) && (intPropertyValue >= findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsNotLess) && (intPropertyValue < findObjectCondition.IntValue)))) ||
						((findObjectCondition.Property == Property.Name) &&
						(((findObjectCondition.StringConditionType == StringConditionType.StartsWith) && (!stringPropertyValue.StartsWith(findObjectCondition.StringValue))) ||
						((findObjectCondition.StringConditionType == StringConditionType.EndsWith) && (!stringPropertyValue.EndsWith(findObjectCondition.StringValue))) ||
						((findObjectCondition.StringConditionType == StringConditionType.Contains) && (!stringPropertyValue.Contains(findObjectCondition.StringValue))))))
					{ tempObjects.Remove(item); result.ObjectsUids.Remove(itemUid); }
				}
				resultObjects = new List<object>(tempObjects);
			}
		}

		public static void ControlGKDevice(ProcedureStep procedureStep)
		{
			var device = XManager.Devices.FirstOrDefault(x => x.UID == procedureStep.ControlGKDeviceArguments.DeviceUid);
			if (device == null)
				return;
			FiresecServiceManager.SafeFiresecService.GKExecuteDeviceCommand(device.BaseUID, procedureStep.ControlGKDeviceArguments.Command);
		}

		public static List<WinFormsPlayer> WinFormsPlayers { get; private set; }
		public static void ControlCamera(ProcedureStep procedureStep)
		{
			var camera = ConfigurationCashHelper.SystemConfiguration.AllCameras.FirstOrDefault(x => x.UID == procedureStep.ControlCameraArguments.CameraUid);
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
			var zone = XManager.Zones.FirstOrDefault(x => x.UID == procedureStep.ControlGKFireZoneArguments.ZoneUid);
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
			var zone = XManager.GuardZones.FirstOrDefault(x => x.UID == procedureStep.ControlGKGuardZoneArguments.ZoneUid);
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
			var direction = XManager.Directions.FirstOrDefault(x => x.UID == procedureStep.ControlDirectionArguments.DirectionUid);
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
			var door = SKDManager.Doors.FirstOrDefault(x => x.UID == procedureStep.ControlDoorArguments.DoorUid);
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
			var sKDZone = SKDManager.Zones.FirstOrDefault(x => x.UID == procedureStep.ControlSKDZoneArguments.ZoneUid);
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

		public static void IncrementGlobalValue(ProcedureStep procedureStep)
		{
			var incrementGlobalValueArguments = procedureStep.IncrementGlobalValueArguments;
			var globalVariable = ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.GlobalVariables.FirstOrDefault
				(x => x.Uid == incrementGlobalValueArguments.GlobalVariableUid);
			if (globalVariable == null)
				return;
			globalVariable.Value = incrementGlobalValueArguments.IncrementType == IncrementType.Inc ? globalVariable.Value + 1 : globalVariable.Value - 1;
		}

		public static void SetGlobalValue(ProcedureStep procedureStep)
		{
			var setGlobalValueArguments = procedureStep.SetGlobalValueArguments;
			var globalVariable = ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.GlobalVariables.FirstOrDefault
				(x => x.Uid == setGlobalValueArguments.GlobalVariableUid);
			if (globalVariable == null)
				return;
			globalVariable.Value = setGlobalValueArguments.Value;
		}
	}
}
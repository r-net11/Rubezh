﻿using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Automation;
using FiresecAPI.GK;
using FiresecAPI.SKD;
using FiresecClient;
using FiresecService.Service;
using Infrastructure.Common.Video.RVI_VSS;
using Property = FiresecAPI.Automation.Property;
using FiresecAPI.Journal;
using System.Threading;

namespace FiresecService.Processor
{
	public static class ProcedureHelper
	{
		static ProcedureHelper()
		{
			WinFormsPlayers = new List<WinFormsPlayer>();
		}

		static public Procedure Procedure { get; set; }

		public static void AddJournalItem(ProcedureStep procedureStep)
		{			
			var journalItem = new JournalItem();
			journalItem.SystemDateTime = DateTime.Now;
			journalItem.DeviceDateTime = DateTime.Now;
			journalItem.JournalEventNameType = JournalEventNameType.Сообщение_автоматизации;
			journalItem.DescriptionText = GetValue<object>(procedureStep.JournalArguments.MessageParameter).ToString();
			Service.FiresecService.AddCommonJournalItem(journalItem);
		}

		public static bool Compare(ProcedureStep procedureStep)
		{
			var conditionArguments = procedureStep.ConditionArguments;
			var result = conditionArguments.JoinOperator == JoinOperator.And;
			foreach (var condition in conditionArguments.Conditions)
			{
				var variable1 = GetValue<object>(condition.Parameter1);
				var variable2 = GetValue<object>(condition.Parameter2);
				var comparer = Compare(variable1, variable2, condition.ConditionType);
				if ((comparer != null))
					result = conditionArguments.JoinOperator == JoinOperator.And ? result & comparer.Value : result | comparer.Value;
			}
			return result;
		}

		public static AutomationCallbackResult ShowMessage(ProcedureStep procedureStep)
		{
			var automationCallbackResult = new AutomationCallbackResult();
			var messageValue = GetValue<object>(procedureStep.ShowMessageArguments.MessageParameter);
			automationCallbackResult.Message = messageValue.GetType().IsEnum ? ((Enum)messageValue).ToDescription() : messageValue.ToString();
			return automationCallbackResult;
		}

		public static void Calculate(ProcedureStep procedureStep)
		{
			var arithmeticArguments = procedureStep.ArithmeticArguments;
			object variable1;
			object variable2;
			var resultVariable = GetAllVariables(Procedure).FirstOrDefault(x => x.Uid == arithmeticArguments.ResultParameter.VariableUid);
			switch (arithmeticArguments.ExplicitType)
			{
				case ExplicitType.Boolean:
					{
						variable1 = GetValue<bool>(arithmeticArguments.Parameter1);
						variable2 = GetValue<bool>(arithmeticArguments.Parameter2);
						bool result = false;
						if (arithmeticArguments.ArithmeticOperationType == ArithmeticOperationType.And)
							result = (bool)variable1 & (bool)variable2;
						if (arithmeticArguments.ArithmeticOperationType == ArithmeticOperationType.Or)
							result = (bool)variable1 || (bool)variable2;
						if (resultVariable != null) resultVariable.ExplicitValue.BoolValue = result;
						break;
					}

				case ExplicitType.Integer:
					{
						variable1 = GetValue<int>(arithmeticArguments.Parameter1);
						variable2 = GetValue<int>(arithmeticArguments.Parameter2);
						int result = 0;
						if (arithmeticArguments.ArithmeticOperationType == ArithmeticOperationType.Add)
							result = (int)variable1 + (int)variable2;
						if (arithmeticArguments.ArithmeticOperationType == ArithmeticOperationType.Sub)
							result = (int)variable1 - (int)variable2;
						if (arithmeticArguments.ArithmeticOperationType == ArithmeticOperationType.Multi)
							result = (int)variable1 * (int)variable2;
						if ((arithmeticArguments.ArithmeticOperationType == ArithmeticOperationType.Div) && ((int)variable2 != 0))
							result = (int)variable1 / (int)variable2;
						if (resultVariable != null) resultVariable.ExplicitValue.IntValue = result;
						break;
					}

				case ExplicitType.DateTime:
					{
						variable1 = GetValue<DateTime>(arithmeticArguments.Parameter1);
						variable2 = new TimeSpan();
						switch (arithmeticArguments.TimeType)
						{
							case TimeType.Sec:
								variable2 = TimeSpan.FromSeconds(GetValue<int>(arithmeticArguments.Parameter2));
								break;
							case TimeType.Min:
								variable2 = TimeSpan.FromMinutes(GetValue<int>(arithmeticArguments.Parameter2));
								break;
							case TimeType.Hour:
								variable2 = TimeSpan.FromHours(GetValue<int>(arithmeticArguments.Parameter2));
								break;
							case TimeType.Day:
								variable2 = TimeSpan.FromDays(GetValue<int>(arithmeticArguments.Parameter2));
								break;
						}
						var result = new DateTime();
						if (arithmeticArguments.ArithmeticOperationType == ArithmeticOperationType.Add)
							result = (DateTime)variable1 + (TimeSpan)variable2;
						if (arithmeticArguments.ArithmeticOperationType == ArithmeticOperationType.Sub)
							result = (DateTime)variable1 - (TimeSpan)variable2;

						if (resultVariable != null) resultVariable.ExplicitValue.DateTimeValue = result;
						break;
					}
				case ExplicitType.String:
					{
						variable1 = GetValue<string>(arithmeticArguments.Parameter1);
						variable2 = GetValue<string>(arithmeticArguments.Parameter2);
						if (arithmeticArguments.ArithmeticOperationType == ArithmeticOperationType.Add)
							if (resultVariable != null)
								resultVariable.ExplicitValue.StringValue = String.Concat((string)variable1, (string)variable2);
						break;
					}
			}
		}

		public static List<Variable> GetAllVariables(Procedure procedure)
		{
			var allVariables = new List<Variable>(ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.GlobalVariables);
			allVariables.AddRange(procedure.Variables);
			allVariables.AddRange(procedure.Arguments);
			return allVariables;
		}

		public static void FindObjects(ProcedureStep procedureStep)
		{
			var findObjectArguments = procedureStep.FindObjectArguments;
			var variable = GetAllVariables(Procedure).FirstOrDefault(x => x.Uid == findObjectArguments.ResultParameter.VariableUid);
			if (findObjectArguments.JoinOperator == JoinOperator.Or)
				FindObjectsOr(variable, findObjectArguments.FindObjectConditions);
			else
				FindObjectsAnd(variable, findObjectArguments.FindObjectConditions);
		}

		public static void GetObjectProperty(ProcedureStep procedureStep)
		{
			var getObjectPropertyArguments = procedureStep.GetObjectPropertyArguments;
			var target = GetAllVariables(Procedure).FirstOrDefault(x => x.Uid == getObjectPropertyArguments.ResultParameter.VariableUid);
			var objectUid = GetValue<Guid>(getObjectPropertyArguments.ObjectParameter);
			var item = InitializeItem(objectUid);
			if (item == null)
				return;
			var guid = new Guid();
			var propertyValue = GetPropertyValue(ref guid, getObjectPropertyArguments.Property, item);
			SetPropertyValue(target, propertyValue);
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
						propertyValue = (item as XDevice).Driver.DriverType;
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
						propertyValue = (item as XZone).ObjectType;
						break;
					case Property.State:
						propertyValue = (int)(item as XZone).State.StateClass;
						break;
					case Property.Name:
						propertyValue = (item as XZone).Name.Trim();
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
			var explicitValues = new List<ExplicitValue>();
			if (result.ObjectType == ObjectType.Device)
			{
				items = new List<XDevice>(XManager.DeviceConfiguration.Devices);
				foreach (var objectUid in new List<Guid>(XManager.DeviceConfiguration.Devices.Select(x => x.UID)))
					explicitValues.Add(new ExplicitValue { UidValue = objectUid });
			}
			if (result.ObjectType == ObjectType.Zone)
			{
				items = new List<XZone>(XManager.Zones);
				foreach (var objectUid in new List<Guid>(XManager.Zones.Select(x => x.UID)))
					explicitValues.Add(new ExplicitValue { UidValue = objectUid });
			}
			if (result.ObjectType == ObjectType.Direction)
			{
				items = new List<XDirection>(XManager.Directions);
				foreach (var objectUid in new List<Guid>(XManager.Directions.Select(x => x.UID)))
					explicitValues.Add(new ExplicitValue { UidValue = objectUid });
			}
			result.ExplicitValues = explicitValues;
		}

		static object InitializeItem(Guid itemUid)
		{
			var device = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == itemUid);
			var zone = XManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == itemUid);
			var guardZone = XManager.DeviceConfiguration.GuardZones.FirstOrDefault(x => x.UID == itemUid);
			var sKDDevice = SKDManager.Devices.FirstOrDefault(x => x.UID == itemUid);
			var sKDZone = SKDManager.Zones.FirstOrDefault(x => x.UID == itemUid);
			var camera = ConfigurationCashHelper.SystemConfiguration.AllCameras.FirstOrDefault(x => x.UID == itemUid);
			var sKDDoor = SKDManager.Doors.FirstOrDefault(x => x.UID == itemUid);
			var direction = XManager.DeviceConfiguration.Directions.FirstOrDefault(x => x.UID == itemUid);
			if (device != null) return device;
			if (zone != null) return zone;
			if (guardZone != null) return guardZone;
			if (sKDDevice != null) return sKDDevice;
			if (sKDZone != null) return sKDZone;
			if (camera != null) return camera;
			if (sKDDoor != null) return sKDDoor;
			if (direction != null) return direction;
			return null;
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
					var conditionValue = GetValue<object>(findObjectCondition.SourceParameter);
					var comparer = Compare(propertyValue, conditionValue, findObjectCondition.ConditionType);
					if ((comparer != null) && (comparer.Value))
					{
						resultObjects.Add(item);
						result.ExplicitValues.Add(new ExplicitValue { UidValue = itemUid });
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
					var conditionValue = GetValue<object>(findObjectCondition.SourceParameter);
					var comparer = Compare(propertyValue, conditionValue, findObjectCondition.ConditionType);
					if ((comparer != null) && (!comparer.Value))
					{
						tempObjects.Remove(item);
						result.ExplicitValues.RemoveAll(x => x.UidValue == itemUid);
					}
				}
				resultObjects = new List<object>(tempObjects);
			}
		}

		public static bool? Compare(object param1, object param2, ConditionType conditionType)
		{
			if (param1.GetType() != param2.GetType())
				return null;
			if (param1.GetType().IsEnum|| param1 is int)
			{
				return (((conditionType == ConditionType.IsEqual) && ((int)param1 == (int)param2))
					|| ((conditionType == ConditionType.IsNotEqual) && ((int)param1 != (int)param2))
					|| ((conditionType == ConditionType.IsMore) && ((int)param1 > (int)param2))
					|| ((conditionType == ConditionType.IsNotMore) && ((int)param1 <= (int)param2))
					|| ((conditionType == ConditionType.IsLess) && ((int)param1 < (int)param2))
					|| ((conditionType == ConditionType.IsNotLess) && ((int)param1 >= (int)param2)));
			}

			if (param1 is DateTime)
			{
				return (((conditionType == ConditionType.IsEqual) && ((DateTime)param1 == (DateTime)param2))
					|| ((conditionType == ConditionType.IsNotEqual) && ((DateTime)param1 != (DateTime)param2))
					|| ((conditionType == ConditionType.IsMore) && ((DateTime)param1 > (DateTime)param2))
					|| ((conditionType == ConditionType.IsNotMore) && ((DateTime)param1 <= (DateTime)param2))
					|| ((conditionType == ConditionType.IsLess) && ((DateTime)param1 < (DateTime)param2))
					|| ((conditionType == ConditionType.IsNotLess) && ((DateTime)param1 >= (DateTime)param2)));
			}

			if (param1 is string)
			{
				return (((conditionType == ConditionType.IsEqual) && ((string)param1 == (string)param2))
					|| ((conditionType == ConditionType.IsNotEqual) && ((string)param1 != (string)param2))
					|| ((conditionType == ConditionType.StartsWith) && (((string)param1).StartsWith((string)param2)))
					|| ((conditionType == ConditionType.EndsWith) && (((string)param1).EndsWith((string)param2)))
					|| ((conditionType == ConditionType.Contains) && (((string)param1).Contains((string)param2))));
			}
			if (param1 is bool)
			{
				return (((conditionType == ConditionType.IsEqual) && ((bool) param1 == (bool) param2))
						|| ((conditionType == ConditionType.IsNotEqual) && ((bool) param1 != (bool) param2)));
			}
			return null;
		}

		public static void ControlGKDevice(ProcedureStep procedureStep)
		{
			var deviceUid = GetValue<Guid>(procedureStep.ControlGKDeviceArguments.GKDeviceParameter);
			var device = XManager.Devices.FirstOrDefault(x => x.UID == deviceUid);
			if (device == null)
				return;
			FiresecServiceManager.SafeFiresecService.GKExecuteDeviceCommand(device.UID, procedureStep.ControlGKDeviceArguments.Command);
		}

		public static List<WinFormsPlayer> WinFormsPlayers { get; private set; }
		public static void ControlCamera(ProcedureStep procedureStep)
		{
			var camera = ConfigurationCashHelper.SystemConfiguration.AllCameras.FirstOrDefault(x => x.UID == procedureStep.ControlCameraArguments.CameraParameter.ExplicitValue.UidValue);
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
			var zoneUid = GetValue<Guid>(procedureStep.ControlGKFireZoneArguments.GKFireZoneParameter);
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
			var zoneUid = GetValue<Guid>(procedureStep.ControlGKGuardZoneArguments.GKGuardZoneParameter);
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
			var directionUid = GetValue<Guid>(procedureStep.ControlDirectionArguments.DirectionParameter);
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
			var door = SKDManager.Doors.FirstOrDefault(x => x.UID == procedureStep.ControlDoorArguments.DoorParameter.ExplicitValue.UidValue);
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
			var sKDZone = SKDManager.Zones.FirstOrDefault(x => x.UID == procedureStep.ControlSKDZoneArguments.SKDZoneParameter.ExplicitValue.UidValue);
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
			//if (procedureStep.ControlSKDZoneArguments.SKDZoneCommandType == SKDZoneCommandType.DetectEmployees)
		}

		public static void Pause(ProcedureStep procedureStep)
		{
			var pauseArguments = procedureStep.PauseArguments;
			var pause = new TimeSpan();
			switch (pauseArguments.TimeType)
			{
				case TimeType.Sec:
					pause = TimeSpan.FromSeconds(GetValue<int>(pauseArguments.PauseParameter));
					break;
				case TimeType.Min:
					pause = TimeSpan.FromMinutes(GetValue<int>(pauseArguments.PauseParameter));
					break;
				case TimeType.Hour:
					pause = TimeSpan.FromHours(GetValue<int>(pauseArguments.PauseParameter));
					break;
				case TimeType.Day:
					pause = TimeSpan.FromDays(GetValue<int>(pauseArguments.PauseParameter));
					break;
			}
			Thread.Sleep(pause);
		}

		public static void IncrementValue(ProcedureStep procedureStep)
		{
			var incrementValueArguments = procedureStep.IncrementValueArguments;
			var variable = GetAllVariables(Procedure).FirstOrDefault(x => x.Uid == incrementValueArguments.ResultParameter.VariableUid);
			if (variable != null)
				variable.ExplicitValue.IntValue = incrementValueArguments.IncrementType == IncrementType.Inc ? variable.ExplicitValue.IntValue + 1 : variable.ExplicitValue.IntValue - 1;
		}

		public static void GetRandomValue(ProcedureStep procedureStep)
		{
			var randomArguments = procedureStep.RandomArguments;
			var resultVariable = GetAllVariables(Procedure).FirstOrDefault(x => x.Uid == randomArguments.ResultParameter.VariableUid);
			var maxValue = GetValue<int>(randomArguments.MaxValueParameter);
			if (resultVariable != null) resultVariable.ExplicitValue.IntValue = new Random().Next(0, maxValue);
		}

		public static void ChangeList(ProcedureStep procedureStep)
		{
			var changeListArguments = procedureStep.ChangeListArguments;
			var listVariable = GetAllVariables(Procedure).FirstOrDefault(x => x.Uid == changeListArguments.ListArgument.VariableUid);
			var variable = new Variable();
			variable.ExplicitType = changeListArguments.ItemArgument.ExplicitType;
			variable.EnumType = changeListArguments.ItemArgument.EnumType;
			variable.ObjectType = changeListArguments.ItemArgument.ObjectType;
			var itemValue = GetValue<object>(changeListArguments.ItemArgument);
			SetPropertyValue(variable, itemValue);
			var explicitValue = variable.ExplicitValue;
			if (listVariable != null)
			{
				switch (changeListArguments.ChangeType)
				{
					case ChangeType.AddLast:
						listVariable.ExplicitValues.Add(explicitValue);
						break;
					case ChangeType.RemoveFirst:
							listVariable.ExplicitValues.Remove(listVariable.ExplicitValues.FirstOrDefault
								(x => ExplicitCompare(x, explicitValue, changeListArguments.ListArgument.ExplicitType,
									changeListArguments.ListArgument.EnumType)));
							break;
					case ChangeType.RemoveAll:
						listVariable.ExplicitValues.RemoveAll(
							(x => ExplicitCompare(x, explicitValue, changeListArguments.ListArgument.ExplicitType,
								changeListArguments.ListArgument.EnumType)));
						break;
				}
			}
		}

		static bool ExplicitCompare(ExplicitValue explicitValue1, ExplicitValue explicitValue2, ExplicitType explicitType, EnumType enumType)
		{
			if (explicitType == ExplicitType.Integer)
				return explicitValue1.IntValue == explicitValue2.IntValue;
			if (explicitType == ExplicitType.String)
				return explicitValue1.StringValue == explicitValue2.StringValue;
			if (explicitType == ExplicitType.Boolean)
				return explicitValue1.BoolValue == explicitValue2.BoolValue;
			if (explicitType == ExplicitType.DateTime)
				return explicitValue1.DateTimeValue == explicitValue2.DateTimeValue;
			if (explicitType == ExplicitType.Enum)
			{
				if (enumType == EnumType.DriverType)
					return explicitValue1.DriverTypeValue == explicitValue2.DriverTypeValue;
				if (enumType == EnumType.StateType)
					return explicitValue1.StateTypeValue == explicitValue2.StateTypeValue;
			}
			if (explicitType == ExplicitType.Object)
			{
				return explicitValue1.UidValue == explicitValue2.UidValue;
			}
			return false;
		}

		public static void SetValue(ProcedureStep procedureStep)
		{
			var setValueArguments = procedureStep.SetValueArguments;
			var sourceVariable = GetAllVariables(Procedure).FirstOrDefault(x => x.Uid == setValueArguments.SourceParameter.VariableUid);
			var targetVariable = GetAllVariables(Procedure).FirstOrDefault(x => x.Uid == setValueArguments.TargetParameter.VariableUid);
			if (targetVariable == null)
				return;
			PropertyCopy.Copy(sourceVariable != null ? sourceVariable.ExplicitValue : setValueArguments.SourceParameter.ExplicitValue, targetVariable.ExplicitValue);
		}

		static void SetPropertyValue(Variable target, object propertyValue)
		{
			if (target.ExplicitType == ExplicitType.Integer)
				target.ExplicitValue.IntValue = (int) propertyValue;
			if (target.ExplicitType == ExplicitType.String)
				target.ExplicitValue.StringValue = (string)propertyValue;
			if (target.ExplicitType == ExplicitType.Boolean)
				target.ExplicitValue.BoolValue = (bool)propertyValue;
			if (target.ExplicitType == ExplicitType.DateTime)
				target.ExplicitValue.DateTimeValue = (DateTime)propertyValue;
			if (target.ExplicitType == ExplicitType.Enum)
			{
				if (target.EnumType == EnumType.DriverType)
					target.ExplicitValue.DriverTypeValue = (XDriverType) propertyValue;
				if (target.EnumType == EnumType.StateType)
					target.ExplicitValue.StateTypeValue = (XStateClass) propertyValue;
			}
		}

		public static T GetValue<T>(Argument variable)
		{
			var result = new object();
			var variableScope = variable.VariableScope;
			var explicitType = variable.ExplicitType;
			var enumType = variable.EnumType;
			var explicitValue = variable.ExplicitValue;
			if (variableScope != VariableScope.ExplicitValue)
			{
				var argument = GetAllVariables(Procedure).FirstOrDefault(x => x.Uid == variable.VariableUid);
				if (argument != null)
				{
					explicitValue = argument.ExplicitValue;
					explicitType = argument.ExplicitType;
					enumType = argument.EnumType;
				}
			}
			if (explicitType == ExplicitType.Boolean)
				result = explicitValue.BoolValue;
			if (explicitType == ExplicitType.DateTime)
				result = explicitValue.DateTimeValue;
			if (explicitType == ExplicitType.Integer)
				result = explicitValue.IntValue;
			if (explicitType == ExplicitType.String)
				result = explicitValue.StringValue;
			if (explicitType == ExplicitType.Object)
				result = explicitValue.UidValue;
			if (explicitType == ExplicitType.Enum)
			{
				if (enumType == EnumType.DriverType)
					result = explicitValue.DriverTypeValue;
				if (enumType == EnumType.StateType)
					result = explicitValue.StateTypeValue;
			}
			return (T)result;
		}
	}
}
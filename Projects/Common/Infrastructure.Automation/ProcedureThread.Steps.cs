using RubezhAPI;
using RubezhAPI.Automation;
using RubezhAPI.AutomationCallback;
using RubezhAPI.GK;
using RubezhAPI.Journal;
using RubezhAPI.License;
using RubezhAPI.Models;
using RubezhAPI.SKD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Infrastructure.Automation
{
	public partial class ProcedureThread
	{
		public void GenerateGuidStep(ProcedureStep procedureStep)
		{
			var generateGuidStep = (GenerateGuidStep)procedureStep;
			SetValue(generateGuidStep.ResultArgument, Guid.NewGuid());
		}

		public void SetJournalItemGuidStep(ProcedureStep procedureStep)
		{
			//var setJournalItemGuidArguments = procedureStep.SetJournalItemGuidArguments;
			//if (JournalItem != null)
			//{
			//	using (var dbService = new DbService())
			//	{
			//		var eventUIDString = GetValue<String>(setJournalItemGuidArguments.ValueArgument);
			//		Guid eventUID;
			//		if (CheckGuid(eventUIDString))
			//		{
			//			eventUID = new Guid(eventUIDString);
			//		}
			//		else
			//		{
			//			return;
			//		}
			//		dbService.JournalTranslator.SaveVideoUID(JournalItem.UID, eventUID, Guid.Empty);
			//	}
			//}
		}

		public void ProcedureSelectionStep(ProcedureStep procedureStep)
		{
			var procedureSelectionStep = (ProcedureSelectionStep)procedureStep;
			var childProcedure = ProcedureExecutionContext.SystemConfiguration.AutomationConfiguration.Procedures.
									FirstOrDefault(x => x.Uid == procedureSelectionStep.ScheduleProcedure.ProcedureUid);
			if (childProcedure != null)
				AutomationProcessor.RunProcedure(childProcedure, procedureSelectionStep.ScheduleProcedure.Arguments, AllVariables, User, JournalItem, ClientUID);
		}

		public void GetObjectPropertyStep(ProcedureStep procedureStep)
		{
			var getObjectPropertyStep = (GetObjectPropertyStep)procedureStep;
			var target = AllVariables.FirstOrDefault(x => x.Uid == getObjectPropertyStep.ResultArgument.VariableUid);
			var objRef = GetValue<ObjectReference>(getObjectPropertyStep.ObjectArgument);
			var item = InitializeItem(objRef);
			if (item == null)
				return;
			var propertyValue = GetPropertyValue(getObjectPropertyStep.Property, item);
			ProcedureExecutionContext.SetVariableValue(target, propertyValue, ClientUID);
		}

		public void ArithmeticStep(ProcedureStep procedureStep)
		{
			var arithmeticStep = (ArithmeticStep)procedureStep;
			object value1;
			object value2;
			var resultVariable = AllVariables.FirstOrDefault(x => x.Uid == arithmeticStep.ResultArgument.VariableUid);
			switch (arithmeticStep.ExplicitType)
			{
				case ExplicitType.Boolean:
					{
						value1 = GetValue<bool>(arithmeticStep.Argument1);
						value2 = GetValue<bool>(arithmeticStep.Argument2);
						bool result = false;
						if (arithmeticStep.ArithmeticOperationType == ArithmeticOperationType.And)
							result = (bool)value1 & (bool)value2;
						if (arithmeticStep.ArithmeticOperationType == ArithmeticOperationType.Or)
							result = (bool)value1 || (bool)value2;
						if (resultVariable != null)
							resultVariable.ExplicitValue.BoolValue = result;
						break;
					}

				case ExplicitType.Integer:
					{
						value1 = GetValue<int>(arithmeticStep.Argument1);
						value2 = GetValue<int>(arithmeticStep.Argument2);
						int result = 0;
						if (arithmeticStep.ArithmeticOperationType == ArithmeticOperationType.Add)
							result = (int)value1 + (int)value2;
						if (arithmeticStep.ArithmeticOperationType == ArithmeticOperationType.Sub)
							result = (int)value1 - (int)value2;
						if (arithmeticStep.ArithmeticOperationType == ArithmeticOperationType.Multi)
							result = (int)value1 * (int)value2;
						if ((arithmeticStep.ArithmeticOperationType == ArithmeticOperationType.Div) && ((int)value2 != 0))
							result = (int)value1 / (int)value2;
						if (resultVariable != null)
							resultVariable.ExplicitValue.IntValue = result;
						break;
					}
				case ExplicitType.Float:
					{
						value1 = GetValue<double>(arithmeticStep.Argument1);
						value2 = GetValue<double>(arithmeticStep.Argument2);
						double result = 0;
						if (arithmeticStep.ArithmeticOperationType == ArithmeticOperationType.Add)
							result = (double)value1 + (double)value2;
						if (arithmeticStep.ArithmeticOperationType == ArithmeticOperationType.Sub)
							result = (double)value1 - (double)value2;
						if (arithmeticStep.ArithmeticOperationType == ArithmeticOperationType.Multi)
							result = (double)value1 * (double)value2;
						if ((arithmeticStep.ArithmeticOperationType == ArithmeticOperationType.Div) && ((int)value2 != 0))
							result = (double)value1 / (double)value2;
						if (resultVariable != null)
							resultVariable.ExplicitValue.FloatValue = result;
						break;
					}

				case ExplicitType.DateTime:
					{
						value1 = GetValue<DateTime>(arithmeticStep.Argument1);
						value2 = new TimeSpan();
						switch (arithmeticStep.TimeType)
						{
							case TimeType.Millisec:
								value2 = TimeSpan.FromMilliseconds(GetValue<int>(arithmeticStep.Argument2));
								break;
							case TimeType.Sec:
								value2 = TimeSpan.FromSeconds(GetValue<int>(arithmeticStep.Argument2));
								break;
							case TimeType.Min:
								value2 = TimeSpan.FromMinutes(GetValue<int>(arithmeticStep.Argument2));
								break;
							case TimeType.Hour:
								value2 = TimeSpan.FromHours(GetValue<int>(arithmeticStep.Argument2));
								break;
							case TimeType.Day:
								value2 = TimeSpan.FromDays(GetValue<int>(arithmeticStep.Argument2));
								break;
						}
						var result = new DateTime();
						if (arithmeticStep.ArithmeticOperationType == ArithmeticOperationType.Add)
							result = (DateTime)value1 + (TimeSpan)value2;
						if (arithmeticStep.ArithmeticOperationType == ArithmeticOperationType.Sub)
							result = (DateTime)value1 - (TimeSpan)value2;

						if (resultVariable != null)
							resultVariable.ExplicitValue.DateTimeValue = result;
						break;
					}
				case ExplicitType.String:
					{
						if (arithmeticStep.ArithmeticOperationType == ArithmeticOperationType.Add)
							if (resultVariable != null)
								resultVariable.ExplicitValue.StringValue = String.Concat(GetStringValue(arithmeticStep.Argument1), GetStringValue(arithmeticStep.Argument2));
						break;
					}
			}
			ProcedureExecutionContext.SynchronizeVariable(resultVariable, ClientUID);
		}

		public void CreateColorStep(ProcedureStep procedureStep)
		{
			var createColorStep = (CreateColorStep)procedureStep;
			var a = GetValue<int>(createColorStep.AArgument);
			var r = GetValue<int>(createColorStep.RArgument);
			var g = GetValue<int>(createColorStep.GArgument);
			var b = GetValue<int>(createColorStep.BArgument);
			var resultVariable = AllVariables.FirstOrDefault(x => x.Uid == createColorStep.ResultArgument.VariableUid);

			if (resultVariable != null)
			{
				resultVariable.ExplicitValue.ColorValue = System.Windows.Media.Color.FromArgb(IntToByte(a), IntToByte(r), IntToByte(g), IntToByte(b));
				ProcedureExecutionContext.SynchronizeVariable(resultVariable, ClientUID);
			}
		}

		public void SoundStep(ProcedureStep procedureStep)
		{
			var soundStep = (SoundStep)procedureStep;
			var automationCallbackResult = new AutomationCallbackResult()
			{
				AutomationCallbackType = AutomationCallbackType.Sound,
				Data = new SoundCallbackData()
				{
					SoundUID = soundStep.SoundUid,
				},
			};
			SendCallback(soundStep, automationCallbackResult);
		}

		public void PauseStep(ProcedureStep procedureStep)
		{
			var pauseStep = (PauseStep)procedureStep;
			var pause = new TimeSpan();
			switch (pauseStep.TimeType)
			{
				case TimeType.Millisec:
					pause = TimeSpan.FromMilliseconds(GetValue<int>(pauseStep.PauseArgument));
					break;
				case TimeType.Sec:
					pause = TimeSpan.FromSeconds(GetValue<int>(pauseStep.PauseArgument));
					break;
				case TimeType.Min:
					pause = TimeSpan.FromMinutes(GetValue<int>(pauseStep.PauseArgument));
					break;
				case TimeType.Hour:
					pause = TimeSpan.FromHours(GetValue<int>(pauseStep.PauseArgument));
					break;
				case TimeType.Day:
					pause = TimeSpan.FromDays(GetValue<int>(pauseStep.PauseArgument));
					break;
			}

			AutoResetEvent.WaitOne(pause);
		}

		public void JournalStep(ProcedureStep procedureStep)
		{
			var journalStep = (JournalStep)procedureStep;
			var messageValue = GetStringValue(journalStep.MessageArgument);
			ProcedureExecutionContext.AddJournalItem(ClientUID, messageValue);
		}

		public void ShowMessageStep(ProcedureStep procedureStep)
		{
			var showMessageStep = (ShowMessageStep)procedureStep;
			var messageString = GetStringValue(showMessageStep.MessageArgument);
			var automationCallbackResult = new AutomationCallbackResult()
			{
				AutomationCallbackType = AutomationCallbackType.Message,
				Data = new MessageCallbackData()
				{
					IsModalWindow = showMessageStep.IsModalWindow,
					Message = messageString,
					WithConfirmation = showMessageStep.WithConfirmation
				},
			};

			if (showMessageStep.WithConfirmation)
			{
				var value = SendCallback(showMessageStep, automationCallbackResult, true);
				SetValue(showMessageStep.ConfirmationValueArgument, value);
			}
			else
				SendCallback(showMessageStep, automationCallbackResult);
		}

		public void FindObjectStep(ProcedureStep procedureStep)
		{
			var findObjectStep = (FindObjectStep)procedureStep;
			var variable = AllVariables.FirstOrDefault(x => x.Uid == findObjectStep.ResultArgument.VariableUid);
			if (findObjectStep.JoinOperator == JoinOperator.Or)
				FindObjectsOr(variable, findObjectStep.FindObjectConditions);
			else
				FindObjectsAnd(variable, findObjectStep.FindObjectConditions);
		}

		public void ControlGKDeviceStep(ProcedureStep procedureStep)
		{
			var controlGKDeviceStep = (ControlGKDeviceStep)procedureStep;
			var deviceRef = GetValue<ObjectReference>(controlGKDeviceStep.GKDeviceArgument);
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceRef.UID);
			if (device == null)
				return;
			ProcedureExecutionContext.ControlGKDevice(ClientUID, device.UID, controlGKDeviceStep.Command);
		}

		public void ControlGKFireZoneStep(ProcedureStep procedureStep)
		{
			var controlGKFireZoneStep = (ControlGKFireZoneStep)procedureStep;
			var zoneRef = GetValue<ObjectReference>(controlGKFireZoneStep.GKFireZoneArgument);
			if (!LicenseManager.CurrentLicenseInfo.HasFirefighting)
			{
				ProcedureExecutionContext.AddJournalItem(ClientUID, "Выполнение функции \"Управление пожарной зоной\" заблокировано в связи с отсутствием лицензии", zoneRef.UID);
				return;
			}
			if (!HasPermission(PermissionType.Oper_Zone_Control))
			{
				ProcedureExecutionContext.AddJournalItem(ClientUID, "Выполнение функции \"Управление пожарной зоной\" заблокировано в связи с отсутствием прав пользователя", zoneRef.UID);
				return;
			}

			ProcedureExecutionContext.ControlFireZone(ClientUID, zoneRef.UID, controlGKFireZoneStep.ZoneCommandType);
		}

		public void ControlGKGuardZoneStep(ProcedureStep procedureStep)
		{
			var controlGKGuardZoneStep = (ControlGKGuardZoneStep)procedureStep;
			var zoneRef = GetValue<ObjectReference>(controlGKGuardZoneStep.GKGuardZoneArgument);
			if (!LicenseManager.CurrentLicenseInfo.HasGuard)
			{
				ProcedureExecutionContext.AddJournalItem(ClientUID, "Выполнение функции \"Управление охранной зоной\" заблокировано в связи с отсутствием лицензии", zoneRef.UID);
				return;
			}
			if (!HasPermission(PermissionType.Oper_GuardZone_Control))
			{
				ProcedureExecutionContext.AddJournalItem(ClientUID, "Выполнение функции \"Управление охранной зоной\" заблокировано в связи с отсутствием прав пользователя", zoneRef.UID);
				return;
			}

			ProcedureExecutionContext.ControlGuardZone(ClientUID, zoneRef.UID, controlGKGuardZoneStep.GuardZoneCommandType);
		}

		public void ControlDirectionStep(ProcedureStep procedureStep)
		{
			var controlDirectionStep = (ControlDirectionStep)procedureStep;
			var directionRef = GetValue<ObjectReference>(controlDirectionStep.DirectionArgument);
			ProcedureExecutionContext.ControlDirection(ClientUID, directionRef.UID, controlDirectionStep.DirectionCommandType);
		}

		public void ControlGKDoorStep(ProcedureStep procedureStep)
		{
			var controlGKDoorStep = (ControlGKDoorStep)procedureStep;
			var doorRef = GetValue<ObjectReference>(controlGKDoorStep.DoorArgument);
			if (!LicenseManager.CurrentLicenseInfo.HasSKD)
			{
				ProcedureExecutionContext.AddJournalItem(ClientUID, "Выполнение функции \"Управление точкой доступа\" заблокировано в связи с отсутствием лицензии", doorRef.UID);
				return;
			}
			if (!HasPermission(PermissionType.Oper_ZonesSKD))
			{
				ProcedureExecutionContext.AddJournalItem(ClientUID, "Выполнение функции \"Управление точкой доступа\" заблокировано в связи с отсутствием прав пользователя", doorRef.UID);
				return;
			}

			ProcedureExecutionContext.ControlGKDoor(ClientUID, doorRef.UID, controlGKDoorStep.DoorCommandType);
		}

		public void ControlDelayStep(ProcedureStep procedureStep)
		{
			var controlDelayStep = (ControlDelayStep)procedureStep;
			var delayRef = GetValue<ObjectReference>(controlDelayStep.DelayArgument);
			ProcedureExecutionContext.ControlDelay(ClientUID, delayRef.UID, controlDelayStep.DelayCommandType);
		}

		public void ControlPumpStationStep(ProcedureStep procedureStep)
		{
			var controlPumpStationStep = (ControlPumpStationStep)procedureStep;
			var pumpStationRef = GetValue<ObjectReference>(controlPumpStationStep.PumpStationArgument);
			ProcedureExecutionContext.ControlPumpStation(ClientUID, pumpStationRef.UID, controlPumpStationStep.PumpStationCommandType);
		}

		public void ControlMPTStep(ProcedureStep procedureStep)
		{
			var controlMPTStep = (ControlMPTStep)procedureStep;
			var mptRef = GetValue<ObjectReference>(controlMPTStep.MPTArgument);
			ProcedureExecutionContext.ControlMPT(ClientUID, mptRef.UID, controlMPTStep.MPTCommandType);
		}

		public void IncrementValueStep(ProcedureStep procedureStep)
		{
			var incrementValueStep = (IncrementValueStep)procedureStep;
			var variable = AllVariables.FirstOrDefault(x => x.Uid == incrementValueStep.ResultArgument.VariableUid);
			var value = GetValue<double>(incrementValueStep.ResultArgument);
			if (incrementValueStep.IncrementType == IncrementType.Inc)
				ProcedureExecutionContext.SetVariableValue(variable, value + 1, ClientUID);
			else
				ProcedureExecutionContext.SetVariableValue(variable, value - 1, ClientUID);
		}

		public void SetValueStep(ProcedureStep procedureStep)
		{
			var setValueStep = (SetValueStep)procedureStep;
			var value = setValueStep.ExplicitType == ExplicitType.String ?
				GetStringValue(setValueStep.SourceArgument) :
				GetValue<object>(setValueStep.SourceArgument);
			SetValue(setValueStep.TargetArgument, value);
		}

		public void RandomStep(ProcedureStep procedureStep)
		{
			var randomStep = (RandomStep)procedureStep;
			var resultVariable = AllVariables.FirstOrDefault(x => x.Uid == randomStep.ResultArgument.VariableUid);
			var maxValue = GetValue<int>(randomStep.MaxValueArgument);
			if (resultVariable != null)
				resultVariable.ExplicitValue.IntValue = new Random().Next(0, maxValue);
		}

		public void ChangeListStep(ProcedureStep procedureStep)
		{
			var changeListStep = (ChangeListStep)procedureStep;
			var listVariable = AllVariables.FirstOrDefault(x => x.Uid == changeListStep.ListArgument.VariableUid);
			var variable = new Variable();
			variable.ExplicitType = changeListStep.ItemArgument.ExplicitType;
			variable.EnumType = changeListStep.ItemArgument.EnumType;
			variable.ObjectType = changeListStep.ItemArgument.ObjectType;
			var itemValue = GetValue<object>(changeListStep.ItemArgument);
			ProcedureExecutionContext.SetVariableValue(variable, itemValue, ClientUID);
			var explicitValue = variable.ExplicitValue;
			if (listVariable != null)
			{
				//ProcedureExecutionContext.SynchronizeVariable(listVariable, ContextType.Client);
				switch (changeListStep.ChangeType)
				{
					case ChangeType.AddLast:
						listVariable.ExplicitValues.Add(explicitValue);
						break;
					case ChangeType.RemoveFirst:
						listVariable.ExplicitValues.Remove(listVariable.ExplicitValues.FirstOrDefault
							(x => ExplicitCompare(x, explicitValue, changeListStep.ItemArgument.ExplicitType,
								changeListStep.ItemArgument.EnumType)));
						break;
					case ChangeType.RemoveAll:
						listVariable.ExplicitValues.RemoveAll(
							(x => ExplicitCompare(x, explicitValue, changeListStep.ItemArgument.ExplicitType,
								changeListStep.ItemArgument.EnumType)));
						break;
				}
				//ProcedureExecutionContext.SynchronizeVariable(listVariable, ContextType.Server);
			}
		}

		public void CheckPermissionStep(ProcedureStep procedureStep)
		{
			var checkPermissionStep = (CheckPermissionStep)procedureStep;
			var resultVariable = AllVariables.FirstOrDefault(x => x.Uid == checkPermissionStep.ResultArgument.VariableUid);
			var permissionValue = GetValue<PermissionType>(checkPermissionStep.PermissionArgument);
			if (resultVariable != null && User != null)
				resultVariable.ExplicitValue.BoolValue = User.HasPermission(permissionValue);
		}

		public void GetListCountStep(ProcedureStep procedureStep)
		{
			var getListCountStep = (GetListCountStep)procedureStep;
			var listVariable = AllVariables.FirstOrDefault(x => x.Uid == getListCountStep.ListArgument.VariableUid);
			var countVariable = AllVariables.FirstOrDefault(x => x.Uid == getListCountStep.CountArgument.VariableUid);
			if ((countVariable != null) && (listVariable != null))
			{
				//ProcedureExecutionContext.SynchronizeVariable(listVariable, ContextType.Client);
				countVariable.ExplicitValue.IntValue = listVariable.ExplicitValues.Count;
			}
		}

		public void GetListItemStep(ProcedureStep procedureStep)
		{
			var getListItemStep = (GetListItemStep)procedureStep;
			var listVariable = AllVariables.FirstOrDefault(x => x.Uid == getListItemStep.ListArgument.VariableUid);
			var itemVariable = AllVariables.FirstOrDefault(x => x.Uid == getListItemStep.ItemArgument.VariableUid);
			if (itemVariable != null && listVariable != null)
			{
				ProcedureExecutionContext.SynchronizeVariable(listVariable, ClientUID);
				if (getListItemStep.PositionType == PositionType.First)
				{
					var firstValue = listVariable.ExplicitValues.FirstOrDefault();
					ProcedureExecutionContext.SetVariableValue(itemVariable, firstValue == null ? null : firstValue.Value, ClientUID);
				}
				if (getListItemStep.PositionType == PositionType.Last)
				{
					var lastValue = listVariable.ExplicitValues.LastOrDefault();
					ProcedureExecutionContext.SetVariableValue(itemVariable, lastValue == null ? null : lastValue.Value, ClientUID);
				}
				if (getListItemStep.PositionType == PositionType.ByIndex)
				{
					var indexValue = GetValue<int>(getListItemStep.IndexArgument);
					if (listVariable.ExplicitValues.Count > indexValue)
						ProcedureExecutionContext.SetVariableValue(itemVariable, listVariable.ExplicitValues[indexValue].Value, ClientUID);
				}
			}
		}

		public void GetJournalItemStep(ProcedureStep procedureStep)
		{
			var getJournalItemStep = (GetJournalItemStep)procedureStep;
			var resultVariable = AllVariables.FirstOrDefault(x => x.Uid == getJournalItemStep.ResultArgument.VariableUid);
			var value = new object();
			if (JournalItem != null)
			{
				if (getJournalItemStep.JournalColumnType == JournalColumnType.DeviceDateTime)
					value = JournalItem.DeviceDateTime;
				if (getJournalItemStep.JournalColumnType == JournalColumnType.SystemDateTime)
					value = JournalItem.SystemDateTime;
				if (getJournalItemStep.JournalColumnType == JournalColumnType.JournalEventNameType)
					value = JournalItem.JournalEventNameType;
				if (getJournalItemStep.JournalColumnType == JournalColumnType.JournalEventDescriptionType)
					value = JournalItem.JournalEventDescriptionType;
				if (getJournalItemStep.JournalColumnType == JournalColumnType.JournalObjectType)
					value = JournalItem.JournalObjectType;
				if (getJournalItemStep.JournalColumnType == JournalColumnType.JournalObjectUid)
					value = JournalItem.ObjectUID.ToString();
				ProcedureExecutionContext.SetVariableValue(resultVariable, value, ClientUID);
			}
		}

		public void ControlVisualGetStep(ProcedureStep procedureStep)
		{
			var controlVisualGetStep = (ControlVisualGetStep)procedureStep;
			var automationCallbackResult = new AutomationCallbackResult()
			{
				AutomationCallbackType = AutomationCallbackType.GetVisualProperty,
				Data = new VisualPropertyCallbackData()
				{
					LayoutPart = controlVisualGetStep.LayoutPart,
					Property = controlVisualGetStep.Property.Value
				},
			};

			var value = SendCallback(controlVisualGetStep, automationCallbackResult, true);
			SetValue(controlVisualGetStep.Argument, value);
		}

		public void ControlVisualSetStep(ProcedureStep procedureStep)
		{
			var controlVisualSetStep = (ControlVisualSetStep)procedureStep;

			var automationCallbackResult = new AutomationCallbackResult()
			{
				AutomationCallbackType = AutomationCallbackType.SetVisualProperty,
				Data = new VisualPropertyCallbackData()
				{
					LayoutPart = controlVisualSetStep.LayoutPart,
					Property = controlVisualSetStep.Property.Value
				},
			};
			SendCallback(controlVisualSetStep, automationCallbackResult);
			if (controlVisualSetStep.ForAllClients)
				ProcedurePropertyCache.SetProperty(controlVisualSetStep.Layout, (VisualPropertyCallbackData)automationCallbackResult.Data);
		}

		public void ControlPlanGetStep(ProcedureStep procedureStep)
		{
			var controlPlanGetStep = (ControlPlanGetStep)procedureStep;

			var automationCallbackResult = new AutomationCallbackResult()
			{
				AutomationCallbackType = AutomationCallbackType.GetPlanProperty,
				Data = new PlanCallbackData()
				{
					PlanUid = controlPlanGetStep.PlanUid,
					ElementUid = controlPlanGetStep.ElementUid,
					ElementPropertyType = controlPlanGetStep.ElementPropertyType
				},
			};

			var value = SendCallback(controlPlanGetStep, automationCallbackResult, true);
			SetValue(controlPlanGetStep.ValueArgument, value);

		}

		public void ControlPlanSetStep(ProcedureStep procedureStep)
		{
			var controlPlanSetStep = (ControlPlanSetStep)procedureStep;

			var value = GetValue<object>(controlPlanSetStep.ValueArgument);
			if (value is int && (int)value < 0)
				return;

			var automationCallbackResult = new AutomationCallbackResult()
			{
				AutomationCallbackType = AutomationCallbackType.SetPlanProperty,
				Data = new PlanCallbackData()
				{
					PlanUid = controlPlanSetStep.PlanUid,
					ElementUid = controlPlanSetStep.ElementUid,
					ElementPropertyType = controlPlanSetStep.ElementPropertyType,
					Value = value
				},
			};

			SendCallback(controlPlanSetStep, automationCallbackResult);
			if (controlPlanSetStep.ForAllClients)
				ProcedurePropertyCache.SetProperty((PlanCallbackData)automationCallbackResult.Data);
		}

		public void ControlOpcDaTagGetStep(ProcedureStep procedureStep)
		{
			var controlOpcDaTagGetStep = (ControlOpcDaTagGetStep)procedureStep;
			var value = OpcDaHelper.GetTagValue(controlOpcDaTagGetStep.OpcDaTagUID);
			SetValue(controlOpcDaTagGetStep.ValueArgument, value);
		}

		public void ControlOpcDaTagSetStep(ProcedureStep procedureStep)
		{
			var controlOpcDaTagSetStep = (ControlOpcDaTagSetStep)procedureStep;
			var value = GetValue<object>(controlOpcDaTagSetStep.ValueArgument);
			OpcDaHelper.OnWriteTagValue(controlOpcDaTagSetStep.OpcDaTagUID, value);
		}

		public void ShowDialogStep(ProcedureStep procedureStep)
		{
			var showDialogStep = (ShowDialogStep)procedureStep;
			var windowID = GetValue<string>(showDialogStep.WindowIDArgument);
			var automationCallbackResult = new AutomationCallbackResult()
			{
				AutomationCallbackType = AutomationCallbackType.ShowDialog,
				Data = new ShowDialogCallbackData()
				{
					IsModalWindow = showDialogStep.IsModalWindow,
					Layout = showDialogStep.Layout,
					Title = showDialogStep.Title,
					AllowClose = showDialogStep.AllowClose,
					AllowMaximize = showDialogStep.AllowMaximize,
					Height = showDialogStep.Height,
					MinHeight = showDialogStep.MinHeight,
					MinWidth = showDialogStep.MinWidth,
					Sizable = showDialogStep.Sizable,
					TopMost = showDialogStep.TopMost,
					Width = showDialogStep.Width,
					CustomPosition = showDialogStep.CustomPosition,
					Left = showDialogStep.Left,
					Top = showDialogStep.Top,
					WindowID = windowID
				},
			};
			SendCallback(showDialogStep, automationCallbackResult);
		}

		public void CloseDialogStep(ProcedureStep procedureStep)
		{
			var closeDialogStep = (CloseDialogStep)procedureStep;
			var windowID = GetValue<string>(closeDialogStep.WindowIDArgument);
			var automationCallbackResult = new AutomationCallbackResult()
			{
				AutomationCallbackType = AutomationCallbackType.CloseDialog,
				Data = new CloseDialogCallbackData()
				{
					WindowID = windowID
				}
			};
			SendCallback(closeDialogStep, automationCallbackResult);
		}

		public void ShowPropertyStep(ProcedureStep procedureStep)
		{
			var showPropertyStep = (ShowPropertyStep)procedureStep;
			var objectRef = GetValue<ObjectReference>(showPropertyStep.ObjectArgument);
			if (showPropertyStep.ObjectType == ObjectType.Zone && !LicenseManager.CurrentLicenseInfo.HasFirefighting ||
				showPropertyStep.ObjectType == ObjectType.GuardZone && !LicenseManager.CurrentLicenseInfo.HasGuard ||
				showPropertyStep.ObjectType == ObjectType.GKDoor && !LicenseManager.CurrentLicenseInfo.HasSKD ||
				showPropertyStep.ObjectType == ObjectType.VideoDevice && !LicenseManager.CurrentLicenseInfo.HasVideo)
			{
				ProcedureExecutionContext.AddJournalItem(ClientUID, "Выполнение функции \"Показать свойства объекта\" заблокировано в связи с отсутствием лицензии", objectRef.UID);
				return;
			}
			var automationCallbackResult = new AutomationCallbackResult()
			{
				AutomationCallbackType = AutomationCallbackType.Property,
				Data = new PropertyCallBackData()
				{
					ObjectType = showPropertyStep.ObjectType,
					ObjectUid = objectRef.UID
				},
			};
			SendCallback(showPropertyStep, automationCallbackResult);
		}

		public void SendEmailStep(ProcedureStep procedureStep)
		{
			var sendEmailStep = (SendEmailStep)procedureStep;
			var smtp = GetValue<string>(sendEmailStep.SmtpArgument);
			var port = GetValue<int>(sendEmailStep.PortArgument);
			var login = GetValue<string>(sendEmailStep.LoginArgument);
			var password = GetValue<string>(sendEmailStep.PasswordArgument);
			var eMailAddressFrom = GetValue<string>(sendEmailStep.EMailAddressFromArgument);
			var eMailAddressTo = GetValue<string>(sendEmailStep.EMailAddressToArgument);
			var title = GetValue<string>(sendEmailStep.EMailTitleArgument);
			var content = GetValue<string>(sendEmailStep.EMailContentArgument);
			var Smtp = new SmtpClient(smtp, port);
			Smtp.Credentials = new NetworkCredential(login, password);
			Smtp.EnableSsl = true;
			var Message = new MailMessage();
			Message.From = new MailAddress(eMailAddressFrom);
			Message.To.Add(new MailAddress(eMailAddressTo));
			Message.Subject = title;
			Message.Body = content;

			try
			{
				Smtp.Send(Message);
			}
			catch
			{
			}
		}

		public void HttpRequestStep(ProcedureStep procedureStep)
		{
			var httpRequestStep = (HttpRequestStep)procedureStep;
			var url = GetValue<string>(httpRequestStep.UrlArgument);
			var content = GetValue<string>(httpRequestStep.ContentArgument);
			var responseVariable = AllVariables.FirstOrDefault(x => x.Uid == httpRequestStep.ResponseArgument.VariableUid);
			var webClient = new WebClient();
			webClient.Encoding = System.Text.Encoding.UTF8;
			var response = "";
			switch (httpRequestStep.HttpMethod)
			{
				case HttpMethod.Get:
					response = webClient.DownloadString(url);
					break;

				case HttpMethod.Post:
					response = webClient.UploadString(url, content);
					break;
			}

			SetValue(httpRequestStep.ResponseArgument, response);
		}

		public void StartRecordStep(ProcedureStep procedureStep)
		{
			var startRecordStep = (StartRecordStep)procedureStep;
			var cameraRef = GetValue<ObjectReference>(startRecordStep.CameraArgument);
			if (LicenseManager.CurrentLicenseInfo.HasVideo)
			{
				var timeout = GetValue<int>(startRecordStep.TimeoutArgument);
				switch (startRecordStep.TimeType)
				{
					case TimeType.Millisec: timeout = (int)((double)timeout * 0.001); break;
					case TimeType.Min: timeout *= 60; break;
					case TimeType.Hour: timeout *= 3600; break;
					case TimeType.Day: timeout *= 86400; break;
				}

				if (JournalItem != null)
				{

					Guid eventUid = Guid.NewGuid();
					SetValue(startRecordStep.EventUIDArgument, eventUid);
					ProcedureExecutionContext.StartRecord(ClientUID, cameraRef.UID, JournalItem.UID, eventUid, timeout);
				}
			}
			else
				ProcedureExecutionContext.AddJournalItem(ClientUID, "Выполнение функции \"Начать запись\" заблокировано в связи с отсутствием лицензии", cameraRef.UID);
		}

		public void StopRecordStep(ProcedureStep procedureStep)
		{
			var stopRecordStep = (StopRecordStep)procedureStep;
			var cameraRef = GetValue<ObjectReference>(stopRecordStep.CameraArgument);
			var eventUid = GetValue<Guid>(stopRecordStep.EventUIDArgument);
			if (LicenseManager.CurrentLicenseInfo.HasVideo)
				ProcedureExecutionContext.StopRecord(ClientUID, cameraRef.UID, eventUid);
			else
				ProcedureExecutionContext.AddJournalItem(ClientUID, "Выполнение функции \"Остановить запись\" заблокировано в связи с отсутствием лицензии", cameraRef.UID);
		}

		public void PtzStep(ProcedureStep procedureStep)
		{
			var ptzStep = (PtzStep)procedureStep;
			var cameraRef = GetValue<ObjectReference>(ptzStep.CameraArgument);
			var ptzNumber = GetValue<int>(ptzStep.PtzNumberArgument);
			if (LicenseManager.CurrentLicenseInfo.HasVideo)
				ProcedureExecutionContext.Ptz(ClientUID, cameraRef.UID, ptzNumber);
			else
				ProcedureExecutionContext.AddJournalItem(ClientUID, "Выполнение функции \"Ptz камеры\" заблокировано в связи с отсутствием лицензии", cameraRef.UID);
		}

		public void RviAlarmStep(ProcedureStep procedureStep)
		{
			var rviAlarmStep = (RviAlarmStep)procedureStep;
			var name = GetValue<string>(rviAlarmStep.NameArgument);
			if (LicenseManager.CurrentLicenseInfo.HasVideo)
				ProcedureExecutionContext.RviAlarm(ClientUID, name);
			else
				ProcedureExecutionContext.AddJournalItem(ClientUID, "Выполнение функции \"Вызвать тревогу в Rvi Оператор\" заблокировано в связи с отсутствием лицензии");
		}

		public void RviOpenWindowStep(ProcedureStep procedureStep)
		{
			var rviOpenWindowStep = (RviOpenWindowStep)procedureStep;
			var name = GetValue<string>(rviOpenWindowStep.NameArgument);
			var x = GetValue<int>(rviOpenWindowStep.XArgument);
			var y = GetValue<int>(rviOpenWindowStep.YArgument);
			var monitorNumber = GetValue<int>(rviOpenWindowStep.MonitorNumberArgument);
			var login = GetValue<string>(rviOpenWindowStep.LoginArgument);
			var ip = GetValue<string>(rviOpenWindowStep.IpArgument);
			if (LicenseManager.CurrentLicenseInfo.HasVideo)
				ProcedureExecutionContext.RviOpenWindow(ClientUID, name, x, y, monitorNumber, login, ip);
			else
				ProcedureExecutionContext.AddJournalItem(ClientUID, "Выполнение функции \"Показать раскладку в Rvi Оператор\" заблокировано в связи с отсуствием лицензии");
		}

		public void NowStep(ProcedureStep procedureStep)
		{
			var nowStep = (NowStep)procedureStep;
			SetValue(nowStep.ResultArgument, DateTime.Now);
		}

		public void RunProgramStep(ProcedureStep procedureStep)
		{
			var runProgramStep = (RunProgramStep)procedureStep;
			var processName = GetValue<string>(runProgramStep.PathArgument);
			var parameters = GetValue<string>(runProgramStep.ParametersArgument);
			System.Diagnostics.Process.Start(processName, parameters);
		}

		public void ExportJournalStep(ProcedureStep procedureStep)
		{
			var exportJournalStep = (ExportJournalStep)procedureStep;
			var isExportJournal = GetValue<bool>(exportJournalStep.IsExportJournalArgument);
			var isExportPassJournal = GetValue<bool>(exportJournalStep.IsExportPassJournalArgument);
			var minDate = GetValue<DateTime>(exportJournalStep.MinDateArgument);
			var maxDate = GetValue<DateTime>(exportJournalStep.MaxDateArgument);
			var path = GetValue<string>(exportJournalStep.PathArgument);
			ProcedureExecutionContext.ExportJournal(ClientUID, isExportJournal, isExportPassJournal, minDate, maxDate, path);
		}

		public void ExportOrganisationStep(ProcedureStep procedureStep)
		{
			var exportOrganisationStep = (ExportOrganisationStep)procedureStep;
			var isWithDeleted = GetValue<bool>(exportOrganisationStep.IsWithDeleted);
			var organisationRef = GetValue<ObjectReference>(exportOrganisationStep.Organisation);
			var path = GetValue<string>(exportOrganisationStep.PathArgument);
			ProcedureExecutionContext.ExportOrganisation(ClientUID, isWithDeleted, organisationRef.UID, path);
		}

		public void ExportOrganisationListStep(ProcedureStep procedureStep)
		{
			var exportOrganisationListStep = (ExportOrganisationListStep)procedureStep;
			var isWithDeleted = GetValue<bool>(exportOrganisationListStep.IsWithDeleted);
			var path = GetValue<string>(exportOrganisationListStep.PathArgument);
			ProcedureExecutionContext.ExportOrganisationList(ClientUID, isWithDeleted, path);
		}

		public void ExportConfigurationStep(ProcedureStep procedureStep)
		{
			var exportConfigurationStep = (ExportConfigurationStep)procedureStep;
			var isExportDevices = GetValue<bool>(exportConfigurationStep.IsExportDevices);
			var isExportDoors = GetValue<bool>(exportConfigurationStep.IsExportDoors);
			var isExportZones = GetValue<bool>(exportConfigurationStep.IsExportZones);
			var path = GetValue<string>(exportConfigurationStep.PathArgument);
			ProcedureExecutionContext.ExportConfiguration(ClientUID, isExportDevices, isExportDoors, isExportZones, path);
		}

		public void ImportOrganisationStep(ProcedureStep procedureStep)
		{
			var importOrganisationStep = (ImportOrganisationStep)procedureStep;
			var isWithDeleted = GetValue<bool>(importOrganisationStep.IsWithDeleted);
			var path = GetValue<string>(importOrganisationStep.PathArgument);
			ProcedureExecutionContext.ImportOrganisation(ClientUID, isWithDeleted, path);
		}

		public void ImportOrganisationListStep(ProcedureStep procedureStep)
		{
			var importOrganisationListStep = (ImportOrganisationListStep)procedureStep;
			var isWithDeleted = GetValue<bool>(importOrganisationListStep.IsWithDeleted);
			var path = GetValue<string>(importOrganisationListStep.PathArgument);
			ProcedureExecutionContext.ImportOrganisationList(ClientUID, isWithDeleted, path);
		}

		#region Common operations

		string GetStringValue(Argument argument)
		{
			if (argument == null)
				return null;
			if (argument.VariableScope == VariableScope.ExplicitValue)
				return argument.ExplicitValue.ToString();

			var variable = AllVariables.FirstOrDefault(x => x.Uid == argument.VariableUid);
			return variable == null ? "" : variable.ExplicitValue.ToString();
		}

		static byte IntToByte(int value)
		{
			if (value < 0)
				value = 0;
			if (value > 255)
				value = 255;
			return Convert.ToByte(value);
		}

		Guid GetObjectUid(object item)
		{
			if (item is ModelBase)
				return (item as ModelBase).UID;

			if (item is RubezhAPI.SKD.Organisation)
				return (item as RubezhAPI.SKD.Organisation).UID;

			if (item is Camera)
				return (item as Camera).UID;

			return Guid.Empty;
		}

		object GetPropertyValue(Property property, object item)
		{
			var propertyValue = new object();
			if (item is GKDevice)
			{
				var gkDevice = item as GKDevice;
				switch (property)
				{
					case Property.ShleifNo:
						propertyValue = (int)gkDevice.ShleifNo;
						break;
					case Property.IntAddress:
						propertyValue = gkDevice.IntAddress;
						break;
					case Property.State:
						propertyValue = (int)gkDevice.State.StateClass;
						break;
					case Property.Type:
						propertyValue = gkDevice.Driver.DriverType;
						break;
					case Property.Description:
						propertyValue = gkDevice.Description == null ? "" : gkDevice.Description.Trim();
						break;
					case Property.Uid:
						propertyValue = Convert.ToString(gkDevice.UID);
						break;
				}
			}

			if (item is GKDelay)
			{
				var gkDelay = item as GKDelay;
				switch (property)
				{
					case Property.Name:
						propertyValue = gkDelay.Name.Trim();
						break;
					case Property.No:
						propertyValue = gkDelay.No;
						break;
					case Property.Delay:
						propertyValue = (int)gkDelay.DelayTime;
						break;
					case Property.CurrentDelay:
						propertyValue = gkDelay.State.OnDelay;
						break;
					case Property.State:
						propertyValue = (int)gkDelay.State.StateClass;
						break;
					case Property.Hold:
						propertyValue = gkDelay.Hold;
						break;
					case Property.CurrentHold:
						propertyValue = gkDelay.State.HoldDelay;
						break;
					case Property.DelayRegime:
						propertyValue = gkDelay.DelayRegime;
						break;
					case Property.Description:
						propertyValue = gkDelay.Description == null ? "" : gkDelay.Description.Trim(); ;
						break;
					case Property.Uid:
						propertyValue = gkDelay.UID.ToString();
						break;
				}
			}

			if (item is GKZone)
			{
				var gkZone = item as GKZone;
				switch (property)
				{
					case Property.No:
						propertyValue = gkZone.No;
						break;
					case Property.Type:
						propertyValue = gkZone.ObjectType;
						break;
					case Property.State:
						propertyValue = (int)gkZone.State.StateClass;
						break;
					case Property.Name:
						propertyValue = gkZone.Name.Trim();
						break;
					case Property.Description:
						propertyValue = gkZone.Description == null ? "" : gkZone.Description.Trim();
						break;
					case Property.Uid:
						propertyValue = gkZone.UID.ToString();
						break;
				}
			}

			if (item is GKDirection)
			{
				var gkDirection = item as GKDirection;
				switch (property)
				{
					case Property.Name:
						propertyValue = gkDirection.Name.Trim();
						break;
					case Property.No:
						propertyValue = gkDirection.No;
						break;
					case Property.Delay:
						propertyValue = (int)gkDirection.Delay;
						break;
					case Property.CurrentDelay:
						propertyValue = (int)gkDirection.State.OnDelay;
						break;
					case Property.Hold:
						propertyValue = (int)gkDirection.Hold;
						break;
					case Property.CurrentHold:
						propertyValue = (int)gkDirection.State.HoldDelay;
						break;
					case Property.DelayRegime:
						propertyValue = (int)gkDirection.DelayRegime;
						break;
					case Property.Description:
						propertyValue = gkDirection.Description.Trim();
						break;
					case Property.Uid:
						propertyValue = gkDirection.UID.ToString();
						break;
					case Property.State:
						propertyValue = (int)gkDirection.State.StateClass;
						break;
				}
			}

			if (item is GKGuardZone)
			{
				var gkGuardZone = item as GKGuardZone;
				switch (property)
				{
					case Property.No:
						propertyValue = gkGuardZone.No;
						break;
					case Property.Type:
						propertyValue = gkGuardZone.ObjectType;
						break;
					case Property.State:
						propertyValue = gkGuardZone.State.StateClass;
						break;
					case Property.Name:
						propertyValue = gkGuardZone.Name.Trim();
						break;
					case Property.Description:
						propertyValue = gkGuardZone.Description == null ? "" : gkGuardZone.Description.Trim();
						break;
					case Property.Uid:
						propertyValue = gkGuardZone.UID.ToString();
						break;
				}
			}

			if (item is GKDoor)
			{
				var gkDoor = item as GKDoor;
				switch (property)
				{
					case Property.Name:
						propertyValue = gkDoor.Name.Trim();
						break;
					case Property.No:
						propertyValue = gkDoor.No;
						break;
					case Property.Delay:
						propertyValue = gkDoor.Delay;
						break;
					case Property.CurrentDelay:
						propertyValue = gkDoor.State.OnDelay;
						break;
					case Property.State:
						propertyValue = (int)gkDoor.State.StateClass;
						break;
					case Property.Hold:
						propertyValue = gkDoor.Hold;
						break;
					case Property.CurrentHold:
						propertyValue = gkDoor.State.HoldDelay;
						break;
					case Property.Description:
						propertyValue = gkDoor.Description == null ? "" : gkDoor.Description.Trim();
						break;
					case Property.Uid:
						propertyValue = gkDoor.UID.ToString();
						break;
				}
			}

			if (item is GKPumpStation)
			{
				var gkPumpStation = item as GKPumpStation;
				switch (property)
				{
					case Property.Name:
						propertyValue = gkPumpStation.Name.Trim();
						break;
					case Property.No:
						propertyValue = gkPumpStation.No;
						break;
					case Property.Delay:
						propertyValue = gkPumpStation.Delay;
						break;
					case Property.CurrentDelay:
						propertyValue = gkPumpStation.State.OnDelay;
						break;
					case Property.State:
						propertyValue = (int)gkPumpStation.State.StateClass;
						break;
					case Property.Hold:
						propertyValue = gkPumpStation.Hold;
						break;
					case Property.CurrentHold:
						propertyValue = gkPumpStation.State.HoldDelay;
						break;
					case Property.Description:
						propertyValue = gkPumpStation.Description == null ? "" : gkPumpStation.Description.Trim();
						break;
					case Property.Uid:
						propertyValue = gkPumpStation.UID.ToString();
						break;
				}
			}

			if (item is GKMPT)
			{
				var gkMPT = item as GKMPT;
				switch (property)
				{
					case Property.Name:
						propertyValue = gkMPT.Name.Trim();
						break;
					case Property.No:
						propertyValue = gkMPT.No;
						break;
					case Property.Delay:
						propertyValue = gkMPT.Delay;
						break;
					case Property.CurrentDelay:
						propertyValue = gkMPT.State.OnDelay;
						break;
					case Property.State:
						propertyValue = (int)gkMPT.State.StateClass;
						break;
					case Property.Hold:
						propertyValue = gkMPT.Hold;
						break;
					case Property.CurrentHold:
						propertyValue = gkMPT.State.HoldDelay;
						break;
					case Property.Description:
						propertyValue = gkMPT.Description == null ? "" : gkMPT.Description.Trim();
						break;
					case Property.Uid:
						propertyValue = gkMPT.UID.ToString();
						break;
				}
			}

			if (item is Camera)
			{
				var camera = item as Camera;
				switch (property)
				{
					case Property.Name:
						propertyValue = camera.Name.Trim();
						break;
					case Property.Uid:
						propertyValue = camera.UID.ToString();
						break;
				}
			}

			if (item is Organisation)
			{
				var organisation = item as Organisation;
				switch (property)
				{
					case Property.Name:
						propertyValue = organisation.Name.Trim();
						break;
					case Property.Description:
						propertyValue = organisation.Description == null ? "" : organisation.Description.Trim();
						break;
					case Property.Uid:
						propertyValue = organisation.UID.ToString();
						break;
				}
			}

			return propertyValue;
		}

		IEnumerable<object> GetObjects(ObjectType objectType)
		{
			switch (objectType)
			{
				case ObjectType.Delay:
					return new List<GKDelay>(GKManager.DeviceConfiguration.Delays);
				case ObjectType.Device:
					return new List<GKDevice>(GKManager.DeviceConfiguration.Devices);
				case ObjectType.Direction:
					return new List<GKDirection>(GKManager.DeviceConfiguration.Directions);
				case ObjectType.GKDoor:
					return new List<GKDoor>(GKManager.DeviceConfiguration.Doors);
				case ObjectType.GuardZone:
					return new List<GKGuardZone>(GKManager.DeviceConfiguration.GuardZones);
				case ObjectType.VideoDevice:
					return new List<Camera>(ProcedureExecutionContext.SystemConfiguration.Cameras);
				case ObjectType.Zone:
					return new List<GKZone>(GKManager.DeviceConfiguration.Zones);
				case ObjectType.PumpStation:
					return new List<GKPumpStation>(GKManager.DeviceConfiguration.PumpStations);
				case ObjectType.MPT:
					return new List<GKMPT>(GKManager.DeviceConfiguration.MPTs);
				case ObjectType.Organisation:
					return new List<Organisation>(ProcedureExecutionContext.GetOrganisations(ClientUID));
			}
			return new List<object>();
		}

		object InitializeItem(ObjectReference objRef)
		{
			switch (objRef.ObjectType)
			{
				case ObjectType.Device:
					return GKManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == objRef.UID);
				case ObjectType.Zone:
					return GKManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == objRef.UID);
				case ObjectType.Direction:
					return GKManager.DeviceConfiguration.Directions.FirstOrDefault(x => x.UID == objRef.UID);
				case ObjectType.Delay:
					return GKManager.DeviceConfiguration.Delays.FirstOrDefault(x => x.UID == objRef.UID);
				case ObjectType.GuardZone:
					return GKManager.DeviceConfiguration.GuardZones.FirstOrDefault(x => x.UID == objRef.UID);
				case ObjectType.PumpStation:
					return GKManager.DeviceConfiguration.PumpStations.FirstOrDefault(x => x.UID == objRef.UID);
				case ObjectType.MPT:
					return GKManager.DeviceConfiguration.MPTs.FirstOrDefault(x => x.UID == objRef.UID);
				case ObjectType.VideoDevice:
					return ProcedureExecutionContext.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == objRef.UID);
				case ObjectType.GKDoor:
					return GKManager.Doors.FirstOrDefault(x => x.UID == objRef.UID);
				case ObjectType.Organisation:
					var organisations = ProcedureExecutionContext.GetOrganisations(ClientUID);
					return organisations == null ? null : organisations.FirstOrDefault(x => x.UID == objRef.UID);
			}

			return null;
		}

		void FindObjectsOr(Variable result, IEnumerable<FindObjectCondition> findObjectConditions)
		{
			var items = GetObjects(result.ObjectType);
			result.ExplicitValues.Clear();
			foreach (var item in items)
			{
				var itemUid = GetObjectUid(item);
				foreach (var findObjectCondition in findObjectConditions)
				{
					var propertyValue = GetPropertyValue(findObjectCondition.Property, item);
					var conditionValue = GetValue<object>(findObjectCondition.SourceArgument);
					var comparer = Compare(propertyValue, conditionValue, findObjectCondition.ConditionType);
					if (comparer != null && comparer.Value)
					{
						result.ExplicitValues.Add(new ExplicitValue { ExplicitType = ExplicitType.Object, ObjectReferenceValue = new ObjectReference { UID = itemUid, ObjectType = result.ObjectType } });
						break;
					}
				}
			}
		}

		void FindObjectsAnd(Variable result, IEnumerable<FindObjectCondition> findObjectConditions)
		{
			var items = GetObjects(result.ObjectType);
			result.ExplicitValues.Clear();
			bool allTrue;
			foreach (var item in items)
			{
				allTrue = true;
				var itemUid = GetObjectUid(item);
				foreach (var findObjectCondition in findObjectConditions)
				{
					var propertyValue = GetPropertyValue(findObjectCondition.Property, item);
					var conditionValue = GetValue<object>(findObjectCondition.SourceArgument);
					var comparer = Compare(propertyValue, conditionValue, findObjectCondition.ConditionType);
					if (comparer == null || !comparer.Value)
					{
						allTrue = false;
						break;
					}
				}
				if (allTrue)
					result.ExplicitValues.Add(new ExplicitValue { ExplicitType = ExplicitType.Object, ObjectReferenceValue = new ObjectReference { UID = itemUid, ObjectType = result.ObjectType } });
			}
		}

		bool? Compare(object param1, object param2, ConditionType conditionType)
		{
			if (param1.GetType().IsEnum || param1 is int)
			{
				return conditionType == ConditionType.IsEqual && (int)param1 == (int)param2
					|| conditionType == ConditionType.IsNotEqual && (int)param1 != (int)param2
					|| conditionType == ConditionType.IsMore && (int)param1 > (int)param2
					|| conditionType == ConditionType.IsNotMore && (int)param1 <= (int)param2
					|| conditionType == ConditionType.IsLess && (int)param1 < (int)param2
					|| conditionType == ConditionType.IsNotLess && (int)param1 >= (int)param2;
			}

			if (param1.GetType() != param2.GetType())
				return null;

			if (param1 is DateTime)
			{
				return conditionType == ConditionType.IsEqual && (DateTime)param1 == (DateTime)param2
					|| conditionType == ConditionType.IsNotEqual && (DateTime)param1 != (DateTime)param2
					|| conditionType == ConditionType.IsMore && (DateTime)param1 > (DateTime)param2
					|| conditionType == ConditionType.IsNotMore && (DateTime)param1 <= (DateTime)param2
					|| conditionType == ConditionType.IsLess && (DateTime)param1 < (DateTime)param2
					|| conditionType == ConditionType.IsNotLess && (DateTime)param1 >= (DateTime)param2;
			}

			if (param1 is string)
			{
				return conditionType == ConditionType.IsEqual && (string)param1 == (string)param2
					|| conditionType == ConditionType.IsNotEqual && (string)param1 != (string)param2
					|| conditionType == ConditionType.StartsWith && ((string)param1).StartsWith((string)param2)
					|| conditionType == ConditionType.EndsWith && ((string)param1).EndsWith((string)param2)
					|| conditionType == ConditionType.Contains && ((string)param1).Contains((string)param2);
			}

			if (param1 is bool)
			{
				return conditionType == ConditionType.IsEqual && (bool)param1 == (bool)param2
						|| conditionType == ConditionType.IsNotEqual && (bool)param1 != (bool)param2;
			}

			return null;
		}

		bool ExplicitCompare(ExplicitValue explicitValue1, ExplicitValue explicitValue2, ExplicitType explicitType, EnumType enumType)
		{
			if (explicitType == ExplicitType.Integer)
				return explicitValue1.IntValue == explicitValue2.IntValue;
			if (explicitType == ExplicitType.Float)
				return explicitValue1.FloatValue == explicitValue2.FloatValue;
			if (explicitType == ExplicitType.String)
				return explicitValue1.StringValue == explicitValue2.StringValue;
			if (explicitType == ExplicitType.Boolean)
				return explicitValue1.BoolValue == explicitValue2.BoolValue;
			if (explicitType == ExplicitType.DateTime)
				return explicitValue1.DateTimeValue == explicitValue2.DateTimeValue;
			if (explicitType == ExplicitType.Object)
				return explicitValue1.ObjectReferenceValue.UID == explicitValue2.ObjectReferenceValue.UID
					&& explicitValue1.ObjectReferenceValue.ObjectType == explicitValue2.ObjectReferenceValue.ObjectType;
			if (explicitType == ExplicitType.Enum)
			{
				if (enumType == EnumType.DriverType)
					return explicitValue1.DriverTypeValue == explicitValue2.DriverTypeValue;
				if (enumType == EnumType.StateType)
					return explicitValue1.StateTypeValue == explicitValue2.StateTypeValue;
				if (enumType == EnumType.PermissionType)
					return explicitValue1.PermissionTypeValue == explicitValue2.PermissionTypeValue;
				if (enumType == EnumType.JournalEventNameType)
					return explicitValue1.JournalEventNameTypeValue == explicitValue2.JournalEventNameTypeValue;
				if (enumType == EnumType.JournalEventDescriptionType)
					return explicitValue1.JournalEventDescriptionTypeValue == explicitValue2.JournalEventDescriptionTypeValue;
				if (enumType == EnumType.JournalObjectType)
					return explicitValue1.JournalObjectTypeValue == explicitValue2.JournalObjectTypeValue;
				if (enumType == EnumType.ColorType)
					return explicitValue1.ColorValue == explicitValue2.ColorValue;
			}

			return false;
		}

		void SetValue(Argument argument, object value)
		{
			ProcedureExecutionContext.SetVariableValue(AllVariables.FirstOrDefault(x => x.Uid == argument.VariableUid), value, ClientUID);
		}

		T GetValue<T>(Argument argument)
		{
			var result = argument.VariableScope == VariableScope.ExplicitValue ?
				argument.ExplicitValue.Value :
				ProcedureExecutionContext.GetVariableValue(ClientUID, AllVariables.FirstOrDefault(x => x.Uid == argument.VariableUid));
			if (result is string && typeof(T) == typeof(Guid))
				result = CheckGuid(result.ToString()) ? new Guid(result.ToString()) : Guid.Empty;
			if (result is int && typeof(T) == typeof(double))
				result = Convert.ToDouble(result);
			if (result is double && typeof(T) == typeof(int))
				result = Convert.ToInt32(Math.Round((double)result));
			return (T)result;
		}

		static bool CheckGuid(string guidString)
		{
			var guidRegEx = new Regex("^[A-Fa-f0-9]{32}$|" + "^({|\\()?[A-Fa-f0-9]{8}-([A-Fa-f0-9]{4}-){3}[A-Fa-f0-9]{12}(}|\\))?$|" +
				"^({)?[0xA-Fa-f0-9]{3,10}(, {0,1}[0xA-Fa-f0-9]{3,6}){2}, {0,1}({)([0xA-Fa-f0-9]{3,4}, {0,1}){7}[0xA-Fa-f0-9]{3,4}(}})$", RegexOptions.Compiled);
			if (!String.IsNullOrEmpty(guidString) && guidRegEx.IsMatch(guidString))
				return true;
			return false;
		}

		public void InitializeArguments(List<Variable> variables, List<Argument> arguments, List<Variable> callingProcedureVariables)
		{
			int i = 0;
			foreach (var variable in variables)
			{
				variable.ExplicitValues = new List<ExplicitValue>();
				if (arguments.Count <= i)
					break;
				var argument = arguments[i];
				if (argument == null)
					break;
				if (argument.VariableScope == VariableScope.ExplicitValue)
				{
					PropertyCopy.Copy(argument.ExplicitValue, variable.ExplicitValue);
					foreach (var explicitVal in argument.ExplicitValues)
					{
						var newExplicitValue = new ExplicitValue();
						PropertyCopy.Copy(explicitVal, newExplicitValue);
						variable.ExplicitValues.Add(newExplicitValue);
					}
				}
				else
				{
					var argumentVariable = callingProcedureVariables.FirstOrDefault(x => x.Uid == argument.VariableUid);
					if (argumentVariable == null)
						continue;
					if (argumentVariable.IsReference)
					{
						variable.ExplicitValue = argumentVariable.ExplicitValue;
						variable.ExplicitValues = argumentVariable.ExplicitValues;
					}
					else
					{
						PropertyCopy.Copy(argumentVariable.ExplicitValue, variable.ExplicitValue);
						foreach (var explicitVal in argumentVariable.ExplicitValues)
						{
							var newExplicitValue = new ExplicitValue();
							PropertyCopy.Copy(explicitVal, newExplicitValue);
							variable.ExplicitValues.Add(newExplicitValue);
						}
					}
				}
				i++;
			}
		}

		bool HasPermission(PermissionType permissionType)
		{
			return User == null ? false : User.HasPermission(permissionType);
		}

		bool Compare(ProcedureStep procedureStep)
		{
			var conditionStep = (ConditionStep)procedureStep;
			var result = conditionStep.JoinOperator == JoinOperator.And;
			foreach (var condition in conditionStep.Conditions)
			{
				var variable1 = GetValue<object>(condition.Argument1);
				var variable2 = GetValue<object>(condition.Argument2);
				var comparer = Compare(variable1, variable2, condition.ConditionType);
				if ((comparer != null))
					result = conditionStep.JoinOperator == JoinOperator.And ? result & comparer.Value : result | comparer.Value;
			}
			return result;
		}

		#endregion
	}
}
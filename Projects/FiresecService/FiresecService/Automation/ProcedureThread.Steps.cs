﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Windows.Media;
using FiresecAPI;
using FiresecAPI.Automation;
using FiresecAPI.AutomationCallback;
using FiresecAPI.GK;
using FiresecAPI.Journal;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using FiresecClient;
using FiresecService.Automation;
using FiresecService.Service;
using SKDDriver.Translators;
using Property = FiresecAPI.Automation.Property;

namespace FiresecService
{
	public partial class ProcedureThread
	{
		void AddJournalItem(ProcedureStep procedureStep)
		{
			var journalItem = new JournalItem();
			journalItem.SystemDateTime = DateTime.Now;
			journalItem.JournalEventNameType = JournalEventNameType.Сообщение_автоматизации;
			var messageValue = GetValue<object>(procedureStep.JournalArguments.MessageArgument);
			journalItem.DescriptionText = messageValue.GetType().IsEnum ? ((Enum)messageValue).ToDescription() : messageValue.ToString();
			Service.FiresecService.AddCommonJournalItem(journalItem);
		}

		bool Compare(ProcedureStep procedureStep)
		{
			var conditionArguments = procedureStep.ConditionArguments;
			var result = conditionArguments.JoinOperator == JoinOperator.And;
			foreach (var condition in conditionArguments.Conditions)
			{
				var variable1 = GetValue<object>(condition.Argument1);
				var variable2 = GetValue<object>(condition.Argument2);
				var comparer = Compare(variable1, variable2, condition.ConditionType);
				if ((comparer != null))
					result = conditionArguments.JoinOperator == JoinOperator.And ? result & comparer.Value : result | comparer.Value;
			}
			return result;
		}

		void ShowMessage(ProcedureStep procedureStep)
		{
			var showMessageArguments = procedureStep.ShowMessageArguments;
			var messageValue = GetValue<object>(showMessageArguments.MessageArgument);
			var message = messageValue.GetType().IsEnum ? ((Enum)messageValue).ToDescription() : messageValue.ToString();
			var automationCallbackResult = new AutomationCallbackResult()
			{
				AutomationCallbackType = AutomationCallbackType.Message,
				Data = new MessageCallbackData()
				{
					IsModalWindow = showMessageArguments.IsModalWindow,
					Message = message,
					WithConfirmation = showMessageArguments.WithConfirmation
				},
			};

			if (showMessageArguments.WithConfirmation)
			{
				var value = SendCallback(showMessageArguments, automationCallbackResult, true);
				SetValue(showMessageArguments.ConfirmationValueArgument, value);
			}
			else
				SendCallback(showMessageArguments, automationCallbackResult);
		}

		void ShowDialog(ProcedureStep procedureStep)
		{
			var automationCallbackResult = new AutomationCallbackResult()
			{
				AutomationCallbackType = AutomationCallbackType.Dialog,
				Data = new DialogCallbackData()
				{
					IsModalWindow = procedureStep.ShowDialogArguments.IsModalWindow,
					Layout = procedureStep.ShowDialogArguments.Layout,
					Title = procedureStep.ShowDialogArguments.Title,
					AllowClose = procedureStep.ShowDialogArguments.AllowClose,
					AllowMaximize = procedureStep.ShowDialogArguments.AllowMaximize,
					Height = procedureStep.ShowDialogArguments.Height,
					MinHeight = procedureStep.ShowDialogArguments.MinHeight,
					MinWidth = procedureStep.ShowDialogArguments.MinWidth,
					Sizable = procedureStep.ShowDialogArguments.Sizable,
					TopMost = procedureStep.ShowDialogArguments.TopMost,
					Width = procedureStep.ShowDialogArguments.Width,
				},
			};
			SendCallback(procedureStep.ShowDialogArguments, automationCallbackResult);
		}

		void ShowProperty(ProcedureStep procedureStep)
		{
			var showPropertyArguments = procedureStep.ShowPropertyArguments;
			var automationCallbackResult = new AutomationCallbackResult()
			{
				AutomationCallbackType = AutomationCallbackType.Property,
				Data = new PropertyCallBackData()
				{
					ObjectType = showPropertyArguments.ObjectType,
					ObjectUid = showPropertyArguments.ObjectArgument.ExplicitValue.UidValue
				},
			};
			SendCallback(showPropertyArguments, automationCallbackResult);
		}

		void SendEmail(ProcedureStep procedureStep)
		{
			var sendEmailArguments = procedureStep.SendEmailArguments;
			var smtp = GetValue<string>(sendEmailArguments.SmtpArgument);
			var port = GetValue<int>(sendEmailArguments.PortArgument);
			var login = GetValue<string>(sendEmailArguments.LoginArgument);
			var password = GetValue<string>(sendEmailArguments.PasswordArgument);
			var eMailAddressFrom = GetValue<string>(sendEmailArguments.EMailAddressFromArgument);
			var eMailAddressTo = GetValue<string>(sendEmailArguments.EMailAddressToArgument);
			var title = GetValue<string>(sendEmailArguments.EMailTitleArgument);
			var content = GetValue<string>(sendEmailArguments.EMailContentArgument);
			var Smtp = new SmtpClient(smtp, port);
			Smtp.Credentials = new NetworkCredential(login, password);
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

		void PlaySound(ProcedureStep procedureStep)
		{
			var automationCallbackResult = new AutomationCallbackResult()
			{
				AutomationCallbackType = AutomationCallbackType.Sound,
				Data = new SoundCallbackData()
				{
					SoundUID = procedureStep.SoundArguments.SoundUid,
				},
			};
			SendCallback(procedureStep.SoundArguments, automationCallbackResult);
		}

		void ControlVisual(ProcedureStep procedureStep, ControlElementType type)
		{
			if (procedureStep.ControlVisualArguments == null || !procedureStep.ControlVisualArguments.Property.HasValue)
				return;
			AutomationCallbackType callbackType;
			object value = null;
			var waitResponse = false;
			switch (type)
			{
				case ControlElementType.Get:
					callbackType = AutomationCallbackType.GetVisualProperty;
					waitResponse = true;
					break;
				case ControlElementType.Set:
					callbackType = AutomationCallbackType.SetVisualProperty;
					value = GetValue<object>(procedureStep.ControlVisualArguments.Argument);
					break;
				default:
					return;
			}
			var automationCallbackResult = new AutomationCallbackResult()
			{
				AutomationCallbackType = callbackType,
				Data = new VisualPropertyCallbackData()
				{
					LayoutPart = procedureStep.ControlVisualArguments.LayoutPart,
					Property = procedureStep.ControlVisualArguments.Property.Value,
					Value = value,
				},
			};
			if (waitResponse)
			{
				value = SendCallback(procedureStep.ControlVisualArguments, automationCallbackResult, true);
				SetValue(procedureStep.ControlVisualArguments.Argument, value);
			}
			else
			{
				SendCallback(procedureStep.ControlVisualArguments, automationCallbackResult);
				if (procedureStep.ControlVisualArguments.StoreOnServer && procedureStep.ControlVisualArguments.ForAllClients)
					ProcedurePropertyCache.SetProperty(procedureStep.ControlVisualArguments.Layout, (VisualPropertyCallbackData)automationCallbackResult.Data);
			}
		}

		void ControlPlan(ProcedureStep procedureStep, ControlElementType controlElementType)
		{
			var controlPlanArguments = procedureStep.ControlPlanArguments;
			var callbackType = new AutomationCallbackType();
			object value = null;

			if (controlElementType == ControlElementType.Get)
				callbackType = AutomationCallbackType.GetPlanProperty;
			if (controlElementType == ControlElementType.Set)
			{
				callbackType = AutomationCallbackType.SetPlanProperty;
				value = GetValue<object>(controlPlanArguments.ValueArgument);
			}

			var automationCallbackResult = new AutomationCallbackResult()
			{
				AutomationCallbackType = callbackType,
				Data = new PlanCallbackData()
				{
					PlanUid = controlPlanArguments.PlanUid,
					ElementUid = controlPlanArguments.ElementUid,
					ElementPropertyType = controlPlanArguments.ElementPropertyType,
					Value = value,
				},
			};
			switch (controlElementType)
			{
				case ControlElementType.Get:
					value = SendCallback(controlPlanArguments, automationCallbackResult, true);
					SetValue(controlPlanArguments.ValueArgument, value);
					break;
				case ControlElementType.Set:
					SendCallback(controlPlanArguments, automationCallbackResult);
					if (controlPlanArguments.ForAllClients)
						ProcedurePropertyCache.SetProperty((PlanCallbackData)automationCallbackResult.Data);
					break;
			}
		}

		void Calculate(ProcedureStep procedureStep)
		{
			var arithmeticArguments = procedureStep.ArithmeticArguments;
			object variable1;
			object variable2;
			var resultVariable = AllVariables.FirstOrDefault(x => x.Uid == arithmeticArguments.ResultArgument.VariableUid);
			switch (arithmeticArguments.ExplicitType)
			{
				case ExplicitType.Boolean:
					{
						variable1 = GetValue<bool>(arithmeticArguments.Argument1);
						variable2 = GetValue<bool>(arithmeticArguments.Argument2);
						bool result = false;
						if (arithmeticArguments.ArithmeticOperationType == ArithmeticOperationType.And)
							result = (bool)variable1 & (bool)variable2;
						if (arithmeticArguments.ArithmeticOperationType == ArithmeticOperationType.Or)
							result = (bool)variable1 || (bool)variable2;
						if (resultVariable != null)
							resultVariable.ExplicitValue.BoolValue = result;
						break;
					}

				case ExplicitType.Integer:
					{
						variable1 = GetValue<int>(arithmeticArguments.Argument1);
						variable2 = GetValue<int>(arithmeticArguments.Argument2);
						int result = 0;
						if (arithmeticArguments.ArithmeticOperationType == ArithmeticOperationType.Add)
							result = (int)variable1 + (int)variable2;
						if (arithmeticArguments.ArithmeticOperationType == ArithmeticOperationType.Sub)
							result = (int)variable1 - (int)variable2;
						if (arithmeticArguments.ArithmeticOperationType == ArithmeticOperationType.Multi)
							result = (int)variable1 * (int)variable2;
						if ((arithmeticArguments.ArithmeticOperationType == ArithmeticOperationType.Div) && ((int)variable2 != 0))
							result = (int)variable1 / (int)variable2;
						if (resultVariable != null)
							resultVariable.ExplicitValue.IntValue = result;
						break;
					}

				case ExplicitType.DateTime:
					{
						variable1 = GetValue<DateTime>(arithmeticArguments.Argument1);
						variable2 = new TimeSpan();
						switch (arithmeticArguments.TimeType)
						{
							case TimeType.Sec:
								variable2 = TimeSpan.FromSeconds(GetValue<int>(arithmeticArguments.Argument2));
								break;
							case TimeType.Min:
								variable2 = TimeSpan.FromMinutes(GetValue<int>(arithmeticArguments.Argument2));
								break;
							case TimeType.Hour:
								variable2 = TimeSpan.FromHours(GetValue<int>(arithmeticArguments.Argument2));
								break;
							case TimeType.Day:
								variable2 = TimeSpan.FromDays(GetValue<int>(arithmeticArguments.Argument2));
								break;
						}
						var result = new DateTime();
						if (arithmeticArguments.ArithmeticOperationType == ArithmeticOperationType.Add)
							result = (DateTime)variable1 + (TimeSpan)variable2;
						if (arithmeticArguments.ArithmeticOperationType == ArithmeticOperationType.Sub)
							result = (DateTime)variable1 - (TimeSpan)variable2;

						if (resultVariable != null)
							resultVariable.ExplicitValue.DateTimeValue = result;
						break;
					}
				case ExplicitType.String:
					{
						variable1 = GetValue<string>(arithmeticArguments.Argument1);
						variable2 = GetValue<string>(arithmeticArguments.Argument2);
						if (arithmeticArguments.ArithmeticOperationType == ArithmeticOperationType.Add)
							if (resultVariable != null)
								resultVariable.ExplicitValue.StringValue = String.Concat((string)variable1, (string)variable2);
						break;
					}
			}
		}

		void FindObjects(ProcedureStep procedureStep)
		{
			var findObjectArguments = procedureStep.FindObjectArguments;
			var variable = AllVariables.FirstOrDefault(x => x.Uid == findObjectArguments.ResultArgument.VariableUid);
			if (findObjectArguments.JoinOperator == JoinOperator.Or)
				FindObjectsOr(variable, findObjectArguments.FindObjectConditions);
			else
				FindObjectsAnd(variable, findObjectArguments.FindObjectConditions);
		}

		void GetObjectProperty(ProcedureStep procedureStep)
		{
			var getObjectPropertyArguments = procedureStep.GetObjectPropertyArguments;
			var target = AllVariables.FirstOrDefault(x => x.Uid == getObjectPropertyArguments.ResultArgument.VariableUid);
			var objectUid = GetValue<Guid>(getObjectPropertyArguments.ObjectArgument);
			var item = InitializeItem(objectUid);
			if (item == null)
				return;
			var guid = new Guid();
			var propertyValue = GetPropertyValue(ref guid, getObjectPropertyArguments.Property, item);
			SetValue(target, propertyValue);
		}

		object GetPropertyValue(ref Guid itemUid, Property property, object item)
		{
			var propertyValue = new object();
			if (item is GKDevice)
			{
				switch (property)
				{
					case Property.ShleifNo:
						propertyValue = (item as GKDevice).ShleifNo;
						break;
					case Property.IntAddress:
						propertyValue = (item as GKDevice).IntAddress;
						break;
					case Property.State:
						propertyValue = (int)(item as GKDevice).State.StateClass;
						break;
					case Property.Type:
						propertyValue = (item as GKDevice).Driver.DriverType;
						break;
					case Property.Description:
						propertyValue = (item as GKDevice).Description.Trim();
						break;
					case Property.Uid:
						propertyValue = (item as GKDevice).UID.ToString();
						break;
				}
				itemUid = (item as GKDevice).UID;
			}

			if (item is GKDelay)
			{
				switch (property)
				{
					case Property.Name:
						propertyValue = (item as GKDelay).Name.Trim();
						break;
					case Property.No:
						propertyValue = (item as GKDelay).No;
						break;
					case Property.Delay:
						propertyValue = (item as GKDelay).DelayTime;
						break;
					case Property.CurrentDelay:
						propertyValue = (item as GKDelay).State.OnDelay;
						break;
					case Property.State:
						propertyValue = (int)(item as GKDelay).State.StateClass;
						break;
					case Property.Hold:
						propertyValue = (item as GKDelay).Hold;
						break;
					case Property.CurrentHold:
						propertyValue = (item as GKDelay).State.HoldDelay;
						break;
					case Property.DelayRegime:
						propertyValue = (item as GKDelay).DelayRegime;
						break;
					case Property.Description:
						propertyValue = (item as GKDelay).Description.Trim();
						break;
					case Property.Uid:
						propertyValue = (item as GKDelay).UID.ToString();
						break;
				}
				itemUid = (item as GKDelay).UID;
			}

			if (item is GKZone)
			{
				switch (property)
				{
					case Property.No:
						propertyValue = (item as GKZone).No;
						break;
					case Property.Type:
						propertyValue = (item as GKZone).ObjectType;
						break;
					case Property.State:
						propertyValue = (int)(item as GKZone).State.StateClass;
						break;
					case Property.Name:
						propertyValue = (item as GKZone).Name.Trim();
						break;
					case Property.Uid:
						propertyValue = (item as GKZone).UID.ToString();
						break;
				}
				itemUid = (item as GKZone).UID;
			}

			if (item is GKDirection)
			{
				switch (property)
				{
					case Property.Name:
						propertyValue = (item as GKDirection).Name.Trim();
						break;
					case Property.No:
						propertyValue = (item as GKDirection).No;
						break;
					case Property.Delay:
						propertyValue = (int)(item as GKDirection).Delay;
						break;
					case Property.CurrentDelay:
						propertyValue = (int)(item as GKDirection).State.OnDelay;
						break;
					case Property.Hold:
						propertyValue = (int)(item as GKDirection).Hold;
						break;
					case Property.CurrentHold:
						propertyValue = (int)(item as GKDirection).State.HoldDelay;
						break;
					case Property.DelayRegime:
						propertyValue = (int)(item as GKDirection).DelayRegime;
						break;
					case Property.Description:
						propertyValue = (item as GKDirection).Description.Trim();
						break;
					case Property.Uid:
						propertyValue = (item as GKDirection).UID.ToString();
						break;
					case Property.State:
						propertyValue = (int)(item as GKDirection).State.StateClass;
						break;
				}
				itemUid = (item as GKDirection).UID;
			}

			return propertyValue;
		}

		void InitializeItems(ref IEnumerable<object> items, ref Variable result)
		{
			var explicitValues = new List<ExplicitValue>();
			if (result.ObjectType == ObjectType.Device)
			{
				items = new List<GKDevice>(GKManager.DeviceConfiguration.Devices);
				foreach (var objectUid in new List<Guid>(GKManager.DeviceConfiguration.Devices.Select(x => x.UID)))
					explicitValues.Add(new ExplicitValue { UidValue = objectUid });
			}
			if (result.ObjectType == ObjectType.Zone)
			{
				items = new List<GKZone>(GKManager.Zones);
				foreach (var objectUid in new List<Guid>(GKManager.Zones.Select(x => x.UID)))
					explicitValues.Add(new ExplicitValue { UidValue = objectUid });
			}
			if (result.ObjectType == ObjectType.Direction)
			{
				items = new List<GKDirection>(GKManager.Directions);
				foreach (var objectUid in new List<Guid>(GKManager.Directions.Select(x => x.UID)))
					explicitValues.Add(new ExplicitValue { UidValue = objectUid });
			}
			result.ExplicitValues = explicitValues;
		}

		object InitializeItem(Guid itemUid)
		{
			var device = GKManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == itemUid);
			var zone = GKManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == itemUid);
			var guardZone = GKManager.DeviceConfiguration.GuardZones.FirstOrDefault(x => x.UID == itemUid);
			var sKDDevice = SKDManager.Devices.FirstOrDefault(x => x.UID == itemUid);
			var sKDZone = SKDManager.Zones.FirstOrDefault(x => x.UID == itemUid);
			var camera = ConfigurationCashHelper.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == itemUid);
			var sKDDoor = SKDManager.Doors.FirstOrDefault(x => x.UID == itemUid);
			var direction = GKManager.DeviceConfiguration.Directions.FirstOrDefault(x => x.UID == itemUid);
			var delay = GKManager.DeviceConfiguration.Delays.FirstOrDefault(x => x.UID == itemUid);
			if (device != null)
				return device;
			if (zone != null)
				return zone;
			if (guardZone != null)
				return guardZone;
			if (sKDDevice != null)
				return sKDDevice;
			if (sKDZone != null)
				return sKDZone;
			if (camera != null)
				return camera;
			if (sKDDoor != null)
				return sKDDoor;
			if (direction != null)
				return direction;
			if (delay != null)
				return delay;
			return null;
		}

		void FindObjectsOr(Variable result, IEnumerable<FindObjectCondition> findObjectConditions)
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
					var conditionValue = GetValue<object>(findObjectCondition.SourceArgument);
					var comparer = Compare(propertyValue, conditionValue, findObjectCondition.ConditionType);
					if ((comparer != null) && (comparer.Value))
					{
						resultObjects.Add(item);
						result.ExplicitValues.Add(new ExplicitValue { UidValue = itemUid });
					}
				}
			}
		}

		void FindObjectsAnd(Variable result, IEnumerable<FindObjectCondition> findObjectConditions)
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
					var conditionValue = GetValue<object>(findObjectCondition.SourceArgument);
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

		bool? Compare(object param1, object param2, ConditionType conditionType)
		{
			if (param1.GetType() != param2.GetType())
				return null;
			if (param1.GetType().IsEnum || param1 is int)
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
				return (((conditionType == ConditionType.IsEqual) && ((bool)param1 == (bool)param2))
						|| ((conditionType == ConditionType.IsNotEqual) && ((bool)param1 != (bool)param2)));
			}
			return null;
		}

		void ControlGKDevice(ProcedureStep procedureStep)
		{
			var deviceUid = GetValue<Guid>(procedureStep.ControlGKDeviceArguments.GKDeviceArgument);
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUid);
			if (device == null)
				return;
			FiresecServiceManager.SafeFiresecService.GKExecuteDeviceCommand(device.UID, procedureStep.ControlGKDeviceArguments.Command);
		}

		public void SetJournalItemGuid(ProcedureStep procedureStep)
		{
			var setJournalItemGuidArguments = procedureStep.SetJournalItemGuidArguments;
			if (JournalItem != null)
			{
				using (var journalTranslator = new JounalTranslator())
				{
					var eventUIDString = GetValue<String>(setJournalItemGuidArguments.ValueArgument);
					Guid eventUID;
					if (CheckGuid(eventUIDString))
					{
						eventUID = new Guid(eventUIDString);
					}
					else
					{
						return;
					}
					journalTranslator.SaveVideoUID(JournalItem.UID, eventUID, Guid.Empty);
				}
			}
		}

		void StartRecord(ProcedureStep procedureStep)
		{
			var startRecordArguments = procedureStep.StartRecordArguments;
			var cameraUid = GetValue<Guid>(startRecordArguments.CameraArgument);
			var camera = ConfigurationCashHelper.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == cameraUid);
			if (camera == null)
				return;
			var eventUIDString = GetValue<String>(startRecordArguments.EventUIDArgument);
			Guid eventUID;
			if (CheckGuid(eventUIDString))
			{
				eventUID = new Guid(eventUIDString);
			}
			else
			{
				return;
			}
			using (var journalTranslator = new JounalTranslator())
			{
				journalTranslator.SaveVideoUID(JournalItem.UID, eventUID, cameraUid);
			}
			var timeout = GetValue<int>(startRecordArguments.TimeoutArgument);
			RviClient.RviClientHelper.VideoRecordStart(ConfigurationCashHelper.SystemConfiguration, camera, eventUID, timeout);
		}

		private void StopRecord(ProcedureStep procedureStep)
		{
			var stopRecordArguments = procedureStep.StopRecordArguments;
			var cameraUid = GetValue<Guid>(stopRecordArguments.CameraArgument);
			var camera = ConfigurationCashHelper.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == cameraUid);
			if (camera == null)
				return;
			var eventUID = GetValue<Guid>(stopRecordArguments.EventUIDArgument);
			RviClient.RviClientHelper.VideoRecordStop(ConfigurationCashHelper.SystemConfiguration, camera, eventUID);
		}

		public void Ptz(ProcedureStep procedureStep)
		{
			var ptzArguments = procedureStep.PtzArguments;
			var cameraUid = GetValue<Guid>(ptzArguments.CameraArgument);
			var camera = ConfigurationCashHelper.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == cameraUid);
			if (camera == null)
				return;
			var ptzNumber = GetValue<int>(ptzArguments.PtzNumberArgument);
			RviClient.RviClientHelper.SetPtzPreset(ConfigurationCashHelper.SystemConfiguration, camera, ptzNumber - 1);
		}

		public void RviAlarm(ProcedureStep procedureStep)
		{
			var rviAlarmArguments = procedureStep.RviAlarmArguments;
			var name = GetValue<string>(rviAlarmArguments.NameArgument);
			RviClient.RviClientHelper.AlarmRuleExecute(ConfigurationCashHelper.SystemConfiguration, name);
		}

		void ControlFireZone(ProcedureStep procedureStep)
		{
			var zoneUid = GetValue<Guid>(procedureStep.ControlGKFireZoneArguments.GKFireZoneArgument);
			var zoneCommandType = procedureStep.ControlGKFireZoneArguments.ZoneCommandType;
			if (zoneCommandType == ZoneCommandType.Ignore)
				FiresecServiceManager.SafeFiresecService.GKSetIgnoreRegime(zoneUid, GKBaseObjectType.Zone);
			if (zoneCommandType == ZoneCommandType.ResetIgnore)
				FiresecServiceManager.SafeFiresecService.GKSetAutomaticRegime(zoneUid, GKBaseObjectType.Zone);
			if (zoneCommandType == ZoneCommandType.Reset)
				FiresecServiceManager.SafeFiresecService.GKReset(zoneUid, GKBaseObjectType.Zone);
		}

		void ControlGuardZone(ProcedureStep procedureStep)
		{
			var zoneUid = GetValue<Guid>(procedureStep.ControlGKGuardZoneArguments.GKGuardZoneArgument);
			var guardZoneCommandType = procedureStep.ControlGKGuardZoneArguments.GuardZoneCommandType;
			if (guardZoneCommandType == GuardZoneCommandType.Automatic)
				FiresecServiceManager.SafeFiresecService.GKSetAutomaticRegime(zoneUid, GKBaseObjectType.GuardZone);
			if (guardZoneCommandType == GuardZoneCommandType.Ignore)
				FiresecServiceManager.SafeFiresecService.GKSetIgnoreRegime(zoneUid, GKBaseObjectType.GuardZone);
			if (guardZoneCommandType == GuardZoneCommandType.Manual)
				FiresecServiceManager.SafeFiresecService.GKSetManualRegime(zoneUid, GKBaseObjectType.GuardZone);
			if (guardZoneCommandType == GuardZoneCommandType.Reset)
				FiresecServiceManager.SafeFiresecService.GKReset(zoneUid, GKBaseObjectType.GuardZone);
			if (guardZoneCommandType == GuardZoneCommandType.TurnOff)
				FiresecServiceManager.SafeFiresecService.GKTurnOff(zoneUid, GKBaseObjectType.GuardZone);
			if (guardZoneCommandType == GuardZoneCommandType.TurnOn)
				FiresecServiceManager.SafeFiresecService.GKTurnOn(zoneUid, GKBaseObjectType.GuardZone);
			if (guardZoneCommandType == GuardZoneCommandType.TurnOnNow)
				FiresecServiceManager.SafeFiresecService.GKTurnOnNow(zoneUid, GKBaseObjectType.GuardZone);
		}

		void ControlDirection(ProcedureStep procedureStep)
		{
			var directionUid = GetValue<Guid>(procedureStep.ControlDirectionArguments.DirectionArgument);
			var directionCommandType = procedureStep.ControlDirectionArguments.DirectionCommandType;
			if (directionCommandType == DirectionCommandType.Automatic)
				FiresecServiceManager.SafeFiresecService.GKSetAutomaticRegime(directionUid, GKBaseObjectType.Direction);
			if (directionCommandType == DirectionCommandType.ForbidStart)
				FiresecServiceManager.SafeFiresecService.GKStop(directionUid, GKBaseObjectType.Direction);
			if (directionCommandType == DirectionCommandType.Ignore)
				FiresecServiceManager.SafeFiresecService.GKSetIgnoreRegime(directionUid, GKBaseObjectType.Direction);
			if (directionCommandType == DirectionCommandType.Manual)
				FiresecServiceManager.SafeFiresecService.GKSetManualRegime(directionUid, GKBaseObjectType.Direction);
			if (directionCommandType == DirectionCommandType.TurnOff)
				FiresecServiceManager.SafeFiresecService.GKTurnOff(directionUid, GKBaseObjectType.Direction);
			if (directionCommandType == DirectionCommandType.TurnOn)
				FiresecServiceManager.SafeFiresecService.GKTurnOn(directionUid, GKBaseObjectType.Direction);
			if (directionCommandType == DirectionCommandType.TurnOnNow)
				FiresecServiceManager.SafeFiresecService.GKTurnOnNow(directionUid, GKBaseObjectType.Direction);
		}

		void ControlDoor(ProcedureStep procedureStep)
		{
			var door = SKDManager.Doors.FirstOrDefault(x => x.UID == procedureStep.ControlDoorArguments.DoorArgument.ExplicitValue.UidValue);
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

		void ControlSKDZone(ProcedureStep procedureStep)
		{
			var sKDZone = SKDManager.Zones.FirstOrDefault(x => x.UID == procedureStep.ControlSKDZoneArguments.SKDZoneArgument.ExplicitValue.UidValue);
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

		void ControlDelay(ProcedureStep procedureStep)
		{
			var delayUid = GetValue<Guid>(procedureStep.ControlDelayArguments.DelayArgument);
			var delayCommandType = procedureStep.ControlDelayArguments.DelayCommandType;
			if (delayCommandType == DelayCommandType.Automatic)
				FiresecServiceManager.SafeFiresecService.GKSetAutomaticRegime(delayUid, GKBaseObjectType.Delay);
			if (delayCommandType == DelayCommandType.Ignore)
				FiresecServiceManager.SafeFiresecService.GKSetIgnoreRegime(delayUid, GKBaseObjectType.Delay);
			if (delayCommandType == DelayCommandType.Manual)
				FiresecServiceManager.SafeFiresecService.GKSetManualRegime(delayUid, GKBaseObjectType.Delay);
			if (delayCommandType == DelayCommandType.TurnOff)
				FiresecServiceManager.SafeFiresecService.GKTurnOff(delayUid, GKBaseObjectType.Delay);
			if (delayCommandType == DelayCommandType.TurnOn)
				FiresecServiceManager.SafeFiresecService.GKTurnOn(delayUid, GKBaseObjectType.Delay);
			if (delayCommandType == DelayCommandType.TurnOnNow)
				FiresecServiceManager.SafeFiresecService.GKTurnOnNow(delayUid, GKBaseObjectType.Delay);
		}

		void Pause(ProcedureStep procedureStep)
		{
			var pauseArguments = procedureStep.PauseArguments;
			var pause = new TimeSpan();
			switch (pauseArguments.TimeType)
			{
				case TimeType.Sec:
					pause = TimeSpan.FromSeconds(GetValue<int>(pauseArguments.PauseArgument));
					break;
				case TimeType.Min:
					pause = TimeSpan.FromMinutes(GetValue<int>(pauseArguments.PauseArgument));
					break;
				case TimeType.Hour:
					pause = TimeSpan.FromHours(GetValue<int>(pauseArguments.PauseArgument));
					break;
				case TimeType.Day:
					pause = TimeSpan.FromDays(GetValue<int>(pauseArguments.PauseArgument));
					break;
			}
			if (AutoResetEvent.WaitOne(TimeSpan.FromSeconds(pause.TotalSeconds)))
			{
			}
		}

		void IncrementValue(ProcedureStep procedureStep)
		{
			var incrementValueArguments = procedureStep.IncrementValueArguments;
			var variable = AllVariables.FirstOrDefault(x => x.Uid == incrementValueArguments.ResultArgument.VariableUid);
			if (variable != null)
				variable.ExplicitValue.IntValue = incrementValueArguments.IncrementType == IncrementType.Inc ? variable.ExplicitValue.IntValue + 1 : variable.ExplicitValue.IntValue - 1;
		}

		void GetRandomValue(ProcedureStep procedureStep)
		{
			var randomArguments = procedureStep.RandomArguments;
			var resultVariable = AllVariables.FirstOrDefault(x => x.Uid == randomArguments.ResultArgument.VariableUid);
			var maxValue = GetValue<int>(randomArguments.MaxValueArgument);
			if (resultVariable != null)
				resultVariable.ExplicitValue.IntValue = new Random().Next(0, maxValue);
		}

		void ChangeList(ProcedureStep procedureStep)
		{
			var changeListArguments = procedureStep.ChangeListArguments;
			var listVariable = AllVariables.FirstOrDefault(x => x.Uid == changeListArguments.ListArgument.VariableUid);
			var variable = new Variable();
			variable.ExplicitType = changeListArguments.ItemArgument.ExplicitType;
			variable.EnumType = changeListArguments.ItemArgument.EnumType;
			variable.ObjectType = changeListArguments.ItemArgument.ObjectType;
			var itemValue = GetValue<object>(changeListArguments.ItemArgument);
			SetValue(variable, itemValue);
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

		void CheckPermission(ProcedureStep procedureStep)
		{
			var checkPermissionArguments = procedureStep.CheckPermissionArguments;
			var resultVariable = AllVariables.FirstOrDefault(x => x.Uid == checkPermissionArguments.ResultArgument.VariableUid);
			var permissionValue = GetValue<PermissionType>(checkPermissionArguments.PermissionArgument);
			if (resultVariable != null && User != null)
				resultVariable.ExplicitValue.BoolValue = User.HasPermission(permissionValue);
		}

		void GetListCount(ProcedureStep procedureStep)
		{
			var getListCountArgument = procedureStep.GetListCountArguments;
			var listVariable = AllVariables.FirstOrDefault(x => x.Uid == getListCountArgument.ListArgument.VariableUid);
			var countVariable = AllVariables.FirstOrDefault(x => x.Uid == getListCountArgument.CountArgument.VariableUid);
			if ((countVariable != null) && (listVariable != null))
				countVariable.ExplicitValue.IntValue = listVariable.ExplicitValues.Count;
		}

		void GetListItem(ProcedureStep procedureStep)
		{
			var getListItemArgument = procedureStep.GetListItemArguments;
			var listVariable = AllVariables.FirstOrDefault(x => x.Uid == getListItemArgument.ListArgument.VariableUid);
			var itemVariable = AllVariables.FirstOrDefault(x => x.Uid == getListItemArgument.ItemArgument.VariableUid);
			if ((itemVariable != null) && (listVariable != null))
			{
				if (getListItemArgument.PositionType == PositionType.First)
					SetValue(itemVariable, GetValue<object>(listVariable.ExplicitValues.FirstOrDefault(), itemVariable.ExplicitType, itemVariable.EnumType));
				if (getListItemArgument.PositionType == PositionType.Last)
					SetValue(itemVariable, GetValue<object>(listVariable.ExplicitValues.LastOrDefault(), itemVariable.ExplicitType, itemVariable.EnumType));
				if (getListItemArgument.PositionType == PositionType.ByIndex)
				{
					var indexValue = GetValue<int>(getListItemArgument.IndexArgument);
					if (listVariable.ExplicitValues.Count > indexValue)
						SetValue(itemVariable, GetValue<object>(listVariable.ExplicitValues[indexValue], itemVariable.ExplicitType, itemVariable.EnumType));
				}
			}
		}

		void GetJournalItem(ProcedureStep procedureStep)
		{
			var getJournalItemArguments = procedureStep.GetJournalItemArguments;
			var resultVariable = AllVariables.FirstOrDefault(x => x.Uid == getJournalItemArguments.ResultArgument.VariableUid);
			var value = new object();
			if (JournalItem != null)
			{
				if (getJournalItemArguments.JournalColumnType == JournalColumnType.DeviceDateTime)
					value = JournalItem.DeviceDateTime;
				if (getJournalItemArguments.JournalColumnType == JournalColumnType.SystemDateTime)
					value = JournalItem.SystemDateTime;
				if (getJournalItemArguments.JournalColumnType == JournalColumnType.JournalEventNameType)
					value = JournalItem.JournalEventNameType;
				if (getJournalItemArguments.JournalColumnType == JournalColumnType.JournalEventDescriptionType)
					value = JournalItem.JournalEventDescriptionType;
				if (getJournalItemArguments.JournalColumnType == JournalColumnType.JournalObjectType)
					value = JournalItem.JournalObjectType;
				if (getJournalItemArguments.JournalColumnType == JournalColumnType.JournalObjectUid)
					value = JournalItem.ObjectUID.ToString();
				SetValue(resultVariable, value);
			}
		}

		bool ExplicitCompare(ExplicitValue explicitValue1, ExplicitValue explicitValue2, ExplicitType explicitType, EnumType enumType)
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
			if (explicitType == ExplicitType.Object)
			{
				return explicitValue1.UidValue == explicitValue2.UidValue;
			}
			return false;
		}

		void SetValue(ProcedureStep procedureStep)
		{
			var setValueArguments = procedureStep.SetValueArguments;
			var sourceVariable = AllVariables.FirstOrDefault(x => x.Uid == setValueArguments.SourceArgument.VariableUid);
			var targetVariable = AllVariables.FirstOrDefault(x => x.Uid == setValueArguments.TargetArgument.VariableUid);
			if (targetVariable == null)
				return;
			if (setValueArguments.ExplicitType == ExplicitType.String)
			{
				var value = GetValue<object>(setValueArguments.SourceArgument);
				targetVariable.ExplicitValue.StringValue = value.GetType().IsEnum ? ((Enum)value).ToDescription() : value.ToString();
			}
			else
				PropertyCopy.Copy(
					sourceVariable != null ? sourceVariable.ExplicitValue : setValueArguments.SourceArgument.ExplicitValue,
					targetVariable.ExplicitValue);
		}

		void ExportJournal(ProcedureStep procedureStep)
		{
			var arguments = procedureStep.ExportJournalArguments;
			var isExportJournal = GetValue<bool>(arguments.IsExportJournalArgument);
			var isExportPassJournal = GetValue<bool>(arguments.IsExportPassJournalArgument);
			var minDate = GetValue<DateTime>(arguments.MinDateArgument);
			var maxDate = GetValue<DateTime>(arguments.MaxDateArgument);
			var path = GetValue<string>(arguments.PathArgument);
			var result = FiresecServiceManager.SafeFiresecService.ExportJournal(
				new JournalExportFilter
				{
					IsExportJournal = isExportJournal,
					IsExportPassJournal = isExportPassJournal,
					MaxDate = maxDate,
					MinDate = minDate,
					Path = path
				});
			//if (result.HasError)
			//    BalloonHelper.ShowFromServer("Экспорт журнала " + result.Error);
		}

		void ExportOrganisation(ProcedureStep procedureStep)
		{
			var arguments = procedureStep.ExportOrganisationArguments;
			var isWithDeleted = GetValue<bool>(arguments.IsWithDeleted);
			var organisationUID = GetValue<Guid>(arguments.Organisation);
			var path = GetValue<string>(arguments.PathArgument);
			var result = FiresecServiceManager.SafeFiresecService.ExportOrganisation(
				new ExportFilter
				{
					IsWithDeleted = isWithDeleted,
					OrganisationUID = organisationUID,
					Path = path
				});
			//if (result.HasError)
			//    MessageBoxService.Show(result.Error);
		}

		void ExportOrganisationList(ProcedureStep procedureStep)
		{
			var arguments = procedureStep.ImportOrganisationArguments;
			var isWithDeleted = GetValue<bool>(arguments.IsWithDeleted);
			var path = GetValue<string>(arguments.PathArgument);
			var result = FiresecServiceManager.SafeFiresecService.ExportOrganisationList(
				new ExportFilter
				{
					IsWithDeleted = isWithDeleted,
					Path = path
				});
			//if (result.HasError)
			//    MessageBoxService.Show(result.Error);
		}

		void ExportConfiguration(ProcedureStep procedureStep)
		{
			var arguments = procedureStep.ExportConfigurationArguments;
			var isExportDevices = GetValue<bool>(arguments.IsExportDevices);
			var isExportDoors = GetValue<bool>(arguments.IsExportDoors);
			var isExportZones = GetValue<bool>(arguments.IsExportZones);
			var path = GetValue<string>(arguments.PathArgument);
			var result = FiresecServiceManager.SafeFiresecService.ExportConfiguration(
				new ConfigurationExportFilter
				{
					IsExportDevices = isExportDevices,
					IsExportDoors = isExportDoors,
					IsExportZones = isExportZones,
					Path = path
				});
			//if (result.HasError)
			//    MessageBoxService.Show(result.Error);
		}

		void ImportOrganisation(ProcedureStep procedureStep)
		{
			var arguments = procedureStep.ImportOrganisationArguments;
			var isWithDeleted = GetValue<bool>(arguments.IsWithDeleted);
			var path = GetValue<string>(arguments.PathArgument);
			var result = FiresecServiceManager.SafeFiresecService.ImportOrganisation(
				new ImportFilter
				{
					IsWithDeleted = isWithDeleted,
					Path = path
				});
			//if (result.HasError)
			//    MessageBoxService.Show(result.Error);
		}

		void ImportOrganisationList(ProcedureStep procedureStep)
		{
			var arguments = procedureStep.ImportOrganisationArguments;
			var isWithDeleted = GetValue<bool>(arguments.IsWithDeleted);
			var path = GetValue<string>(arguments.PathArgument);
			var result = FiresecServiceManager.SafeFiresecService.ImportOrganisationList(
				new ImportFilter
				{
					IsWithDeleted = isWithDeleted,
					Path = path
				});
			//if (result.HasError)
			//    MessageBoxService.Show(result.Error);
		}

		void SetValue(Argument argument, object propertyValue)
		{
			var variable = AllVariables.FirstOrDefault(x => x.Uid == argument.VariableUid);
			if (variable != null)
				SetValue(variable, propertyValue);
		}
		void SetValue(Variable target, object propertyValue)
		{
			if (target.ExplicitType == ExplicitType.Integer)
				target.ExplicitValue.IntValue = Convert.ToInt32(propertyValue);
			if (target.ExplicitType == ExplicitType.String)
				target.ExplicitValue.StringValue = Convert.ToString(propertyValue);
			if (target.ExplicitType == ExplicitType.Boolean)
				target.ExplicitValue.BoolValue = Convert.ToBoolean(propertyValue);
			if (target.ExplicitType == ExplicitType.DateTime)
				target.ExplicitValue.DateTimeValue = Convert.ToDateTime(propertyValue);
			if (target.ExplicitType == ExplicitType.Enum)
			{
				if (target.EnumType == EnumType.DriverType)
					target.ExplicitValue.DriverTypeValue = (GKDriverType)propertyValue;
				if (target.EnumType == EnumType.StateType)
					target.ExplicitValue.StateTypeValue = (XStateClass)propertyValue;
				if (target.EnumType == EnumType.PermissionType)
					target.ExplicitValue.PermissionTypeValue = (PermissionType)propertyValue;
				if (target.EnumType == EnumType.JournalEventNameType)
					target.ExplicitValue.JournalEventNameTypeValue = (JournalEventNameType)propertyValue;
				if (target.EnumType == EnumType.JournalEventDescriptionType)
					target.ExplicitValue.JournalEventDescriptionTypeValue = (JournalEventDescriptionType)propertyValue;
				if (target.EnumType == EnumType.JournalObjectType)
					target.ExplicitValue.JournalObjectTypeValue = (JournalObjectType)propertyValue;
				if (target.EnumType == EnumType.ColorType)
					target.ExplicitValue.ColorValue = (Color)propertyValue;
			}
		}

		T GetValue<T>(Argument variable)
		{
			var variableScope = variable.VariableScope;
			var explicitType = variable.ExplicitType;
			var enumType = variable.EnumType;
			var explicitValue = variable.ExplicitValue;
			if (variableScope != VariableScope.ExplicitValue)
			{
				var argument = AllVariables.FirstOrDefault(x => x.Uid == variable.VariableUid);
				if (argument != null)
				{
					explicitValue = argument.ExplicitValue;
					explicitType = argument.ExplicitType;
					enumType = argument.EnumType;
				}
			}
			return (T)GetValue<object>(explicitValue, explicitType, enumType);
		}

		bool CheckGuid(string guidString)
		{
			var guidRegEx = new Regex("^[A-Fa-f0-9]{32}$|" + "^({|\\()?[A-Fa-f0-9]{8}-([A-Fa-f0-9]{4}-){3}[A-Fa-f0-9]{12}(}|\\))?$|" +
				"^({)?[0xA-Fa-f0-9]{3,10}(, {0,1}[0xA-Fa-f0-9]{3,6}){2}, {0,1}({)([0xA-Fa-f0-9]{3,4}, {0,1}){7}[0xA-Fa-f0-9]{3,4}(}})$", RegexOptions.Compiled);
			if (!String.IsNullOrEmpty(guidString) && guidRegEx.IsMatch(guidString))
				return true;
			return false;
		}

		T GetValue<T>(ExplicitValue explicitValue, ExplicitType explicitType, EnumType enumType)
		{
			var result = new object();
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
				if (enumType == EnumType.PermissionType)
					result = explicitValue.PermissionTypeValue;
				if (enumType == EnumType.JournalEventNameType)
					result = explicitValue.JournalEventNameTypeValue;
				if (enumType == EnumType.JournalEventDescriptionType)
					result = explicitValue.JournalEventDescriptionTypeValue;
				if (enumType == EnumType.JournalObjectType)
					result = explicitValue.JournalObjectTypeValue;
				if (enumType == EnumType.ColorType)
					result = explicitValue.ColorValue.ToString();
			}
			return (T)result;
		}
	}
}
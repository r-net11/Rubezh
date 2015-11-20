using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using RubezhAPI;
using RubezhAPI.Automation;
using RubezhAPI.AutomationCallback;
using RubezhAPI.GK;
using RubezhAPI.Journal;
using RubezhAPI.Models;
using RubezhClient;
using Property = RubezhAPI.Automation.Property;
using RubezhAPI.SKD;
using RubezhClient.SKDHelpers;
using System.Diagnostics;

namespace Infrastructure.Automation
{
	public partial class ProcedureThread
	{
		void AddJournalItem(ProcedureStep procedureStep)
		{
			var messageValue = GetValue<object>(procedureStep.JournalArguments.MessageArgument);
			ProcedureExecutionContext.AddJournalItem(AutomationHelper.GetStringValue(messageValue));
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
			var message = GetValue<object>(showMessageArguments.MessageArgument);
			var messageString = AutomationHelper.GetStringValue(message);
			var automationCallbackResult = new AutomationCallbackResult()
			{
				AutomationCallbackType = AutomationCallbackType.Message,
				Data = new MessageCallbackData()
				{
					IsModalWindow = showMessageArguments.IsModalWindow,
					Message = messageString,
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
					CustomPosition = procedureStep.ShowDialogArguments.CustomPosition,
					Left = procedureStep.ShowDialogArguments.Left,
					Top = procedureStep.ShowDialogArguments.Top
				},
			};
			SendCallback(procedureStep.ShowDialogArguments, automationCallbackResult);
		}

		void ShowProperty(ProcedureStep procedureStep)
		{
			var showPropertyArguments = procedureStep.ShowPropertyArguments;
			var objectUid = GetValue<Guid>(showPropertyArguments.ObjectArgument);
			var automationCallbackResult = new AutomationCallbackResult()
			{
				AutomationCallbackType = AutomationCallbackType.Property,
				Data = new PropertyCallBackData()
				{
					ObjectType = showPropertyArguments.ObjectType,
					ObjectUid = objectUid
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
				if (procedureStep.ControlVisualArguments.ForAllClients)
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
				if (value is int && (int)value < 0)
					return;
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
			object value1;
			object value2;
			var resultVariable = AllVariables.FirstOrDefault(x => x.Uid == arithmeticArguments.ResultArgument.VariableUid);
			switch (arithmeticArguments.ExplicitType)
			{
				case ExplicitType.Boolean:
					{
						value1 = GetValue<bool>(arithmeticArguments.Argument1);
						value2 = GetValue<bool>(arithmeticArguments.Argument2);
						bool result = false;
						if (arithmeticArguments.ArithmeticOperationType == ArithmeticOperationType.And)
							result = (bool)value1 & (bool)value2;
						if (arithmeticArguments.ArithmeticOperationType == ArithmeticOperationType.Or)
							result = (bool)value1 || (bool)value2;
						if (resultVariable != null)
							resultVariable.ExplicitValue.BoolValue = result;
						break;
					}

				case ExplicitType.Integer:
					{
						value1 = GetValue<int>(arithmeticArguments.Argument1);
						value2 = GetValue<int>(arithmeticArguments.Argument2);
						int result = 0;
						if (arithmeticArguments.ArithmeticOperationType == ArithmeticOperationType.Add)
							result = (int)value1 + (int)value2;
						if (arithmeticArguments.ArithmeticOperationType == ArithmeticOperationType.Sub)
							result = (int)value1 - (int)value2;
						if (arithmeticArguments.ArithmeticOperationType == ArithmeticOperationType.Multi)
							result = (int)value1 * (int)value2;
						if ((arithmeticArguments.ArithmeticOperationType == ArithmeticOperationType.Div) && ((int)value2 != 0))
							result = (int)value1 / (int)value2;
						if (resultVariable != null)
							resultVariable.ExplicitValue.IntValue = result;
						break;
					}

				case ExplicitType.DateTime:
					{
						value1 = GetValue<DateTime>(arithmeticArguments.Argument1);
						value2 = new TimeSpan();
						switch (arithmeticArguments.TimeType)
						{
							case TimeType.Sec:
								value2 = TimeSpan.FromSeconds(GetValue<int>(arithmeticArguments.Argument2));
								break;
							case TimeType.Min:
								value2 = TimeSpan.FromMinutes(GetValue<int>(arithmeticArguments.Argument2));
								break;
							case TimeType.Hour:
								value2 = TimeSpan.FromHours(GetValue<int>(arithmeticArguments.Argument2));
								break;
							case TimeType.Day:
								value2 = TimeSpan.FromDays(GetValue<int>(arithmeticArguments.Argument2));
								break;
						}
						var result = new DateTime();
						if (arithmeticArguments.ArithmeticOperationType == ArithmeticOperationType.Add)
							result = (DateTime)value1 + (TimeSpan)value2;
						if (arithmeticArguments.ArithmeticOperationType == ArithmeticOperationType.Sub)
							result = (DateTime)value1 - (TimeSpan)value2;

						if (resultVariable != null)
							resultVariable.ExplicitValue.DateTimeValue = result;
						break;
					}
				case ExplicitType.String:
					{
						value1 = GetValue<object>(arithmeticArguments.Argument1);
						value2 = GetValue<object>(arithmeticArguments.Argument2);
						if (arithmeticArguments.ArithmeticOperationType == ArithmeticOperationType.Add)
							if (resultVariable != null)
								resultVariable.ExplicitValue.StringValue = String.Concat(AutomationHelper.GetStringValue(value1), AutomationHelper.GetStringValue(value2));
						break;
					}
			}
			ProcedureExecutionContext.SynchronizeVariable(resultVariable, ContextType.Server);
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
			var propertyValue = GetPropertyValue(getObjectPropertyArguments.Property, item);
			ProcedureExecutionContext.SetVariableValue(target, propertyValue);
		}

		Guid GetObjectUid(object item)
		{
			if (item is GKDevice)
				return (item as GKDevice).UID;

			if (item is GKDelay)
				return (item as GKDelay).UID;
			
			if (item is GKZone)
				return (item as GKZone).UID;

			if (item is GKDirection)
				return (item as GKDirection).UID;

			if (item is GKDoor)
				return (item as GKDoor).UID;

			if (item is GKGuardZone)
				return (item as GKGuardZone).UID;

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
						propertyValue = gkDelay.Description == null ? "" : gkDelay.Description.Trim();;
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
				case ObjectType.Organisation:
					return new List<Organisation>(ProcedureExecutionContext.GetOrganisations());
			}
			return new List<object>();
		}
		
		object InitializeItem(Guid itemUid)
		{
			var device = GKManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == itemUid);
			if (device != null)
				return device;

			var zone = GKManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == itemUid);
			if (zone != null)
				return zone;

			var guardZone = GKManager.DeviceConfiguration.GuardZones.FirstOrDefault(x => x.UID == itemUid);
			if (guardZone != null)
				return guardZone;

			var camera = ProcedureExecutionContext.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == itemUid);
			if (camera != null)
				return camera;

			var direction = GKManager.DeviceConfiguration.Directions.FirstOrDefault(x => x.UID == itemUid);
			if (direction != null)
				return direction;

			var delay = GKManager.DeviceConfiguration.Delays.FirstOrDefault(x => x.UID == itemUid);
			if (delay != null)
				return delay;
			
			var door = GKManager.Doors.FirstOrDefault(x => x.UID == itemUid);
			if (door != null)
				return door;

			var organisations = ProcedureExecutionContext.GetOrganisations();
			var organisation = organisations == null ? null : organisations.FirstOrDefault(x => x.UID == itemUid);
			if (organisation != null)
				return organisation;

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
						result.ExplicitValues.Add(new ExplicitValue { UidValue = itemUid });
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
					result.ExplicitValues.Add(new ExplicitValue { UidValue = itemUid });
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

		void ControlGKDevice(ProcedureStep procedureStep)
		{
			var deviceUid = GetValue<Guid>(procedureStep.ControlGKDeviceArguments.GKDeviceArgument);
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUid);
			if (device == null)
				return;
			ProcedureExecutionContext.ControlGKDevice(device.UID, procedureStep.ControlGKDeviceArguments.Command);
		}

		public void SetJournalItemGuid(ProcedureStep procedureStep)
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

		void StartRecord(ProcedureStep procedureStep)
		{
			var startRecordArguments = procedureStep.StartRecordArguments;
			var cameraUid = GetValue<Guid>(startRecordArguments.CameraArgument);
			var timeout = GetValue<int>(startRecordArguments.TimeoutArgument);
			switch (startRecordArguments.TimeType)
			{
				case TimeType.Min: timeout *= 60; break;
				case TimeType.Hour: timeout *= 3600; break;
				case TimeType.Day: timeout *= 86400; break;
			}
			
			if (JournalItem != null)
			{
				Guid eventUid = Guid.NewGuid();
				SetValue(startRecordArguments.EventUIDArgument, eventUid);
				ProcedureExecutionContext.StartRecord(cameraUid, JournalItem.UID , eventUid, timeout);
			}
		}

		private void StopRecord(ProcedureStep procedureStep)
		{
			var stopRecordArguments = procedureStep.StopRecordArguments;
			var cameraUid = GetValue<Guid>(stopRecordArguments.CameraArgument);
			var eventUid = GetValue<Guid>(stopRecordArguments.EventUIDArgument);
			ProcedureExecutionContext.StopRecord(cameraUid, eventUid);
		}

		public void Ptz(ProcedureStep procedureStep)
		{
			var ptzArguments = procedureStep.PtzArguments;
			var cameraUid = GetValue<Guid>(ptzArguments.CameraArgument);
			var ptzNumber = GetValue<int>(ptzArguments.PtzNumberArgument);
			ProcedureExecutionContext.Ptz(cameraUid, ptzNumber);
		}

		public void RviAlarm(ProcedureStep procedureStep)
		{
			var rviAlarmArguments = procedureStep.RviAlarmArguments;
			var name = GetValue<string>(rviAlarmArguments.NameArgument);
			ProcedureExecutionContext.RviAlarm(name);
		}

		public void Now(ProcedureStep procedureStep)
		{
			var nowArguments = procedureStep.NowArguments;
			SetValue(nowArguments.ResultArgument, DateTime.Now);
		}

		public void RunProgram(ProcedureStep procedureStep)
		{
			var processName = GetValue<string>(procedureStep.RunProgramArguments.PathArgument);
			var parameters = GetValue<string>(procedureStep.RunProgramArguments.ParametersArgument);
			Process.Start(processName, parameters);
		}

		void ControlFireZone(ProcedureStep procedureStep)
		{
			var zoneUid = GetValue<Guid>(procedureStep.ControlGKFireZoneArguments.GKFireZoneArgument);
			var zoneCommandType = procedureStep.ControlGKFireZoneArguments.ZoneCommandType;
			ProcedureExecutionContext.ControlFireZone(zoneUid, zoneCommandType);
		}

		void ControlGuardZone(ProcedureStep procedureStep)
		{
			var zoneUid = GetValue<Guid>(procedureStep.ControlGKGuardZoneArguments.GKGuardZoneArgument);
			var guardZoneCommandType = procedureStep.ControlGKGuardZoneArguments.GuardZoneCommandType;
			ProcedureExecutionContext.ControlGuardZone(zoneUid, guardZoneCommandType);
		}

		void ControlDirection(ProcedureStep procedureStep)
		{
			var directionUid = GetValue<Guid>(procedureStep.ControlDirectionArguments.DirectionArgument);
			var directionCommandType = procedureStep.ControlDirectionArguments.DirectionCommandType;
			ProcedureExecutionContext.ControlDirection(directionUid, directionCommandType);
		}

		void ControlGKDoor(ProcedureStep procedureStep)
		{
			var doorUid = GetValue<Guid>(procedureStep.ControlGKDoorArguments.DoorArgument);
			var doorCommandType = procedureStep.ControlGKDoorArguments.DoorCommandType;
			ProcedureExecutionContext.ControlGKDoor(doorUid, doorCommandType);
		}

		void ControlDelay(ProcedureStep procedureStep)
		{
			var delayUid = GetValue<Guid>(procedureStep.ControlDelayArguments.DelayArgument);
			var delayCommandType = procedureStep.ControlDelayArguments.DelayCommandType;
			ProcedureExecutionContext.ControlDelay(delayUid, delayCommandType);
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
			var value = GetValue<int>(incrementValueArguments.ResultArgument);
			if (incrementValueArguments.IncrementType == IncrementType.Inc)
				ProcedureExecutionContext.SetVariableValue(variable, value + 1);
			else
				ProcedureExecutionContext.SetVariableValue(variable, value - 1);
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
			ProcedureExecutionContext.SetVariableValue(variable, itemValue);
			var explicitValue = variable.ExplicitValue;
			if (listVariable != null)
			{
				//ProcedureExecutionContext.SynchronizeVariable(listVariable, ContextType.Client);
				switch (changeListArguments.ChangeType)
				{
					case ChangeType.AddLast:
						listVariable.ExplicitValues.Add(explicitValue);
						break;
					case ChangeType.RemoveFirst:
						listVariable.ExplicitValues.Remove(listVariable.ExplicitValues.FirstOrDefault
							(x => ExplicitCompare(x, explicitValue, changeListArguments.ItemArgument.ExplicitType,
								changeListArguments.ListArgument.EnumType)));
						break;
					case ChangeType.RemoveAll:
						listVariable.ExplicitValues.RemoveAll(
							(x => ExplicitCompare(x, explicitValue, changeListArguments.ListArgument.ExplicitType,
								changeListArguments.ListArgument.EnumType)));
						break;
				}
				//ProcedureExecutionContext.SynchronizeVariable(listVariable, ContextType.Server);
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
			{
				//ProcedureExecutionContext.SynchronizeVariable(listVariable, ContextType.Client);
				countVariable.ExplicitValue.IntValue = listVariable.ExplicitValues.Count;
			}
		}

		void GetListItem(ProcedureStep procedureStep)
		{
			var getListItemArgument = procedureStep.GetListItemArguments;
			var listVariable = AllVariables.FirstOrDefault(x => x.Uid == getListItemArgument.ListArgument.VariableUid);
			var itemVariable = AllVariables.FirstOrDefault(x => x.Uid == getListItemArgument.ItemArgument.VariableUid);
			if ((itemVariable != null) && (listVariable != null))
			{
				ProcedureExecutionContext.SynchronizeVariable(listVariable, ContextType.Client);
				if (getListItemArgument.PositionType == PositionType.First)
					ProcedureExecutionContext.SetVariableValue(itemVariable, ProcedureExecutionContext.GetValue(listVariable.ExplicitValues.FirstOrDefault(), itemVariable.ExplicitType, itemVariable.EnumType));
				if (getListItemArgument.PositionType == PositionType.Last)
					ProcedureExecutionContext.SetVariableValue(itemVariable, ProcedureExecutionContext.GetValue(listVariable.ExplicitValues.LastOrDefault(), itemVariable.ExplicitType, itemVariable.EnumType));
				if (getListItemArgument.PositionType == PositionType.ByIndex)
				{
					var indexValue = GetValue<int>(getListItemArgument.IndexArgument);
					if (listVariable.ExplicitValues.Count > indexValue)
						ProcedureExecutionContext.SetVariableValue(itemVariable, ProcedureExecutionContext.GetValue(listVariable.ExplicitValues[indexValue], itemVariable.ExplicitType, itemVariable.EnumType));
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
				ProcedureExecutionContext.SetVariableValue(resultVariable, value);
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
			if (explicitType == ExplicitType.Object)
				return explicitValue1.UidValue == explicitValue2.UidValue;
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
			ProcedureExecutionContext.ExportJournal(isExportJournal, isExportPassJournal, minDate, maxDate, path);
		}

		void ExportOrganisation(ProcedureStep procedureStep)
		{
			var arguments = procedureStep.ExportOrganisationArguments;
			var isWithDeleted = GetValue<bool>(arguments.IsWithDeleted);
			var organisationUid = GetValue<Guid>(arguments.Organisation);
			var path = GetValue<string>(arguments.PathArgument);
			ProcedureExecutionContext.ExportOrganisation(isWithDeleted, organisationUid, path);
		}

		void ExportOrganisationList(ProcedureStep procedureStep)
		{
			var arguments = procedureStep.ImportOrganisationArguments;
			var isWithDeleted = GetValue<bool>(arguments.IsWithDeleted);
			var path = GetValue<string>(arguments.PathArgument);
			ProcedureExecutionContext.ExportOrganisationList(isWithDeleted, path);
		}

		void ExportConfiguration(ProcedureStep procedureStep)
		{
			var arguments = procedureStep.ExportConfigurationArguments;
			var isExportDevices = GetValue<bool>(arguments.IsExportDevices);
			var isExportDoors = GetValue<bool>(arguments.IsExportDoors);
			var isExportZones = GetValue<bool>(arguments.IsExportZones);
			var path = GetValue<string>(arguments.PathArgument);
			ProcedureExecutionContext.ExportConfiguration(isExportDevices, isExportDoors, isExportZones, path);
		}

		void ImportOrganisation(ProcedureStep procedureStep)
		{
			var arguments = procedureStep.ImportOrganisationArguments;
			var isWithDeleted = GetValue<bool>(arguments.IsWithDeleted);
			var path = GetValue<string>(arguments.PathArgument);
			ProcedureExecutionContext.ImportOrganisation(isWithDeleted, path);
		}

		void ImportOrganisationList(ProcedureStep procedureStep)
		{
			var arguments = procedureStep.ImportOrganisationArguments;
			var isWithDeleted = GetValue<bool>(arguments.IsWithDeleted);
			var path = GetValue<string>(arguments.PathArgument);
			ProcedureExecutionContext.ImportOrganisationList(isWithDeleted, path);
		}

		void SetValue(Argument argument, object propertyValue)
		{
			ProcedureExecutionContext.SetVariableValue(AllVariables.FirstOrDefault(x => x.Uid == argument.VariableUid), propertyValue);
		}
		
		T GetValue<T>(Argument argument)
		{
			return argument.VariableScope == VariableScope.ExplicitValue ?
				(T)ProcedureExecutionContext.GetValue(argument.ExplicitValue, argument.ExplicitType, argument.EnumType) :
				(T)ProcedureExecutionContext.GetVariableValue(AllVariables.FirstOrDefault(x => x.Uid == argument.VariableUid));
		}

		bool CheckGuid(string guidString)
		{
			var guidRegEx = new Regex("^[A-Fa-f0-9]{32}$|" + "^({|\\()?[A-Fa-f0-9]{8}-([A-Fa-f0-9]{4}-){3}[A-Fa-f0-9]{12}(}|\\))?$|" +
				"^({)?[0xA-Fa-f0-9]{3,10}(, {0,1}[0xA-Fa-f0-9]{3,6}){2}, {0,1}({)([0xA-Fa-f0-9]{3,4}, {0,1}){7}[0xA-Fa-f0-9]{3,4}(}})$", RegexOptions.Compiled);
			if (!String.IsNullOrEmpty(guidString) && guidRegEx.IsMatch(guidString))
				return true;
			return false;
		}
	}
}
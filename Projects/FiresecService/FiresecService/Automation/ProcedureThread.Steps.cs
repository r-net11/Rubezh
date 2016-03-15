using System.IO;
using System.Text;
using Common;
using FiresecAPI;
using FiresecAPI.Automation;
using FiresecAPI.Automation.Enums;
using FiresecAPI.AutomationCallback;
using FiresecAPI.Enums;
using FiresecAPI.Extensions;
using FiresecAPI.GK;
using FiresecAPI.Journal;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using FiresecService.Automation;
using FiresecService.Service;
using HigLabo.Mime;
using HigLabo.Net.Smtp;
using SKDDriver;
using SKDDriver.Translators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Media;
using Property = FiresecAPI.Automation.Property;

namespace FiresecService
{
	public partial class ProcedureThread
	{
		private void AddJournalItem(ProcedureStep procedureStep)
		{
			var journalItem = new JournalItem();
			journalItem.SystemDateTime = DateTime.Now;
			journalItem.JournalEventNameType = JournalEventNameType.Сообщение_автоматизации;
			var messageValue = GetValue<object>(procedureStep.JournalArguments.MessageArgument);
			journalItem.DescriptionText = messageValue.GetType().IsEnum ? ((Enum)messageValue).ToDescription() : messageValue.ToString();
			Service.FiresecService.AddCommonJournalItem(journalItem);
		}

		private bool Compare(ProcedureStep procedureStep)
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

		private void ShowMessage(ProcedureStep procedureStep)
		{
			var showMessageArguments = procedureStep.ShowMessageArguments;
			var messageValue = GetValue<object>(showMessageArguments.MessageArgument);
			var message = messageValue.GetType().IsEnum ? ((Enum)messageValue).ToDescription() : messageValue.ToString();
			var automationCallbackResult = new AutomationCallbackResult
			{
				AutomationCallbackType = AutomationCallbackType.Message,
				Data = new MessageCallbackData
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

		private void ShowDialog(ProcedureStep procedureStep)
		{
			var automationCallbackResult = new AutomationCallbackResult
			{
				AutomationCallbackType = AutomationCallbackType.Dialog,
				Data = new DialogCallbackData
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

		private void ShowProperty(ProcedureStep procedureStep)
		{
			var showPropertyArguments = procedureStep.ShowPropertyArguments;
			var automationCallbackResult = new AutomationCallbackResult
			{
				AutomationCallbackType = AutomationCallbackType.Property,
				Data = new PropertyCallBackData
				{
					ObjectType = showPropertyArguments.ObjectType,
					ObjectUid = showPropertyArguments.ObjectArgument.ExplicitValue.UidValue
				},
			};
			SendCallback(showPropertyArguments, automationCallbackResult);
		}

		/// <summary>
		/// Отправляет сообщение по электронной почте
		/// </summary>
		/// <param name="procedureStep">Шаг процедуры</param>
		private void SendEmail(ProcedureStep procedureStep)
		{
			var sendEmailArguments = procedureStep.SendEmailArguments;
			var smtp = GetValue<string>(sendEmailArguments.SmtpArgument);
			var port = GetValue<int>(sendEmailArguments.PortArgument);
			var login = GetValue<string>(sendEmailArguments.LoginArgument);
			var password = GetValue<string>(sendEmailArguments.PasswordArgument);
			var eMailAddressFrom = GetValue<string>(sendEmailArguments.EMailAddressFromArgument);
			var eMailAddressTos = sendEmailArguments.EMailAddressToArguments.Select(x => GetValue<string>(x)).ToList();
			var title = GetValue<string>(sendEmailArguments.EMailTitleArgument);
			var content = GetValue<string>(sendEmailArguments.EMailContentArgument);
			var eMailAttachedFiles = sendEmailArguments.EMailAttachedFileArguments.Select(x => GetValue<string>(x)).ToList();
			
			using (var smtpClient = new SmtpClient(smtp, port, login, password))
			{
				var message = new SmtpMessage();
				// От кого
				message.From = new MailAddress(eMailAddressFrom);
				// Кому
				foreach (var eMailAddressTo in eMailAddressTos)
				{
					message.To.Add(new MailAddress(eMailAddressTo));
				}
				// Тема
				message.Subject = title;
				// Содержимое
				message.BodyText = content;
				// Вложения
				foreach (var eMailAttachedFile in eMailAttachedFiles)
				{
					if (File.Exists(eMailAttachedFile))
					{
						var cnt = new SmtpContent();
						cnt.LoadData(File.ReadAllBytes(eMailAttachedFile));
						cnt.ContentType = new HigLabo.Net.Smtp.ContentType("application/octet-stream");
						cnt.ContentType.Name = String.Format("=?utf-8?B?{0}?=", Convert.ToBase64String(Encoding.UTF8.GetBytes(Path.GetFileName(eMailAttachedFile))));
						message.Contents.Add(cnt);
					}
				}
				// Протокол защиты соединения
				switch (sendEmailArguments.SecureProtocol)
				{
					case EmailSecureProtocol.None:
						smtpClient.EncryptedCommunication = SmtpEncryptedCommunication.None;
						break;
					case EmailSecureProtocol.Ssl:
						smtpClient.EncryptedCommunication = SmtpEncryptedCommunication.Ssl;
						break;
					case EmailSecureProtocol.Tls:
						smtpClient.EncryptedCommunication = SmtpEncryptedCommunication.Tls;
						break;
				}

				try
				{
					smtpClient.SendMail(message);
				}
				catch (Exception e)
				{
					Logger.Error(e);
					throw;
				}
			}
		}

		private void PlaySound(ProcedureStep procedureStep)
		{
			var automationCallbackResult = new AutomationCallbackResult
			{
				AutomationCallbackType = AutomationCallbackType.Sound,
				Data = new SoundCallbackData
				{
					SoundUID = procedureStep.SoundArguments.SoundUid,
				},
			};
			SendCallback(procedureStep.SoundArguments, automationCallbackResult);
		}

		private void ControlVisual(ProcedureStep procedureStep, ControlElementType type)
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
			var automationCallbackResult = new AutomationCallbackResult
			{
				AutomationCallbackType = callbackType,
				Data = new VisualPropertyCallbackData
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

		private void ControlPlan(ProcedureStep procedureStep, ControlElementType controlElementType)
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

			var automationCallbackResult = new AutomationCallbackResult
			{
				AutomationCallbackType = callbackType,
				Data = new PlanCallbackData
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

		private void Calculate(ProcedureStep procedureStep)
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
						result = (bool) variable1 & (bool) variable2;
					if (arithmeticArguments.ArithmeticOperationType == ArithmeticOperationType.Or)
						result = (bool) variable1 || (bool) variable2;
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
						result = (int) variable1 + (int) variable2;
					if (arithmeticArguments.ArithmeticOperationType == ArithmeticOperationType.Sub)
						result = (int) variable1 - (int) variable2;
					if (arithmeticArguments.ArithmeticOperationType == ArithmeticOperationType.Multi)
						result = (int) variable1*(int) variable2;
					if ((arithmeticArguments.ArithmeticOperationType == ArithmeticOperationType.Div) && ((int) variable2 != 0))
						result = (int) variable1/(int) variable2;
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
						result = (DateTime) variable1 + (TimeSpan) variable2;
					if (arithmeticArguments.ArithmeticOperationType == ArithmeticOperationType.Sub)
						result = (DateTime) variable1 - (TimeSpan) variable2;

					if (resultVariable != null)
						resultVariable.ExplicitValue.DateTimeValue = result;
					break;
				}
				case ExplicitType.Time:
				{
					variable1 = GetValue<TimeSpan>(arithmeticArguments.Argument1);
					variable2 = GetValue<TimeSpan>(arithmeticArguments.Argument2);

					var result = new TimeSpan();
					if (arithmeticArguments.ArithmeticOperationType == ArithmeticOperationType.Add)
						result = (TimeSpan) variable1 + (TimeSpan) variable2;
					if (arithmeticArguments.ArithmeticOperationType == ArithmeticOperationType.Sub)
						result = (TimeSpan) variable1 - (TimeSpan) variable2;

					if (resultVariable != null)
						resultVariable.ExplicitValue.TimeSpanValue = result.WithoutDays();
					break;
				}
				case ExplicitType.String:
				{
					variable1 = GetValue<string>(arithmeticArguments.Argument1);
					variable2 = GetValue<string>(arithmeticArguments.Argument2);
					if (arithmeticArguments.ArithmeticOperationType == ArithmeticOperationType.Add)
						if (resultVariable != null)
							resultVariable.ExplicitValue.StringValue = String.Concat((string) variable1, (string) variable2);
					break;
				}
			}
		}

		private void FindObjects(ProcedureStep procedureStep)
		{
			var findObjectArguments = procedureStep.FindObjectArguments;
			var variable = AllVariables.FirstOrDefault(x => x.Uid == findObjectArguments.ResultArgument.VariableUid);
			if (findObjectArguments.JoinOperator == JoinOperator.Or)
				FindObjectsOr(variable, findObjectArguments.FindObjectConditions);
			else
				FindObjectsAnd(variable, findObjectArguments.FindObjectConditions);
		}

		private void GetObjectProperty(ProcedureStep procedureStep)
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

		private object GetPropertyValue(ref Guid itemUid, Property property, object item)
		{
			var propertyValue = new object();
			if (item is SKDDevice)
			{
				switch (property)
				{
					case Property.IntAddress:
						propertyValue = (item as SKDDevice).IntAddress;
						break;

					case Property.Description:
						propertyValue = (item as SKDDevice).Description.Trim();
						break;

					case Property.Uid:
						propertyValue = (item as SKDDevice).UID.ToString();
						break;
				}
				itemUid = (item as SKDDevice).UID;
			}

			return propertyValue;
		}

		private void InitializeItems(ref IEnumerable<object> items, ref Variable result)
		{
			var explicitValues = new List<ExplicitValue>();
			if (result.ObjectType == ObjectType.SKDDevice)
			{
				//	items = new List<GKDevice>(GKManager.DeviceConfiguration.Devices);
				//	foreach (var objectUid in new List<Guid>(GKManager.DeviceConfiguration.Devices.Select(x => x.UID)))
				//		explicitValues.Add(new ExplicitValue { UidValue = objectUid });
			}
			result.ExplicitValues = explicitValues;
		}

		private object InitializeItem(Guid itemUid)
		{
			var skdDevice = SKDManager.Devices.FirstOrDefault(x => x.UID == itemUid);
			var skdZone = SKDManager.Zones.FirstOrDefault(x => x.UID == itemUid);
			var camera = ConfigurationCashHelper.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == itemUid);
			var skdDoor = SKDManager.Doors.FirstOrDefault(x => x.UID == itemUid);
			if (skdDevice != null)
				return skdDevice;
			if (skdZone != null)
				return skdZone;
			if (camera != null)
				return camera;
			if (skdDoor != null)
				return skdDoor;
			return null;
		}

		private void FindObjectsOr(Variable result, IEnumerable<FindObjectCondition> findObjectConditions)
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

		private void FindObjectsAnd(Variable result, IEnumerable<FindObjectCondition> findObjectConditions)
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

		private static bool? Compare(object param1, object param2, ConditionType conditionType) //TODO: Move to extensions
		{
			if (param1.GetType() != param2.GetType())
				return null;

			if (param1.GetType().IsEnum || param1 is int)
				return (conditionType == ConditionType.IsEqual && (int) param1 == (int) param2)
				       || (conditionType == ConditionType.IsNotEqual && (int) param1 != (int) param2)
				       || (conditionType == ConditionType.IsMore && (int) param1 > (int) param2)
				       || (conditionType == ConditionType.IsNotMore && (int) param1 <= (int) param2)
				       || (conditionType == ConditionType.IsLess && (int) param1 < (int) param2)
				       || (conditionType == ConditionType.IsNotLess && (int) param1 >= (int) param2);

			if (param1 is DateTime)
				return (conditionType == ConditionType.IsEqual && (DateTime) param1 == (DateTime) param2)
				       || (conditionType == ConditionType.IsNotEqual && (DateTime) param1 != (DateTime) param2)
				       || (conditionType == ConditionType.IsMore && (DateTime) param1 > (DateTime) param2)
				       || (conditionType == ConditionType.IsNotMore && (DateTime) param1 <= (DateTime) param2)
				       || (conditionType == ConditionType.IsLess && (DateTime) param1 < (DateTime) param2)
				       || (conditionType == ConditionType.IsNotLess && (DateTime) param1 >= (DateTime) param2);

			if (param1 is TimeSpan)
				return (conditionType == ConditionType.IsEqual && (TimeSpan) param1 == (TimeSpan) param2)
				       || (conditionType == ConditionType.IsNotEqual && (TimeSpan) param1 != (TimeSpan) param2)
				       || (conditionType == ConditionType.IsMore && (TimeSpan) param1 > (TimeSpan) param2)
				       || (conditionType == ConditionType.IsNotMore && (TimeSpan) param1 <= (TimeSpan) param2)
				       || (conditionType == ConditionType.IsLess && (TimeSpan) param1 < (TimeSpan) param2)
				       || (conditionType == ConditionType.IsNotLess && (TimeSpan) param1 >= (TimeSpan) param2);

			if (param1 is string)
				return (conditionType == ConditionType.IsEqual && (string) param1 == (string) param2)
				       || (conditionType == ConditionType.IsNotEqual && (string) param1 != (string) param2)
				       || (conditionType == ConditionType.StartsWith && ((string) param1).StartsWith((string) param2))
				       || (conditionType == ConditionType.EndsWith && ((string) param1).EndsWith((string) param2))
				       || (conditionType == ConditionType.Contains && ((string) param1).Contains((string) param2));

			if (param1 is bool)
				return (conditionType == ConditionType.IsEqual && (bool) param1 == (bool) param2)
				       || (conditionType == ConditionType.IsNotEqual && (bool) param1 != (bool) param2);

			return null;
		}

		public void SetJournalItemGuid(ProcedureStep procedureStep)
		{
			var setJournalItemGuidArguments = procedureStep.SetJournalItemGuidArguments;

			if (JournalItem == null) return;

			using (var journalTranslator = new JournalTranslator())
			{
				var eventUIDString = GetValue<String>(setJournalItemGuidArguments.ValueArgument);
				Guid eventUID;

				if (CheckGuid(eventUIDString))
					eventUID = new Guid(eventUIDString);
				else
					return;

				journalTranslator.SaveVideoUID(JournalItem.UID, eventUID, Guid.Empty);
			}
		}

		private void StartRecord(ProcedureStep procedureStep)
		{
			var startRecordArguments = procedureStep.StartRecordArguments;
			var cameraUID = GetValue<Guid>(startRecordArguments.CameraArgument);
			var camera = ConfigurationCashHelper.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == cameraUID);
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
			if (JournalItem != null)
			{
				using (var journalTranslator = new JournalTranslator())
				{
					journalTranslator.SaveVideoUID(JournalItem.UID, eventUID, cameraUID);
				}
				JournalItem.VideoUID = eventUID;
				JournalItem.CameraUID = cameraUID;
				Service.FiresecService.NotifyNewJournalItems(new List<JournalItem> { JournalItem });
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
			RviClient.RviClientHelper.SetPtzPreset(ConfigurationCashHelper.SystemConfiguration, camera, ptzNumber);
		}

		public void RviAlarm(ProcedureStep procedureStep)
		{
			var rviAlarmArguments = procedureStep.RviAlarmArguments;
			var name = GetValue<string>(rviAlarmArguments.NameArgument);
			RviClient.RviClientHelper.AlarmRuleExecute(ConfigurationCashHelper.SystemConfiguration, name);
		}

		private void ControlSKDDevice(ProcedureStep procedureStep)
		{
			var deviceUid = GetValue<Guid>(procedureStep.ControlSKDDeviceArguments.SKDDeviceArgument);
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUid);
			if (device == null)
				return;
			// Открыть замок
			if (procedureStep.ControlSKDDeviceArguments.Command == SKDDeviceCommandType.Open)
				FiresecServiceManager.SafeFiresecService.SKDOpenDevice(device.UID);
			// Закрыть замок
			if (procedureStep.ControlSKDDeviceArguments.Command == SKDDeviceCommandType.Close)
				FiresecServiceManager.SafeFiresecService.SKDCloseDevice(device.UID);
			// Перевести замок в режим "Открыто"
			if (procedureStep.ControlSKDDeviceArguments.Command == SKDDeviceCommandType.AccessStateOpenAlways)
				FiresecServiceManager.SafeFiresecService.SKDDeviceAccessStateOpenAlways(device.UID);
			// Перевести замок в режим "Норма"
			if (procedureStep.ControlSKDDeviceArguments.Command == SKDDeviceCommandType.AccessStateNormal)
				FiresecServiceManager.SafeFiresecService.SKDDeviceAccessStateNormal(device.UID);
			// Перевести замок в режим "Закрыто"
			if (procedureStep.ControlSKDDeviceArguments.Command == SKDDeviceCommandType.AccessStateCloseAlways)
				FiresecServiceManager.SafeFiresecService.SKDDeviceAccessStateCloseAlways(device.UID);
			// Сброс тревоги
			if (procedureStep.ControlSKDDeviceArguments.Command == SKDDeviceCommandType.ClearPromptWarning)
				FiresecServiceManager.SafeFiresecService.SKDClearDevicePromptWarning(device.UID);
		}

		private void ControlDoor(ProcedureStep procedureStep)
		{
			var doorUid = GetValue<Guid>(procedureStep.ControlDoorArguments.DoorArgument);
			var door = SKDManager.Doors.FirstOrDefault(x => x.UID == doorUid);
			if (door == null)
				return;
			// Открыть точку доступа
			if (procedureStep.ControlDoorArguments.DoorCommandType == DoorCommandType.Open)
				FiresecServiceManager.SafeFiresecService.SKDOpenDoor(door.UID);
			// Закрыть точку доступа
			if (procedureStep.ControlDoorArguments.DoorCommandType == DoorCommandType.Close)
				FiresecServiceManager.SafeFiresecService.SKDCloseDoor(door.UID);
			// Перевести точку доступа в режим "Открыто"
			if (procedureStep.ControlDoorArguments.DoorCommandType == DoorCommandType.AccessStateOpenAlways)
				FiresecServiceManager.SafeFiresecService.SKDDoorAccessStateOpenAlways(door.UID);
			// Перевести точку доступа в режим "Норма"
			if (procedureStep.ControlDoorArguments.DoorCommandType == DoorCommandType.AccessStateNormal)
				FiresecServiceManager.SafeFiresecService.SKDDoorAccessStateNormal(door.UID);
			// Перевести точку доступа в режим "Закрыто"
			if (procedureStep.ControlDoorArguments.DoorCommandType == DoorCommandType.AccessStateCloseAlways)
				FiresecServiceManager.SafeFiresecService.SKDDoorAccessStateCloseAlways(door.UID);
			// Сброс тревоги
			if (procedureStep.ControlDoorArguments.DoorCommandType == DoorCommandType.ClearPromptWarning)
				FiresecServiceManager.SafeFiresecService.SKDClearDoorPromptWarning(door.UID);
		}

		private void ControlSKDZone(ProcedureStep procedureStep)
		{
			var zoneUid = GetValue<Guid>(procedureStep.ControlSKDZoneArguments.SKDZoneArgument);
			var zone = SKDManager.Zones.FirstOrDefault(x => x.UID == zoneUid);
			if (zone == null)
				return;
			// Открыть зону
			if (procedureStep.ControlSKDZoneArguments.SKDZoneCommandType == SKDZoneCommandType.Open)
				FiresecServiceManager.SafeFiresecService.SKDOpenZone(zone.UID);
			// Закрыть зону
			if (procedureStep.ControlSKDZoneArguments.SKDZoneCommandType == SKDZoneCommandType.Close)
				FiresecServiceManager.SafeFiresecService.SKDCloseZone(zone.UID);
			// Перевести зону в режим "Открыто"
			if (procedureStep.ControlSKDZoneArguments.SKDZoneCommandType == SKDZoneCommandType.AccessStateOpenAlways)
				FiresecServiceManager.SafeFiresecService.SKDZoneAccessStateOpenAlways(zone.UID);
			// Перевести зону в режим "Норма"
			if (procedureStep.ControlSKDZoneArguments.SKDZoneCommandType == SKDZoneCommandType.AccessStateNormal)
				FiresecServiceManager.SafeFiresecService.SKDZoneAccessStateNormal(zone.UID);
			// Перевести зону в режим "Закрыто"
			if (procedureStep.ControlSKDZoneArguments.SKDZoneCommandType == SKDZoneCommandType.AccessStateCloseAlways)
				FiresecServiceManager.SafeFiresecService.SKDZoneAccessStateCloseAlways(zone.UID);
			// Сброс тревоги
			if (procedureStep.ControlSKDZoneArguments.SKDZoneCommandType == SKDZoneCommandType.ClearPromptWarning)
				FiresecServiceManager.SafeFiresecService.SKDClearZonePromptWarning(zone.UID);
		}

		private void Pause(ProcedureStep procedureStep)
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

		private void IncrementValue(ProcedureStep procedureStep)
		{
			var incrementValueArguments = procedureStep.IncrementValueArguments;
			var variable = AllVariables.FirstOrDefault(x => x.Uid == incrementValueArguments.ResultArgument.VariableUid);
			if (variable != null)
				variable.ExplicitValue.IntValue = incrementValueArguments.IncrementType == IncrementType.Inc ? variable.ExplicitValue.IntValue + 1 : variable.ExplicitValue.IntValue - 1;
		}

		private void GetRandomValue(ProcedureStep procedureStep)
		{
			var randomArguments = procedureStep.RandomArguments;
			var resultVariable = AllVariables.FirstOrDefault(x => x.Uid == randomArguments.ResultArgument.VariableUid);
			var maxValue = GetValue<int>(randomArguments.MaxValueArgument);
			if (resultVariable != null)
				resultVariable.ExplicitValue.IntValue = new Random().Next(0, maxValue);
		}

		private void ChangeList(ProcedureStep procedureStep)
		{
			var changeListArguments = procedureStep.ChangeListArguments;
			var listVariable = AllVariables.FirstOrDefault(x => x.Uid == changeListArguments.ListArgument.VariableUid);
			var variable = new Variable
			{
				ExplicitType = changeListArguments.ItemArgument.ExplicitType,
				EnumType = changeListArguments.ItemArgument.EnumType,
				ObjectType = changeListArguments.ItemArgument.ObjectType
			};
			var itemValue = GetValue<object>(changeListArguments.ItemArgument);
			SetValue(variable, itemValue);
			var explicitValue = variable.ExplicitValue;

			if (listVariable == null) return;

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

		private void CheckPermission(ProcedureStep procedureStep)
		{
			var checkPermissionArguments = procedureStep.CheckPermissionArguments;
			var resultVariable = AllVariables.FirstOrDefault(x => x.Uid == checkPermissionArguments.ResultArgument.VariableUid);
			var permissionValue = GetValue<PermissionType>(checkPermissionArguments.PermissionArgument);
			if (resultVariable != null && User != null)
				resultVariable.ExplicitValue.BoolValue = User.HasPermission(permissionValue);
		}

		private void GetListCount(ProcedureStep procedureStep)
		{
			var getListCountArgument = procedureStep.GetListCountArguments;
			var listVariable = AllVariables.FirstOrDefault(x => x.Uid == getListCountArgument.ListArgument.VariableUid);
			var countVariable = AllVariables.FirstOrDefault(x => x.Uid == getListCountArgument.CountArgument.VariableUid);
			if ((countVariable != null) && (listVariable != null))
				countVariable.ExplicitValue.IntValue = listVariable.ExplicitValues.Count;
		}

		private void GetListItem(ProcedureStep procedureStep)
		{
			var getListItemArgument = procedureStep.GetListItemArguments;
			var listVariable = AllVariables.FirstOrDefault(x => x.Uid == getListItemArgument.ListArgument.VariableUid);
			var itemVariable = AllVariables.FirstOrDefault(x => x.Uid == getListItemArgument.ItemArgument.VariableUid);

			if (itemVariable == null || listVariable == null) return;

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

		private void GetJournalItem(ProcedureStep procedureStep)
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
				// Пользователь
				if (getJournalItemArguments.JournalColumnType == JournalColumnType.UserUid)
				{
					if (!string.IsNullOrEmpty(JournalItem.UserName) && JournalItem.EmployeeUID == Guid.Empty)
					{
						var user =	FiresecServiceManager.SafeFiresecService.GetSecurityConfiguration()
								.Users.FirstOrDefault(u => u.Name == JournalItem.UserName);
						if (user != null)
							value = user.UID;
					}
				}
				// Сотрудник
				if (getJournalItemArguments.JournalColumnType == JournalColumnType.EmployeeUid)
				{
					using (var databaseService = new SKDDatabaseService())
					{
						var employeeTranslatorGetSingle = databaseService.EmployeeTranslator.GetSingle(JournalItem.EmployeeUID);
						if (!employeeTranslatorGetSingle.HasError && employeeTranslatorGetSingle.Result.Type == PersonType.Employee)
						{
							value = JournalItem.EmployeeUID;
						}
					}
				}
				// Посетитель
				if (getJournalItemArguments.JournalColumnType == JournalColumnType.VisitorUid)
				{
					using (var databaseService = new SKDDatabaseService())
					{
						var employeeTranslatorGetSingle = databaseService.EmployeeTranslator.GetSingle(JournalItem.EmployeeUID);
						if (!employeeTranslatorGetSingle.HasError && employeeTranslatorGetSingle.Result.Type == PersonType.Guest)
						{
							value = JournalItem.EmployeeUID;
						}
					}
				}
				// Номер пропуска
				if (getJournalItemArguments.JournalColumnType == JournalColumnType.CardNo)
					value = JournalItem.CardNo;
				// Тип пропуска
				if (getJournalItemArguments.JournalColumnType == JournalColumnType.CardType)
				{
					using (var databaseService = new SKDDatabaseService())
					{
						var cardTranslatorGet = databaseService.CardTranslator.Get(JournalItem.CardNo);
						if (!cardTranslatorGet.HasError)
						{
							value = cardTranslatorGet.Result.CardType;
						}
					}
				}

				SetValue(resultVariable, value);
			}
		}

		private bool ExplicitCompare(ExplicitValue explicitValue1, ExplicitValue explicitValue2, ExplicitType explicitType, EnumType enumType) //TODO: add Time
		{
			if (explicitType == ExplicitType.Integer)
				return explicitValue1.IntValue == explicitValue2.IntValue;
			if (explicitType == ExplicitType.String)
				return explicitValue1.StringValue == explicitValue2.StringValue;
			if (explicitType == ExplicitType.Boolean)
				return explicitValue1.BoolValue == explicitValue2.BoolValue;
			if (explicitType == ExplicitType.DateTime)
				return explicitValue1.DateTimeValue == explicitValue2.DateTimeValue;
			if (explicitType == ExplicitType.Time)
				return explicitValue1.TimeSpanValue == explicitValue2.TimeSpanValue;
			if (explicitType == ExplicitType.Enum)
			{
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

		private void SetValue(ProcedureStep procedureStep)
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
				PropertyCopy.Copy(sourceVariable != null
								? sourceVariable.ExplicitValue
								: setValueArguments.SourceArgument.ExplicitValue,
								targetVariable.ExplicitValue);
		}

		private void GetDateTimeNowStep(ProcedureStep procedureStep)
		{
			var getDateTimeNowArguments = procedureStep.GetDateTimeNowArguments;
			var resultVariable = AllVariables.FirstOrDefault(x => x.Uid == getDateTimeNowArguments.Result.VariableUid);

			if (resultVariable == null) return;

			switch (getDateTimeNowArguments.RoundingType)
			{
				case RoundingType.None:
					resultVariable.ExplicitValue.TimeSpanValue = DateTime.Now.TimeOfDay.WithoutMilliseconds();
					break;
				case RoundingType.RoundToHour:
					resultVariable.ExplicitValue.TimeSpanValue = DateTime.Now.TimeOfDay.WithoutMilliseconds().WithoutSeconds().WithoutMinutes();
					break;
				case RoundingType.RoundToMin:
					resultVariable.ExplicitValue.TimeSpanValue = DateTime.Now.TimeOfDay.WithoutMilliseconds().WithoutSeconds();
					break;
			}
		}

		private void ExportJournal(ProcedureStep procedureStep)
		{
			var arguments = procedureStep.ExportJournalArguments;
			var isExportJournal = GetValue<bool>(arguments.IsExportJournalArgument);
			var isExportPassJournal = GetValue<bool>(arguments.IsExportPassJournalArgument);
			var minDate = GetValue<DateTime>(arguments.MinDateArgument);
			var maxDate = GetValue<DateTime>(arguments.MaxDateArgument);
			var path = GetValue<string>(arguments.PathArgument);
			FiresecServiceManager.SafeFiresecService.ExportJournal(
				new JournalExportFilter
				{
					IsExportJournal = isExportJournal,
					IsExportPassJournal = isExportPassJournal,
					MaxDate = maxDate,
					MinDate = minDate,
					Path = path
				});
		}

		private void ExportOrganisation(ProcedureStep procedureStep)
		{
			var arguments = procedureStep.ExportOrganisationArguments;
			var isWithDeleted = GetValue<bool>(arguments.IsWithDeleted);
			var organisationUID = GetValue<Guid>(arguments.Organisation);
			var path = GetValue<string>(arguments.PathArgument);
			FiresecServiceManager.SafeFiresecService.ExportOrganisation(
				new ExportFilter
				{
					IsWithDeleted = isWithDeleted,
					OrganisationUID = organisationUID,
					Path = path
				});
		}

		private void ExportOrganisationList(ProcedureStep procedureStep)
		{
			var arguments = procedureStep.ImportOrganisationArguments;
			var isWithDeleted = GetValue<bool>(arguments.IsWithDeleted);
			var path = GetValue<string>(arguments.PathArgument);
			FiresecServiceManager.SafeFiresecService.ExportOrganisationList(
				new ExportFilter
				{
					IsWithDeleted = isWithDeleted,
					Path = path
				});
		}

		private void ExportConfiguration(ProcedureStep procedureStep)
		{
			var arguments = procedureStep.ExportConfigurationArguments;
			var isExportDevices = GetValue<bool>(arguments.IsExportDevices);
			var isExportDoors = GetValue<bool>(arguments.IsExportDoors);
			var isExportZones = GetValue<bool>(arguments.IsExportZones);
			var path = GetValue<string>(arguments.PathArgument);
			FiresecServiceManager.SafeFiresecService.ExportConfiguration(
				new ConfigurationExportFilter
				{
					IsExportDevices = isExportDevices,
					IsExportDoors = isExportDoors,
					IsExportZones = isExportZones,
					Path = path
				});
		}

		private void ExportReport(ProcedureStep procedureStep)
		{
			var arguments = procedureStep.ExportReportArguments;
			FiresecServiceManager.SafeFiresecService.ExportReport(
				new ReportExportFilter
				{
					EndDate = GetValue<DateTime>(arguments.EndDate),
					StartDate = GetValue<DateTime>(arguments.StartDate),
					Path = GetValue<string>(arguments.FilePath),
					ReportFilter = arguments.ReportFilter,
					ReportType = arguments.ReportType,
					ReportFormat = arguments.ReportFormat,
					IsFilterNameInHeader = arguments.IsFilterNameInHeader,
					IsShowArchive = arguments.IsUseArchive,
					IsUseExpirationDate = arguments.IsUseExpirationDate,
					ReportEndDateType = arguments.ReportEndDateType,
					IsUseDateTimeNow = arguments.IsUseDateTimeNow,
					ReportPeriodType = arguments.ReportPeriodType
				});
		}

		private void ImportOrganisation(ProcedureStep procedureStep)
		{
			var arguments = procedureStep.ImportOrganisationArguments;
			var isWithDeleted = GetValue<bool>(arguments.IsWithDeleted);
			var path = GetValue<string>(arguments.PathArgument);
			FiresecServiceManager.SafeFiresecService.ImportOrganisation(
				new ImportFilter
				{
					IsWithDeleted = isWithDeleted,
					Path = path
				});
		}

		private void ImportOrganisationList(ProcedureStep procedureStep)
		{
			var arguments = procedureStep.ImportOrganisationArguments;
			var isWithDeleted = GetValue<bool>(arguments.IsWithDeleted);
			var path = GetValue<string>(arguments.PathArgument);
			FiresecServiceManager.SafeFiresecService.ImportOrganisationList(
				new ImportFilter
				{
					IsWithDeleted = isWithDeleted,
					Path = path
				});
		}

		private void SetValue(Argument argument, object propertyValue)
		{
			var variable = AllVariables.FirstOrDefault(x => x.Uid == argument.VariableUid);
			if (variable != null)
				SetValue(variable, propertyValue);
		}

		private void SetValue(Variable target, object propertyValue)
		{
			if (target.ExplicitType == ExplicitType.Integer)
				target.ExplicitValue.IntValue = Convert.ToInt32(propertyValue);
			if (target.ExplicitType == ExplicitType.String)
				target.ExplicitValue.StringValue = Convert.ToString(propertyValue);
			if (target.ExplicitType == ExplicitType.Boolean)
				target.ExplicitValue.BoolValue = Convert.ToBoolean(propertyValue);
			if (target.ExplicitType == ExplicitType.DateTime)
				target.ExplicitValue.DateTimeValue = Convert.ToDateTime(propertyValue);
			if (target.ExplicitType == ExplicitType.Time)
				target.ExplicitValue.TimeSpanValue = Convert.ToDateTime(propertyValue).TimeOfDay;
			if (target.ExplicitType == ExplicitType.Enum)
			{
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
				if (target.EnumType == EnumType.CardType)
					target.ExplicitValue.CardTypeValue = (CardType)propertyValue;
				// Режим доступа
				if (target.EnumType == EnumType.AccessState)
					target.ExplicitValue.AccessStateValue = (FiresecAPI.Automation.Enums.AccessState?)propertyValue;
				// Статус двери
				if (target.EnumType == EnumType.DoorStatus)
					target.ExplicitValue.DoorStatusValue = (FiresecAPI.Automation.Enums.DoorStatus?)propertyValue;
				// Статус по взлому
				if (target.EnumType == EnumType.BreakInStatus)
					target.ExplicitValue.BreakInStatusValue = (FiresecAPI.Automation.Enums.BreakInStatus?)propertyValue;
				// Статус соединения
				if (target.EnumType == EnumType.ConnectionStatus)
					target.ExplicitValue.ConnectionStatusValue = (FiresecAPI.Automation.Enums.ConnectionStatus?)propertyValue;
			}
		}

		private T GetValue<T>(Argument variable)
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

		private bool CheckGuid(string guidString)
		{
			var guidRegEx = new Regex("^[A-Fa-f0-9]{32}$|" + "^({|\\()?[A-Fa-f0-9]{8}-([A-Fa-f0-9]{4}-){3}[A-Fa-f0-9]{12}(}|\\))?$|" +
				"^({)?[0xA-Fa-f0-9]{3,10}(, {0,1}[0xA-Fa-f0-9]{3,6}){2}, {0,1}({)([0xA-Fa-f0-9]{3,4}, {0,1}){7}[0xA-Fa-f0-9]{3,4}(}})$", RegexOptions.Compiled);
			return !String.IsNullOrEmpty(guidString) && guidRegEx.IsMatch(guidString);
		}

		private T GetValue<T>(ExplicitValue explicitValue, ExplicitType explicitType, EnumType enumType)
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
			if (explicitType == ExplicitType.Time)
				result = explicitValue.TimeSpanValue;
			if (explicitType == ExplicitType.Enum)
			{
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
				if (enumType == EnumType.CardType)
					result = explicitValue.CardTypeValue;
				if (enumType == EnumType.AccessState)
					result = explicitValue.AccessStateValue;
				if (enumType == EnumType.DoorStatus)
					result = explicitValue.DoorStatusValue;
				if (enumType == EnumType.BreakInStatus)
					result = explicitValue.BreakInStatusValue;
				if (enumType == EnumType.ConnectionStatus)
					result = explicitValue.ConnectionStatusValue;
			}
			return (T)result;
		}

		/// <summary>
		/// Получает свойство устройства СКД
		/// </summary>
		private void GetSkdDeviceProperty(ProcedureStep procedureStep)
		{
			var getObjectPropertyArguments = procedureStep.GetObjectPropertyArguments;
			var target = AllVariables.FirstOrDefault(x => x.Uid == getObjectPropertyArguments.ResultArgument.VariableUid);
			var deviceUid = GetValue<Guid>(getObjectPropertyArguments.ObjectArgument);
			var device = SKDManager.Devices.FirstOrDefault(x => x.UID == deviceUid);
			if (device == null)
				return;
			var propertyValue = GetSkdDevicePropertyValue(device, getObjectPropertyArguments.Property);
			SetValue(target, propertyValue);
		}

		private object GetSkdDevicePropertyValue(SKDDevice device, Property property)
		{
			// Режим доступа
			if (property == Property.AccessState)
				return (FiresecAPI.Automation.Enums.AccessState)device.State.AccessState;
			// Статус двери
			if (property == Property.DoorStatus)
				return XStateClassToDoorStatus(device.State.StateClass);
			// Статус по взлому
			if (property == Property.BreakInStatus)
				return XStateClassToBreakInStatus(device.State.StateClass);
			// Статус соединения
			if (property == Property.ConnectionStatus)
				return device.State.StateClass == XStateClass.ConnectionLost ? FiresecAPI.Automation.Enums.ConnectionStatus.Disconnected : FiresecAPI.Automation.Enums.ConnectionStatus.Connected;
			return null;
		}

		private DoorStatus? XStateClassToDoorStatus(XStateClass xStateClass)
		{
			switch (xStateClass)
			{
				case XStateClass.On:
					return DoorStatus.Opened;
				case XStateClass.Off:
					return DoorStatus.Closed;
				default:
					return null;
			}
		}

		private BreakInStatus? XStateClassToBreakInStatus(XStateClass xStateClass)
		{
			switch (xStateClass)
			{
				case XStateClass.On:
				case XStateClass.Off:
					return BreakInStatus.Normal;
				case XStateClass.Attention:
					return BreakInStatus.BreakIn;
				default:
					return null;
			}
		}

		/// <summary>
		/// Получает свойство точки доступа
		/// </summary>
		private void GetDoorProperty(ProcedureStep procedureStep)
		{
			var getObjectPropertyArguments = procedureStep.GetObjectPropertyArguments;
			var target = AllVariables.FirstOrDefault(x => x.Uid == getObjectPropertyArguments.ResultArgument.VariableUid);
			var doorUid = GetValue<Guid>(getObjectPropertyArguments.ObjectArgument);
			var door = SKDManager.Doors.FirstOrDefault(x => x.UID == doorUid);
			if (door == null)
				return;
			var propertyValue = GetDoorPropertyValue(door, getObjectPropertyArguments.Property);
			SetValue(target, propertyValue);
		}

		private object GetDoorPropertyValue(SKDDoor door, Property property)
		{
			// Режим доступа
			if (property == Property.AccessState)
				return (FiresecAPI.Automation.Enums.AccessState)door.State.AccessState;
			// Статус двери
			if (property == Property.DoorStatus)
				return XStateClassToDoorStatus(door.State.StateClass);
			// Статус по взлому
			if (property == Property.BreakInStatus)
				return XStateClassToBreakInStatus(door.State.StateClass);
			// Статус соединения
			if (property == Property.ConnectionStatus)
				return door.State.StateClass == XStateClass.ConnectionLost ? FiresecAPI.Automation.Enums.ConnectionStatus.Disconnected : FiresecAPI.Automation.Enums.ConnectionStatus.Connected;
			return null;
		}

		/// <summary>
		/// Получает свойство зоны СКД
		/// </summary>
		private void GetSkdZoneProperty(ProcedureStep procedureStep)
		{
			var getObjectPropertyArguments = procedureStep.GetObjectPropertyArguments;
			var target = AllVariables.FirstOrDefault(x => x.Uid == getObjectPropertyArguments.ResultArgument.VariableUid);
			var zoneUid = GetValue<Guid>(getObjectPropertyArguments.ObjectArgument);
			var zone = SKDManager.Zones.FirstOrDefault(x => x.UID == zoneUid);
			if (zone == null)
				return;
			var propertyValue = GetSkdZonePropertyValue(zone, getObjectPropertyArguments.Property);
			SetValue(target, propertyValue);
		}

		private object GetSkdZonePropertyValue(SKDZone zone, Property property)
		{
			// Статус двери
			if (property == Property.DoorStatus)
				return XStateClassToDoorStatus(zone.State.StateClass);
			// Статус по взлому
			if (property == Property.BreakInStatus)
				return XStateClassToBreakInStatus(zone.State.StateClass);
			// Статус соединения
			if (property == Property.ConnectionStatus)
				return zone.State.StateClass == XStateClass.ConnectionLost ? FiresecAPI.Automation.Enums.ConnectionStatus.Disconnected : FiresecAPI.Automation.Enums.ConnectionStatus.Connected;
			return null;
		}
	}
}
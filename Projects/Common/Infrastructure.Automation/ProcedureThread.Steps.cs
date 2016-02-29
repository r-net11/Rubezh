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
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using Property = RubezhAPI.Automation.Property;

namespace Infrastructure.Automation
{
	public partial class ProcedureThread
	{
		void AddJournalItem(ProcedureStep procedureStep)
		{
			var messageValue = GetValue<object>(procedureStep.JournalArguments.MessageArgument);
			ProcedureExecutionContext.AddJournalItem(ClientUID, GetStringValue(messageValue));
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
			var messageString = GetStringValue(message);
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
			var windowID = GetValue<string>(procedureStep.ShowDialogArguments.WindowIDArgument);
			var automationCallbackResult = new AutomationCallbackResult()
			{
				AutomationCallbackType = AutomationCallbackType.ShowDialog,
				Data = new ShowDialogCallbackData()
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
					Top = procedureStep.ShowDialogArguments.Top,
					WindowID = windowID
				},
			};
			SendCallback(procedureStep.ShowDialogArguments, automationCallbackResult);
		}

		void CloseDialog(ProcedureStep procedureStep)
		{
			var windowID = GetValue<string>(procedureStep.CloseDialogArguments.WindowIDArgument);
			var automationCallbackResult = new AutomationCallbackResult()
			{
				AutomationCallbackType = AutomationCallbackType.CloseDialog,
				Data = new CloseDialogCallbackData()
				{
					WindowID = windowID
				}
			};
			SendCallback(procedureStep.CloseDialogArguments, automationCallbackResult);
		}

		void ShowProperty(ProcedureStep procedureStep)
		{
			var showPropertyArguments = procedureStep.ShowPropertyArguments;
			var objectUid = GetValue<Guid>(showPropertyArguments.ObjectArgument);
			if (showPropertyArguments.ObjectType == ObjectType.Zone && !LicenseManager.CurrentLicenseInfo.HasFirefighting ||
				showPropertyArguments.ObjectType == ObjectType.GuardZone && !LicenseManager.CurrentLicenseInfo.HasGuard ||
				showPropertyArguments.ObjectType == ObjectType.GKDoor && !LicenseManager.CurrentLicenseInfo.HasSKD ||
				showPropertyArguments.ObjectType == ObjectType.VideoDevice && !LicenseManager.CurrentLicenseInfo.HasVideo)
			{
				ProcedureExecutionContext.AddJournalItem(ClientUID, "Выполнение функции \"Показать свойства объекта\" заблокировано в связи с отсутствием лицензии", objectUid);
				return;
			}
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

		void ControlOpcDaTag(ProcedureStep procedureStep, ControlElementType controlElementType)
		{
			var controlOpcDaTagArguments = procedureStep.ControlOpcDaTagArguments;

			if (controlElementType == ControlElementType.Get)
			{
				var value = OpcDaHelper.GetTagValue(controlOpcDaTagArguments.OpcDaTagUID);
				SetValue(controlOpcDaTagArguments.ValueArgument, value);
			}
			else
			{
				var value = GetValue<object>(controlOpcDaTagArguments.ValueArgument);
				OpcDaHelper.OnWriteTagValue(controlOpcDaTagArguments.OpcDaTagUID, value);
			}
		}

		void HttpRequest(ProcedureStep procedureStep)
		{
			var httpRequestArguments = procedureStep.HttpRequestArguments;
			var url = GetValue<string>(httpRequestArguments.UrlArgument);
			var content = GetValue<string>(httpRequestArguments.ContentArgument);
			var responseVariable = AllVariables.FirstOrDefault(x => x.Uid == httpRequestArguments.ResponseArgument.VariableUid);
			var webClient = new WebClient();
			webClient.Encoding = System.Text.Encoding.UTF8;
			var response = "";
			switch (httpRequestArguments.HttpMethod)
			{
				case HttpMethod.Get:
					response = webClient.DownloadString(url);
					break;

				case HttpMethod.Post:
					response = webClient.UploadString(url, content);
					break;
			}

			SetValue(httpRequestArguments.ResponseArgument, response);
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
				case ExplicitType.Float:
					{
						value1 = GetValue<double>(arithmeticArguments.Argument1);
						value2 = GetValue<double>(arithmeticArguments.Argument2);
						double result = 0;
						if (arithmeticArguments.ArithmeticOperationType == ArithmeticOperationType.Add)
							result = (double)value1 + (double)value2;
						if (arithmeticArguments.ArithmeticOperationType == ArithmeticOperationType.Sub)
							result = (double)value1 - (double)value2;
						if (arithmeticArguments.ArithmeticOperationType == ArithmeticOperationType.Multi)
							result = (double)value1 * (double)value2;
						if ((arithmeticArguments.ArithmeticOperationType == ArithmeticOperationType.Div) && ((int)value2 != 0))
							result = (double)value1 / (double)value2;
						if (resultVariable != null)
							resultVariable.ExplicitValue.FloatValue = result;
						break;
					}

				case ExplicitType.DateTime:
					{
						value1 = GetValue<DateTime>(arithmeticArguments.Argument1);
						value2 = new TimeSpan();
						switch (arithmeticArguments.TimeType)
						{
							case TimeType.Millisec:
								value2 = TimeSpan.FromMilliseconds(GetValue<int>(arithmeticArguments.Argument2));
								break;
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
								resultVariable.ExplicitValue.StringValue = String.Concat(GetStringValue(value1), GetStringValue(value2));
						break;
					}
			}
			ProcedureExecutionContext.SynchronizeVariable(resultVariable, ClientUID);
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
			ProcedureExecutionContext.SetVariableValue(target, propertyValue, ClientUID);
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

			var pumpStation = GKManager.DeviceConfiguration.PumpStations.FirstOrDefault(x => x.UID == itemUid);
			if (pumpStation != null)
				return pumpStation;

			var mpt = GKManager.DeviceConfiguration.MPTs.FirstOrDefault(x => x.UID == itemUid);
			if (mpt != null)
				return mpt;

			var door = GKManager.Doors.FirstOrDefault(x => x.UID == itemUid);
			if (door != null)
				return door;

			var organisations = ProcedureExecutionContext.GetOrganisations(ClientUID);
			var organisation = organisations == null ? null : organisations.FirstOrDefault(x => x.UID == itemUid);
			if (organisation != null)
				return organisation;

			return null;
		}

		string GetStringValue(object obj)
		{
			if (obj == null)
				return "";

			var objType = obj.GetType();
			if (objType == typeof(bool))
				return (bool)obj ? "Да" : "Нет";

			if (objType.IsEnum)
				return ((Enum)obj).ToDescription();

			if (objType == typeof(Guid))
				return UidToObjectName((Guid)obj);

			return obj.ToString();
		}

		string UidToObjectName(Guid uid)
		{
			if (uid == Guid.Empty)
				return "";
			var device = GKManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == uid);
			if (device != null)
				return device.PresentationName;
			var zone = GKManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == uid);
			if (zone != null)
				return zone.PresentationName;
			var guardZone = GKManager.DeviceConfiguration.GuardZones.FirstOrDefault(x => x.UID == uid);
			if (guardZone != null)
				return guardZone.PresentationName;
			var camera = ProcedureExecutionContext.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == uid);
			if (camera != null)
				return camera.PresentationName;
			var gKDoor = GKManager.Doors.FirstOrDefault(x => x.UID == uid);
			if (gKDoor != null)
				return gKDoor.PresentationName;
			var direction = GKManager.DeviceConfiguration.Directions.FirstOrDefault(x => x.UID == uid);
			if (direction != null)
				return direction.PresentationName;
			var delay = GKManager.DeviceConfiguration.Delays.FirstOrDefault(x => x.UID == uid);
			if (delay != null)
				return delay.PresentationName;
			var pumpStation = GKManager.DeviceConfiguration.PumpStations.FirstOrDefault(x => x.UID == uid);
			if (pumpStation != null)
				return pumpStation.PresentationName;
			var mpt = GKManager.DeviceConfiguration.MPTs.FirstOrDefault(x => x.UID == uid);
			if (mpt != null)
				return mpt.PresentationName;

			var organisations = ProcedureExecutionContext.GetOrganisations(ClientUID);
			var organisation = organisations == null ? null : organisations.FirstOrDefault(x => x.UID == uid);
			if (organisation != null)
				return organisation.Name;
			return "";
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
			ProcedureExecutionContext.ControlGKDevice(ClientUID, device.UID, procedureStep.ControlGKDeviceArguments.Command);
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
			if (LicenseManager.CurrentLicenseInfo.HasVideo)
			{
				var timeout = GetValue<int>(startRecordArguments.TimeoutArgument);
				switch (startRecordArguments.TimeType)
				{
					case TimeType.Millisec: timeout = (int)((double)timeout * 0.001); break;
					case TimeType.Min: timeout *= 60; break;
					case TimeType.Hour: timeout *= 3600; break;
					case TimeType.Day: timeout *= 86400; break;
				}

				if (JournalItem != null)
				{

					Guid eventUid = Guid.NewGuid();
					SetValue(startRecordArguments.EventUIDArgument, eventUid);
					ProcedureExecutionContext.StartRecord(ClientUID, cameraUid, JournalItem.UID, eventUid, timeout);
				}
			}
			else
				ProcedureExecutionContext.AddJournalItem(ClientUID, "Выполнение функции \"Начать запись\" заблокировано в связи с отсутствием лицензии", cameraUid);
		}

		private void StopRecord(ProcedureStep procedureStep)
		{
			var stopRecordArguments = procedureStep.StopRecordArguments;
			var cameraUid = GetValue<Guid>(stopRecordArguments.CameraArgument);
			var eventUid = GetValue<Guid>(stopRecordArguments.EventUIDArgument);
			if (LicenseManager.CurrentLicenseInfo.HasVideo)
				ProcedureExecutionContext.StopRecord(ClientUID, cameraUid, eventUid);
			else
				ProcedureExecutionContext.AddJournalItem(ClientUID, "Выполнение функции \"Остановить запись\" заблокировано в связи с отсутствием лицензии", cameraUid);
		}

		public void Ptz(ProcedureStep procedureStep)
		{
			var ptzArguments = procedureStep.PtzArguments;
			var cameraUid = GetValue<Guid>(ptzArguments.CameraArgument);
			var ptzNumber = GetValue<int>(ptzArguments.PtzNumberArgument);
			if (LicenseManager.CurrentLicenseInfo.HasVideo)
				ProcedureExecutionContext.Ptz(ClientUID, cameraUid, ptzNumber);
			else
				ProcedureExecutionContext.AddJournalItem(ClientUID, "Выполнение функции \"Ptz камеры\" заблокировано в связи с отсутствием лицензии", cameraUid);
		}

		public void RviAlarm(ProcedureStep procedureStep)
		{
			var rviAlarmArguments = procedureStep.RviAlarmArguments;
			var name = GetValue<string>(rviAlarmArguments.NameArgument);
			if (LicenseManager.CurrentLicenseInfo.HasVideo)
				ProcedureExecutionContext.RviAlarm(ClientUID, name);
			else
				ProcedureExecutionContext.AddJournalItem(ClientUID, "Выполнение функции \"Вызвать тревогу в RVI Оператор\" заблокировано в связи с отсутствием лицензии");
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
			if (!LicenseManager.CurrentLicenseInfo.HasFirefighting)
			{
				ProcedureExecutionContext.AddJournalItem(ClientUID, "Выполнение функции \"Управление пожарной зоной\" заблокировано в связи с отсутствием лицензии", zoneUid);
				return;
			}
			if (!HasPermission(PermissionType.Oper_Zone_Control))
			{
				ProcedureExecutionContext.AddJournalItem(ClientUID, "Выполнение функции \"Управление пожарной зоной\" заблокировано в связи с отсутствием прав пользователя", zoneUid);
				return;
			}

			ProcedureExecutionContext.ControlFireZone(ClientUID, zoneUid, zoneCommandType);
		}

		void ControlGuardZone(ProcedureStep procedureStep)
		{
			var zoneUid = GetValue<Guid>(procedureStep.ControlGKGuardZoneArguments.GKGuardZoneArgument);
			var guardZoneCommandType = procedureStep.ControlGKGuardZoneArguments.GuardZoneCommandType;
			if (!LicenseManager.CurrentLicenseInfo.HasGuard)
			{
				ProcedureExecutionContext.AddJournalItem(ClientUID, "Выполнение функции \"Управление охранной зоной\" заблокировано в связи с отсутствием лицензии", zoneUid);
				return;
			}
			if (!HasPermission(PermissionType.Oper_GuardZone_Control))
			{
				ProcedureExecutionContext.AddJournalItem(ClientUID, "Выполнение функции \"Управление охранной зоной\" заблокировано в связи с отсутствием прав пользователя", zoneUid);
				return;
			}

			ProcedureExecutionContext.ControlGuardZone(ClientUID, zoneUid, guardZoneCommandType);
		}

		void ControlDirection(ProcedureStep procedureStep)
		{
			var directionUid = GetValue<Guid>(procedureStep.ControlDirectionArguments.DirectionArgument);
			var directionCommandType = procedureStep.ControlDirectionArguments.DirectionCommandType;
			ProcedureExecutionContext.ControlDirection(ClientUID, directionUid, directionCommandType);
		}

		void ControlGKDoor(ProcedureStep procedureStep)
		{
			var doorUid = GetValue<Guid>(procedureStep.ControlGKDoorArguments.DoorArgument);
			var doorCommandType = procedureStep.ControlGKDoorArguments.DoorCommandType;
			if (!LicenseManager.CurrentLicenseInfo.HasSKD)
			{
				ProcedureExecutionContext.AddJournalItem(ClientUID, "Выполнение функции \"Управление точкой доступа ГК\" заблокировано в связи с отсутствием лицензии", doorUid);
				return;
			}
			if (!HasPermission(PermissionType.Oper_ZonesSKD))
			{
				ProcedureExecutionContext.AddJournalItem(ClientUID, "Выполнение функции \"Управление точкой доступа ГК\" заблокировано в связи с отсутствием прав пользователя", doorUid);
				return;
			}

			ProcedureExecutionContext.ControlGKDoor(ClientUID, doorUid, doorCommandType);
		}

		void ControlDelay(ProcedureStep procedureStep)
		{
			var delayUid = GetValue<Guid>(procedureStep.ControlDelayArguments.DelayArgument);
			var delayCommandType = procedureStep.ControlDelayArguments.DelayCommandType;
			ProcedureExecutionContext.ControlDelay(ClientUID, delayUid, delayCommandType);
		}

		void ControlPumpStation(ProcedureStep procedureStep)
		{
			var pumpStationUid = GetValue<Guid>(procedureStep.ControlPumpStationArguments.PumpStationArgument);
			var pumpStationCommandType = procedureStep.ControlPumpStationArguments.PumpStationCommandType;
			ProcedureExecutionContext.ControlPumpStation(ClientUID, pumpStationUid, pumpStationCommandType);
		}

		void ControlMPT(ProcedureStep procedureStep)
		{
			var mptUid = GetValue<Guid>(procedureStep.ControlMPTArguments.MPTArgument);
			var mptCommandType = procedureStep.ControlMPTArguments.MPTCommandType;
			ProcedureExecutionContext.ControlMPT(ClientUID, mptUid, mptCommandType);
		}

		void Pause(ProcedureStep procedureStep)
		{
			var pauseArguments = procedureStep.PauseArguments;
			var pause = new TimeSpan();
			switch (pauseArguments.TimeType)
			{
				case TimeType.Millisec:
					pause = TimeSpan.FromMilliseconds(GetValue<int>(pauseArguments.PauseArgument));
					break;
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
			if (AutoResetEvent.WaitOne(pause))
			{
			}
		}

		void IncrementValue(ProcedureStep procedureStep)
		{
			var incrementValueArguments = procedureStep.IncrementValueArguments;
			var variable = AllVariables.FirstOrDefault(x => x.Uid == incrementValueArguments.ResultArgument.VariableUid);
			var value = GetValue<double>(incrementValueArguments.ResultArgument);
			if (incrementValueArguments.IncrementType == IncrementType.Inc)
				ProcedureExecutionContext.SetVariableValue(variable, value + 1, ClientUID);
			else
				ProcedureExecutionContext.SetVariableValue(variable, value - 1, ClientUID);
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
			ProcedureExecutionContext.SetVariableValue(variable, itemValue, ClientUID);
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
			if (itemVariable != null && listVariable != null)
			{
				ProcedureExecutionContext.SynchronizeVariable(listVariable, ClientUID);
				if (getListItemArgument.PositionType == PositionType.First)
					ProcedureExecutionContext.SetVariableValue(itemVariable, ProcedureExecutionContext.GetValue(listVariable.ExplicitValues.FirstOrDefault(), itemVariable.ExplicitType, itemVariable.EnumType), ClientUID);
				if (getListItemArgument.PositionType == PositionType.Last)
					ProcedureExecutionContext.SetVariableValue(itemVariable, ProcedureExecutionContext.GetValue(listVariable.ExplicitValues.LastOrDefault(), itemVariable.ExplicitType, itemVariable.EnumType), ClientUID);
				if (getListItemArgument.PositionType == PositionType.ByIndex)
				{
					var indexValue = GetValue<int>(getListItemArgument.IndexArgument);
					if (listVariable.ExplicitValues.Count > indexValue)
						ProcedureExecutionContext.SetVariableValue(itemVariable, ProcedureExecutionContext.GetValue(listVariable.ExplicitValues[indexValue], itemVariable.ExplicitType, itemVariable.EnumType), ClientUID);
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
				ProcedureExecutionContext.SetVariableValue(resultVariable, value, ClientUID);
			}
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
			var value = GetValue<object>(setValueArguments.SourceArgument);
			if (setValueArguments.ExplicitType == ExplicitType.String)
				value = GetStringValue(value);
			SetValue(setValueArguments.TargetArgument, value);
		}

		void SetValue(Argument argument, object value)
		{
			ProcedureExecutionContext.SetVariableValue(AllVariables.FirstOrDefault(x => x.Uid == argument.VariableUid), value, ClientUID);
		}

		void ExportJournal(ProcedureStep procedureStep)
		{
			var arguments = procedureStep.ExportJournalArguments;
			var isExportJournal = GetValue<bool>(arguments.IsExportJournalArgument);
			var isExportPassJournal = GetValue<bool>(arguments.IsExportPassJournalArgument);
			var minDate = GetValue<DateTime>(arguments.MinDateArgument);
			var maxDate = GetValue<DateTime>(arguments.MaxDateArgument);
			var path = GetValue<string>(arguments.PathArgument);
			ProcedureExecutionContext.ExportJournal(ClientUID, isExportJournal, isExportPassJournal, minDate, maxDate, path);
		}

		void ExportOrganisation(ProcedureStep procedureStep)
		{
			var arguments = procedureStep.ExportOrganisationArguments;
			var isWithDeleted = GetValue<bool>(arguments.IsWithDeleted);
			var organisationUid = GetValue<Guid>(arguments.Organisation);
			var path = GetValue<string>(arguments.PathArgument);
			ProcedureExecutionContext.ExportOrganisation(ClientUID, isWithDeleted, organisationUid, path);
		}

		void ExportOrganisationList(ProcedureStep procedureStep)
		{
			var arguments = procedureStep.ImportOrganisationArguments;
			var isWithDeleted = GetValue<bool>(arguments.IsWithDeleted);
			var path = GetValue<string>(arguments.PathArgument);
			ProcedureExecutionContext.ExportOrganisationList(ClientUID, isWithDeleted, path);
		}

		void ExportConfiguration(ProcedureStep procedureStep)
		{
			var arguments = procedureStep.ExportConfigurationArguments;
			var isExportDevices = GetValue<bool>(arguments.IsExportDevices);
			var isExportDoors = GetValue<bool>(arguments.IsExportDoors);
			var isExportZones = GetValue<bool>(arguments.IsExportZones);
			var path = GetValue<string>(arguments.PathArgument);
			ProcedureExecutionContext.ExportConfiguration(ClientUID, isExportDevices, isExportDoors, isExportZones, path);
		}

		void ImportOrganisation(ProcedureStep procedureStep)
		{
			var arguments = procedureStep.ImportOrganisationArguments;
			var isWithDeleted = GetValue<bool>(arguments.IsWithDeleted);
			var path = GetValue<string>(arguments.PathArgument);
			ProcedureExecutionContext.ImportOrganisation(ClientUID, isWithDeleted, path);
		}

		void ImportOrganisationList(ProcedureStep procedureStep)
		{
			var arguments = procedureStep.ImportOrganisationArguments;
			var isWithDeleted = GetValue<bool>(arguments.IsWithDeleted);
			var path = GetValue<string>(arguments.PathArgument);
			ProcedureExecutionContext.ImportOrganisationList(ClientUID, isWithDeleted, path);
		}

		T GetValue<T>(Argument argument)
		{
			var result = argument.VariableScope == VariableScope.ExplicitValue ?
				ProcedureExecutionContext.GetValue(argument.ExplicitValue, argument.ExplicitType, argument.EnumType) :
				ProcedureExecutionContext.GetVariableValue(ClientUID, AllVariables.FirstOrDefault(x => x.Uid == argument.VariableUid));
			if (result is string && typeof(T) == typeof(Guid))
				result = CheckGuid(result.ToString()) ? new Guid(result.ToString()) : Guid.Empty;
			if (result is int && typeof(T) == typeof(double))
				result = Convert.ToDouble(result);
			if (result is double && typeof(T) == typeof(int))
				result = Convert.ToInt32(Math.Round((double)result));
			return (T)result;
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
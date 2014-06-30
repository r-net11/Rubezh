using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using FiresecAPI.GK;
using FiresecClient;
using GKProcessor;
using FiresecAPI.Events;

namespace FiresecService.Processor
{
	public static class AutomationProcessorRunner
	{
		public static bool Run(Procedure procedure, List<Argument> arguments)
		{
			try
			{
				foreach (var step in procedure.Steps)
				{
					RunStep(step, procedure);
				}
			}
			catch
			{
				return false;
			}
			return true;
		}

		static void RunStep(ProcedureStep procedureStep, Procedure procedure)
		{
			switch(procedureStep.ProcedureStepType)
			{
				case ProcedureStepType.If:
					if (true)
					{
						foreach(var childStep in procedureStep.Children[0].Children)
						{
							RunStep(childStep, procedure);
						}
					}
					else
					{
						foreach (var childStep in procedureStep.Children[1].Children)
						{
							RunStep(childStep, procedure);
						}
					}
					break;

				case ProcedureStepType.Foreach:
					while (true)
					{
						foreach (var childStep in procedureStep.Children[0].Children)
						{
							RunStep(childStep, procedure);
						}
					}
					break;

				case ProcedureStepType.PlaySound:
					break;

				case ProcedureStepType.SendMessage:
					GKProcessorManager.AddGKMessage(GlobalEventNameEnum.Команда_оператора, "Запуск процедуры", null, null);
					break;

				case ProcedureStepType.FindObjects:
					FindObjects(procedureStep, procedure);
					break;
			}
		}

		static void FindObjects(ProcedureStep procedureStep, Procedure procedure)
		{
			var findObjectArguments = procedureStep.FindObjectArguments;
			var variable = procedure.Variables.FirstOrDefault(x => x.Uid == findObjectArguments.ResultUid);
			if (findObjectArguments.JoinOperator == JoinOperator.Or)
				FindObjectsOr(variable, findObjectArguments.FindObjectConditions);
			else
				FindObjectsAnd(variable, findObjectArguments.FindObjectConditions);
		}

		static void InitializeProperties(ref int intPropertyValue, ref string stringPropertyValue, ref Guid itemUid, FindObjectCondition findObjectCondition, object item)
		{
			if (item is XDevice)
			{
				switch (findObjectCondition.DevicePropertyType)
				{
					case DevicePropertyType.ShleifNo:
						intPropertyValue = (item as XDevice).ShleifNo;
						break;
					case DevicePropertyType.IntAddress:
						intPropertyValue = (item as XDevice).IntAddress;
						break;
					case DevicePropertyType.DeviceState:
						intPropertyValue = (int)(item as XDevice).State.StateClass;
						break;
					case DevicePropertyType.Name:
						stringPropertyValue = (item as XDevice).PresentationName.Trim();
						break;
				}
				itemUid = (item as XDevice).UID;
			}

			if (item is XZone)
			{
				switch (findObjectCondition.ZonePropertyType)
				{
					case ZonePropertyType.No:
						intPropertyValue = (item as XZone).No;
						break;
					case ZonePropertyType.ZoneType:
						intPropertyValue = (int)(item as XZone).ObjectType;
						break;
					case ZonePropertyType.Name:
						stringPropertyValue = (item as XZone).Name.Trim();
						break;
				}
				itemUid = (item as XZone).UID;
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
					InitializeProperties(ref intPropertyValue, ref stringPropertyValue, ref itemUid, findObjectCondition, item);
					if (resultObjects.Contains(item))
						continue;
					if (((findObjectCondition.PropertyType == PropertyType.Integer) &&
						(((findObjectCondition.ConditionType == ConditionType.IsEqual) && (intPropertyValue == findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsNotEqual) && (intPropertyValue != findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsMore) && (intPropertyValue > findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsNotMore) && (intPropertyValue <= findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsLess) && (intPropertyValue < findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsNotLess) && (intPropertyValue >= findObjectCondition.IntValue)))) ||
						((findObjectCondition.PropertyType == PropertyType.String) &&
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
					InitializeProperties(ref intPropertyValue, ref stringPropertyValue, ref itemUid, findObjectCondition, item);
					if (((findObjectCondition.PropertyType == PropertyType.Integer) &&
						(((findObjectCondition.ConditionType == ConditionType.IsEqual) && (intPropertyValue != findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsNotEqual) && (intPropertyValue == findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsMore) && (intPropertyValue <= findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsNotMore) && (intPropertyValue > findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsLess) && (intPropertyValue >= findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsNotLess) && (intPropertyValue < findObjectCondition.IntValue)))) ||
						((findObjectCondition.PropertyType == PropertyType.String) &&
						(((findObjectCondition.StringConditionType == StringConditionType.StartsWith) && (!stringPropertyValue.StartsWith(findObjectCondition.StringValue))) ||
						((findObjectCondition.StringConditionType == StringConditionType.EndsWith) && (!stringPropertyValue.EndsWith(findObjectCondition.StringValue))) ||
						((findObjectCondition.StringConditionType == StringConditionType.Contains) && (!stringPropertyValue.Contains(findObjectCondition.StringValue))))))
					{ tempObjects.Remove(item); result.ObjectsUids.Remove(itemUid); }
				}
				resultObjects = new List<object>(tempObjects);
			}
		}

	}
}
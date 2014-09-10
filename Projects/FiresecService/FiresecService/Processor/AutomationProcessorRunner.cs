using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FiresecAPI.Automation;
using FiresecAPI.Journal;
using GKProcessor;
using FiresecAPI;
using System;
using VariableScope = FiresecAPI.Automation.VariableScope;

namespace FiresecService.Processor
{
	public static class AutomationProcessorRunner
	{
		public static List<Thread> ProceduresThreads { get; private set; }

		static AutomationProcessorRunner()
		{
			ProceduresThreads = new List<Thread>();

		}

		public static bool RunInThread(Procedure procedure, List<Argument> arguments)
		{
			try
			{
				if (procedure.Steps.Any(step => !RunStep(step, procedure, arguments)))
				{
					return true;
				}
			}
			catch
			{
				return false;
			}
			return true;
		}

		public static bool Run(Procedure procedure, List<Argument> arguments)
		{
			procedure.ResetVariables(arguments);
			var procedureThread = new Thread(() => RunInThread(procedure, arguments));
			procedureThread.Start();
			ProceduresThreads.Add(procedureThread);
			ProceduresThreads = new List<Thread>(ProceduresThreads.FindAll(x => x.IsAlive));
			return true;
		}

		static bool RunStep(ProcedureStep procedureStep, Procedure procedure, List<Argument> arguments)
		{
			switch (procedureStep.ProcedureStepType)
			{
				case ProcedureStepType.If:
					if (ProcedureHelper.Compare(procedureStep, procedure, arguments))
					{
						if (procedureStep.Children[0].Children.Any(childStep => !RunStep(childStep, procedure, arguments)))
						{
							return false;
						}
					}
					else
					{
						if (procedureStep.Children[1].Children.Any(childStep => !RunStep(childStep, procedure, arguments)))
						{
							return false;
						}
					}
					break;

				case ProcedureStepType.Arithmetics:
					ProcedureHelper.Calculate(procedureStep, procedure, arguments);
					break;

				case ProcedureStepType.Foreach:
					var variablesAndArguments = new List<Variable>(procedure.Variables);
					variablesAndArguments.AddRange(procedure.Arguments);
					var foreachArguments = procedureStep.ForeachArguments;
					var listVariable = variablesAndArguments.FirstOrDefault(x => x.Uid == foreachArguments.ListVariable.VariableUid) ??
						procedure.Arguments.FirstOrDefault(x => x.Uid == foreachArguments.ListVariable.VariableUid);
					var itemVariable = variablesAndArguments.FirstOrDefault(x => x.Uid == foreachArguments.ItemVariable.VariableUid) ??
						procedure.Arguments.FirstOrDefault(x => x.Uid == foreachArguments.ItemVariable.VariableUid);
					foreach (var itemUid in listVariable.VariableItems.Select(x => x.UidValue))
					{
						itemVariable.VariableItem.UidValue = itemUid;
						if (procedureStep.Children[0].Children.Any(childStep => !RunStep(childStep, procedure, arguments)))
							return false;
					}
					break;

				case ProcedureStepType.PlaySound:
					var automationCallbackResult = new AutomationCallbackResult();
					automationCallbackResult.SoundUID = procedureStep.SoundArguments.SoundUid;
					automationCallbackResult.AutomationCallbackType = AutomationCallbackType.Sound;
					Service.FiresecService.NotifyAutomation(automationCallbackResult);
					break;

				case ProcedureStepType.Pause:
					Thread.Sleep(TimeSpan.FromSeconds(procedureStep.PauseArguments.Pause.VariableItem.IntValue));
					break;

				case ProcedureStepType.AddJournalItem:
					var journalItem = new JournalItem();
					journalItem.SystemDateTime = DateTime.Now;
					journalItem.DeviceDateTime = DateTime.Now;
					journalItem.JournalEventNameType = JournalEventNameType.Сообщение_автоматизации;
					//journalItem.DescriptionText = procedureStep.JournalArguments.Message;
					Service.FiresecService.AddCommonJournalItem(journalItem);
					break;

				case ProcedureStepType.ShowMessage:
					automationCallbackResult = ProcedureHelper.ShowMessage(procedureStep, procedure);
					automationCallbackResult.AutomationCallbackType = AutomationCallbackType.Message;
					Service.FiresecService.NotifyAutomation(automationCallbackResult);
					break;

				case ProcedureStepType.FindObjects:
					ProcedureHelper.FindObjects(procedureStep, procedure);
					break;

				case ProcedureStepType.ControlGKDevice:
					ProcedureHelper.ControlGKDevice(procedureStep);
					break;

				case ProcedureStepType.ControlCamera:
					ProcedureHelper.ControlCamera(procedureStep);
					break;

				case ProcedureStepType.ControlGKFireZone:
					ProcedureHelper.ControlFireZone(procedureStep);
					break;

				case ProcedureStepType.ControlGKGuardZone:
					ProcedureHelper.ControlGuardZone(procedureStep);
					break;

				case ProcedureStepType.ControlDirection:
					ProcedureHelper.ControlDirection(procedureStep);
					break;

				case ProcedureStepType.ControlDoor:
					ProcedureHelper.ControlDoor(procedureStep);
					break;

				case ProcedureStepType.ProcedureSelection:
					{
						var childProcedure = ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.Procedures.
								FirstOrDefault(x => x.Uid == procedureStep.ProcedureSelectionArguments.ScheduleProcedure.ProcedureUid);
						Run(childProcedure, procedureStep.ProcedureSelectionArguments.ScheduleProcedure.Arguments);
					}
					break;

				case ProcedureStepType.IncrementValue:
					ProcedureHelper.IncrementValue(procedureStep);
					break;

				case ProcedureStepType.SetValue:
					ProcedureHelper.SetValue(procedureStep);
					break;

				case ProcedureStepType.Exit:
					return false;

			}
			return true;
		}
	}
}
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FiresecAPI.Automation;
using FiresecAPI.Journal;
using GKProcessor;
using FiresecAPI;
using System;

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
				foreach (var step in procedure.Steps)
				{
					RunStep(step, procedure, arguments);
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
			var procedureThread = new Thread(() => RunInThread(procedure, arguments));
			procedureThread.Start();
			ProceduresThreads.Add(procedureThread);
			ProceduresThreads = new List<Thread>(ProceduresThreads.FindAll(x => x.IsAlive));
			return true;
		}

		static void RunStep(ProcedureStep procedureStep, Procedure procedure, List<Argument> arguments)
		{
			switch(procedureStep.ProcedureStepType)
			{
				case ProcedureStepType.If:
					if (true)
					{
						foreach(var childStep in procedureStep.Children[0].Children)
						{
							RunStep(childStep, procedure, arguments);
						}
					}
					else
					{
						foreach (var childStep in procedureStep.Children[1].Children)
						{
							RunStep(childStep, procedure, arguments);
						}
					}
					break;
				
				case ProcedureStepType.Arithmetics:
					ProcedureHelper.Calculate(procedureStep, procedure, arguments);
					break;

				case ProcedureStepType.Foreach:
					while (true)
					{
						foreach (var childStep in procedureStep.Children[0].Children)
						{
							RunStep(childStep, procedure, arguments);
						}
					}
					break;

				case ProcedureStepType.PlaySound:
					var automationCallbackResult = new AutomationCallbackResult();
					automationCallbackResult.SoundUID = Guid.Empty;
					automationCallbackResult.AutomationCallbackType = AutomationCallbackType.Sound;
					Service.FiresecService.NotifyAutomation(automationCallbackResult);
					break;

				case ProcedureStepType.AddJournalItem:
					var journalItem = new JournalItem();
					journalItem.SystemDateTime = DateTime.Now;
					journalItem.DeviceDateTime = DateTime.Now;
					journalItem.JournalEventNameType = JournalEventNameType.Сообщение_автоматизации;
					journalItem.DescriptionText = procedureStep.JournalArguments.Message;
					Service.FiresecService.AddCommonJournalItem(journalItem);
					break;

				case ProcedureStepType.SendMessage:
					automationCallbackResult = new AutomationCallbackResult();
					automationCallbackResult.Message = "Запуск процедуры";
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
					foreach (var scheduleProcedure in procedureStep.ProcedureSelectionArguments.ScheduleProcedures)
					{
						var childProcedure = ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.Procedures.
							FirstOrDefault(x => x.Uid == scheduleProcedure.ProcedureUid);
						Run(childProcedure, scheduleProcedure.Arguments);
					}
				}
				break;
					
			}
		}
	}
}
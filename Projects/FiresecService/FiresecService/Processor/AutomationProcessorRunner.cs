using System.Collections.Generic;
using System.Threading;
using FiresecAPI.Automation;
using FiresecAPI.Journal;
using GKProcessor;

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
					RunStep(step, procedure);
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
					GKProcessorManager.AddGKMessage(JournalEventNameType.Команда_оператора, "Запуск процедуры", null, null);
					break;

				case ProcedureStepType.FindObjects:
					ProcedureHelper.FindObjects(procedureStep, procedure);
					break;
			}
		}
	}
}
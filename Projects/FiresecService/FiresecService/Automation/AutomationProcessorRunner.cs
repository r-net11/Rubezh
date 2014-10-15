using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using FiresecAPI.Automation;
using FiresecAPI.Journal;

namespace FiresecService.Processor
{
	public static class AutomationProcessorRunner
	{
		public static List<ProcedureThread> ProceduresThreads { get; private set; }

		static AutomationProcessorRunner()
		{
			ProceduresThreads = new List<ProcedureThread>();
			AutoResetEvent = new AutoResetEvent(false);
			new Thread(CheckProcedureThread).Start();
		}

		public static void RunOnJournal(JournalItem journalItem)
		{
			foreach (var procedure in ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.Procedures)
			{
				foreach (var filtersUID in procedure.FiltersUids)
				{
					var filter = ConfigurationCashHelper.SystemConfiguration.JournalFilters.FirstOrDefault(x => x.UID == filtersUID);
					if (filter != null)
					{
						if (filter.JournalSubsystemTypes.Count > 0 && !filter.JournalSubsystemTypes.Contains(journalItem.JournalSubsystemType))
							continue;
						if (filter.JournalEventNameTypes.Count > 0 && !filter.JournalEventNameTypes.Contains(journalItem.JournalEventNameType))
							continue;
						if (filter.JournalEventDescriptionTypes.Count > 0 && !filter.JournalEventDescriptionTypes.Contains(journalItem.JournalEventDescriptionType))
							continue;
						if (filter.JournalObjectTypes.Count > 0 && !filter.JournalObjectTypes.Contains(journalItem.JournalObjectType))
							continue;
						if (filter.ObjectUIDs.Count > 0 && !filter.ObjectUIDs.Contains(journalItem.ObjectUID))
							continue;
						Run(procedure, new List<Argument>(), null, null);
					}
				}
			}
		}

		public static void RunOnStateChanged()
		{
			foreach (var procedure in ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.Procedures)
			{
			}
		}

		public static ProcedureThread Run(Procedure procedure, List<Argument> arguments, Procedure callingProcedure, List<Variable> globalVariables)
		{
			var procedureThread = new ProcedureThread(procedure, arguments, callingProcedure, Run);
			procedureThread.Start();
			ProceduresThreads.Add(procedureThread);
			return procedureThread;
		}

		static AutoResetEvent AutoResetEvent { get; set; }
		static void CheckProcedureThread()
		{
			while (true)
			{
				if (AutoResetEvent.WaitOne(TimeSpan.FromSeconds(1)))
				{
					return;
				}
				ProceduresThreads = new List<ProcedureThread>(ProceduresThreads.FindAll(x => x.IsAlive));
				foreach (var procedureThread in ProceduresThreads)
				{
					if ((procedureThread.TimeOut > 0) && ((int) ((DateTime.Now - procedureThread.StartTime).TotalSeconds) >= procedureThread.TimeOut))
					{
						procedureThread.IsTimeOut = true;
					}
				}
			}
		}
	}
}
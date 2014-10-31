using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FiresecAPI.Automation;
using FiresecAPI.Journal;
using FiresecAPI.Models;

namespace FiresecService
{
	public static class ProcedureRunner
	{
		public static List<ProcedureThread> ProceduresThreads { get; private set; }

		static ProcedureRunner()
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
						Run(procedure, new List<Argument>(), null, null, null, journalItem);
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

		public static ProcedureThread Run(Procedure procedure, List<Argument> arguments, List<Variable> callingProcedureVariables, List<Variable> globalVariables, User user = null, JournalItem journalItem = null, Guid? clientUID = null)
		{
			var procedureThread = new ProcedureThread(procedure, arguments, callingProcedureVariables, journalItem, user, clientUID);
			ProceduresThreads.Add(procedureThread);
			procedureThread.Start();
			return procedureThread;
		}

		public static void Stop()
		{
			foreach (var procedureThread in ProceduresThreads)
			{
				procedureThread.IsTimeOut = true;
			}
		}

		public static void SetNewConfig()
		{
			Stop();
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
				var timeOut = new TimeSpan();
				foreach (var procedureThread in ProceduresThreads)
				{
					switch (procedureThread.TimeType)
					{
						case TimeType.Sec:
							timeOut = TimeSpan.FromSeconds(procedureThread.TimeOut);
							break;
						case TimeType.Min:
							timeOut = TimeSpan.FromMinutes(procedureThread.TimeOut);
							break;
						case TimeType.Hour:
							timeOut = TimeSpan.FromHours(procedureThread.TimeOut);
							break;
						case TimeType.Day:
							timeOut = TimeSpan.FromDays(procedureThread.TimeOut);
							break;
					}
					if ((procedureThread.TimeOut > 0) && (DateTime.Now - procedureThread.StartTime >= timeOut))
					{
						procedureThread.IsTimeOut = true;
					}
				}
			}
		}
	}
}
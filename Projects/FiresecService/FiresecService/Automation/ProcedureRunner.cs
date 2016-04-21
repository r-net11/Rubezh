using FiresecAPI.Automation;
using FiresecAPI.Journal;
using FiresecAPI.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FiresecAPI.Models.Automation;
using FiresecService.Service;

namespace FiresecService
{
	public static class ProcedureRunner
	{
		private static readonly ConcurrentDictionary<Guid, ProcedureThread> ProceduresThreads;
		private static readonly AutoResetEvent ResetEvent;
		private static readonly Thread CheckThread;

		static ProcedureRunner()
		{
			ProceduresThreads = new ConcurrentDictionary<Guid, ProcedureThread>();
			ResetEvent = new AutoResetEvent(false);
			CheckThread = new Thread(CheckProcedureThread)
			{
				Name = "CheckProcedureThread",
			};
			CheckThread.Start();
		}

		public static void RunOnJournal(JournalItem journalItem)
		{
			// Оффлайн события не обрабатываем
			if (journalItem.JournalItemType == JournalItemType.Offline)
				return;

			foreach (var procedure in ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.Procedures)
			{
				foreach (var filtersUID in procedure.FiltersUids)
				{
					var filter = ConfigurationCashHelper.SystemConfiguration.JournalFilters.FirstOrDefault(x => x.UID == filtersUID);
					if (filter != null)
					{
						if (filter.JournalSubsystemTypes.Any() && !filter.JournalSubsystemTypes.Contains(journalItem.JournalSubsystemType))
							continue;
						if (filter.JournalEventNameTypes.Any() && !filter.JournalEventNameTypes.Contains(journalItem.JournalEventNameType))
							continue;
						if (filter.JournalEventDescriptionTypes.Any() && !filter.JournalEventDescriptionTypes.Contains(journalItem.JournalEventDescriptionType))
							continue;
						if (filter.JournalObjectTypes.Any() && !filter.JournalObjectTypes.Contains(journalItem.JournalObjectType))
							continue;
						if (filter.ObjectUIDs.Any() && !filter.ObjectUIDs.Contains(journalItem.ObjectUID))
							continue;
						Run(procedure, new List<Argument>(), null, null, journalItem);
					}
				}
			}
		}

		public static void RunOnServerRun()
		{
			FiresecServiceManager.SafeFiresecService.ResetGlobalVariables();
			ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.Procedures.ForEach(x => { if (x.StartWithServer) Run(x, new List<Argument>(), null); });
		}

		public static ProcedureThread Run(Procedure procedure, List<Argument> arguments, List<IVariable> callingProcedureVariables, User user = null, JournalItem journalItem = null, Guid? clientUID = null)
		{
			var procedureThread = new ProcedureThread(procedure, arguments, callingProcedureVariables, journalItem, user, clientUID);
			ProceduresThreads.TryAdd(procedureThread.UID, procedureThread);
			procedureThread.Start();
			return procedureThread;
		}

		public static void Stop()
		{
			foreach (var procedureThread in ProceduresThreads.Values)
				procedureThread.IsTimeOut = true;
		}

		public static void Terminate()
		{
			if (ResetEvent == null) return;

			ResetEvent.Set();
			CheckThread.Join(TimeSpan.FromMinutes(1));
		}

		public static void SetNewConfig()
		{
			Stop();
		}

		private static void CheckProcedureThread()
		{
			while (true)
			{
				if (ResetEvent.WaitOne(TimeSpan.FromSeconds(1)))
					return;
				var oldProcedures = ProceduresThreads.Where(item => !item.Value.IsAlive).Select(item => item.Key).ToArray();
				ProcedureThread procedure;
				foreach (var procedureUID in oldProcedures)
					ProceduresThreads.TryRemove(procedureUID, out procedure);
				var timeOut = new TimeSpan();
				foreach (var procedureThread in ProceduresThreads.Values)
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
						procedureThread.IsTimeOut = true;
				}
			}
		}
	}
}
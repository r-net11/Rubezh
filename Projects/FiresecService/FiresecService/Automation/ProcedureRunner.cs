using FiresecAPI.Automation;
using FiresecAPI.Journal;
using FiresecAPI.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace FiresecService
{
	public static class ProcedureRunner
	{
		private static ConcurrentDictionary<Guid, ProcedureThread> _proceduresThreads;
		private static AutoResetEvent _resetEvent;
		private static Thread _checkThread;
		private static object _lock;

		static ProcedureRunner()
		{
			_lock = new object();
			_proceduresThreads = new ConcurrentDictionary<Guid, ProcedureThread>();
			_resetEvent = new AutoResetEvent(false);
			_checkThread = new Thread(CheckProcedureThread)
			{
				Name = "CheckProcedureThread",
			};
			_checkThread.Start();
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
						Run(procedure, new List<Argument>(), null, null, journalItem);
					}
				}
			}
		}

		public static void RunOnServerRun()
		{
			ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.Procedures.ForEach(x => { if (x.StartWithServer) Run(x, new List<Argument>(), null, null); });
		}

		public static void RunOnStateChanged()
		{
			foreach (var procedure in ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.Procedures)
			{
			}
		}

		public static ProcedureThread Run(Procedure procedure, List<Argument> arguments, List<Variable> callingProcedureVariables, User user = null, JournalItem journalItem = null, Guid? clientUID = null)
		{
			var procedureThread = new ProcedureThread(procedure, arguments, callingProcedureVariables, journalItem, user, clientUID);
			_proceduresThreads.TryAdd(procedureThread.UID, procedureThread);
			procedureThread.Start();
			return procedureThread;
		}

		public static void Stop()
		{
			foreach (var procedureThread in _proceduresThreads.Values)
			{
				procedureThread.IsTimeOut = true;
			}
		}

		public static void Terminate()
		{
			if (_resetEvent == null) return;

			_resetEvent.Set();
			_checkThread.Join(TimeSpan.FromMinutes(1));
		}

		public static void SetNewConfig()
		{
			Stop();
		}

		private static void CheckProcedureThread()
		{
			while (true)
			{
				if (_resetEvent.WaitOne(TimeSpan.FromSeconds(1)))
					return;
				var oldProcedures = _proceduresThreads.Where(item => !item.Value.IsAlive).Select(item => item.Key).ToArray();
				ProcedureThread procedure;
				foreach (var procedureUID in oldProcedures)
					_proceduresThreads.TryRemove(procedureUID, out procedure);
				var timeOut = new TimeSpan();
				foreach (var procedureThread in _proceduresThreads.Values)
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
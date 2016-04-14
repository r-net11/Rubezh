using RubezhAPI.Automation;
using RubezhAPI.Journal;
using RubezhAPI.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Infrastructure.Automation
{
	public static class AutomationProcessor
	{
		static ConcurrentDictionary<Guid, ProcedureThread> _procedureThreads;
		static AutoResetEvent _resetEvent;
		static Thread _checkThread;
		static List<OpcDaTagFilter> _opcDaTagFilters;

		public static void RunOnJournal(JournalItem journalItem, User user, Guid? clientUID)
		{
			if (ProcedureExecutionContext.SystemConfiguration == null)
				return;

			foreach (var procedure in ProcedureExecutionContext.SystemConfiguration.AutomationConfiguration.Procedures.Where(x => x.ContextType == ProcedureExecutionContext.ContextType))
			{
				foreach (var filtersUID in procedure.FiltersUids)
				{
					var filter = ProcedureExecutionContext.SystemConfiguration.JournalFilters.FirstOrDefault(x => x.UID == filtersUID);
					if (filter != null)
					{
						if (filter.JournalSubsystemTypes.Count +
							filter.JournalEventNameTypes.Count +
							filter.JournalEventDescriptionTypes.Count +
							filter.JournalObjectTypes.Count +
							filter.ObjectUIDs.Count == 0)
							continue;

						if (filter.JournalSubsystemTypes.Count > 0 && !filter.JournalSubsystemTypes.Contains(journalItem.JournalSubsystemType))
							continue;
						if ((filter.JournalEventNameTypes.Count > 0 && !filter.JournalEventNameTypes.Contains(journalItem.JournalEventNameType)) ||
							(filter.JournalEventDescriptionTypes.Count > 0 && !filter.JournalEventDescriptionTypes.Contains(journalItem.JournalEventDescriptionType)))
							continue;
						if (filter.JournalObjectTypes.Count > 0 && !filter.JournalObjectTypes.Contains(journalItem.JournalObjectType))
							continue;
						if (filter.ObjectUIDs.Count > 0 && !filter.ObjectUIDs.Contains(journalItem.ObjectUID))
							continue;
						RunProcedure(procedure, new List<Argument>(), null, user, journalItem, clientUID);
					}
				}
			}
		}

		public static void RunOnOpcTagFilters(User user, Guid? clientUID)
		{
			if (ProcedureExecutionContext.SystemConfiguration == null)
				return;

			var filters = ProcedureExecutionContext.SystemConfiguration.AutomationConfiguration.Procedures
				.Where(x => x.ContextType == ProcedureExecutionContext.ContextType)
				.SelectMany(x => x.OpcDaTagFiltersUids).Distinct();

			foreach (var filter in filters)
			{
				var opcTagFilter = _opcDaTagFilters.FirstOrDefault(x => x.UID == filter);

				if (opcTagFilter != null)
				{
					var procedures = ProcedureExecutionContext.SystemConfiguration.AutomationConfiguration.Procedures
						.Where(x => x.ContextType == ProcedureExecutionContext.ContextType
							&& x.OpcDaTagFiltersUids.Contains(opcTagFilter.UID));

					var value = OpcDaHelper.GetTagValue(opcTagFilter.TagUID);

					if (opcTagFilter.CheckCondition(value))
					{
						foreach (var procedure in procedures)
						{
							RunProcedure(procedure, new List<Argument>(), null, user, null, clientUID);
						}
					}
				}
			}
		}

		public static void RunOnApplicationRun(User user = null, Guid? clientUID = null)
		{
			ProcedureExecutionContext.SystemConfiguration.AutomationConfiguration.Procedures.ForEach(x =>
			{
				if (x.StartWithApplication && x.ContextType == ProcedureExecutionContext.ContextType)
					RunProcedure(x, new List<Argument>(), null, user, null, clientUID);
			});
		}

		public static void RunProcedure(Procedure procedure, List<Argument> arguments, List<Variable> callingProcedureVariables, User user = null, JournalItem journalItem = null, Guid? clientUID = null)
		{
			if (procedure.IsActive && _procedureThreads != null)
			{
				var procedureThread = new ProcedureThread(procedure, arguments, callingProcedureVariables, journalItem, user, clientUID);
				_procedureThreads.TryAdd(procedureThread.UID, procedureThread);
				procedureThread.Start();
			}
		}

		public static void Start()
		{
			ProcedureExecutionContext.InitializeGlobalVariables();

			_procedureThreads = new ConcurrentDictionary<Guid, ProcedureThread>();
			_resetEvent = new AutoResetEvent(false);

			_opcDaTagFilters = new List<OpcDaTagFilter>();
			foreach (var filter in ProcedureExecutionContext.SystemConfiguration.AutomationConfiguration.OpcDaTagFilters)
			{
				_opcDaTagFilters.Add(new OpcDaTagFilter(filter.UID, filter.Name, filter.Description,
					filter.TagUID, filter.Hysteresis, filter.ValueType));
			}

			_checkThread = new Thread(CheckProcedureThread)
			{
				Name = "CheckProcedureThread",
			};
			_checkThread.Start();
		}

		public static void Stop()
		{
			_opcDaTagFilters.Clear();

			foreach (var procedureThread in _procedureThreads.Values)
			{
				procedureThread.IsTimeOut = true;
			}
			Terminate();
		}

		public static void Terminate()
		{
			if (_resetEvent != null)
			{
				_resetEvent.Set();
				_checkThread.Join(TimeSpan.FromMinutes(1));
			}
		}

		public static void SetNewConfig()
		{
			Stop();
			Start();
			RunOnApplicationRun();
		}

		static void CheckProcedureThread()
		{
			while (true)
			{
				if (_resetEvent.WaitOne(TimeSpan.FromSeconds(1)))
					return;
				var oldProcedures = _procedureThreads.Where(item => !item.Value.IsAlive).Select(item => item.Key).ToArray();
				ProcedureThread procedure;
				foreach (var procedureUID in oldProcedures)
					_procedureThreads.TryRemove(procedureUID, out procedure);
				var timeOut = new TimeSpan();
				foreach (var procedureThread in _procedureThreads.Values)
				{
					switch (procedureThread.TimeType)
					{
						case TimeType.Millisec:
							timeOut = TimeSpan.FromMilliseconds(procedureThread.TimeOut);
							break;
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
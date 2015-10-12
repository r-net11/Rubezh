using System;
using System.Linq;
using RubezhAPI.GK;
using RubezhAPI.Journal;
using RubezhClient;
using RubezhDAL;

namespace GKProcessor
{
	public partial class Watcher
	{
		int LastId = -1;

		void PingJournal()
		{
			using (var gkLifecycleManager = new GKLifecycleManager(GkDatabase.RootDevice, "Проверка журнала"))
			{
				var newLastId = GetLastId();
				if (newLastId == -1)
					return;
				if (LastId == -1)
					LastId = newLastId;
				if (newLastId > LastId)
				{
					for (int index = LastId + 1; index <= newLastId; index++)
					{
						gkLifecycleManager.Progress(index - LastId, newLastId - LastId);
						ReadAndPublish(index);
					}
					LastId = newLastId;

					gkLifecycleManager.AddItem("Изменение индекса в БД");
					using (var skdDatabaseService = new RubezhDAL.DataClasses.DbService())
					{
						var gkIpAddress = GKManager.GetIpAddress(GkDatabase.RootDevice);
						skdDatabaseService.GKMetadataTranslator.SetLastJournalNo(gkIpAddress, LastId);
					}
				}
			}
		}

		int GetLastId()
		{
			var sendResult = SendManager.Send(GkDatabase.RootDevice, 0, 6, 64);
			if (IsStopping)
				return -1;
			if (sendResult.HasError)
			{
				ConnectionChanged(false);
				return -1;
			}
			ConnectionChanged(true);
			var journalParser = new JournalParser(GkDatabase.RootDevice, sendResult.Bytes);
			return journalParser.GKJournalRecordNo;
		}

		JournalParser ReadJournal(int index)
		{
			LastUpdateTime = DateTime.Now;
			if (IsStopping)
				return null;
			var data = BitConverter.GetBytes(index).ToList();
			var sendResult = SendManager.Send(GkDatabase.RootDevice, 4, 7, 64, data);
			if (sendResult.HasError)
			{
				ConnectionChanged(false);
				return null;
			}
			if (sendResult.Bytes.Count == 64)
			{
				var journalParser = new JournalParser(GkDatabase.RootDevice, sendResult.Bytes);
#if !DEBUG
				if (journalParser.JournalItem.JournalObjectType == JournalObjectType.GKPim)
					return null;
#endif
				return journalParser;
			}
			return null;
		}

		void ReadAndPublish(int index)
		{
			var journalParser = ReadJournal(index);
			if (journalParser != null)
			{
				AddJournalItem(journalParser.JournalItem);
				var descriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.GetDescriptorNo() == journalParser.GKObjectNo);
				if (descriptor != null)
				{
					CheckServiceRequired(descriptor.GKBase, journalParser.JournalItem);
					if (journalParser.JournalSourceType == JournalSourceType.Object)
					{
						descriptor.GKBase.InternalState.StateBits = GKStatesHelper.StatesFromInt(journalParser.ObjectState);
					}
					if (descriptor.GKBase.InternalState.StateClass == XStateClass.On)
					{
						descriptor.GKBase.InternalState.ZeroHoldDelayCount = 0;
						CheckDelay(descriptor.GKBase);
					}
					ParseAdditionalStates(journalParser);
					OnObjectStateChanged(descriptor.GKBase);
					if (descriptor.GKBase is GKDevice)
					{
						GKDevice device = descriptor.GKBase as GKDevice;
						foreach(var parent in device.AllParents)
						{
							if(parent.Driver.IsGroupDevice || parent.DriverType == GKDriverType.RSR2_KAU_Shleif || parent.DriverType == GKDriverType.RSR2_MVP_Part)
								OnObjectStateChanged(parent);
						}
					}
				}

				if (journalParser.JournalItem.JournalEventNameType == JournalEventNameType.Перевод_в_технологический_режим || journalParser.JournalItem.JournalEventNameType == JournalEventNameType.Перевод_в_рабочий_режим)
				{
					MustCheckTechnologicalRegime = true;
					LastTechnologicalRegimeCheckTime = DateTime.Now;
					TechnologicalRegimeCheckCount = 0;

					CheckTechnologicalRegime();
					NotifyAllObjectsStateChanged();
				}
			}
		}

		void CheckServiceRequired(GKBase gkBase, JournalItem journalItem)
		{
			if (journalItem.JournalEventNameType == JournalEventNameType.Запыленность || journalItem.JournalEventNameType == JournalEventNameType.Запыленность_устранена)
			{
				if (gkBase is GKDevice)
				{
					var device = gkBase as GKDevice;
					if (journalItem.JournalEventNameType == JournalEventNameType.Запыленность)
						device.InternalState.IsService = true;
					if (journalItem.JournalEventNameType == JournalEventNameType.Запыленность_устранена)
						device.InternalState.IsService = false;
				}
			}
		}
	}
}
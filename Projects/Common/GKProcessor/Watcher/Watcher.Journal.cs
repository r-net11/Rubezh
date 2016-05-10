using System;
using System.Linq;
using RubezhAPI.GK;
using RubezhAPI.Journal;
using RubezhAPI;
using RubezhDAL;

namespace GKProcessor
{
	public partial class Watcher
	{
		int LastId = -1;
		int LastKauId = -1;
		string IpAddress = "";
		bool IsFirstTimeReadJournal = true;
		bool isWrite;

		void PingJournal()
		{
			JournalParser journalParser;
			if (IsFirstTimeReadJournal && !String.IsNullOrEmpty(GkDatabase.RootDevice.GetReservedIpAddress())) // Находим последнее событие на КАУ первый раз (при запуске сервера)
			{
				journalParser = GetKauJournalById(-1);
				if (journalParser != null)
					LastKauId = journalParser.KauJournalRecordNo;
				IsFirstTimeReadJournal = false;
			}
			using (var gkLifecycleManager = new GKLifecycleManager(GkDatabase.RootDevice, "Проверка журнала"))
			{
				if (IpAddress != GkDatabase.RootDevice.GetGKIpAddress())
				{
					if (!String.IsNullOrEmpty(IpAddress))
					{
						var lastKauJournal = GetKauJournalById(LastKauId);
						if (lastKauJournal != null)
							LastId = lastKauJournal.GKJournalRecordNo;
					}
					IpAddress = GkDatabase.RootDevice.GetGKIpAddress();
				}
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
						journalParser = ReadAndPublish(index);
						if (journalParser != null && journalParser.KauJournalRecordNo != 0)
							LastKauId = journalParser.KauJournalRecordNo;
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

			if (!isWrite)
				isWrite = journalParser.JournalItem.JournalEventNameType == JournalEventNameType.Запись_конфигурации_в_прибор;

			if (journalParser.JournalItem.JournalEventNameType == JournalEventNameType.Смена_БД)
			{
				IsWritedConfingurationOutServer = !isWrite;
				isWrite = false;
			}

			return journalParser.GKJournalRecordNo;
		}

		JournalParser GetKauJournalById(int lastKauId)
		{
			var lastGkId = GetLastId(); // Находим номер последней записи
			for (int index = lastGkId; index > 0; index--) // Ищем последнюю запись на КАУ, записанную в систему
			{
				var journalParser = ReadJournal(index);
				if (journalParser.KauJournalRecordNo != 0 && (journalParser.KauJournalRecordNo <= lastKauId || lastKauId == -1))
					return journalParser;
				if (lastGkId - index > 10000) // Если последнии 10000 записей не содержат записи на КАУ, то возвращаем null
					return null;
			}
			return null;
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

		JournalParser ReadAndPublish(int index)
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
							if (parent.Driver.IsGroupDevice || parent.DriverType == GKDriverType.RSR2_KAU_Shleif || parent.DriverType == GKDriverType.RSR2_MVP_Part
								|| parent.DriverType == GKDriverType.GKIndicatorsGroup || parent.DriverType == GKDriverType.GKRelaysGroup || parent.DriverType == GKDriverType.RSR2_KDKR_Part)
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
				return journalParser;
			}
			return null;
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
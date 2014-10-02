using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using FiresecAPI.Journal;

namespace GKProcessor
{
	public partial class Watcher
	{
		int LastId = -1;

		void PingJournal()
		{
			var newLastId = GetLastId();
			if (newLastId == -1)
				return;
			if (LastId == -1)
				LastId = newLastId;
			if (newLastId > LastId)
			{
				ReadAndPublish(LastId, newLastId);
				LastId = newLastId;
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
			return journalParser.JournalItem.GKJournalRecordNo.Value;
		}

		GKJournalItem ReadJournal(int index)
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
				return journalParser.JournalItem;
			}
			return null;
		}

		void ReadAndPublish(int startIndex, int endIndex)
		{
			var journalItems = new List<GKJournalItem>();
			for (int index = startIndex + 1; index <= endIndex; index++)
			{
				var journalItem = ReadJournal(index);
				if (journalItem != null)
				{
					journalItems.Add(journalItem);
					var descriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.GetDescriptorNo() == journalItem.GKObjectNo);
					if (descriptor != null)
					{
						ChangeJournalOnDevice(descriptor, journalItem);
						CheckServiceRequired(descriptor.GKBase, journalItem);
						descriptor.GKBase.InternalState.StateBits = GKStatesHelper.StatesFromInt(journalItem.ObjectState);
						if (descriptor.GKBase.InternalState.StateClass == XStateClass.On)
						{
							descriptor.GKBase.InternalState.ZeroHoldDelayCount = 0;
							CheckDelay(descriptor.GKBase);
						}
						ParseAdditionalStates(journalItem);
						OnObjectStateChanged(descriptor.GKBase);
						if (descriptor.GKBase is GKDevice)
						{
							GKDevice device = descriptor.GKBase as GKDevice;
							if (device.Parent != null && device.Parent.Driver.IsGroupDevice)
							{
								OnObjectStateChanged(device.Parent);
							}
							var shleifParent = device.AllParents.FirstOrDefault(x => x.Driver.DriverType == GKDriverType.KAU_Shleif || x.Driver.DriverType == GKDriverType.RSR2_KAU_Shleif);
							if (shleifParent != null)
							{
								OnObjectStateChanged(shleifParent);
							}
						}
					}

					if (journalItem.JournalEventNameType == JournalEventNameType.Перевод_в_технологический_режим || journalItem.JournalEventNameType == JournalEventNameType.Перевод_в_рабочий_режим)
					{
						MustCheckTechnologicalRegime = true;
						LastTechnologicalRegimeCheckTime = DateTime.Now;
						TechnologicalRegimeCheckCount = 0;

						CheckTechnologicalRegime();
						NotifyAllObjectsStateChanged();
					}
				}
			}
			if (journalItems.Count > 0)
			{
				AddJournalItems(journalItems);
			}
		}

		void ChangeJournalOnDevice(BaseDescriptor descriptor, GKJournalItem journalItem)
		{
			if (descriptor.Device != null)
			{
				var device = descriptor.Device;
				if (device.DriverType == GKDriverType.AM1_T)
				{
					if (journalItem.JournalEventNameType == JournalEventNameType.Сработка_2)
					{
						var property = device.Properties.FirstOrDefault(x => x.Name == "OnMessage");
						if (property != null)
						{
							journalItem.Description = property.StringValue;
						}
					}
					if (journalItem.JournalEventNameType == JournalEventNameType.Норма)
					{
						var property = device.Properties.FirstOrDefault(x => x.Name == "NormMessage");
						if (property != null)
						{
							journalItem.Description = property.StringValue;
						}
					}
				}
				if (device.DriverType == GKDriverType.Valve)
				{
					switch (journalItem.Name)
					{
						case "Включено":
							journalItem.Name = "Открыто";
							break;

						case "Выключено":
							journalItem.Name = "Закрыто";
							break;

						case "Включается":
							journalItem.Name = "Открывается";
							break;

						case "Выключается":
							journalItem.Name = "Закрывается";
							break;
					}
				}
			}
		}

		void CheckServiceRequired(GKBase xBase, GKJournalItem journalItem)
		{
			if (journalItem.JournalEventNameType == JournalEventNameType.Запыленность || journalItem.JournalEventNameType == JournalEventNameType.Запыленность_устранена)
			{
				if (xBase is GKDevice)
				{
					var device = xBase as GKDevice;
					if (journalItem.JournalEventNameType == JournalEventNameType.Запыленность)
						device.InternalState.IsService = true;
					if (journalItem.JournalEventNameType == JournalEventNameType.Запыленность_устранена)
						device.InternalState.IsService = false;
				}
			}
		}
	}
}
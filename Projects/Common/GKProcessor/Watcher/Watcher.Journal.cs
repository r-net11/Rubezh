﻿using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;

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

		FiresecAPI.GK.JournalItem ReadJournal(int index)
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
			var journalItems = new List<FiresecAPI.GK.JournalItem>();
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
						CheckServiceRequired(descriptor.XBase, journalItem);
						descriptor.XBase.InternalState.StateBits = XStatesHelper.StatesFromInt(journalItem.ObjectState);
						if (descriptor.XBase.InternalState.StateClass == XStateClass.On)
						{
							descriptor.XBase.InternalState.ZeroHoldDelayCount = 0;
							CheckDelay(descriptor.XBase);
						}
						ParseAdditionalStates(journalItem);
						OnObjectStateChanged(descriptor.XBase);
						if (descriptor.XBase is XDevice)
						{
							XDevice device = descriptor.XBase as XDevice;
							if (device.Parent != null && device.Parent.Driver.IsGroupDevice)
							{
								OnObjectStateChanged(device.Parent);
							}
							var shleifParent = device.AllParents.FirstOrDefault(x => x.Driver.DriverType == XDriverType.KAU_Shleif || x.Driver.DriverType == XDriverType.RSR2_KAU_Shleif);
							if (shleifParent != null)
							{
								OnObjectStateChanged(shleifParent);
							}
						}
					}

					if (journalItem.JournalEventNameType == FiresecAPI.Journal.JournalEventNameType.Перевод_в_технологический_режим || journalItem.JournalEventNameType == FiresecAPI.Journal.JournalEventNameType.Перевод_в_рабочий_режим)
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

		void ChangeJournalOnDevice(BaseDescriptor descriptor, FiresecAPI.GK.JournalItem journalItem)
		{
			if (descriptor.Device != null)
			{
				var device = descriptor.Device;
				if (device.DriverType == XDriverType.AM1_T)
				{
					if (journalItem.JournalEventNameType == FiresecAPI.Journal.JournalEventNameType.Сработка_2)
					{
						var property = device.Properties.FirstOrDefault(x => x.Name == "OnMessage");
						if (property != null)
						{
							journalItem.Description = property.StringValue;
						}
					}
					if (journalItem.JournalEventNameType == FiresecAPI.Journal.JournalEventNameType.Норма)
					{
						var property = device.Properties.FirstOrDefault(x => x.Name == "NormMessage");
						if (property != null)
						{
							journalItem.Description = property.StringValue;
						}
					}
				}
				if (device.DriverType == XDriverType.Valve)
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

		void CheckServiceRequired(XBase xBase, JournalItem journalItem)
		{
			if (journalItem.JournalEventNameType == FiresecAPI.Journal.JournalEventNameType.Запыленность || journalItem.JournalEventNameType == FiresecAPI.Journal.JournalEventNameType.Запыленность_устранена)
			{
				if (xBase is XDevice)
				{
					var device = xBase as XDevice;
					if (journalItem.JournalEventNameType == FiresecAPI.Journal.JournalEventNameType.Запыленность)
						device.InternalState.IsService = true;
					if (journalItem.JournalEventNameType == FiresecAPI.Journal.JournalEventNameType.Запыленность_устранена)
						device.InternalState.IsService = false;
				}
			}
		}
	}
}
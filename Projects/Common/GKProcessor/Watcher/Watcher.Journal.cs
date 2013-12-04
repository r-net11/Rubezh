using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.XModels;
using Infrastructure;
using Infrastructure.Common.Windows;
using XFiresecAPI;
using FiresecClient;
using Infrastructure.Common.Services;
using System.Diagnostics;
using System.Threading;

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

		JournalItem ReadJournal(int index)
		{
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
            var journalItems = new List<JournalItem>();
			for (int index = startIndex + 1; index <= endIndex; index++)
			{
				var journalItem = ReadJournal(index);
				if (journalItem != null)
				{
					GKProcessorManager.OnDoProgress(journalItem.GKJournalRecordNo.ToString());

					journalItems.Add(journalItem);
					var descriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.GetDescriptorNo() == journalItem.GKObjectNo);
					if (descriptor != null)
					{
						ChangeJournalOnDevice(descriptor, journalItem);
						CheckAdditionalStates(descriptor);
						CheckServiceRequired(descriptor.XBase, journalItem);
						descriptor.XBase.BaseState.StateBits = XStatesHelper.StatesFromInt(journalItem.ObjectState);
						ParseAdditionalStates(journalItem);
						OnObjectStateChanged(descriptor.XBase);
					}
				}
			}
            if (journalItems.Count > 0)
            {
				AddJournalItems(journalItems);
            }
        }

		void ChangeJournalOnDevice(BaseDescriptor descriptor, JournalItem journalItem)
		{
			if (descriptor.Device != null)
			{
				var device = descriptor.Device;
				if (device.DriverType == XDriverType.AM1_T)
				{
					if (journalItem.Name == "Сработка-2")
					{
						var property = device.Properties.FirstOrDefault(x => x.Name == "OnMessage");
						if (property != null)
						{
							journalItem.Description = property.StringValue;
						}
					}
					if (journalItem.Name == "Норма")
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

		void CheckDelays()
		{
			foreach (var direction in XManager.Directions)
			{
				bool mustGetState = false;
				switch (direction.InternalState.StateClass)
				{
					case XStateClass.TurningOn:
						mustGetState = direction.InternalState.OnDelay > 0 || (DateTime.Now - direction.InternalState.LastDateTime).Seconds > 1;
						break;
					case XStateClass.On:
						mustGetState = direction.InternalState.HoldDelay > 0 || (DateTime.Now - direction.InternalState.LastDateTime).Seconds > 1;
						break;
					case XStateClass.TurningOff:
						mustGetState = direction.InternalState.OffDelay > 0 || (DateTime.Now - direction.InternalState.LastDateTime).Seconds > 1;
						break;
				}
				if (mustGetState)
				{
					var onDelay = direction.InternalState.OnDelay;
					var holdDelay = direction.InternalState.HoldDelay;
					var offDelay = direction.InternalState.OffDelay;
					GetState(direction);
					if(onDelay != direction.InternalState.OnDelay || holdDelay != direction.InternalState.HoldDelay || offDelay != direction.InternalState.OffDelay)
						OnObjectStateChanged(direction);
				}
			}

			var delays = new List<XDelay>();
			foreach (var gkDatabase in DescriptorsManager.GkDatabases)
			{
				foreach (var delay in gkDatabase.Delays)
				{
					delays.Add(delay);
				}
			}
			foreach (var delay in delays)
			{
				bool mustGetState = false;
				switch (delay.InternalState.StateClass)
				{
					case XStateClass.TurningOn:
						mustGetState = delay.InternalState.OnDelay > 0 || (DateTime.Now - delay.InternalState.LastDateTime).Seconds > 1;
						break;
					case XStateClass.On:
						mustGetState = delay.InternalState.HoldDelay > 0 || (DateTime.Now - delay.InternalState.LastDateTime).Seconds > 1;
						break;
					case XStateClass.TurningOff:
						mustGetState = delay.InternalState.OffDelay > 0 || (DateTime.Now - delay.InternalState.LastDateTime).Seconds > 1;
						break;
				}
				if (mustGetState)
				{
					var onDelay = delay.InternalState.OnDelay;
					var holdDelay = delay.InternalState.HoldDelay;
					var offDelay = delay.InternalState.OffDelay;
					GetState(delay);
					if (onDelay != delay.InternalState.OnDelay || holdDelay != delay.InternalState.HoldDelay || offDelay != delay.InternalState.OffDelay)
						OnObjectStateChanged(delay);
				}
			}

			foreach (var device in XManager.Devices)
			{
				if (!device.Driver.IsGroupDevice && device.AllParents.Any(x=>x.DriverType == XDriverType.RSR2_KAU))
				{
					bool mustGetState = false;
					switch (device.InternalState.StateClass)
					{
						case XStateClass.TurningOn:
							mustGetState = device.InternalState.OnDelay > 0 || (DateTime.Now - device.InternalState.LastDateTime).Seconds > 1;
							break;
						case XStateClass.On:
							mustGetState = device.InternalState.HoldDelay > 0 || (DateTime.Now - device.InternalState.LastDateTime).Seconds > 1;
							break;
						case XStateClass.TurningOff:
							mustGetState = device.InternalState.OffDelay > 0 || (DateTime.Now - device.InternalState.LastDateTime).Seconds > 1;
							break;
					}
					if (mustGetState)
					{
						var onDelay = device.InternalState.OnDelay;
						var holdDelay = device.InternalState.HoldDelay;
						var offDelay = device.InternalState.OffDelay;
						GetState(device);
						if (onDelay != device.InternalState.OnDelay || holdDelay != device.InternalState.HoldDelay || offDelay != device.InternalState.OffDelay)
							OnObjectStateChanged(device);
					}
				}
			}
		}
	}
}
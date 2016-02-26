using System;
using RubezhAPI.Journal;
using RubezhAPI;

namespace GKProcessor
{
	public partial class Watcher
	{
		bool IsConnected = true;
		int ConnectionLostCount = 0;
		object connectionChangedLocker = new object();

		public void ConnectionChanged(bool isConnected)
		{
			lock (connectionChangedLocker)
			{
				if (!isConnected)
				{
					DeviceBytesHelper.Ping(GkDatabase.RootDevice);
					ConnectionLostCount++;
					if (ConnectionLostCount < 3)
						return;
				}
				else
					ConnectionLostCount = 0;

				if (IsConnected != isConnected)
				{
					var journalItem = new JournalItem()
					{
						SystemDateTime = DateTime.Now,
						DeviceDateTime = DateTime.Now,
						JournalObjectType = JournalObjectType.GKDevice,
						ObjectUID = GkDatabase.RootDevice.UID,
						JournalEventNameType = isConnected ? JournalEventNameType.Восстановление_связи_с_прибором : JournalEventNameType.Потеря_связи_с_прибором,
					};
					//var gkIpAddress = GKManager.GetIpAddress(GkDatabase.RootDevice);
					//if (!string.IsNullOrEmpty(gkIpAddress))
					//	journalItem.JournalDetalisationItems.Add(new JournalDetalisationItem("IP-адрес ГК", gkIpAddress.ToString()));
					AddJournalItem(journalItem);

					IsConnected = isConnected;
					if (isConnected)
					{
						var hashBytes = GKFileInfo.CreateHash1(GkDatabase.RootDevice);
						var gkFileReaderWriter = new GKFileReaderWriter();
						var gkFileInfo = gkFileReaderWriter.ReadInfoBlock(GkDatabase.RootDevice);
						IsHashFailure = gkFileInfo == null || !GKFileInfo.CompareHashes(hashBytes, gkFileInfo.Hash1);
						if (!IsHashFailure)
						{
							GetAllStates();
						}
					}

					foreach (var descriptor in GkDatabase.Descriptors)
					{
						descriptor.GKBase.InternalState.IsConnectionLost = !isConnected;
					}
					NotifyAllObjectsStateChanged();
				}
			}
		}
	}
}
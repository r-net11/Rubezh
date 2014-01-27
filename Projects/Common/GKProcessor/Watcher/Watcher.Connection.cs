using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecClient;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using FiresecAPI;
using XFiresecAPI;
using Infrastructure.Common;

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
						GKIpAddress = XManager.GetIpAddress(GkDatabase.RootDevice),
						JournalItemType = JournalItemType.System,
						StateClass = XStateClass.Unknown,
						ObjectStateClass = XStateClass.Norm,
                        Name = isConnected ? EventName.Восстановление_связи_с_прибором.ToDescription() : EventName.Потеря_связи_с_прибором.ToDescription()
					};
					AddJournalItem(journalItem);

					IsConnected = isConnected;
					if (isConnected)
					{
						var hashBytes = GKFileInfo.CreateHash1(XManager.DeviceConfiguration, GkDatabase.RootDevice);
						var gkFileReaderWriter = new GKFileReaderWriter();
						var gkFileInfo = gkFileReaderWriter.ReadInfoBlock(GkDatabase.RootDevice);
						IsHashFailure = gkFileInfo == null || !GKFileInfo.CompareHashes(hashBytes, gkFileInfo.Hash1);
					}

					foreach (var descriptor in GkDatabase.Descriptors)
					{
						descriptor.XBase.InternalState.IsConnectionLost = !isConnected;
					}
					NotifyAllObjectsStateChanged();
				}
			}
		}
	}
}
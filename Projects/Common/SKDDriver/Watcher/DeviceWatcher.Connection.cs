﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;
using System.Threading;
using Common;
using XFiresecAPI;

namespace SKDDriver
{
	public partial class DeviceWatcher
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
					var journalItem = new SKDJournalItem()
					{
						SystemDateTime = DateTime.Now,
						DeviceDateTime = DateTime.Now,
						//GKIpAddress = XManager.GetIpAddress(GkDatabase.RootDevice),
						JournalItemType = JournalItemType.System,
						StateClass = XStateClass.Unknown,
						ObjectStateClass = XStateClass.Norm,
						Name = isConnected ? "Восстановление связи с прибором" : "Потеря связи с прибором"
					};
					AddJournalItem(journalItem);

					IsConnected = isConnected;
					if (isConnected)
					{
						// check hash
					}

					foreach (var child in Device.Children)
					{
						child.State.IsConnectionLost = !isConnected;
					}
					NotifyAllObjectsStateChanged();
				}
			}
		}
	}
}
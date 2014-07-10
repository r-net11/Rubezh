using System;
using FiresecAPI.GK;
using FiresecAPI.Journal;

namespace SKDDriver
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
						JournalSubsystemType = JournalSubsystemType.System,
						StateClass = XStateClass.Unknown,
						JournalEventNameType = isConnected ? JournalEventNameType.Восстановление_связи_с_прибором : JournalEventNameType.Потеря_связи_с_прибором
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
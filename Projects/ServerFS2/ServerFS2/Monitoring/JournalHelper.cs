using System;
using System.Linq;
using FiresecAPI.Models;
using FS2Api;

namespace ServerFS2.Monitoring
{
	public static class JournalHelper
	{
		static readonly object Locker = new object();

		public static FS2JournalItem ReadItem(DeviceConfiguration deviceConfiguration, Device device, int i, byte journalType)
		{
			for (int j = 0; j < 15; j++)
			{
				var response = USBManager.Send(device, "Чтение конкретной записи в журнале", 0x01, 0x20, journalType, BitConverter.GetBytes(i).Reverse());
				if (response != null)
				{
					lock (Locker)
					{
						var journalParser = new JournalParser();
						try
						{
							var fsJournalItem = journalParser.Parce(deviceConfiguration, device, response.Bytes);
							if (fsJournalItem != null)
								return fsJournalItem;
						}
						catch
						{
							return null;
						}
					}
				}
			}
			return null;
		}
	}
}
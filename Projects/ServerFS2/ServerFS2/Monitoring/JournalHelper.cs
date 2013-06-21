using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using FiresecAPI.Models;
using FS2Api;

namespace ServerFS2.Monitoring
{
	public static class JournalHelper
	{
		static readonly object Locker = new object();

		public static FS2JournalItem ReadItem(Device device, int i)
		{
			for (int j = 0; j < 15; j++)
			{
				var response = USBManager.Send(device, 0x01, 0x20, 0x00, BitConverter.GetBytes(i).Reverse());
				if (response != null)
				{
					lock (Locker)
					{
						var journalParser = new JournalParser();
						try
						{
							var fsJournalItem = journalParser.Parce(device, response.Bytes);
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
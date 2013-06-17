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
				var fsJournalItem = SendBytesAndParse(device, 0x20, 0x00, BitConverter.GetBytes(i).Reverse());
				if (fsJournalItem != null)
					return fsJournalItem;
			}
			return null;
		}

		static FS2JournalItem SendBytesAndParse(Device device, params object[] value)
		{
			var response = USBManager.SendCodeToPanel(device, 0x01, value);
			if (response == null)
				return null;
			lock (Locker)
			{
				var journalParser = new JournalParser();
				try
				{
					return journalParser.Parce(device, response);
				}
				catch
				{
					return null;
				}
			}
		}
	}
}
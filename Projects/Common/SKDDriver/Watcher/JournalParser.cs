using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;

namespace SKDDriver
{
	public class JournalParser
	{
		public SKDJournalItem JournalItem { get; private set; }

		public JournalParser(SKDDevice device, List<byte> bytes)
		{
			JournalItem = new SKDJournalItem();

			//JournalItem.GKIpAddress = XManager.GetIpAddress(gkDevice);
			JournalItem.DeviceJournalRecordNo = BytesHelper.SubstructInt(bytes, 0);
			JournalItem.GKObjectNo = BytesHelper.SubstructShort(bytes, 4);

			switch (bytes[4])
			{
				case 0:
					JournalItem.Name = "Событие 1";
					break;

				case 1:
					JournalItem.Name = "Событие 2";
					break;
			}
		}
	}
}
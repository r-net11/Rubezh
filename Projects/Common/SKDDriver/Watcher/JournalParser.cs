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
			var no = BytesHelper.SubstructInt(bytes, 0);
			var source = bytes[4];
			var address = bytes[5];
			var code = bytes[6];
			var cardSeries = BytesHelper.SubstructInt(bytes, 7);
			var cardNo = BytesHelper.SubstructInt(bytes, 11);

			JournalItem = new SKDJournalItem();
			JournalItem.IpAddress = device.Address;
			JournalItem.DeviceJournalRecordNo = no;
			JournalItem.CardSeries = cardSeries;
			JournalItem.CardNo = cardNo;

			var skdEvent = SKDEventsHelper.SKDEvents.FirstOrDefault(x => x.No == code);
			if (skdEvent != null)
			{
				JournalItem.Name = skdEvent.Name;
			}

			if (source == 1)
			{
				JournalItem.DeviceUID = device.UID;
				JournalItem.DeviceName = device.Name;
			}
			if (source > 1)
			{
				if (device.Children.Count > source - 2)
				{
					var childDevice = device.Children.First(x => x.Address == address.ToString());
					if (childDevice != null)
					{
						JournalItem.DeviceUID = childDevice.UID;
						JournalItem.DeviceName = childDevice.Name;
					}
				}
			}
		}
	}
}
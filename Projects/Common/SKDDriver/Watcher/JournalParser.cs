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
			var code = bytes[5];
			var cardNo = BytesHelper.SubstructInt(bytes, 6);

			JournalItem = new SKDJournalItem();
			JournalItem.IpAddress = device.Address;
			JournalItem.DeviceJournalRecordNo = no;
			JournalItem.CardNo = cardNo;

			var skdEvent = SKDEventsHelper.SKDEvents.FirstOrDefault(x => x.No == code);
			if (skdEvent != null)
			{
				JournalItem.Name = skdEvent.Name;
			}

			if (source == 1)
			{
				JournalItem.DeviceUID = device.UID;
				JournalItem.DeviceName = device.PresentationName;
			}
			if (source > 1)
			{
				if (device.Children.Count > source - 2)
				{
					var childDevice = device.Children[source - 2];
					JournalItem.DeviceUID = childDevice.UID;
					JournalItem.DeviceName = childDevice.PresentationName;
				}
			}
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.SKD;
using JournalItem = FiresecAPI.SKD.JournalItem;

namespace SKDDriver
{
	public class JournalParser
	{
		public JournalItem JournalItem { get; private set; }

		public JournalParser(SKDDevice device, List<byte> bytes)
		{
			var no = BytesHelper.SubstructInt(bytes, 0);
			var deviceDateTime = DateTime.MinValue;
			try
			{
				deviceDateTime = new DateTime(bytes[4] + 2000, bytes[5], bytes[6], bytes[7], bytes[8], bytes[9]);
			}
			catch
			{

			}
			var source = bytes[10];
			var address = bytes[11];
			var eventNameCode = bytes[12];
			var evenDescriptionCode = bytes[13];
			var cardSeries = BytesHelper.SubstructInt(bytes, 14);
			var cardNo = BytesHelper.SubstructInt(bytes, 18);

			JournalItem = new JournalItem();
			JournalItem.DeviceDateTime = deviceDateTime;
			JournalItem.CardNo = cardNo;

			if (source == 1)
			{
				SetDevice(device);
			}
			if (source == 2)
			{
				var childDevice = device.Children.First(x => x.IntAddress == address);
				if (childDevice != null)
				{
					SetDevice(childDevice);
				}
			}
		}

		void SetDevice(SKDDevice device)
		{
			JournalItem.ObjectUID = device.UID;
			JournalItem.ObjectName = device.Name;
		}
	}
}
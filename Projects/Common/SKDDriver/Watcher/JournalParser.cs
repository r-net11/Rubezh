using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
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
			JournalItem.CardSeries = cardSeries;
			JournalItem.CardNo = cardNo;

			var skdEvent = SKDEventsHelper.SKDEvents.FirstOrDefault(x => x.No == eventNameCode);
			if (skdEvent != null)
			{
				JournalItem.Name = skdEvent.Name;
				JournalItem.State = skdEvent.StateClass;
				switch (eventNameCode)
				{
					case 22:
						switch (evenDescriptionCode)
						{
							case 1:
								JournalItem.Description = EventDescription.Установить_режим_ОТКРЫТО;
								break;
							case 2:
								JournalItem.Description = EventDescription.Установить_режим_ЗАКРЫТО;
								break;
							case 3:
								JournalItem.Description = EventDescription.Установить_режим_КОНТРОЛЬ;
								break;
							case 4:
								JournalItem.Description = EventDescription.Установить_режим_СОВЕЩАНИЕ;
								break;
							case 5:
								JournalItem.Description = EventDescription.Открыть;
								break;
							case 6:
								JournalItem.Description = EventDescription.Закрыть;
								break;
							case 7:
								JournalItem.Description = EventDescription.Разрешить_проход;
								break;
							case 8:
								JournalItem.Description = EventDescription.Запретить_проход;
								break;
						}
						break;

					case 23:
						switch (evenDescriptionCode)
						{
							case 1:
								JournalItem.Description = EventDescription.Запись_одного_идентификатора;
								break;
							case 2:
								JournalItem.Description = EventDescription.Запись_всех_временных_интервалов;
								break;
						}
						break;

					default:
						JournalItem.DescriptionText = evenDescriptionCode.ToString();
						break;
				}
			}

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
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

			JournalItem = new SKDJournalItem();
			JournalItem.DeviceDateTime = deviceDateTime;
			JournalItem.IpAddress = device.Address;
			JournalItem.DeviceJournalRecordNo = no;
			JournalItem.CardSeries = cardSeries;
			JournalItem.CardNo = cardNo;

			var skdEvent = SKDEventsHelper.SKDEvents.FirstOrDefault(x => x.No == eventNameCode);
			if (skdEvent != null)
			{
				JournalItem.Name = skdEvent.Name;
				JournalItem.StateClass = skdEvent.StateClass;
				switch (eventNameCode)
				{
					case 22:
						switch (evenDescriptionCode)
						{
							case 1:
								JournalItem.Description = "Установить режим ОТКРЫТО";
								break;
							case 2:
								JournalItem.Description = "Установить режим ЗАКРЫТО";
								break;
							case 3:
								JournalItem.Description = "Установить режим КОНТРОЛЬ";
								break;
							case 4:
								JournalItem.Description = "Установить режим СОВЕЩАНИЕ";
								break;
							case 5:
								JournalItem.Description = "Открыть";
								break;
							case 6:
								JournalItem.Description = "Закрыть";
								break;
							case 7:
								JournalItem.Description = "Разрешить проход";
								break;
							case 8:
								JournalItem.Description = "Запретить проход";
								break;
						}
						break;

					case 23:
						switch (evenDescriptionCode)
						{
							case 1:
								JournalItem.Description = "Запись одного идентификатора";
								break;
							case 2:
								JournalItem.Description = "Запись всех временнх интервалов";
								break;
						}
						break;

					default:
						JournalItem.Description = evenDescriptionCode.ToString();
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
			JournalItem.DeviceUID = device.UID;
			JournalItem.DeviceName = device.Name;
			JournalItem.Device = device;
		}
	}
}
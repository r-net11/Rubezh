using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.SKD;
using ChinaSKDDriverAPI;

namespace ChinaSKDDriver
{
	public class CardWriter
	{
		public List<ControllerCardItem> ControllerCardItems { get; private set; }

		public CardWriter()
		{
			ControllerCardItems = new List<ControllerCardItem>();
		}

		public bool AddCard(SKDCard skdCard)
		{
			var cardDoors = new List<CardDoor>();
			foreach (var cardDoor in skdCard.CardDoors)
			{
				cardDoors.Add(cardDoor);
			}
			if (skdCard.AccessTemplateUID.HasValue)
			{
				var accessTemplate = new AccessTemplate();
				foreach (var cardDoor in accessTemplate.CardDoors)
				{
					cardDoors.Add(cardDoor);
				}
			}

			foreach (var cardDoor in cardDoors)
			{
				var door = Processor.SKDConfiguration.Doors.FirstOrDefault(x => x.UID == cardDoor.DoorUID);
				if (door != null)
				{
					Add(door.InDeviceUID, cardDoor.EnterIntervalID);
					Add(door.OutDeviceUID, cardDoor.ExitIntervalID);
				}
			}

			foreach (var controllerCardItem in ControllerCardItems)
			{
				if (controllerCardItem.Doors.Count > 0)
				{
					var deviceProcessor = Processor.DeviceProcessors.FirstOrDefault(x => x.Device.UID == controllerCardItem.ControllerDevice.UID);
					if (deviceProcessor != null)
					{
						var card = new Card();
						card.CardNo = skdCard.Number.ToString();
						card.CardType = ChinaSKDDriverAPI.CardType.NET_ACCESSCTLCARD_TYPE_GENERAL;
						card.ValidStartDateTime = skdCard.StartDate;
						card.ValidEndDateTime = skdCard.EndDate;

						foreach (var doorDevice in controllerCardItem.Doors)
						{
							card.Doors.Add(doorDevice.IntAddress + 1);
						}
						foreach (var weeklyInterval in controllerCardItem.WeeklyIntervals)
						{
							var index = Processor.SKDConfiguration.TimeIntervalsConfiguration.WeeklyIntervals.IndexOf(weeklyInterval);
							card.TimeSections.Add(index + 1);
						}
						var cardRecordNo = deviceProcessor.Wrapper.AddCard(card);
						controllerCardItem.Result = cardRecordNo >= 0;
					}
				}
			}

			return true;
		}

		void Add(Guid readerUID, int intervalID)
		{
			var readerDevice = Processor.SKDConfiguration.Devices.FirstOrDefault(x => x.UID == readerUID);
			if (readerDevice != null && readerDevice.Parent != null)
			{
				var controllerDevice = readerDevice.Parent;
				var controllerCardItem = ControllerCardItems.FirstOrDefault(x => x.ControllerDevice.UID == controllerDevice.UID);
				if (controllerCardItem == null)
				{
					controllerCardItem = new ControllerCardItem();
					controllerCardItem.ControllerDevice = controllerDevice;
					ControllerCardItems.Add(controllerCardItem);
				}
				if (!controllerCardItem.Doors.Any(x => x.UID == readerDevice.UID))
				{
					controllerCardItem.Doors.Add(readerDevice);
				}

				if (intervalID > 0)
				{
					var weeklyInterval = Processor.SKDConfiguration.TimeIntervalsConfiguration.WeeklyIntervals.FirstOrDefault(x => x.ID == intervalID);
					if (weeklyInterval != null && !controllerCardItem.WeeklyIntervals.Any(x => x.ID == weeklyInterval.ID))
						controllerCardItem.WeeklyIntervals.Add(weeklyInterval);
				}
			}
		}

		public class ControllerCardItem
		{
			public ControllerCardItem()
			{
				Doors = new List<SKDDevice>();
				WeeklyIntervals = new List<SKDWeeklyInterval>();
				Result = false;
			}

			public SKDDevice ControllerDevice { get; set; }
			public List<SKDDevice> Doors { get; set; }
			public List<SKDWeeklyInterval> WeeklyIntervals { get; set; }
			public bool Result { get; set; }
		}
	}
}
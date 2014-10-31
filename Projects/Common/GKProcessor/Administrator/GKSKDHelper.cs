using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecClient;
using FiresecAPI.SKD;
using FiresecAPI.GK;
using FiresecAPI;

namespace GKProcessor
{
	public class GKSKDHelper
	{
		public void AddCard(SKDCard card, AccessTemplate accessTemplate, string employeeName)
		{
			foreach (var gkControllerDevice in GKManager.DeviceConfiguration.RootDevice.Children)
			{
				AddOneCard(gkControllerDevice, card, accessTemplate, employeeName);
			}
		}

		OperationResult<bool> AddOneCard(GKDevice device, SKDCard card, AccessTemplate accessTemplate, string employeeName)
		{
			var cardSchedules = new List<GKCardSchedule>();

			var cardDoors = new List<CardDoor>();
			if (accessTemplate != null)
			{
				cardDoors = accessTemplate.CardDoors.ToList();
			}
			cardDoors.AddRange(card.CardDoors.ToList());

			foreach (var cardDoor in cardDoors)
			{
				var door = GKManager.Doors.FirstOrDefault(x => x.UID == cardDoor.DoorUID);
				if (door != null)
				{
					if (door.EnterDevice != null)
					{
						var enterCardSchedule = cardSchedules.FirstOrDefault(x => x.Device.UID == door.EnterDevice.UID);
						if (enterCardSchedule == null)
						{
							enterCardSchedule = new GKCardSchedule();
							cardSchedules.Add(enterCardSchedule);
						}
						enterCardSchedule.Device = door.EnterDevice;
						enterCardSchedule.ScheduleNo = cardDoor.EnterScheduleNo;
					}
					if (door.DoorType == GKDoorType.TwoWay && door.ExitDevice != null)
					{
						var exitCardSchedule = cardSchedules.FirstOrDefault(x => x.Device.UID == door.ExitDevice.UID);
						if (exitCardSchedule == null)
						{
							exitCardSchedule = new GKCardSchedule();
							cardSchedules.Add(exitCardSchedule);
						}
						exitCardSchedule.Device = door.ExitDevice;
						exitCardSchedule.ScheduleNo = cardDoor.ExitScheduleNo;
					}
				}
			}

			cardSchedules = cardSchedules.OrderBy(x => x.Device.GKDescriptorNo).ToList();

			var no = 10;
			var intPassword = Int32.Parse(card.Number);

			var bytes = new List<byte>();
			bytes.AddRange(BytesHelper.ShortToBytes((ushort)(no)));
			bytes.Add(0);
			bytes.Add(0);
			var nameBytes = BytesHelper.StringDescriptionToBytes(employeeName);
			bytes.AddRange(nameBytes);
			bytes.AddRange(BytesHelper.IntToBytes(intPassword));			
			bytes.Add((byte)card.GKLevel);
			bytes.Add((byte)card.GKLevelSchedule);

			foreach (var cardSchedule in cardSchedules)
			{
				bytes.AddRange(BytesHelper.ShortToBytes(cardSchedule.Device.GKDescriptorNo));
			}
			for (int i = 0; i < 70 - cardSchedules.Count; i++)
			{
				bytes.Add(0);
				bytes.Add(0);
			}
			foreach (var cardSchedule in cardSchedules)
			{
				bytes.Add((byte)cardSchedule.ScheduleNo);
			}
			for (int i = 0; i < 70 - cardSchedules.Count; i++)
			{
				bytes.Add(0);
			}

			bytes.Add(0);
			bytes.Add(0);

			bytes.Add(0);
			bytes.Add(0);

			var packs = new List<List<byte>>();
			for (int packNo = 0; packNo <= bytes.Count / 256; packNo++)
			{
				int packLenght = Math.Min(256, bytes.Count - packNo * 256);
				var packBytes = bytes.Skip(packNo * 256).Take(packLenght).ToList();

				if (packBytes.Count > 0)
				{
					var resultBytes = new List<byte>();
					resultBytes.Add((byte)(packNo));
					resultBytes.AddRange(packBytes);
					packs.Add(resultBytes);
				}
			}

			foreach (var pack in packs)
			{
				var sendResult = SendManager.Send(device, (ushort)(pack.Count), 25, 0, pack);
				if (sendResult.HasError)
				{
					return new OperationResult<bool>(sendResult.Error);
				}
			}

			return new OperationResult<bool>() { Result = true };
		}

		class GKCardSchedule
		{
			public GKDevice Device { get; set; }
			public int ScheduleNo { get; set; }
		}
	}
}
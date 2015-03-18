using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecClient;
using FiresecAPI.SKD;
using FiresecAPI.GK;
using FiresecAPI;
using SKDDriver;
using System.ComponentModel;

namespace GKProcessor
{
	public class GKSKDHelper
	{
		public OperationResult<bool> AddOneCard(GKDevice device, SKDCard card, AccessTemplate accessTemplate, string employeeName)
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

			var no = 1;
			bool isNew = true;
			using (var skdDatabaseService = new SKDDatabaseService())
			{
				no = skdDatabaseService.GKCardTranslator.GetFreeGKNo(device.GetGKIpAddress(), card.Number, out isNew);
			}

			var bytes = new List<byte>();
			bytes.AddRange(BytesHelper.ShortToBytes((ushort)(no)));
			bytes.Add(0);
			bytes.Add(0);
			var nameBytes = BytesHelper.StringDescriptionToBytes(employeeName);
			bytes.AddRange(nameBytes);
			bytes.AddRange(BytesHelper.IntToBytes((int)card.Number));			
			bytes.Add((byte)card.GKLevel);
			bytes.Add((byte)card.GKLevelSchedule);

			bytes.Add(0);
			bytes.Add(0);

			var secondsPeriod = (new DateTime(card.EndDate.Year, card.EndDate.Month, card.EndDate.Day) - new DateTime(2000, 1, 1)).TotalSeconds;
			bytes.AddRange(BytesHelper.IntToBytes((int)secondsPeriod));

			foreach (var cardSchedule in cardSchedules)
			{
				bytes.AddRange(BytesHelper.ShortToBytes(cardSchedule.Device.GKDescriptorNo));
			}
			for (int i = 0; i < 68 - cardSchedules.Count; i++)
			{
				bytes.Add(0);
				bytes.Add(0);
			}
			foreach (var cardSchedule in cardSchedules)
			{
				bytes.Add((byte)cardSchedule.ScheduleNo);
			}
			for (int i = 0; i < 68 - cardSchedules.Count; i++)
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
				var sendResult = SendManager.Send(device, (ushort)(pack.Count), (byte)(isNew ? 25 : 26), 0, pack);
				if (sendResult.HasError)
				{
					return new OperationResult<bool>(sendResult.Error);
				}
			}

			using (var skdDatabaseService = new SKDDatabaseService())
			{
				skdDatabaseService.GKCardTranslator.AddOrEdit(device.GetGKIpAddress(), no, card.Number, employeeName);
			}

			return new OperationResult<bool>() { Result = true };
		}

		public OperationResult<bool> RemoveOneCard(GKDevice device, SKDCard card)
		{
			var no = 1;
			using (var skdDatabaseService = new SKDDatabaseService())
			{
				no = skdDatabaseService.GKCardTranslator.GetGKNoByCardNo(device.GetGKIpAddress(), card.Number);
			}
			if (no == -1)
			{
				return new OperationResult<bool>("По номеру карты не найдена порядковая запись");
			}

			var bytes = new List<byte>();
			bytes.Add(0);
			bytes.AddRange(BytesHelper.ShortToBytes((ushort)(no)));
			bytes.Add(0);
			bytes.Add(1);
			var nameBytes = BytesHelper.StringDescriptionToBytes("Удален");
			bytes.AddRange(nameBytes);
			bytes.AddRange(BytesHelper.IntToBytes(999999));
			bytes.Add(0);
			bytes.Add(0);

			for (int i = 0; i < 256 - 42; i++)
			{
				bytes.Add(0);
			}

			var sendResult = SendManager.Send(device, (ushort)(bytes.Count), 26, 0, bytes);
			if (sendResult.HasError)
			{
				return new OperationResult<bool>(sendResult.Error);
			}

			using (var skdDatabaseService = new SKDDatabaseService())
			{
				skdDatabaseService.GKCardTranslator.Remove(device.GetGKIpAddress(), no, card.Number);
			}

			return new OperationResult<bool>() { Result = true };
		}

		public List<GKUser> ActualizeGKUsers(GKDevice device)
		{
			var users = new List<GKUser>();
			for (int i = 1; i <= 65535; i++)
			{
				var bytes = new List<byte>();
				bytes.Add(0);
				bytes.AddRange(BytesHelper.ShortToBytes((ushort)(i)));

				var sendResult = SendManager.Send(device, (ushort)(bytes.Count), 24, 0, bytes);
				if (sendResult.HasError || sendResult.Bytes.Count == 0)
				{
					break;
				}

				var user = new GKUser();
				user.GKNo = BytesHelper.SubstructShort(sendResult.Bytes, 1);
				user.UserType = (GKUserType)sendResult.Bytes[3];
				user.IsActive = sendResult.Bytes[4] == 0;
				user.FIO = BytesHelper.BytesToStringDescription(sendResult.Bytes, 5);
				user.Number = BytesHelper.SubstructInt(sendResult.Bytes, 37);
				users.Add(user);
			}

			using (var skdDatabaseService = new SKDDatabaseService())
			{
				skdDatabaseService.GKCardTranslator.Actualize(device.GetGKIpAddress(), users);
			}

			return users;
		}

		public bool RemoveGKUsers(GKDevice device)
		{
			for (int no = 1; no <= 65535; no++)
			{
				var bytes = new List<byte>();
				bytes.Add(0);
				bytes.AddRange(BytesHelper.ShortToBytes((ushort)(no)));

				var sendResult = SendManager.Send(device, (ushort)(bytes.Count), 24, 0, bytes);
				if (sendResult.HasError || sendResult.Bytes.Count == 0)
				{
					break;
				}

				var userType = (GKUserType)sendResult.Bytes[3];
				if (userType == 0)
				{
					bytes = new List<byte>();
					bytes.Add(0);
					bytes.AddRange(BytesHelper.ShortToBytes((ushort)(no)));
					bytes.Add(0);
					bytes.Add(1);
					var nameBytes = BytesHelper.StringDescriptionToBytes("Удален");
					bytes.AddRange(nameBytes);
					bytes.AddRange(BytesHelper.IntToBytes(999999));
					bytes.Add(0);
					bytes.Add(0);

					for (int i = 0; i < 256 - 42; i++)
					{
						bytes.Add(0);
					}

					sendResult = SendManager.Send(device, (ushort)(bytes.Count), 26, 0, bytes);
					if (sendResult.HasError || sendResult.Bytes.Count == 256)
					{
						//return new OperationResult<bool>(sendResult.Error);
					}
				}
			}

			return true;
		}

		class GKCardSchedule
		{
			public GKDevice Device { get; set; }
			public int ScheduleNo { get; set; }
		}
	}
}
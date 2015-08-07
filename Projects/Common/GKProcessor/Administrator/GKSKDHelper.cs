using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.SKD;
using FiresecClient;
using SKDDriver;
using System.Diagnostics;

namespace GKProcessor
{
	public static class GKSKDHelper
	{
		public static OperationResult<bool> AddOrEditCard(GKControllerCardSchedule controllerCardSchedule, SKDCard card, string employeeName, int gkCardNo = 0, bool isNew = true)
		{
			if (gkCardNo == 0)
			{
				using (var skdDatabaseService = new SKDDriver.DataClasses.DbService())
				{
					gkCardNo = skdDatabaseService.GKCardTranslator.GetFreeGKNo(controllerCardSchedule.ControllerDevice.GetGKIpAddress(), card.Number, out isNew);
				}
			}

			var bytes = new List<byte>();
			bytes.AddRange(BytesHelper.ShortToBytes((ushort)(gkCardNo)));
			bytes.Add((byte)card.GKCardType);
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

			for (int packNo = 0; packNo < 65535; packNo++)
			{
				var startCardScheduleNo = 0;
				var cardScheduleCount = 68;
				if (packNo > 0)
				{
					startCardScheduleNo = 68 + (packNo - 1) * 84;
					cardScheduleCount = 84;
				}
				var cardSchedules = controllerCardSchedule.CardSchedules.Skip(startCardScheduleNo).Take(cardScheduleCount).ToList();

				foreach (var cardSchedule in cardSchedules)
				{
					bytes.AddRange(BytesHelper.ShortToBytes(cardSchedule.Device.GKDescriptorNo));
				}
				for (int i = 0; i < cardScheduleCount - cardSchedules.Count; i++)
				{
					bytes.Add(0);
					bytes.Add(0);
				}
				foreach (var cardSchedule in cardSchedules)
				{
					bytes.Add((byte)cardSchedule.ScheduleNo);
				}
				for (int i = 0; i < cardScheduleCount - cardSchedules.Count; i++)
				{
					bytes.Add(0);
				}

				bytes.Add(0);
				bytes.Add(0);

				if (startCardScheduleNo + cardScheduleCount < controllerCardSchedule.CardSchedules.Count)
				{
					bytes.AddRange(BytesHelper.ShortToBytes((ushort)(packNo + 1)));
				}
				else
				{
					bytes.Add(0);
					bytes.Add(0);
				}

				if (cardSchedules.Count == 0)
					break;
			}

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
				var sendResult = SendManager.Send(controllerCardSchedule.ControllerDevice, (ushort)(pack.Count), (byte)(isNew ? 25 : 26), 0, pack);
				if (sendResult.HasError)
				{
					return OperationResult<bool>.FromError(sendResult.Error);
				}
				if (sendResult.Bytes.Count > 0)
				{
					return OperationResult<bool>.FromError("Неправильный формат при записи карты в прибор");
				}
			}

			using (var skdDatabaseService = new SKDDriver.DataClasses.DbService())
			{
				skdDatabaseService.GKCardTranslator.AddOrEdit(controllerCardSchedule.ControllerDevice.GetGKIpAddress(), gkCardNo, card.Number, employeeName);
			}

			return new OperationResult<bool>(true);
		}

		public static OperationResult<bool> RemoveCard(GKDevice device, SKDCard card)
		{
			var no = 1;
			using (var skdDatabaseService = new SKDDriver.DataClasses.DbService())
			{
				no = skdDatabaseService.GKCardTranslator.GetGKNoByCardNo(device.GetGKIpAddress(), card.Number);
			}
			if (no == -1)
			{
				return OperationResult<bool>.FromError("По номеру карты не найдена порядковая запись");
			}

			var bytes = new List<byte>();
			bytes.Add(0);
			bytes.AddRange(BytesHelper.ShortToBytes((ushort)(no)));
			bytes.Add(0);
			bytes.Add(1);
			var nameBytes = BytesHelper.StringDescriptionToBytes("-");
			bytes.AddRange(nameBytes);
			bytes.AddRange(BytesHelper.IntToBytes(-1));
			bytes.Add(0);
			bytes.Add(0);

			for (int i = 0; i < 256 - 42; i++)
			{
				bytes.Add(0);
			}

			var sendResult = SendManager.Send(device, (ushort)(bytes.Count), 26, 0, bytes);
			if (sendResult.HasError)
			{
				return OperationResult<bool>.FromError(sendResult.Error);
			}

			using (var skdDatabaseService = new SKDDriver.DataClasses.DbService())
			{
				skdDatabaseService.GKCardTranslator.Remove(device.GetGKIpAddress(), no, card.Number);
			}

			return new OperationResult<bool>(true);
		}

		public static OperationResult<List<GKUser>> GetAllUsers(GKDevice device, GKProgressCallback progressCallback)
		{
			progressCallback = GKProcessorManager.StartProgress("Чтение пользователей прибора " + device.PresentationName, "", 65535, true, GKProgressClientType.Administrator);
			var users = new List<GKUser>();

			for (int i = 1; i <= 65535; i++)
			{
				var bytes = new List<byte>();
				bytes.Add(0);
				bytes.AddRange(BytesHelper.ShortToBytes((ushort)(i)));

				var sendResult = SendManager.Send(device, (ushort)(bytes.Count), 24, 0, bytes);
				if (sendResult.HasError)
				{
					return OperationResult<List<GKUser>>.FromError("Во время выполнения операции возникла ошибка", users);
				}
				if (sendResult.Bytes.Count == 0)
				{
					break;
				}

				var user = new GKUser();
				user.GKNo = BytesHelper.SubstructShort(sendResult.Bytes, 1);
				user.CardType = (GKCardType)sendResult.Bytes[3];
				user.IsActive = sendResult.Bytes[4] == 0;
				user.FIO = BytesHelper.BytesToStringDescription(sendResult.Bytes, 5);
				user.Number = (uint)BytesHelper.SubstructInt(sendResult.Bytes, 37);
				users.Add(user);
				if (progressCallback.IsCanceled)
				{
					return OperationResult<List<GKUser>>.FromError("Операция отменена", users);
				}
				GKProcessorManager.DoProgress("Пользователь " + i, progressCallback);
			}

			GKProcessorManager.StopProgress(progressCallback);
			return new OperationResult<List<GKUser>>(users);
		}

        public static int GetUsersCount(GKDevice device)
        {
            int minNo = 1;
            int maxNo = 65535;
            int currentNo = 65535 / 2;
            int delta = currentNo / 2;
            while (maxNo - minNo > 1)
            {
                var bytes = new List<byte>();
                bytes.Add(0);
                bytes.AddRange(BytesHelper.ShortToBytes((ushort)(currentNo)));

                var sendResult = SendManager.Send(device, (ushort)(bytes.Count), 24, 0, bytes);
                if (sendResult.HasError || sendResult.Bytes.Count == 0)
                {
                    maxNo = currentNo;
                    currentNo = currentNo - delta;
                }
                else
                {
                    minNo = currentNo;
                    currentNo = currentNo + delta;
                }
                delta = delta / 2;
                if (delta == 0)
                    delta = 1;
            }
            return minNo;
        }

		public static bool RemoveAllUsers(GKDevice device, int usersCount, GKProgressCallback progressCallback)
		{
			var result = true;
			int cardsCount = 0;
			for (int no = 1; no <= usersCount; no++)
			{
				var bytes = new List<byte>();
				bytes.Add(0);
				bytes.AddRange(BytesHelper.ShortToBytes((ushort)(no)));

				var sendResult = SendManager.Send(device, (ushort)(bytes.Count), 24, 0, bytes);
				if (sendResult.HasError)
				{
					result = false;
					break;
				}
				if (sendResult.Bytes.Count == 0)
				{
					break;
				}

				bytes = new List<byte>();
				bytes.Add(0);
				bytes.AddRange(BytesHelper.ShortToBytes((ushort)(no)));
				bytes.Add(0);
				bytes.Add(1);
				var nameBytes = BytesHelper.StringDescriptionToBytes("-");
				bytes.AddRange(nameBytes);
				bytes.AddRange(BytesHelper.IntToBytes(-1));
				bytes.Add(0);
				bytes.Add(0);
				for (int i = 0; i < 256 - 42; i++)
				{
					bytes.Add(0);
				}

				sendResult = SendManager.Send(device, (ushort)(bytes.Count), 26, 0, bytes);
				if (sendResult.HasError)
				{
					result = false;
					break;
				}
				if (sendResult.Bytes.Count == 256)
				{
					break;
				}

				cardsCount++;
				GKProcessorManager.DoProgress("Пользователь " + no, progressCallback);
			}
			using (var skdDatabaseService = new SKDDriver.DataClasses.DbService())
			{
				GKProcessorManager.DoProgress("Удаление пользователей прибора из БД", progressCallback);
				skdDatabaseService.GKCardTranslator.RemoveAll(device.GetGKIpAddress(), cardsCount);
			}
			return result;
		}

		public static List<GKControllerCardSchedule> GetGKControllerCardSchedules(SKDCard card, AccessTemplate accessTemplate)
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

			var controllerCardSchedules = new List<GKControllerCardSchedule>();
			foreach (var cardSchedule in cardSchedules)
			{
				var gkParent = cardSchedule.Device.GKParent;
				if(gkParent != null)
				{
					var controllerCardSchedule = controllerCardSchedules.FirstOrDefault(x => x.ControllerDevice.UID == gkParent.UID);
					if(controllerCardSchedule == null)
					{
						controllerCardSchedule = new GKControllerCardSchedule();
						controllerCardSchedule.ControllerDevice = gkParent;
						controllerCardSchedules.Add(controllerCardSchedule);
					}
					controllerCardSchedule.CardSchedules.Add(cardSchedule);
				}
			}

			foreach (var controllerUID in card.GKControllerUIDs)
			{
				var controllerDevice = GKManager.Devices.FirstOrDefault(x => x.UID == controllerUID);
				if (controllerDevice != null)
				{
					if (!controllerCardSchedules.Any(x => x.ControllerDevice.UID == controllerUID) && card.GKCardType != GKCardType.Employee)
					{
						var controllerCardSchedule = new GKControllerCardSchedule()
						{
							ControllerDevice = controllerDevice
						};
						controllerCardSchedules.Add(controllerCardSchedule);
					}
				}
			}

			return controllerCardSchedules;
		}
	}

	public class GKCardSchedule
	{
		public GKDevice Device { get; set; }
		public int ScheduleNo { get; set; }
	}

	public class GKControllerCardSchedule
	{
		public GKControllerCardSchedule()
		{
			CardSchedules = new List<GKCardSchedule>();
		}

		public GKDevice ControllerDevice { get; set; }
		public List<GKCardSchedule> CardSchedules { get; set; }
	}
}
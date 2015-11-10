using System;
using System.Collections.Generic;
using System.Linq;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.SKD;
using RubezhClient;
using RubezhDAL;
using System.Diagnostics;

namespace GKProcessor
{
	public static class GKSKDHelper
	{
		public static OperationResult<bool> AddOrEditCard(GKControllerCardSchedule controllerCardSchedule, 
			SKDCard card, string employeeName, int gkCardNo = 0, bool isNew = true, RubezhDAL.DataClasses.DbService dbService = null)
		{
			if (gkCardNo == 0)
			{
				if (dbService == null)
				{
					using (var skdDatabaseService = new RubezhDAL.DataClasses.DbService())
					{
						gkCardNo = skdDatabaseService.GKCardTranslator.GetFreeGKNo(controllerCardSchedule.ControllerDevice.GetGKIpAddress(), card.Number, out isNew);
					}
				}
				else
					gkCardNo = dbService.GKCardTranslator.GetFreeGKNo(controllerCardSchedule.ControllerDevice.GetGKIpAddress(), card.Number, out isNew);
			}

			var user = new GKUser((ushort)gkCardNo, controllerCardSchedule.ControllerDevice);
			user.ExpirationDate = card.EndDate;
			user.Fio = employeeName;
			user.GkLevel = (byte)card.GKLevel;
			user.GkLevelSchedule = (byte)card.GKLevelSchedule;
			user.Password = card.Number;
			user.UserType = card.GKCardType;

			var result = AddOrEditUser(user, isNew, controllerCardSchedule.CardSchedules);
			if (result.HasError)
				return result;

			using (var skdDatabaseService = new RubezhDAL.DataClasses.DbService())
			{
				skdDatabaseService.GKCardTranslator.AddOrEdit(controllerCardSchedule.ControllerDevice.GetGKIpAddress(), gkCardNo, card.Number, employeeName);
			}

			return new OperationResult<bool>(true);
		}

		public static OperationResult<bool> AddOrEditUser(GKUser user, bool isNew = true, List<GKCardSchedule> allCardSchedules = null)
		{
			if (allCardSchedules == null)
				allCardSchedules = new List<GKCardSchedule>();
			
			var bytes = new List<byte>();
			bytes.AddRange(BytesHelper.ShortToBytes(user.GkNo));
			bytes.Add((byte)user.UserType);
			bytes.Add(0);
			var nameBytes = BytesHelper.StringDescriptionToBytes(user.Fio);
			bytes.AddRange(nameBytes);
			bytes.AddRange(BytesHelper.IntToBytes((int)user.Password));
			bytes.Add(user.GkLevel);
			bytes.Add(user.GkLevelSchedule);

			bytes.Add(0);
			bytes.Add(0);

			var secondsPeriod = (new DateTime(user.ExpirationDate.Year, user.ExpirationDate.Month, user.ExpirationDate.Day) - new DateTime(2000, 1, 1)).TotalSeconds;
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
				var cardSchedules = allCardSchedules.Skip(startCardScheduleNo).Take(cardScheduleCount).ToList();
				if (cardSchedules.Count == 0)
					break;
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

				if (startCardScheduleNo + cardScheduleCount < allCardSchedules.Count)
				{
					bytes.AddRange(BytesHelper.ShortToBytes((ushort)(packNo + 1)));
				}
				else
				{
					bytes.Add(0);
					bytes.Add(0);
				}
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
				var sendResult = SendManager.Send(user.GkDevice, (ushort)(pack.Count), (byte)(isNew ? 25 : 26), 0, pack);
				if (sendResult.HasError)
				{
					return OperationResult<bool>.FromError(sendResult.Error);
				}
				if (sendResult.Bytes.Count > 0)
				{
					return OperationResult<bool>.FromError("Неправильный формат при записи карты в прибор");
				}
			}

			return new OperationResult<bool>(true);
		}

		public static OperationResult<bool> RemoveCard(GKDevice device, SKDCard card, RubezhDAL.DataClasses.DbService dbService = null)
		{
			var no = 1;
			if (dbService == null)
			{
				using (var skdDatabaseService = new RubezhDAL.DataClasses.DbService())
				{
					no = skdDatabaseService.GKCardTranslator.GetGKNoByCardNo(device.GetGKIpAddress(), card.Number);
				}
			}
			else
				no = dbService.GKCardTranslator.GetGKNoByCardNo(device.GetGKIpAddress(), card.Number);
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

			using (var skdDatabaseService = new RubezhDAL.DataClasses.DbService())
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

				var gkNo = BytesHelper.SubstructShort(sendResult.Bytes, 1);
				var user = new GKUser(gkNo, device);
				user.UserType = (GKCardType)sendResult.Bytes[3];
				user.IsActive = sendResult.Bytes[4] == 0;
				user.Fio = BytesHelper.BytesToStringDescription(sendResult.Bytes, 5);
				user.Password = (uint)BytesHelper.SubstructInt(sendResult.Bytes, 37);
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
            int minNo = 0;
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
			var cardsCount = RemoveAllUsersInternal(device, usersCount, progressCallback);
			if (cardsCount == 0)
				return false;

			using (var skdDatabaseService = new RubezhDAL.DataClasses.DbService())
			{
				GKProcessorManager.DoProgress("Удаление пользователей прибора из БД", progressCallback);
				skdDatabaseService.GKCardTranslator.RemoveAll(device.GetGKIpAddress(), cardsCount);
			}
			return true;
		}

		public static List<GKControllerCardSchedule> GetGKControllerCardSchedules(SKDCard card, List<CardDoor> accessTemplateDoors)
		{
			var cardSchedules = new List<GKCardSchedule>();

			var cardDoors = new List<CardDoor>();
			cardDoors.AddRange(accessTemplateDoors);
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
					if (door.DoorType != GKDoorType.OneWay && door.ExitDevice != null)
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

		public static OperationResult<bool> WriteAllUsers(List<GKUser> users)
		{
			try
			{
				var devicesWithUsers = users.GroupBy(x => x.GkDevice);
				foreach (var deviceUsers in devicesWithUsers)
				{
					var usersCount = GetUsersCount(deviceUsers.Key);
					RemoveAllUsersInternal(deviceUsers.Key, usersCount);
				}
				foreach (var user in users)
				{
					AddOrEditUser(user);
				}
				return new OperationResult<bool>(true);
			}
			catch (Exception e)
			{
				return OperationResult<bool>.FromError(e.Message);
			}
		}

		/// <summary>
		/// Возвращает число удалённых пропусков
		/// </summary>
		/// <param name="device"></param>
		/// <param name="usersCount"></param>
		/// <param name="progressCallback"></param>
		/// <returns></returns>
		static int RemoveAllUsersInternal(GKDevice device, int usersCount, GKProgressCallback progressCallback = null)
		{
			int cardsCount = 0;
			for (int no = 1; no <= usersCount; no++)
			{
				var bytes = new List<byte>();
				bytes.Add(0);
				bytes.AddRange(BytesHelper.ShortToBytes((ushort)(no)));

				var sendResult = SendManager.Send(device, (ushort)(bytes.Count), 24, 0, bytes);
				if (sendResult.HasError)
				{
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
					break;
				}
				if (sendResult.Bytes.Count == 256)
				{
					break;
				}

				cardsCount++;
				if(progressCallback != null)
					GKProcessorManager.DoProgress("Пользователь " + no, progressCallback);
			}
			return cardsCount;
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
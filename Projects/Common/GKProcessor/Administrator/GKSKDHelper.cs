using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.SKD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

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

			var user = new GKUser
			{
				GkNo = (ushort)gkCardNo,
				ExpirationDate = card.EndDate,
				Fio = employeeName,
				GkLevel = (byte)card.GKLevel,
				GkLevelSchedule = (byte)card.GKLevelSchedule,
				Password = card.Number,
				UserType = card.GKCardType
			};

			var result = AddOrEditUser(user, controllerCardSchedule.ControllerDevice, isNew, controllerCardSchedule.CardSchedules);
			if (result.HasError)
				return result;

			using (var skdDatabaseService = new RubezhDAL.DataClasses.DbService())
			{
				skdDatabaseService.GKCardTranslator.AddOrEdit(controllerCardSchedule.ControllerDevice.GetGKIpAddress(), gkCardNo, card.Number, employeeName);
			}

			return new OperationResult<bool>(true);
		}

		public static OperationResult<bool> AddOrEditUser(GKUser user, GKDevice device, bool isNew = true, List<GKCardSchedule> allCardSchedules = null)
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

			if (allCardSchedules.Count == 0)
			{
				var bytesCount = bytes.Count;
				for (int i = 0; i < 256 - bytesCount; i++)
				{
					bytes.Add(0);
				}
			}
			else
			{
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
					{
						break;
					}
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
				var sendResult = SendManager.Send(device, (ushort)(pack.Count), (byte)(isNew ? 25 : 26), 0, pack);
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

		public static OperationResult<List<GKUser>> GetAllUsers(GKDevice device, GKProgressCallback progressCallback, Guid clientUID)
		{
			progressCallback = GKProcessorManager.StartProgress("Чтение пользователей прибора " + device.PresentationName, "", 65535, true, GKProgressClientType.Administrator, clientUID);
			var users = new List<GKUser>();
			for (int i = 1; i <= 65535; i++)
			{
				byte j = 0;
				bool hasResponse = true;
				var bytePacks = new List<List<byte>>();
				while (true)
				{
					var bytes = new List<byte>();
					bytes.Add(j);
					bytes.AddRange(BytesHelper.ShortToBytes((ushort)(i)));
					var sendResult = SendManager.Send(device, (ushort)(bytes.Count), 24, 0, bytes);
					if (sendResult.HasError && device.DriverType != GKDriverType.GKMirror)
					{
						return OperationResult<List<GKUser>>.FromError("Во время выполнения операции возникла ошибка", users);
					}
					if (sendResult.Bytes.Count == 0)
					{
						if (j == 0)
							hasResponse = false;
						break;
					}
					bytePacks.Add(sendResult.Bytes);
					j++;
				}
				if (!hasResponse)
					break;
				int packIndex = -1;
				GKUser user = null;
				foreach (var pack in bytePacks)
				{
					packIndex++;
					if (packIndex == 0)
					{
						var gkNo = BytesHelper.SubstructShort(pack, 1);
						user = new GKUser
						{
							GkNo = gkNo,
							UserType = (GKCardType)pack[3],
							IsActive = pack[4] == 0,
							Fio = BytesHelper.BytesToStringDescription(pack, 5),
							Password = (uint)BytesHelper.SubstructInt(pack, 37),
							GkLevel = pack[41],
							GkLevelSchedule = pack[42]
						};
						var totalSeconds = BytesHelper.SubstructInt(pack, 45);
						user.ExpirationDate = new DateTime(2000, 1, 1);
						user.ExpirationDate = user.ExpirationDate.AddSeconds(totalSeconds);
						for (int l = 0; l < 68; l++)
						{
							var deviceNo = BytesHelper.SubstructShort(pack, 49 + l * 2);
							if (deviceNo == 0)
								break;
							var scheduleNo = pack[185 + l];
							user.Descriptors.Add(new GKUserDescriptor { DescriptorNo = deviceNo, ScheduleNo = scheduleNo });
						}
						users.Add(user);
					}
					else
					{
						for (int l = 0; l < 84; l++)
						{
							var deviceNo = BytesHelper.SubstructShort(pack, 1 + l * 2);
							if (deviceNo == 0)
								break;
							var scheduleNo = pack[169 + l];
							user.Descriptors.Add(new GKUserDescriptor { DescriptorNo = deviceNo, ScheduleNo = scheduleNo });
						}
					}
				}

				if (progressCallback.IsCanceled)
				{
					return OperationResult<List<GKUser>>.FromError("Операция отменена", users);
				}
				GKProcessorManager.DoProgress("Пользователь " + i, progressCallback, clientUID);
			}
			GKProcessorManager.StopProgress(progressCallback, clientUID);
			return new OperationResult<List<GKUser>>(users);
		}

		public static OperationResult<List<GKUser>> GetAllUsersTest(GKDevice device, GKProgressCallback progressCallback)
		{
			progressCallback = GKProcessorManager.StartProgress("Чтение пользователей прибора " + device.PresentationName, "", 100, true, GKProgressClientType.Administrator);
			var users = new List<GKUser>();

			for (ushort i = 1; i <= 100; i++)
			{
				Thread.Sleep(100);
				var user = new GKUser
				{
					GkNo = i,
					UserType = GKCardType.Administrator,
					IsActive = true,
					Fio = "Пользователь " + i,
					Password = i
				};
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

		public static OperationResult<int> GetUsersCount(GKDevice device, GKProgressCallback progressCallback = null)
		{
			if (progressCallback != null)
				progressCallback = GKProcessorManager.StartProgress("Подсчёт числа пользователей на приборе", "", 16, true, GKProgressClientType.Administrator);
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
				if (progressCallback != null)
				{
					GKProcessorManager.DoProgress("", progressCallback);
					if (progressCallback.IsCanceled)
						return OperationResult<int>.FromError("Операция отменена");
				}
			}
			if (progressCallback != null)
				GKProcessorManager.StopProgress(progressCallback);
			return new OperationResult<int>(minNo);
		}

		public static bool RemoveAllUsers(GKDevice device, int usersCount, GKProgressCallback progressCallback, Guid clientUID)
		{
			var removeAllUsersInternalResult = RemoveAllUsersInternal(device, usersCount, progressCallback);
			if (removeAllUsersInternalResult.HasError)
				return false;
			var cardsCount = removeAllUsersInternalResult.Result;
			if (cardsCount == 0)
				return false;

			using (var skdDatabaseService = new RubezhDAL.DataClasses.DbService())
			{
				GKProcessorManager.DoProgress("Удаление пользователей прибора из БД", progressCallback, clientUID);
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
				if (gkParent != null)
				{
					var controllerCardSchedule = controllerCardSchedules.FirstOrDefault(x => x.ControllerDevice.UID == gkParent.UID);
					if (controllerCardSchedule == null)
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

		public static OperationResult<bool> RewritePmfUsers(Guid clientUid, Guid deviceUID, List<GKUser> users, GKProgressCallback progressCallback)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				int oldUsersCount = -1;
				var getUsersCountResult = GetUsersCount(device, progressCallback);
				if (getUsersCountResult.HasError)
					return OperationResult<bool>.FromError(getUsersCountResult.Error);
				oldUsersCount = getUsersCountResult.Result;
				var removeAllUsersInternalResult = RemoveAllUsersInternal(device, oldUsersCount, progressCallback);
				if (removeAllUsersInternalResult.HasError)
					return OperationResult<bool>.FromError(removeAllUsersInternalResult.Error);
				progressCallback = GKProcessorManager.StartProgress("Запись пользователей", "", users.Count, true, GKProgressClientType.Administrator);
				ushort gkNo = 0;
				foreach (var user in users.OrderBy(x => x.Password))
				{
					user.GkNo = gkNo++;
					AddOrEditUser(user, device);
					if (progressCallback.IsCanceled)
					{
						return OperationResult<bool>.FromError("Операция отменена");
					}
					GKProcessorManager.DoProgress("Пользователь " + gkNo, progressCallback);
				}
				GKProcessorManager.StopProgress(progressCallback);
				return new OperationResult<bool>(true);
			}
			return OperationResult<bool>.FromError("Устройство не найдено");
		}

		/// <summary>
		/// Возвращает число удалённых пропусков
		/// </summary>
		/// <param name="device"></param>
		/// <param name="usersCount"></param>
		/// <param name="progressCallback"></param>
		/// <returns></returns>
		static OperationResult<int> RemoveAllUsersInternal(GKDevice device, int usersCount, GKProgressCallback progressCallback = null)
		{
			progressCallback = GKProcessorManager.StartProgress("Удаление пользователей", "", usersCount, true, GKProgressClientType.Administrator);
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
				if (progressCallback != null)
				{
					GKProcessorManager.DoProgress("Пользователь " + no, progressCallback);
					if (progressCallback.IsCanceled)
						return OperationResult<int>.FromError("Операция отменена", cardsCount);
				}
			}
			GKProcessorManager.StopProgress(progressCallback);
			return new OperationResult<int>(cardsCount);
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
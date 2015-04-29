using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.SKD;
using FiresecClient;
using SKDDriver;

namespace GKProcessor
{
	public class GKSKDHelper
	{
		public OperationResult<bool> AddOreditCard(GKControllerCardSchedule controllerCardSchedule, SKDCard card, string employeeName, int gkCardNo = 0)
		{
			var isNew = true;
			gkCardNo = 1;
			using (var skdDatabaseService = new SKDDatabaseService())
			{
				gkCardNo = skdDatabaseService.GKCardTranslator.GetFreeGKNo(controllerCardSchedule.ControllerDevice.GetGKIpAddress(), card.Number, out isNew);
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

			foreach (var cardSchedule in controllerCardSchedule.CardSchedules)
			{
				bytes.AddRange(BytesHelper.ShortToBytes(cardSchedule.Device.GKDescriptorNo));
			}
			for (int i = 0; i < 68 - controllerCardSchedule.CardSchedules.Count; i++)
			{
				bytes.Add(0);
				bytes.Add(0);
			}
			foreach (var cardSchedule in controllerCardSchedule.CardSchedules)
			{
				bytes.Add((byte)cardSchedule.ScheduleNo);
			}
			for (int i = 0; i < 68 - controllerCardSchedule.CardSchedules.Count; i++)
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
				var sendResult = SendManager.Send(controllerCardSchedule.ControllerDevice, (ushort)(pack.Count), (byte)(isNew ? 25 : 26), 0, pack);
				if (sendResult.HasError)
				{
					return new OperationResult<bool>(sendResult.Error);
				}
				if (sendResult.Bytes.Count > 0)
				{
					return new OperationResult<bool>("Неправильный формат при записи карты в прибор");
				}
			}

			using (var skdDatabaseService = new SKDDatabaseService())
			{
				skdDatabaseService.GKCardTranslator.AddOrEdit(controllerCardSchedule.ControllerDevice.GetGKIpAddress(), gkCardNo, card.Number, employeeName);
			}

			return new OperationResult<bool>() { Result = true };
		}

		public OperationResult<bool> RemoveCard(GKDevice device, SKDCard card)
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
				return new OperationResult<bool>(sendResult.Error);
			}

			using (var skdDatabaseService = new SKDDatabaseService())
			{
				skdDatabaseService.GKCardTranslator.Remove(device.GetGKIpAddress(), no, card.Number);
			}

			return new OperationResult<bool>() { Result = true };
		}

		public OperationResult<bool> EditCard(SKDCard oldCard, AccessTemplate oldAccessTemplate, SKDCard newCard, AccessTemplate newAccessTemplate)
		{
			var controllerCardSchedules_ToDelete = GetGKControllerCardSchedules(oldCard, oldAccessTemplate);
			var controllerCardSchedules_ToEdit = GetGKControllerCardSchedules(newCard, newAccessTemplate);
			foreach (var controllerCardSchedule_ToEdit in controllerCardSchedules_ToEdit)
			{
				controllerCardSchedules_ToDelete.RemoveAll(x => x.ControllerDevice.UID == controllerCardSchedule_ToEdit.ControllerDevice.UID);
			}

			return new OperationResult<bool>() { Result = true };
		}

		public OperationResult<List<GKUser>> GetAllUsers(GKDevice device)
		{
			var progressCallback = GKProcessorManager.StartProgress("Чтение пользователей прибора " + device.PresentationName, "", 65535, true, GKProgressClientType.Administrator);
			var users = new List<GKUser>();

			for (int i = 1; i <= 65535; i++)
			{
				var bytes = new List<byte>();
				bytes.Add(0);
				bytes.AddRange(BytesHelper.ShortToBytes((ushort)(i)));

				var sendResult = SendManager.Send(device, (ushort)(bytes.Count), 24, 0, bytes);
				if (sendResult.HasError)
				{
					return new OperationResult<List<GKUser>>("Во время выполнения операции возникла ошибка") { Result = users };
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
					return new OperationResult<List<GKUser>>("Операция отменена") { Result = users };
				}
				GKProcessorManager.DoProgress("Пользователь " + i, progressCallback);
			}

			GKProcessorManager.StopProgress(progressCallback);
			return new OperationResult<List<GKUser>>() { Result = users };
		}

		public bool RemoveAllUsers(GKDevice device, GKProgressCallback progressCallback)
		{
			var result = true;
			int cardsCount = 0;
			for (int no = 1; no <= 65535; no++)
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
			using (var skdDatabaseService = new SKDDatabaseService())
			{
				GKProcessorManager.DoProgress("Удаление пользователей прибора из БД", progressCallback);
				skdDatabaseService.GKCardTranslator.RemoveAll(device.GetGKIpAddress(), cardsCount);
			}
			return result;
		}

		public List<GKControllerCardSchedule> GetGKControllerCardSchedules(SKDCard card, AccessTemplate accessTemplate)
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
using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.SKD;
using SKDDriver;

namespace GKProcessor
{
	public static class GKSKDHelper
	{
		public static OperationResult<bool> AddOrEditCard(GKControllerCardSchedule controllerCardSchedule, SKDCard card, string employeeName, int gkCardNo = 0)
		{
			var isNew = true;

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

			return new OperationResult<bool>(true);
		}

		//public static OperationResult<List<GKUser>> GetAllUsers(GKDevice device)
		//{
		//	var progressCallback = GKProcessorManager.StartProgress("Чтение пользователей прибора " + device.PresentationName, "", 65535, true, SKDProgressClientType.Administrator);
		//	var users = new List<GKUser>();

		//	for (int i = 1; i <= 65535; i++)
		//	{
		//		var bytes = new List<byte>();
		//		bytes.Add(0);
		//		bytes.AddRange(BytesHelper.ShortToBytes((ushort)(i)));

		//		var sendResult = SendManager.Send(device, (ushort)(bytes.Count), 24, 0, bytes);
		//		if (sendResult.HasError)
		//		{
		//			return OperationResult<List<GKUser>>.FromError("Во время выполнения операции возникла ошибка", users);
		//		}
		//		if (sendResult.Bytes.Count == 0)
		//		{
		//			break;
		//		}

		//		var user = new GKUser();
		//		user.GKNo = BytesHelper.SubstructShort(sendResult.Bytes, 1);
		//		user.CardType = (GKCardType)sendResult.Bytes[3];
		//		user.IsActive = sendResult.Bytes[4] == 0;
		//		user.FIO = BytesHelper.BytesToStringDescription(sendResult.Bytes, 5);
		//		user.Number = (uint)BytesHelper.SubstructInt(sendResult.Bytes, 37);
		//		users.Add(user);
		//		if (progressCallback.IsCanceled)
		//		{
		//			return OperationResult<List<GKUser>>.FromError("Операция отменена", users);
		//		}
		//		GKProcessorManager.DoProgress("Пользователь " + i, progressCallback);
		//	}

		//	GKProcessorManager.StopProgress(progressCallback);
		//	return new OperationResult<List<GKUser>>(users);
		//}

		public static bool RemoveAllUsers(GKDevice device, SKDProgressCallback progressCallback)
		{
			var result = true;
			int cardsCount = 0;
			for (int no = 1; no <= 65535; no++)
			{
				var bytes = new List<byte>();
				bytes.Add(0);
				bytes.AddRange(BytesHelper.ShortToBytes((ushort)(no)));

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

				cardsCount++;
				GKProcessorManager.DoProgress("Пользователь " + no, progressCallback);
			}
			using (var skdDatabaseService = new SKDDatabaseService())
			{
				GKProcessorManager.DoProgress("Удаление пользователей прибора из БД", progressCallback);
			}
			return result;
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
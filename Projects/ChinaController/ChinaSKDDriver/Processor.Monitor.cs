using System;
using System.Linq;
using FiresecAPI;
using FiresecAPI.SKD;

namespace ChinaSKDDriver
{
	public static partial class Processor
	{
		public static SKDStates SKDGetStates()
		{
			var skdStates = new SKDStates();
			foreach (var device in SKDManager.Devices)
			{
				skdStates.DeviceStates.Add(device.State);
			}
			foreach (var zone in SKDManager.Zones)
			{
				skdStates.ZoneStates.Add(zone.State);
			}
			return skdStates;
		}

		public static OperationResult<bool> OpenDoor(Guid deviceUID)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return new OperationResult<bool>("Нет связи с контроллером");

				var result = deviceProcessor.Wrapper.OpenDoor(deviceProcessor.Device.IntAddress);
				if (result)
					return new OperationResult<bool>() { Result = true };
				else
					return new OperationResult<bool>("Ошибка при выполнении операции в приборе");
			}
			return new OperationResult<bool>("Не найден контроллер в конфигурации");
		}

		public static OperationResult<bool> CloseDoor(Guid deviceUID)
		{
			var deviceProcessor = DeviceProcessors.FirstOrDefault(x => x.Device.UID == deviceUID);
			if (deviceProcessor != null)
			{
				if (!deviceProcessor.IsConnected)
					return new OperationResult<bool>("Нет связи с контроллером");

				var result = deviceProcessor.Wrapper.CloseDoor(deviceProcessor.Device.IntAddress);
				if (result)
					return new OperationResult<bool>() { Result = true };
				else
					return new OperationResult<bool>("Ошибка при выполнении операции в приборе");
			}
			return new OperationResult<bool>("Не найден контроллер в конфигурации");
		}

		public static CardWriter AddCard(SKDCard skdCard)
		{
			var cardWriter = new CardWriter();
			var result = cardWriter.AddCard(skdCard);
			return cardWriter;
		}

		public static CardWriter EditCard(SKDCard skdCard)
		{
			var cardWriter = new CardWriter();
			//var result = cardWriter.AddCard(skdCard);
			return cardWriter;
		}

		public static CardWriter DeleteCard(SKDCard skdCard)
		{
			var cardWriter = new CardWriter();
			//var result = cardWriter.AddCard(skdCard);
			return cardWriter;
		}
	}
}
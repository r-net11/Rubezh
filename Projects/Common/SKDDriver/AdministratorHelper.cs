using System.Collections.Generic;
using FiresecAPI;
using FiresecAPI.SKD;

namespace SKDDriver
{
	internal static class AdministratorHelper
	{
		public static string GetInfo(SKDDevice device)
		{
			var bytes = new List<byte>();
			bytes.Add(1);
			var result = SKDDeviceProcessor.SendBytes(device, bytes);
			if (result.HasError)
				return result.Error;
			return "Версия " + result.Bytes[0].ToString();
		}

		public static bool SynchroniseTime(SKDDevice device)
		{
			var bytes = new List<byte>();
			bytes.Add(2);
			var result = SKDDeviceProcessor.SendBytes(device, bytes);
			return !result.HasError;
		}

		public static OperationResult<bool> WriteConfig(SKDDevice device)
		{
			if (device.DriverType != SKDDriverType.Controller)
				return new OperationResult<bool>("Типом устройства должен быть контроллер");

			var bytes = new List<byte>();
			bytes.Add(5);
			foreach (var childDevice in device.Children)
			{
				bytes.Add((byte)childDevice.DriverType);
				bytes.Add((byte)childDevice.IntAddress);
			}
			var result = SKDDeviceProcessor.SendBytes(device, bytes);
			if (result.HasError)
				return new OperationResult<bool>("Устройство недоступно");

			return new OperationResult<bool>() { Result = true };
		}

		public static OperationResult<bool> UpdateFirmware(SKDDevice device)
		{
			if (device.DriverType != SKDDriverType.Controller)
				return new OperationResult<bool>("Типом устройства должен быть контроллер");

			var bytes = new List<byte>();
			bytes.Add(6);
			for (int i = 0; i < 100; i++)
			{
				bytes.Add((byte)i);
				bytes.Add((byte)i);
			}
			var result = SKDDeviceProcessor.SendBytes(device, bytes);
			if (result.HasError)
				return new OperationResult<bool>("Устройство недоступно");

			return new OperationResult<bool>() { Result = true };
		}

		public static OperationResult<bool> WriteAllIdentifiers(SKDDevice device)
		{
			if (device.DriverType != SKDDriverType.Controller)
				return new OperationResult<bool>("Типом устройства должен быть контроллер");

			var bytes = new List<byte>();
			bytes.Add(7);

			var cardFilter = new CardFilter();
			var operationResult = SKDDatabaseService.CardTranslator.Get(cardFilter);
			if (operationResult.HasError)
				return new OperationResult<bool>(operationResult.Error);

			foreach (var readerDevice in device.Children)
			{
				if (readerDevice.DriverType == SKDDriverType.Reader)
				{
					bytes.Add((byte)readerDevice.IntAddress);
					foreach (var card in operationResult.Result)
					{
						foreach (var cardZone in card.CardZones)
						{
							if (readerDevice.ZoneUID == cardZone.DoorUID)
							{
								bytes.Add((byte)card.Series);
								bytes.Add((byte)card.Number);

								bytes.Add((byte)cardZone.IntervalType);
								//bytes.Add((byte)cardZone.IntervalUID);
								bytes.Add((byte)(cardZone.IsAntiPassback ? 1 : 0));
								bytes.Add((byte)(cardZone.IsComission ? 1 : 0));
							}
						}
					}
				}
			}
			var result = SKDDeviceProcessor.SendBytes(device, bytes);
			if (result.HasError)
				return new OperationResult<bool>("Устройство недоступно");

			return new OperationResult<bool>() { Result = true };
		}
	}
}
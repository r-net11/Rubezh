using System;
using System.Collections.Generic;
using System.Linq;
using ChinaSKDDriverAPI;
using FiresecAPI.GK;
using FiresecAPI.SKD;
using FiresecAPI;

namespace ChinaSKDDriver
{
	public static partial class Processor
	{
		public static SKDConfiguration SKDConfiguration { get; private set; }
		public static List<DeviceProcessor> DeviceProcessors { get; private set; }
		public static event Action<SKDCallbackResult> SKDCallbackResultEvent;

		static Processor()
		{
#if DEBUG
			try
			{
				System.IO.File.Copy(@"D:\Projects\Projects\ChinaController\CPPWrapper\Bin\CPPWrapper.dll", @"D:\Projects\Projects\FiresecService\bin\Debug\CPPWrapper.dll", true);
			}
			catch { }
#endif
		}

		public static void DoCallback(SKDCallbackResult callbackResult)
		{
			if (Processor.SKDCallbackResultEvent != null)
				Processor.SKDCallbackResultEvent(callbackResult);
		}

		public static void Run(SKDConfiguration skdConfiguration)
		{
			DeviceProcessors = new List<DeviceProcessor>();
			SKDConfiguration = skdConfiguration;

			try
			{
				ChinaSKDDriverNativeApi.NativeWrapper.WRAP_Initialize();
			}
			catch { }

			foreach (var device in skdConfiguration.Devices)
			{
				device.State = new SKDDeviceState();
				device.State.UID = device.UID;
				device.State.StateClass = XStateClass.Unknown;
			}
			skdConfiguration.RootDevice.State.StateClass = XStateClass.Norm;

			foreach (var device in skdConfiguration.RootDevice.Children)
			{
				var deviceProcessor = new DeviceProcessor(device);
				DeviceProcessors.Add(deviceProcessor);
				deviceProcessor.Run();
			}
		}

		public static void Stop()
		{
			foreach (var deviceProcessor in DeviceProcessors)
			{
				deviceProcessor.Wrapper.StopWatcher();
			}
		}

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
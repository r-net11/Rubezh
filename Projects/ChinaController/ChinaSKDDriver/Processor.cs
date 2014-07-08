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

		#region Callback
		public static List<GKProgressCallback> GKProgressCallbacks = new List<GKProgressCallback>();

		public static void CancelGKProgress(Guid progressCallbackUID, string userName)
		{
			var progressCallback = GKProgressCallbacks.FirstOrDefault(x => x.UID == progressCallbackUID);
			if (progressCallback != null)
			{
				progressCallback.IsCanceled = true;
				progressCallback.CancelizationDateTime = DateTime.Now;
				StopProgress(progressCallback);
				//AddGKMessage(GlobalEventNameEnum.Отмена_операции, progressCallback.Title, null, userName, true);
			}
		}

		public static GKProgressCallback StartProgress(string title, string text, int stepCount, bool canCancel, GKProgressClientType progressClientType)
		{
			var gkProgressCallback = new GKProgressCallback()
			{
				GKProgressCallbackType = GKProgressCallbackType.Start,
				Title = title,
				Text = text,
				StepCount = stepCount,
				CanCancel = canCancel,
				GKProgressClientType = progressClientType
			};
			GKProgressCallbacks.Add(gkProgressCallback);
			OnGKCallbackResult(gkProgressCallback);
			return gkProgressCallback;
		}

		public static void DoProgress(string text, GKProgressCallback progressCallback)
		{
			var gkProgressCallback = new GKProgressCallback()
			{
				UID = progressCallback.UID,
				LastActiveDateTime = DateTime.Now,
				GKProgressCallbackType = GKProgressCallbackType.Progress,
				Title = progressCallback.Title,
				Text = text,
				StepCount = progressCallback.StepCount,
				CanCancel = progressCallback.CanCancel,
				GKProgressClientType = progressCallback.GKProgressClientType
			};
			OnGKCallbackResult(gkProgressCallback);
		}

		public static void StopProgress(GKProgressCallback progressCallback)
		{
			var gkProgressCallback = new GKProgressCallback()
			{
				UID = progressCallback.UID,
				LastActiveDateTime = DateTime.Now,
				GKProgressCallbackType = GKProgressCallbackType.Stop,
			};
			GKProgressCallbacks.Remove(gkProgressCallback);
			OnGKCallbackResult(gkProgressCallback);
		}

		static void OnGKCallbackResult(GKProgressCallback gkProgressCallback)
		{
			GKProgressCallbacks.RemoveAll(x => x.IsCanceled && (DateTime.Now - x.CancelizationDateTime).TotalMinutes > 5);
			if (gkProgressCallback.GKProgressCallbackType == GKProgressCallbackType.Stop || !gkProgressCallback.IsCanceled)
			{
				if (GKProgressCallbackEvent != null)
					GKProgressCallbackEvent(gkProgressCallback);
			}
		}
		public static event Action<GKProgressCallback> GKProgressCallbackEvent;

		public static void OnGKCallbackResult(GKCallbackResult gkCallbackResult)
		{
			if (gkCallbackResult.JournalItems.Count +
				gkCallbackResult.GKStates.DeviceStates.Count +
				gkCallbackResult.GKStates.ZoneStates.Count +
				gkCallbackResult.GKStates.DirectionStates.Count +
				gkCallbackResult.GKStates.PumpStationStates.Count +
				gkCallbackResult.GKStates.MPTStates.Count +
				gkCallbackResult.GKStates.DelayStates.Count +
				gkCallbackResult.GKStates.PimStates.Count +
				gkCallbackResult.GKStates.DeviceMeasureParameters.Count > 0)
			{
				if (GKCallbackResultEvent != null)
					GKCallbackResultEvent(gkCallbackResult);
			}
		}
		public static event Action<GKCallbackResult> GKCallbackResultEvent;
		#endregion
	}
}
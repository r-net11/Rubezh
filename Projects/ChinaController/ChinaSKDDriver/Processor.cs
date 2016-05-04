using System;
using System.Collections.Generic;
using ChinaSKDDriverAPI;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.Journal;
using System.Linq;
using FiresecAPI.SKD;
using FiresecAPI.SKD.Device;

namespace ChinaSKDDriver
{
	public static partial class Processor
	{
		public static List<DeviceProcessor> DeviceProcessors { get; private set; }

		public static event Action<SKDStates> StatesChangedEvent;

		public static void OnStatesChanged(SKDStates skdStates)
		{
			if (Processor.StatesChangedEvent != null)
				Processor.StatesChangedEvent(skdStates);
		}

		public static event Action<JournalItem> NewJournalItem;

		private static void OnNewJournalItem(JournalItem journalItem)
		{
			if (NewJournalItem != null)
				NewJournalItem(journalItem);
		}

		public static event Action<SKDDeviceSearchInfo> NewSearchDevice;
		static void OnNewSearchDevice(DeviceSearchInfo deviceSearchInfo)
		{
			var skdDeviceSearchInfo = new SKDDeviceSearchInfo
			{
				DeviceType = (SKDDeviceType)deviceSearchInfo.DeviceType,
				Gateway = deviceSearchInfo.Gateway,
				IpAddress = deviceSearchInfo.IpAddress,
				Mac = deviceSearchInfo.Mac,
				Port = deviceSearchInfo.Port,
				Submask = deviceSearchInfo.Submask
			};

			var temp = NewSearchDevice;
			if (temp != null)
				temp(skdDeviceSearchInfo);
		}

		public static void Start()
		{
			Wrapper.Initialize();

			DeviceProcessors = new List<DeviceProcessor>();
			foreach (var device in SKDManager.SKDConfiguration.RootDevice.Children)
			{
				var deviceProcessor = new DeviceProcessor(device);
				DeviceProcessors.Add(deviceProcessor);
				deviceProcessor.Start();
			}

			Wrapper.NewSearchDevice -= OnNewSearchDevice;
			Wrapper.NewSearchDevice += OnNewSearchDevice;
		}

		public static void Stop()
		{
			Wrapper.NewSearchDevice -= new Action<DeviceSearchInfo>(OnNewSearchDevice);

			if (DeviceProcessors != null)
				foreach (var deviceProcessor in DeviceProcessors)
					deviceProcessor.Stop();
			Wrapper.Deinitialize();
		}

		#region Callback

		public static List<SKDProgressCallback> ProgressCallbacks = new List<SKDProgressCallback>();

		public static void CancelProgress(Guid progressCallbackUID, string userName)
		{
			var progressCallback = ProgressCallbacks.FirstOrDefault(x => x.UID == progressCallbackUID);
			if (progressCallback != null)
			{
				progressCallback.IsCanceled = true;
				progressCallback.CancelizationDateTime = DateTime.Now;
				StopProgress(progressCallback);

				var journalItem = new JournalItem();
				journalItem.JournalEventNameType = JournalEventNameType.Отмена_операции;
				journalItem.DescriptionText = progressCallback.Title;
				journalItem.UserName = userName;
				OnNewJournalItem(journalItem);
			}
		}

		public static SKDProgressCallback StartProgress(string title, string text, int stepCount, bool canCancel, SKDProgressClientType progressClientType)
		{
			var SKDProgressCallback = new SKDProgressCallback()
			{
				SKDProgressCallbackType = SKDProgressCallbackType.Start,
				Title = title,
				Text = text,
				StepCount = stepCount,
				CanCancel = canCancel,
				SKDProgressClientType = progressClientType
			};
			ProgressCallbacks.Add(SKDProgressCallback);
			OnSKDCallbackResult(SKDProgressCallback);
			return SKDProgressCallback;
		}

		public static void DoProgress(string text, SKDProgressCallback progressCallback)
		{
			var SKDProgressCallback = new SKDProgressCallback()
			{
				UID = progressCallback.UID,
				LastActiveDateTime = DateTime.Now,
				SKDProgressCallbackType = SKDProgressCallbackType.Progress,
				Title = progressCallback.Title,
				Text = text,
				StepCount = progressCallback.StepCount,
				CanCancel = progressCallback.CanCancel,
				SKDProgressClientType = progressCallback.SKDProgressClientType
			};
			OnSKDCallbackResult(SKDProgressCallback);
		}

		public static void StopProgress(SKDProgressCallback progressCallback)
		{
			var SKDProgressCallback = new SKDProgressCallback()
			{
				UID = progressCallback.UID,
				LastActiveDateTime = DateTime.Now,
				SKDProgressCallbackType = SKDProgressCallbackType.Stop,
			};
			ProgressCallbacks.Remove(SKDProgressCallback);
			OnSKDCallbackResult(SKDProgressCallback);
		}

		private static void OnSKDCallbackResult(SKDProgressCallback SKDProgressCallback)
		{
			ProgressCallbacks.RemoveAll(x => x.IsCanceled && (DateTime.Now - x.CancelizationDateTime).TotalMinutes > 5);
			if (SKDProgressCallback.SKDProgressCallbackType == SKDProgressCallbackType.Stop || !SKDProgressCallback.IsCanceled)
			{
				if (SKDProgressCallbackEvent != null)
					SKDProgressCallbackEvent(SKDProgressCallback);
			}
		}

		public static event Action<SKDProgressCallback> SKDProgressCallbackEvent;

		public static void OnSKDCallbackResult(SKDCallbackResult SKDCallbackResult)
		{
			if (SKDCallbackResult.JournalItems.Count +
				SKDCallbackResult.SKDStates.DeviceStates.Count +
				SKDCallbackResult.SKDStates.ZoneStates.Count > 0)
			{
				if (SKDCallbackResultEvent != null)
					SKDCallbackResultEvent(SKDCallbackResult);
			}
		}

		public static event Action<SKDCallbackResult> SKDCallbackResultEvent;

		#endregion Callback
	}
}
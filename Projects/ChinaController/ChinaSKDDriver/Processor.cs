using System;
using System.Collections.Generic;
using ChinaSKDDriverAPI;
using Common;
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

		static Processor()
		{
#if DEBUG
			try
			{
				//System.IO.File.Copy(@"..\..\..\CPPWrapper\Bin\CPPWrapper.dll", @"CPPWrapper.dll", true);
			}
			catch { }
#endif

			try
			{
				//Wrapper.WrapInitialize();
			}
			catch { }
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

			Wrapper.NewSearchDevice -= new Action<DeviceSearchInfo>(OnNewSearchDevice);
			Wrapper.NewSearchDevice += new Action<DeviceSearchInfo>(OnNewSearchDevice);
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
#if DEBUG
			var logMessage = String.Format("Выполнение Process.StartProgress(title={0}, text={1}, stepCount={2}, canCancel={3})",
				String.IsNullOrEmpty(title) ? "null" : String.Format("'{0}'", title),
				String.IsNullOrEmpty(text) ? "null" : String.Format("'{0}'", text),
				stepCount,
				canCancel);
			Logger.Info(logMessage);
#endif
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
#if DEBUG
			logMessage = String.Format("Завершение Process.StartProgress(title={0}, text={1}, stepCount={2}, canCancel={3})",
				String.IsNullOrEmpty(title) ? "null" : String.Format("'{0}'", title),
				String.IsNullOrEmpty(text) ? "null" : String.Format("'{0}'", text),
				stepCount,
				canCancel);
			Logger.Info(logMessage);
#endif
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

		public static void UpdateProgressCancelStatus(SKDProgressCallback progressCallback)
		{
			var newProgressCallback = new SKDProgressCallback()
			{
				UID = progressCallback.UID,
				LastActiveDateTime = DateTime.Now,
				SKDProgressCallbackType = SKDProgressCallbackType.UpdateCancelStatus,
				Title = progressCallback.Title,
				Text = progressCallback.Text,
				StepCount = progressCallback.StepCount,
				CanCancel = progressCallback.CanCancel,
				SKDProgressClientType = progressCallback.SKDProgressClientType
			};
			OnSKDCallbackResult(newProgressCallback);
		}

		private static void OnSKDCallbackResult(SKDProgressCallback SKDProgressCallback)
		{
#if DEBUG
			Logger.Info("Начало выполнения Processor.OnSKDCallbackResult");
			ProgressCallbacks.Where(x => x.IsCanceled && (DateTime.Now - x.CancelizationDateTime).TotalMinutes > 5).ForEach(
				x => Logger.Info(String.Format("Удаляемый SKDProgressCallback: UID='{0}', IsCanceled={1}", x.UID, x.IsCanceled)));
#endif
			ProgressCallbacks.RemoveAll(x => x.IsCanceled && (DateTime.Now - x.CancelizationDateTime).TotalMinutes > 5);
			if (SKDProgressCallback.SKDProgressCallbackType == SKDProgressCallbackType.Stop || !SKDProgressCallback.IsCanceled)
			{
				if (SKDProgressCallbackEvent != null)
				{
#if DEBUG
					Logger.Info(String.Format("Уведомление подписчиков события SKDProgressCallbackEvent(SKDProgressCallback[UID='{0}'])", SKDProgressCallback.UID));
#endif
					SKDProgressCallbackEvent(SKDProgressCallback);
				}
			}
#if DEBUG
			Logger.Info("Конец выполнения Processor.OnSKDCallbackResult");
#endif
		}

		public static event Action<SKDProgressCallback> SKDProgressCallbackEvent;

		#endregion Callback
	}
}
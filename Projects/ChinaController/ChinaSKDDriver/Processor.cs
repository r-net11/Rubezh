﻿using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.Journal;
using FiresecAPI.SKD;

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
		static void OnNewJournalItem(JournalItem journalItem)
		{
			if (NewJournalItem != null)
				NewJournalItem(journalItem);
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
		}

		public static void Stop()
		{
			if (DeviceProcessors != null)
				foreach (var deviceProcessor in DeviceProcessors)
					deviceProcessor.Stop();
			Wrapper.Deinitialize();
		}

		#region Callback
		public static List<GKProgressCallback> ProgressCallbacks = new List<GKProgressCallback>();

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
			ProgressCallbacks.Add(gkProgressCallback);
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
			ProgressCallbacks.Remove(gkProgressCallback);
			OnGKCallbackResult(gkProgressCallback);
		}

		static void OnGKCallbackResult(GKProgressCallback gkProgressCallback)
		{
			ProgressCallbacks.RemoveAll(x => x.IsCanceled && (DateTime.Now - x.CancelizationDateTime).TotalMinutes > 5);
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
				gkCallbackResult.GKStates.PumpStationStates.Count +
				gkCallbackResult.GKStates.MPTStates.Count +
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
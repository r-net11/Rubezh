using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.GK;

namespace GKProcessor
{
	public static class GKProcessorManager
	{
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
	//			AddGKMessage(JournalEventNameType.Отмена_операции, JournalEventDescriptionType.NULL, progressCallback.Title, null, userName);
			}
		}

		public static GKProgressCallback StartProgress(string title, string text, int stepCount, bool canCancel, GKProgressClientType progressClientType)
		{
			var gkProgressCallback = new GKProgressCallback
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
			progressCallback.CurrentStep++;
			var gkProgressCallback = new GKProgressCallback
			{
				UID = progressCallback.UID,
				LastActiveDateTime = DateTime.Now,
				GKProgressCallbackType = GKProgressCallbackType.Progress,
				Title = progressCallback.Title,
				Text = text,
				StepCount = progressCallback.StepCount,
				CurrentStep = progressCallback.CurrentStep,
				CanCancel = progressCallback.CanCancel,
				GKProgressClientType = progressCallback.GKProgressClientType
			};
			OnGKCallbackResult(gkProgressCallback);
		}

		public static void StopProgress(GKProgressCallback progressCallback)
		{
			var gkProgressCallback = new GKProgressCallback
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
				gkCallbackResult.SKDStates.DeviceStates.Count +
				gkCallbackResult.SKDStates.ZoneStates.Count +
				gkCallbackResult.SKDStates.DoorStates.Count > 0)
			{
				if (GKCallbackResultEvent != null)
					GKCallbackResultEvent(gkCallbackResult);
			}
		}
		public static event Action<GKCallbackResult> GKCallbackResultEvent;
		#endregion

		#region Main
		public static bool MustMonitor = false;

		#endregion

		#region Operations
		public static OperationResult<bool> GKUpdateFirmware(GKDevice device, string fileName, string userName)
		{
			var firmwareUpdateHelper = new FirmwareUpdateHelper();
			firmwareUpdateHelper.Update(device, fileName, userName);
			if (firmwareUpdateHelper.ErrorList.Count > 0)
				return OperationResult<bool>.FromError(firmwareUpdateHelper.ErrorList, false);
			return new OperationResult<bool>(true);
		}

		public static OperationResult<bool> GKUpdateFirmwareFSCS(HexFileCollectionInfo hxcFileInfo, string userName, List<GKDevice> devices)
		{
			var firmwareUpdateHelper = new FirmwareUpdateHelper();
			if (firmwareUpdateHelper.ErrorList.Count > 0)
				return OperationResult<bool>.FromError(firmwareUpdateHelper.ErrorList, false);
			return new OperationResult<bool>(true);
		}

		public static OperationResult<List<byte>> GKGKHash(GKDevice device)
		{
			var gkFileReaderWriter = new GKFileReaderWriter();
			var readInfoBlock = gkFileReaderWriter.ReadInfoBlock(device);
			if (gkFileReaderWriter.Error != null)
				return OperationResult<List<byte>>.FromError(gkFileReaderWriter.Error);
			return new OperationResult<List<byte>>(readInfoBlock.Hash1);
		}

		#endregion
	}
}
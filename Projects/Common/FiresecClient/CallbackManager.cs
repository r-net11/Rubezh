using System;
using System.Collections.Generic;
using System.Linq;
using StrazhAPI;
using StrazhAPI.GK;

namespace FiresecClient
{
	public static class CallbackManager
	{
		#region Callback
		public static List<SKDProgressCallback> SKDProgressCallbacks = new List<SKDProgressCallback>();

		public static void CancelSKDProgress(Guid progressCallbackUID, string userName)
		{
			var progressCallback = SKDProgressCallbacks.FirstOrDefault(x => x.UID == progressCallbackUID);
			if (progressCallback != null)
			{
				progressCallback.IsCanceled = true;
				progressCallback.CancelizationDateTime = DateTime.Now;
				StopProgress(progressCallback);
	//			AddGKMessage(JournalEventNameType.Отмена_операции, JournalEventDescriptionType.NULL, progressCallback.Title, null, userName);
			}
		}

		public static SKDProgressCallback StartProgress(string title, string text, int stepCount, bool canCancel, SKDProgressClientType progressClientType)
		{
			var SKDProgressCallback = new SKDProgressCallback
			{
				SKDProgressCallbackType = SKDProgressCallbackType.Start,
				Title = title,
				Text = text,
				StepCount = stepCount,
				CanCancel = canCancel,
				SKDProgressClientType = progressClientType
			};
			SKDProgressCallbacks.Add(SKDProgressCallback);
			OnSKDCallbackResult(SKDProgressCallback);
			return SKDProgressCallback;
		}

		public static void DoProgress(string text, SKDProgressCallback progressCallback)
		{
			progressCallback.CurrentStep++;
			var SKDProgressCallback = new SKDProgressCallback
			{
				UID = progressCallback.UID,
				LastActiveDateTime = DateTime.Now,
				SKDProgressCallbackType = SKDProgressCallbackType.Progress,
				Title = progressCallback.Title,
				Text = text,
				StepCount = progressCallback.StepCount,
				CurrentStep = progressCallback.CurrentStep,
				CanCancel = progressCallback.CanCancel,
				SKDProgressClientType = progressCallback.SKDProgressClientType
			};
			OnSKDCallbackResult(SKDProgressCallback);
		}

		public static void StopProgress(SKDProgressCallback progressCallback)
		{
			var SKDProgressCallback = new SKDProgressCallback
			{
				UID = progressCallback.UID,
				LastActiveDateTime = DateTime.Now,
				SKDProgressCallbackType = SKDProgressCallbackType.Stop,
			};
			SKDProgressCallbacks.Remove(SKDProgressCallback);
			OnSKDCallbackResult(SKDProgressCallback);
		}

		static void OnSKDCallbackResult(SKDProgressCallback SKDProgressCallback)
		{
			SKDProgressCallbacks.RemoveAll(x => x.IsCanceled && (DateTime.Now - x.CancelizationDateTime).TotalMinutes > 5);
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
				SKDCallbackResult.SKDStates.ZoneStates.Count +
				SKDCallbackResult.SKDStates.DoorStates.Count > 0)
			{
				if (SKDCallbackResultEvent != null)
					SKDCallbackResultEvent(SKDCallbackResult);
			}
		}
		public static event Action<SKDCallbackResult> SKDCallbackResultEvent;
		#endregion
	}
}
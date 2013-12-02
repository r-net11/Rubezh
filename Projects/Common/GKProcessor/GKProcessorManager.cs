using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;
using FiresecClient;
using System.Runtime.Serialization;

namespace GKProcessor
{
	public static class GKProcessorManager
	{
		public static void OnStartProgress(string name, int count, bool canCancel = true)
		{
			if (DoProgress != null)
				StartProgress(name, count, canCancel);
		}
		public static event Action<string, int, bool> StartProgress;

		public static void OnDoProgress(string name)
		{
			if (DoProgress != null)
				DoProgress(name);
		}
		public static event Action<string> DoProgress;

		public static void OnStopProgress()
		{
			if (StopProgress != null)
				StopProgress();
		}
		public static event Action StopProgress;

		public static void OnGKCallbackResult(GKCallbackResult gkCallbackResult)
		{
			if (gkCallbackResult.JournalItems.Count + gkCallbackResult.DeviceStates.Count + gkCallbackResult.ZoneStates.Count + gkCallbackResult.DirectionStates.Count > 0)
			{
				if (GKCallbackResultEvent != null)
					GKCallbackResultEvent(gkCallbackResult);
			}
		}
		public static event Action<GKCallbackResult> GKCallbackResultEvent;
	}
}
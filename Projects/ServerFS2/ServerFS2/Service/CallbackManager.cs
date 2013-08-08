using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FiresecAPI.Models;
using FS2Api;

namespace ServerFS2.Service
{
	public static class CallbackManager
	{
		static object locker = new object();
		static List<FSAgentCallbacCash> FSAgentCallbacCashes = new List<FSAgentCallbacCash>();
		static int LastIndex { get; set; }

		public static void AddProgress(FS2ProgressInfo progressInfo)
		{
			FS2Contract.CheckCancellationRequested();
			Add(new FS2Callbac() { FS2ProgressInfo = progressInfo });
			OnProgressEvent(progressInfo);
		}

		public static void Add(FS2Callbac fs2Callbac)
		{
			lock (FSAgentCallbacCashes)
			{
				FSAgentCallbacCashes.RemoveAll(x => (DateTime.Now - x.DateTime) > TimeSpan.FromMinutes(1));

				LastIndex++;
				var callbackResultSaver = new FSAgentCallbacCash()
				{
					FS2Callbac = fs2Callbac,
					Index = LastIndex,
					DateTime = DateTime.Now
				};
				FSAgentCallbacCashes.Add(callbackResultSaver);
			}
			ClientsManager.ClientInfos.ForEach(x => x.PollWaitEvent.Set());
		}

		public static List<FS2Callbac> Get(ClientInfo clientInfo)
		{
			lock (FSAgentCallbacCashes)
			{
				var result = new List<FS2Callbac>();
				var safeCopy = FSAgentCallbacCashes.ToList();
				foreach (var callbackResultSaver in safeCopy)
				{
					if (callbackResultSaver.Index > clientInfo.CallbackIndex)
					{
						result.Add(callbackResultSaver.FS2Callbac);
					}
				}
				if (safeCopy.Count > 0)
				{
					clientInfo.CallbackIndex = safeCopy.Max(x => x.Index);
				}
				return result;
			}
		}

		public static event Action<FS2ProgressInfo> ProgressEvent;
		static void OnProgressEvent(FS2ProgressInfo fs2ProgressInfo)
		{
			if (ProgressEvent != null)
				ProgressEvent(fs2ProgressInfo);
		}

		public static void DeviceStateChanged(List<DeviceState> deviceStates)
		{
			Trace.WriteLine("CallbackManager.DeviceStateChanged");
			Add(new FS2Callbac() { ChangedDeviceStates = deviceStates });
		}

		public static void DeviceParametersChanged(List<DeviceState> deviceStates)
		{
			Trace.WriteLine("CallbackManager.DeviceParametersChanged");
			Add(new FS2Callbac() { ChangedDeviceParameters = deviceStates });
		}

		public static void ZoneStateChanged(List<ZoneState> zoneStates)
		{
			Trace.WriteLine("CallbackManager.ZoneStateChanged");
			Add(new FS2Callbac() { ChangedZoneStates = zoneStates });
		}

		public static void NewJournalItems(List<FS2JournalItem> journalItems)
		{
			Add(new FS2Callbac() { JournalItems = journalItems });
			foreach (var journalItem in journalItems)
			{
				if (NewJournalItem != null)
					NewJournalItem(journalItem);
			}
		}

		public static event Action<FS2JournalItem> NewJournalItem;

		public static event Action<string> LogEvent;
		public static void AddLog(string value)
		{
			if (LogEvent != null)
				LogEvent(value);
		}
	}

	public class FSAgentCallbacCash
	{
		public FS2Callbac FS2Callbac { get; set; }
		public int Index { get; set; }
		public DateTime DateTime { get; set; }
	}
}
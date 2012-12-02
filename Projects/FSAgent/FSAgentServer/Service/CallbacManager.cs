using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FSAgentAPI;
using System.Diagnostics;
using System.Threading;

namespace FSAgentServer
{
	public static class CallbackManager
	{
		static object locker = new object();
		static List<FSAgentCallbacCash> FSAgentCallbacCashes = new List<FSAgentCallbacCash>();
		static int LastIndex { get; set; }

		public static void Add(FSAgentCallbac fsAgentCallbac)
		{
			lock (FSAgentCallbacCashes)
			{
				FSAgentCallbacCashes.RemoveAll(x => (DateTime.Now - x.DateTime) > TimeSpan.FromMinutes(1));

				LastIndex++;
				var callbackResultSaver = new FSAgentCallbacCash()
				{
					FSAgentCallbac = fsAgentCallbac,
					Index = LastIndex,
					DateTime = DateTime.Now
				};
				FSAgentCallbacCashes.Add(callbackResultSaver);
			}
			ClientsManager.ClientInfos.ForEach(x => x.PollWaitEvent.Set());
		}

		public static List<FSAgentCallbac> Get(ClientInfo clientInfo)
		{
			if (IsConnectionLost)
			{
				Thread.Sleep(TimeSpan.FromSeconds(1));
				var result = new List<FSAgentCallbac>();
				var fsAgentCallbac = new FSAgentCallbac()
				{
					IsConnectionLost = IsConnectionLost
				};
				result.Add(fsAgentCallbac);
				return result;
			}
			if (WatcherManager.Current.LastFSProgressInfo != null)
			{
                lock (WatcherManager.Current.LastFSProgressInfo)
                {
                    var fsProgressInfo = new FSProgressInfo()
                    {
                        Stage = WatcherManager.Current.LastFSProgressInfo.Stage,
                        Comment = WatcherManager.Current.LastFSProgressInfo.Comment,
                        PercentComplete = WatcherManager.Current.LastFSProgressInfo.PercentComplete,
                        BytesRW = WatcherManager.Current.LastFSProgressInfo.BytesRW
                    };
                    WatcherManager.Current.LastFSProgressInfo = null;
                    var result = new List<FSAgentCallbac>();
                    var fsAgentCallbac = new FSAgentCallbac()
                    {
                        FSProgressInfo = fsProgressInfo
                    };
                    result.Add(fsAgentCallbac);
                    return result;
                }
			}

			lock (FSAgentCallbacCashes)
			{
				var result = new List<FSAgentCallbac>();
				var safeCopy = FSAgentCallbacCashes.ToList();
				foreach (var callbackResultSaver in safeCopy)
				{
					if (callbackResultSaver.Index > clientInfo.CallbackIndex)
					{
						foreach (var journalRecord in callbackResultSaver.FSAgentCallbac.JournalRecords)
						{
							//Trace.WriteLine("Callback journal no = " + journalRecord.OldId.ToString());
							if (clientInfo.TempOldCode > 0)
							{
								if (journalRecord.OldId - clientInfo.TempOldCode != 1)
								{
									Trace.WriteLine("Queue is not continuous " + journalRecord.OldId.ToString());
								}
							}
							clientInfo.TempOldCode = journalRecord.OldId;
						}
						result.Add(callbackResultSaver.FSAgentCallbac);
					}
				}
				if (safeCopy.Count > 0)
				{
					clientInfo.CallbackIndex = safeCopy.Max(x => x.Index);
				}
				return result;
			}
		}

		static bool IsConnectionLost = false;
		public static void SetConnectionLost(bool value)
		{
			IsConnectionLost = value;
			ClientsManager.ClientInfos.ForEach(x => x.PollWaitEvent.Set());
		}
	}

	public class FSAgentCallbacCash
	{
		public FSAgentCallbac FSAgentCallbac { get; set; }
		public int Index { get; set; }
		public DateTime DateTime { get; set; }
	}
}
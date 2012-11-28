using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FSAgentAPI;
using System.Diagnostics;

namespace FSAgentServer
{
	public static class CallbackManager
	{
		static object locker = new object();
		static List<FSAgentCallbacCash> FSAgentCallbacCashes = new List<FSAgentCallbacCash>();
		static int LastIndex { get; set; }
		public static int GetLastIndex()
		{
			lock (locker)
			{
				return LastIndex;
			}
		}

		public static void Add(FSAgentCallbac fsAgentCallbac)
		{
			lock (locker)
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
			lock (locker)
			{
				var result = new List<FSAgentCallbac>();
				var safeCopy = FSAgentCallbacCashes.ToList();
				foreach (var callbackResultSaver in safeCopy)
				{
					if (callbackResultSaver.Index > clientInfo.CallbackIndex)
					{
						foreach (var journalRecord in callbackResultSaver.FSAgentCallbac.JournalRecords)
						{
							Trace.WriteLine("Callback journal no = " + journalRecord.OldId.ToString());
						}
						result.Add(callbackResultSaver.FSAgentCallbac);
					}
				}
				if (safeCopy.Count > 0)
				{
					clientInfo.CallbackIndex = safeCopy.Max(x => x.Index);// CallbackManager.GetLastIndex();
				}
				return result;
			}
		}
	}

	public class FSAgentCallbacCash
	{
		public FSAgentCallbac FSAgentCallbac { get; set; }
		public int Index { get; set; }
		public DateTime DateTime { get; set; }
	}
}
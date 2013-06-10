using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FS2Api;
using System.Diagnostics;

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
			Trace.WriteLine("AddProgress: " + progressInfo.Comment);
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
	}

	public class FSAgentCallbacCash
	{
		public FS2Callbac FS2Callbac { get; set; }
		public int Index { get; set; }
		public DateTime DateTime { get; set; }
	}
}
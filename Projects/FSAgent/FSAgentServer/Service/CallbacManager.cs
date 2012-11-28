using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FSAgentAPI;

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
				var maxIndex = FSAgentCallbacCashes.Max(x => x.Index);
				foreach (var callbackResultSaver in FSAgentCallbacCashes)
				{
					if (callbackResultSaver.Index > clientInfo.CallbackIndex)
					{
						result.Add(callbackResultSaver.FSAgentCallbac);
					}
				}
				clientInfo.CallbackIndex = maxIndex;// CallbackManager.GetLastIndex();
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
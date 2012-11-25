using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FSAgentAPI;

namespace FSAgent
{
    public static class CallbackManager
    {
        static List<FSAgentCallbacCash> FSAgentCallbacCashes = new List<FSAgentCallbacCash>();
        public static int LastIndex { get; private set; }

        public static void Add(FSAgentCallbac fsAgentCallbac)
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

        public static List<FSAgentCallbac> Get(int index)
        {
            var result = new List<FSAgentCallbac>();
            foreach (var callbackResultSaver in FSAgentCallbacCashes)
            {
                if (callbackResultSaver.Index > index)
                {
                    result.Add(callbackResultSaver.FSAgentCallbac);
                }
            }
            return result;
        }
    }

    public class FSAgentCallbacCash
    {
        public FSAgentCallbac FSAgentCallbac { get; set; }
        public int Index { get; set; }
        public DateTime DateTime { get; set; }
    }
}
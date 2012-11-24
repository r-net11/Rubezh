using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FSAgentAPI;
using System.ServiceModel;
using FiresecEventTest;

namespace FSAgent
{
    [ServiceBehavior(MaxItemsInObjectGraph = Int32.MaxValue, UseSynchronizationContext = true,
    InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class FSAgentContract : IFSAgentContract
    {
        public void Run()
        {
            Runner.Run();
        }

        public string GetStatus()
        {
            return null;
        }

        public List<ChangeResult> GetChangeResult()
        {
            return null;
        }
    }
}
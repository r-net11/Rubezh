using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FSAgentAPI;
using System.ServiceModel;
using FiresecAPI.Models;
using FSAgent.Service;

namespace FSAgent
{
    [ServiceBehavior(MaxItemsInObjectGraph = Int32.MaxValue, UseSynchronizationContext = true,
    InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class FSAgentContract : IFSAgentContract
    {
        public string GetStatus()
        {
            return null;
        }

        public List<FSAgentCallbac> Poll(Guid clientUID)
        {
            ClientsManager.Add(clientUID);

            var clientInfo = ClientsManager.ClientInfos.FirstOrDefault(x => x.UID == clientUID);
            if (clientInfo != null)
            {
                var result = CallbackManager.Get(clientInfo.CallbackIndex);
                clientInfo.CallbackIndex = CallbackManager.LastIndex;
                return result;
            }
            return new List<FSAgentCallbac>();
        }
    }
}
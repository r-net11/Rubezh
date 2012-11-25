using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FSAgentAPI;
using FiresecAPI.Models;

namespace FSAgentClient
{
    public partial class FSAgent
    {
        public string GetStatus()
        {
            return SafeOperationCall(() => { return FSAgentContract.GetStatus(); }, "GetStatus");
        }

        public List<FSAgentCallbac> Poll(Guid clientUID)
        {
            return SafeOperationCall(() => { return FSAgentContract.Poll(clientUID); }, "GetChangeResult");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSAgentServer
{
    public static class ClientsManager
    {
        public static List<ClientInfo> ClientInfos = new List<ClientInfo>();

        public static void Add(Guid uid)
        {
            if (ClientInfos.Any(x => x.UID == uid))
                return;

            var clientInfo = new ClientInfo();
            clientInfo.UID = uid;
            ClientInfos.Add(clientInfo);
        }

        public static void Remove(Guid uid)
        {
            var clientInfo = ClientInfos.FirstOrDefault(x => x.UID == uid);
            ClientInfos.Remove(clientInfo);
        }
    }

    public class ClientInfo
    {
        public Guid UID { get; set; }
        public int CallbackIndex { get; set; }
    }
}
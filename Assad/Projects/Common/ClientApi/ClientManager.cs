using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientApi
{
    public static class ClientManager
    {
        static ServiceClient serviceClient;

        public static void Start()
        {
            if (serviceClient == null)
            {
                serviceClient = new ServiceClient();
                serviceClient.Start();
            }
        }

        public static ServiceClient Current
        {
            get
            {
                Start();
                return serviceClient;
            }
        }
    }
}

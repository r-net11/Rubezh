using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using ServiceApi;
using ClientApi;

namespace RubezhAX
{
    public static class ServiceClientManager
    {
        public static void Start()
        {
            if (serviceClient == null)
            {
                serviceClient = new ServiceClient();
                serviceClient.Start();
            
            }
        }

        public static void Stop()
        {
            if (serviceClient != null)
            {
                serviceClient.Stop();

            }
        }

        private static ServiceClient serviceClient;
    }
}

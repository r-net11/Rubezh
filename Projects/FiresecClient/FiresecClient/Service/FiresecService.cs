using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace FiresecClient
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Single)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single)]
    public class FiresecService : IFiresecService
    {
        public string GetName()
        {
            return "Hello";
        }

        public Models.CurrentConfiguration GetCoreConfig()
        {
            return FiresecManager.Configuration;
        }
    }
}

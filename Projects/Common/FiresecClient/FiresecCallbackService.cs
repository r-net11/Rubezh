using System;
using FiresecAPI;

namespace FiresecClient
{
    public class FiresecCallbackService : IFiresecCallbackService
    {
        public void ConfigurationChanged()
        {
            if (ConfigurationChangedEvent != null)
                ConfigurationChangedEvent();
        }

        public static event Action ConfigurationChangedEvent;
    }
}
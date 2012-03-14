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

        public bool Progress(int stage, string comment, int percentComplete, int bytesRW)
        {
            if (ProgressEvent != null)
                return ProgressEvent(stage, comment, percentComplete, bytesRW);
            return true;
        }

        public static event Action ConfigurationChangedEvent;
        public delegate bool ProgressDelegate(int stage, string comment, int percentComplete, int bytesRW);
        public static event ProgressDelegate ProgressEvent;
    }
}
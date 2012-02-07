using System;
using FiresecAPI;
using System.Diagnostics;

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

            return false;
        }

        public static event Action ConfigurationChangedEvent;
        public delegate bool ProgressDelegate(int stage, string comment, int percentComplete, int bytesRW);
        public static event ProgressDelegate ProgressEvent;
    }
}
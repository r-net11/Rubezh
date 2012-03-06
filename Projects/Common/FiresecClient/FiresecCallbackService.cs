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

        public void Progress(int stage, string comment, int percentComplete, int bytesRW)
        {
            bool isCanceled;
            if (ProgressEvent != null)
                isCanceled = ProgressEvent(stage, comment, percentComplete, bytesRW);
        }

        public static event Action ConfigurationChangedEvent;
        public delegate bool ProgressDelegate(int stage, string comment, int percentComplete, int bytesRW);
        public static event ProgressDelegate ProgressEvent;
    }
}
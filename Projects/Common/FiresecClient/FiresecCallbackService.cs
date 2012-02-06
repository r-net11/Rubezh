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
            Trace.WriteLine("Progress: " + comment);

            if (ProgressEvent != null)
                ProgressEvent();

            return true;
        }

        public static event Action ConfigurationChangedEvent;
        public static event Action ProgressEvent;
    }
}
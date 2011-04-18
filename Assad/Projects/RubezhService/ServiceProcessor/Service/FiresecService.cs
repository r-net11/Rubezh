using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceApi;
using Firesec;
using System.ServiceModel;

namespace ServiseProcessor.Service
{
    public class FiresecService : IFiresecService
    {
        static IFiresecCallback callback;

        public static void OnNewEventsAvailable(int eventMask)
        {
            try
            {
                if (callback != null)
                {
                    callback.NewEventsAvailable(eventMask);
                }
            }
            catch { ;}
        }

        public void Initialize()
        {
            callback = OperationContext.Current.GetCallbackChannel<IFiresecCallback>();
            FiresecEventAggregator.NewEventAvaliable += new Action<int>(OnNewEventsAvailable);
        }

        public string GetCoreConfig()
        {
            return NativeFiresecClient.GetCoreConfig();
        }

        public string GetCoreState()
        {
            return NativeFiresecClient.GetCoreState();
        }

        public string GetMetaData()
        {
            return NativeFiresecClient.GetMetaData();
        }

        public string GetCoreDeviceParams()
        {
            return NativeFiresecClient.GetCoreDeviceParams();
        }

        public string ReadEvents(int fromId, int limit)
        {
            return NativeFiresecClient.ReadEvents(fromId, limit);
        }

        public void SetNewConfig(string config)
        {
            NativeFiresecClient.SetNewConfig(config);
        }

        public void DeviceWriteConfig(string config, string devicePath)
        {
            NativeFiresecClient.DeviceWriteConfig(config, devicePath);
        }

        public void ResetStates(string config)
        {
            NativeFiresecClient.ResetStates(config);
        }
    }
}

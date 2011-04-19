using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceApi;
using Firesec;
using System.ServiceModel;

namespace ServiseProcessor.Service
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Single)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single)]
    public class FiresecService : IFiresecService
    {
        static IFiresecCallback callback;

        public static void OnNewEventsAvailable(int eventMask, string obj)
        {
            try
            {
                if (callback != null)
                {
                    callback.NewEventsAvailable(eventMask, obj);
                }
            }
            catch { ;}
        }

        public void Initialize()
        {
            callback = OperationContext.Current.GetCallbackChannel<IFiresecCallback>();
            FiresecEventAggregator.NewEventAvaliable += new Action<int, string>(OnNewEventsAvailable);
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

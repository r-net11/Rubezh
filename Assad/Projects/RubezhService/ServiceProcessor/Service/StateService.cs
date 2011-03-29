using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Runtime.Serialization;
using ServiseProcessor;
using ServiceApi;

namespace ServiseProcessor
{
    class StateService : IStateService
    {
        static ICallback callback;

        public void Initialize()
        {
            callback = OperationContext.Current.GetCallbackChannel<ICallback>();
        }

        public static void Notify(string message)
        {
            if (callback != null)
                callback.Notify(message);
        }

        public static void StatesChanged(CurrentStates currentStates)
        {
            if (callback != null)
            {
                callback.StateChanged(currentStates);
            }
        }

        public CurrentConfiguration GetConfiguration()
        {
            return Services.CurrentConfiguration;
        }


        public CurrentStates GetStates()
        {
            return Services.CurrentStates;
        }

        public void ExecuteCommand(string devicePath, string command)
        {
            DeviceState device;
            try
            {
                device = Services.CurrentStates.DeviceStates.First(x => x.Path == devicePath);
            }
            catch
            {
                device = null;
            }
            if (device != null)
            {
                Processor.ExecuteCommand(device, command);
            }
        }

        public void SetConfiguration(CurrentConfiguration currentConfiguration)
        {
            Processor.SetNewConfig(currentConfiguration);
        }
    }
}

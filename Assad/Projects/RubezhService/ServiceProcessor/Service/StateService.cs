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

        public void ResetState(string devicePath, string command)
        {
            if (Services.CurrentStates.DeviceStates.Any(x => x.Path == devicePath))
            {
                DeviceState device = Services.CurrentStates.DeviceStates.FirstOrDefault(x => x.Path == devicePath);
                Processor.ResetState(device, command);
            }
        }

        public void SetConfiguration(CurrentConfiguration currentConfiguration)
        {
            Processor.SetNewConfig(currentConfiguration);
        }

        public List<Firesec.ReadEvents.journalType> ReadJournal(int startIndex, int count)
        {
            Firesec.ReadEvents.document journal = Firesec.FiresecClient.ReadEvents(startIndex, count);
            if (journal.Journal != null)
            {
                if (journal.Journal.Count() > 0)
                {
                    return journal.Journal.ToList();
                }
            }
            return new List<Firesec.ReadEvents.journalType>();
        }
    }
}

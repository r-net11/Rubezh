using System.Collections.Generic;
using System.ServiceModel;
using System.IO;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecService.Converters;

namespace FiresecService
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Single)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single)]
    public class FiresecService : IFiresecService
    {
        public Stream GetFile()
        {
            string filePath = @"D:\xxx.txt";

            if (!File.Exists(filePath))
                throw new FileNotFoundException("File was not found", Path.GetFileName(filePath));

            return new FileStream(filePath, FileMode.Open, FileAccess.Read);
        }

        public CurrentConfiguration GetCoreConfig()
        {
            return FiresecManager.Configuration;
        }

        public CurrentStates GetCurrentStates()
        {
            return FiresecManager.States;
        }

        public List<JournalItem> ReadJournal(int startIndex, int count)
        {
            var internalJournal = FiresecInternalClient.ReadEvents(startIndex, count);
            return JournalConverter.Convert(internalJournal);
        }
    }
}

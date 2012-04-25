using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecService.Converters;
using FiresecService.DatabaseConverter;

namespace FiresecService
{
    public class JournalDataConverter
    {
		FiresecSerializedClient FiresecSerializedClient;

		public JournalDataConverter(FiresecSerializedClient firesecSerializedClient)
		{
			FiresecSerializedClient = firesecSerializedClient;
		}

        public void Convert()
        {
			using (var dataContext = ConnectionManager.CreateFiresecDataContext())
            {
                dataContext.JournalRecords.DeleteAllOnSubmit(from record in dataContext.JournalRecords select record);
                dataContext.SubmitChanges();
                dataContext.JournalRecords.InsertAllOnSubmit(ReadAllJournal());
                dataContext.SubmitChanges();
            }
        }

        public IEnumerable<JournalRecord> ReadAllJournal()
        {
            var internalJournal = FiresecSerializedClient.ReadEvents(0, 100000).Result;
            if (internalJournal != null && internalJournal.Journal.IsNotNullOrEmpty())
            {
                return internalJournal.Journal.
                    Select(innerJournaRecord => JournalConverter.Convert(innerJournaRecord)).
                    OrderBy(JournalRecord => JournalRecord.DeviceTime);
            }
            return new List<JournalRecord>();
        }
    }
}
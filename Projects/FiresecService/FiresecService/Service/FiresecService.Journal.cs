using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecService.Converters;
using FiresecService.Processor;
using FiresecAPI;

namespace FiresecService
{
    public partial class FiresecService
    {
        public OperationResult<List<JournalRecord>> ReadJournal(int startIndex, int count)
        {
            var operationResult = new OperationResult<List<JournalRecord>>();
            try
            {
                lock (Locker)
                {
                    var internalJournal = FiresecSerializedClient.ReadEvents(startIndex, count).Result;
                    if (internalJournal != null && internalJournal.Journal.IsNotNullOrEmpty())
                    {
                        operationResult.Result = new List<JournalRecord>(internalJournal.Journal.Select(x => JournalConverter.Convert(x)));
                    }
                }
            }
            catch (Exception e)
            {
                operationResult.HasError = true;
                operationResult.Error = e.Message.ToString();
            }
            return operationResult;
        }

        public OperationResult<List<JournalRecord>> GetFilteredJournal(JournalFilter journalFilter)
        {
            var operationResult = new OperationResult<List<JournalRecord>>();
            try
            {
                operationResult.Result =
                    DataBaseContext.JournalRecords.AsEnumerable().Reverse().
                    Where(journal => journalFilter.CheckDaysConstraint(journal.SystemTime)).
                    Where(journal => JournalFilterHelper.FilterRecord(journalFilter, journal)).
                    Take(journalFilter.LastRecordsCount).ToList();
            }
            catch (Exception e)
            {
                operationResult.HasError = true;
                operationResult.Error = e.Message.ToString();
            }
            return operationResult;
        }

        public OperationResult<List<JournalRecord>> GetFilteredArchive(ArchiveFilter archiveFilter)
        {
            var operationResult = new OperationResult<List<JournalRecord>>();
            try
            {
                var result =
                    DataBaseContext.JournalRecords.AsEnumerable().Reverse().
                    RangeJournalByTime(archiveFilter).
                    FilterJournalByEvents(archiveFilter);
                    //FilterJournalBySubsystems(archiveFilter).
                    //FilterJournalByDevices(archiveFilter).ToList();

                operationResult.Result = result.FilterJournalByDevices(archiveFilter).
                Where(x => archiveFilter.Subsystems.Contains(x.SubsystemType)).ToList();
            }
            catch (Exception e)
            {
                operationResult.HasError = true;
                operationResult.Error = e.Message.ToString();
            }
            return operationResult;
        }

        public OperationResult<List<JournalRecord>> GetDistinctRecords()
        {
            var operationResult = new OperationResult<List<JournalRecord>>();
            try
            {
                operationResult.Result =
                    DataBaseContext.JournalRecords.AsEnumerable().
                    Select(x => x).Distinct(new JournalRecord()).ToList();
            }
            catch (Exception e)
            {
                operationResult.HasError = true;
                operationResult.Error = e.Message.ToString();
            }
            return operationResult;
        }

        public OperationResult<DateTime> GetArchiveStartDate()
        {
            var operationResult = new OperationResult<DateTime>();
            try
            {
                operationResult.Result = DataBaseContext.JournalRecords.AsEnumerable().First().SystemTime;
            }
            catch (Exception e)
            {
                operationResult.HasError = true;
                operationResult.Error = e.Message.ToString();
            }
            return operationResult;
        }

        public void AddJournalRecord(JournalRecord journalRecord)
        {
            var operationResult = new OperationResult<bool>();
            try
            {
                journalRecord.User = _userName;
                DatabaseHelper.AddJournalRecord(journalRecord);
                CallbackManager.OnNewJournalRecord(journalRecord);
                operationResult.Result = true;
            }
            catch (Exception e)
            {
                operationResult.HasError = true;
                operationResult.Error = e.Message.ToString();
            }
        }
    }
}
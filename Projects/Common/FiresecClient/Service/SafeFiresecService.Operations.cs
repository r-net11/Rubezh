﻿using System;
using System.Collections.Generic;
using System.IO;
using Common;
using FiresecAPI;
using FiresecAPI.Models;

namespace FiresecClient
{
    public partial class SafeFiresecService
    {
        public OperationResult<bool> Reconnect(Guid uid, string userName, string password)
        {
            return SafeOperationCall(() => { return FiresecService.Reconnect(uid, userName, password); }, "Reconnect");
        }

		public void Disconnect(Guid uid)
        {
            SafeOperationCall(() => { FiresecService.Disconnect(uid); }, "Disconnect");
        }

        public List<CallbackResult> Poll(Guid uid)
        {
            return SafeOperationCall(() => { return FiresecService.Poll(uid); }, "Poll");
        }

        public void NotifyClientsOnConfigurationChanged()
        {
            SafeOperationCall(() => { FiresecService.NotifyClientsOnConfigurationChanged(); }, "NotifyClientsOnConfigurationChanged");
        }

		public SecurityConfiguration GetSecurityConfiguration()
		{
			return SafeOperationCall(() => { return FiresecService.GetSecurityConfiguration(); }, "GetSecurityConfiguration");
		}

        public T GetConfiguration<T>(string filename)
            where T : VersionedConfiguration, new()
        {
            var config = new T();
            return SafeOperationCall(() => config, filename);
        }

        public OperationResult<int> GetJournalLastId()
        {
            return SafeOperationCall(() => { return FiresecService.GetJournalLastId(); }, "GetJournalLastId");
        }

        public OperationResult<List<JournalRecord>> GetFilteredJournal(JournalFilter journalFilter)
        {
            return SafeOperationCall(() => { return FiresecService.GetFilteredJournal(journalFilter); }, "GetFilteredJournal");
        }

        public OperationResult<List<JournalRecord>> GetFilteredArchive(ArchiveFilter archiveFilter)
        {
            return SafeOperationCall(() => { return FiresecService.GetFilteredArchive(archiveFilter); }, "GetFilteredArchive");
        }

        public void BeginGetFilteredArchive(ArchiveFilter archiveFilter)
        {
            SafeOperationCall(() => { FiresecService.BeginGetFilteredArchive(archiveFilter); }, "BeginGetFilteredArchive");
        }

        public OperationResult<List<JournalDescriptionItem>> GetDistinctDescriptions()
        {
            return SafeOperationCall(() => { return FiresecService.GetDistinctDescriptions(); }, "GetDistinctDescriptions");
        }

        public OperationResult<DateTime> GetArchiveStartDate()
        {
            return SafeOperationCall(() => { return FiresecService.GetArchiveStartDate(); }, "GetArchiveStartDate");
        }

        public void AddJournalRecords(List<JournalRecord> journalRecords)
        {
            AddTask(() =>
                {
                    SafeOperationCall(() => { FiresecService.AddJournalRecords(journalRecords); }, "AddJournalRecords");
                });
        }

        public List<string> GetFileNamesList(string directory)
        {
            return SafeOperationCall(() => { return FiresecService.GetFileNamesList(directory); }, "GetFileNamesList");
        }

        public Dictionary<string, string> GetDirectoryHash(string directory)
        {
            return SafeOperationCall(() => { return FiresecService.GetDirectoryHash(directory); }, "GetDirectoryHash");
        }

        public System.IO.Stream GetFile(string dirAndFileName)
        {
            return SafeOperationCall(() => { return FiresecService.GetFile(dirAndFileName); }, "GetFile");
        }

		public Stream GetConfig()
		{
			return SafeOperationCall(() => { return FiresecService.GetConfig(); }, "GetConfig");
		}

		public void SetConfig(Stream stream)
		{
			SafeOperationCall(() => { FiresecService.SetConfig(stream); }, "SetConfig");
		}

		public void SetJournal(List<JournalRecord> journalRecords)
        {
            SafeOperationCall(() => { FiresecService.SetJournal(journalRecords); }, "SetJournal");
        }

        public string Test(string arg)
        {
            return SafeOperationCall(() => { return FiresecService.Test(arg); }, "Test");
        }

		#region SKD
		public IEnumerable<Employee> GetEmployees(EmployeeFilter filter)
		{
			return SafeContext.Execute<IEnumerable<Employee>>(() => FiresecService.GetEmployees(filter));
		}

		public IEnumerable<Department> GetDepartments(DepartmentFilter filter)
		{
			return SafeContext.Execute<IEnumerable<Department>>(() => FiresecService.GetDepartments(filter));
		}

		public IEnumerable<Position> GetPositions(PositionFilter filter)
		{
			return SafeContext.Execute<IEnumerable<Position>>(() => FiresecService.GetPositions(filter));
		}

		public IEnumerable<SKDJournalItem> GetSKDJournalItems(SKDJournalFilter filter)
		{
			return SafeContext.Execute<IEnumerable<SKDJournalItem>>(() => FiresecService.GetSKDJournalItems(filter));
		}

		public void SaveSKDJournalItems(IEnumerable<SKDJournalItem> journalItems)
		{
			SafeContext.Execute(() => FiresecService.SaveSKDJournalItems(journalItems));
		}

		public IEnumerable<Frame> GetFrames(FrameFilter filter)
		{
			return SafeContext.Execute<IEnumerable<Frame>>(() => FiresecService.GetFrames(filter));
		}

		public void SaveFrames(IEnumerable<Frame> frames)
		{
			SafeContext.Execute(() => FiresecService.SaveFrames(frames));
		}
		#endregion

		public string Ping()
		{
			return SafeOperationCall(() => { return FiresecService.Ping(); }, "Ping");
		}
	}
}
using System;
using System.Collections.Generic;
using System.Threading;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecAPI.Models.Skud;

namespace FiresecClient
{
    public partial class SafeFiresecService
    {
        public OperationResult<bool> Reconnect(string userName, string password)
        {
            return SafeOperationCall(() => { return FiresecService.Reconnect(userName, password); }, "Reconnect");
        }

        public void Disconnect()
        {
            SafeOperationCall(() => { FiresecService.Disconnect(); }, "Disconnect");
        }

        public void NotifyClientsOnConfigurationChanged()
        {
            SafeOperationCall(() => { FiresecService.NotifyClientsOnConfigurationChanged(); }, "NotifyClientsOnConfigurationChanged");
        }

        public string GetStatus()
        {
            return SafeOperationCall(() => { return FiresecService.GetStatus(); }, "GetStatus");
        }

		public DriversConfiguration GetDriversConfiguration()
        {
            return SafeOperationCall(() => { return FiresecService.GetDriversConfiguration(); }, "GetDriversConfiguration");
        }

        public DeviceConfiguration GetDeviceConfiguration()
        {
            return SafeOperationCall(() => { return FiresecService.GetDeviceConfiguration(); }, "GetDeviceConfiguration");
        }

        public PlansConfiguration GetPlansConfiguration()
        {
            return SafeOperationCall(() => { return FiresecService.GetPlansConfiguration(); }, "GetPlansConfiguration");
        }

        public void SetPlansConfiguration(PlansConfiguration plansConfiguration)
        {
            SafeOperationCall(() => { FiresecService.SetPlansConfiguration(plansConfiguration); }, "SetPlansConfiguration");
        }

        public SystemConfiguration GetSystemConfiguration()
        {
            return SafeOperationCall(() => { return FiresecService.GetSystemConfiguration(); }, "GetSystemConfiguration");
        }

        public void SetSystemConfiguration(SystemConfiguration systemConfiguration)
        {
            SafeOperationCall(() => { FiresecService.SetSystemConfiguration(systemConfiguration); }, "SetSystemConfiguration");
        }

        public DeviceLibraryConfiguration GetDeviceLibraryConfiguration()
        {
            return SafeOperationCall(() => { return FiresecService.GetDeviceLibraryConfiguration(); }, "GetDeviceLibraryConfiguration");
        }

        public void SetDeviceLibraryConfiguration(DeviceLibraryConfiguration deviceLibraryConfiguration)
        {
            SafeOperationCall(() => { FiresecService.SetDeviceLibraryConfiguration(deviceLibraryConfiguration); }, "SetDeviceLibraryConfiguration");
        }

        public SecurityConfiguration GetSecurityConfiguration()
        {
            return SafeOperationCall(() => { return FiresecService.GetSecurityConfiguration(); }, "GetSecurityConfiguration");
        }

        public void SetSecurityConfiguration(SecurityConfiguration securityConfiguration)
        {
            SafeOperationCall(() => { FiresecService.SetSecurityConfiguration(securityConfiguration); }, "SetSecurityConfiguration");
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
            var thread = new Thread(new ThreadStart(() =>
            {
                SafeOperationCall(() => { FiresecService.AddJournalRecords(journalRecords); }, "AddJournalRecords");
            }
                ));
            thread.Start();
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

		public void SetJournal(List<JournalRecord> journalRecords)
        {
            SafeOperationCall(() => { FiresecService.SetJournal(journalRecords); }, "SetJournal");
        }

        public string Test(string arg)
        {
            return SafeOperationCall(() => { return FiresecService.Test(arg); }, "Test");
        }

        public void SetXDeviceConfiguration(XFiresecAPI.XDeviceConfiguration xDeviceConfiguration)
        {
            SafeOperationCall(() => { FiresecService.SetXDeviceConfiguration(xDeviceConfiguration); }, "SetXDeviceConfiguration");
        }

        public XFiresecAPI.XDeviceConfiguration GetXDeviceConfiguration()
        {
            return SafeOperationCall(() => { return FiresecService.GetXDeviceConfiguration(); }, "GetXDeviceConfiguration");
        }

        public void SetXDeviceLibraryConfiguration(XFiresecAPI.XDeviceLibraryConfiguration xDeviceLibraryConfiguration)
        {
            SafeOperationCall(() => { FiresecService.SetXDeviceLibraryConfiguration(xDeviceLibraryConfiguration); }, "SetXDeviceLibraryConfiguration");
        }

        public XFiresecAPI.XDeviceLibraryConfiguration GetXDeviceLibraryConfiguration()
        {
            return SafeOperationCall(() => { return FiresecService.GetXDeviceLibraryConfiguration(); }, "GetXDeviceLibraryConfiguration");
        }

        public OperationResult<bool> SetDeviceConfiguration(DeviceConfiguration deviceConfiguration)
        {
            return SafeOperationCall(() => { return FiresecService.SetDeviceConfiguration(deviceConfiguration); }, "SetDeviceConfiguration");
        }

        public IEnumerable<EmployeeCard> GetEmployees(EmployeeCardIndexFilter filter)
        {
            return SafeContext.Execute<IEnumerable<EmployeeCard>>(() => FiresecService.GetEmployees(filter));
        }

        public bool DeleteEmployee(int id)
        {
            return SafeContext.Execute<bool>(() => FiresecService.DeleteEmployee(id));
        }

        public EmployeeCardDetails GetEmployeeCard(int id)
        {
            return SafeContext.Execute<EmployeeCardDetails>(() => FiresecService.GetEmployeeCard(id));
        }

        public int SaveEmployeeCard(EmployeeCardDetails employeeCard)
        {
            return SafeContext.Execute<int>(() => FiresecService.SaveEmployeeCard(employeeCard));
        }

        public IEnumerable<EmployeeDepartment> GetEmployeeDepartments()
        {
            return SafeContext.Execute<IEnumerable<EmployeeDepartment>>(() => FiresecService.GetEmployeeDepartments());
        }

        public IEnumerable<EmployeeGroup> GetEmployeeGroups()
        {
            return SafeContext.Execute<IEnumerable<EmployeeGroup>>(() => FiresecService.GetEmployeeGroups());
        }

        public IEnumerable<EmployeePosition> GetEmployeePositions()
        {
            return SafeContext.Execute<IEnumerable<EmployeePosition>>(() => FiresecService.GetEmployeePositions());
        }
    }
}
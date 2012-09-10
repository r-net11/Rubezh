using System;
using System.Collections.Generic;
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
            return SafeOperationCall(() => { return FiresecService.Reconnect(userName, password); });
        }

        public void Disconnect()
        {
            SafeOperationCall(() => { FiresecService.Disconnect(); });
        }

        public string GetStatus()
        {
            return SafeOperationCall(() => { return FiresecService.GetStatus(); });
        }

        public List<Driver> GetDrivers()
        {
            return SafeOperationCall(() => { return FiresecService.GetDrivers(); });
        }

        public DeviceConfiguration GetDeviceConfiguration()
        {
            return SafeOperationCall(() => { return FiresecService.GetDeviceConfiguration(); });
        }

        public PlansConfiguration GetPlansConfiguration()
        {
            return SafeOperationCall(() => { return FiresecService.GetPlansConfiguration(); });
        }

        public void SetPlansConfiguration(PlansConfiguration plansConfiguration)
        {
            SafeOperationCall(() => { FiresecService.SetPlansConfiguration(plansConfiguration); });
        }

        public SystemConfiguration GetSystemConfiguration()
        {
            return SafeOperationCall(() => { return FiresecService.GetSystemConfiguration(); });
        }

        public void SetSystemConfiguration(SystemConfiguration systemConfiguration)
        {
            SafeOperationCall(() => { FiresecService.SetSystemConfiguration(systemConfiguration); });
        }

        public LibraryConfiguration GetLibraryConfiguration()
        {
            return SafeOperationCall(() => { return FiresecService.GetLibraryConfiguration(); });
        }

        public void SetLibraryConfiguration(LibraryConfiguration libraryConfiguration)
        {
            SafeOperationCall(() => { FiresecService.SetLibraryConfiguration(libraryConfiguration); });
        }

        public SecurityConfiguration GetSecurityConfiguration()
        {
            return SafeOperationCall(() => { return FiresecService.GetSecurityConfiguration(); });
        }

        public void SetSecurityConfiguration(SecurityConfiguration securityConfiguration)
        {
            SafeOperationCall(() => { FiresecService.SetSecurityConfiguration(securityConfiguration); });
        }

        public OperationResult<int> GetJournalLastId()
        {
            return SafeOperationCall(() => { return FiresecService.GetJournalLastId(); });
        }

        public OperationResult<List<JournalRecord>> GetFilteredJournal(JournalFilter journalFilter)
        {
            return SafeOperationCall(() => { return FiresecService.GetFilteredJournal(journalFilter); });
        }

        public OperationResult<List<JournalRecord>> GetFilteredArchive(ArchiveFilter archiveFilter)
        {
            return SafeOperationCall(() => { return FiresecService.GetFilteredArchive(archiveFilter); });
        }

        public void BeginGetFilteredArchive(ArchiveFilter archiveFilter)
        {
            SafeOperationCall(() => { FiresecService.BeginGetFilteredArchive(archiveFilter); });
        }

        public OperationResult<List<JournalDescriptionItem>> GetDistinctDescriptions()
        {
            return SafeOperationCall(() => { return FiresecService.GetDistinctDescriptions(); });
        }

        public OperationResult<DateTime> GetArchiveStartDate()
        {
            return SafeOperationCall(() => { return FiresecService.GetArchiveStartDate(); });
        }

        public void AddJournalRecords(List<JournalRecord> journalRecords)
        {
            SafeOperationCall(() => { FiresecService.AddJournalRecords(journalRecords); });
        }

        public List<string> GetFileNamesList(string directory)
        {
            return SafeOperationCall(() => { return FiresecService.GetFileNamesList(directory); });
        }

        public Dictionary<string, string> GetDirectoryHash(string directory)
        {
            return SafeOperationCall(() => { return FiresecService.GetDirectoryHash(directory); });
        }

        public System.IO.Stream GetFile(string dirAndFileName)
        {
            return SafeOperationCall(() => { return FiresecService.GetFile(dirAndFileName); });
        }

		public void ConvertJournal(List<JournalRecord> journalRecords)
        {
			SafeOperationCall(() => { FiresecService.ConvertJournal(journalRecords); });
        }

        public string Test()
        {
            return SafeOperationCall(() => { return FiresecService.Test(); });
        }

        public void SetXDeviceConfiguration(XFiresecAPI.XDeviceConfiguration xDeviceConfiguration)
        {
            SafeOperationCall(() => { FiresecService.SetXDeviceConfiguration(xDeviceConfiguration); });
        }

        public XFiresecAPI.XDeviceConfiguration GetXDeviceConfiguration()
        {
            return SafeOperationCall(() => { return FiresecService.GetXDeviceConfiguration(); });
        }

        public OperationResult<bool> SetDeviceConfiguration(DeviceConfiguration deviceConfiguration)
        {
            return SafeOperationCall(() => { return FiresecService.SetDeviceConfiguration(deviceConfiguration); });
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

        public void OPCRefresh()
        {
            SafeOperationCall(() => { FiresecService.OPCRefresh(); });
        }

        public void OPCRegister()
        {
            SafeOperationCall(() => { FiresecService.OPCRegister(); });
        }

        public void OPCUnRegister()
        {
            SafeOperationCall(() => { FiresecService.OPCUnRegister(); });
        }
    }
}
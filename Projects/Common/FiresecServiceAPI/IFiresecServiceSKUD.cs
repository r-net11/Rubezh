using System.Collections.Generic;
using System.ServiceModel;
using System;
using XFiresecAPI;

namespace FiresecAPI
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface IFiresecServiceSKUD
    {
        [OperationContract]
        IEnumerable<Employee> GetEmployees(EmployeeFilter filter);
        [OperationContract]
        IEnumerable<Department> GetDepartments(DepartmentFilter filter);
        [OperationContract]
        IEnumerable<Position> GetPositions(PositionFilter filter);
		[OperationContract]
		IEnumerable<SKDJournalItem> GetSKDJournalItems(SKDJournalFilter filter);
		[OperationContract]
		IEnumerable<Frame> GetFrames(FrameFilter filter);
		[OperationContract]
		IEnumerable<Card> GetCards(CardFilter filter);
		[OperationContract]
		IEnumerable<CardZoneLink> GetCardZoneLinks(CardZoneLinkFilter filter);
		[OperationContract]
		void SaveSKDJournalItems(IEnumerable<SKDJournalItem> journalItems);
		[OperationContract]
		void SaveFrames(IEnumerable<Frame> frames);
		[OperationContract]
		void SaveCards(IEnumerable<Card> items);
		[OperationContract]
		void SaveCardZoneLinks(IEnumerable<CardZoneLink> items);

		#region DeviceCommands
		[OperationContract]
		OperationResult<string> SKDGetDeviceInfo(Guid deviceUID);

		[OperationContract]
		OperationResult<bool> SKDSyncronyseTime(Guid deviceUID);

		[OperationContract]
		OperationResult<bool> SKDWriteConfiguration(Guid deviceUID);

		[OperationContract]
		OperationResult<bool> SKDUpdateFirmware(Guid deviceUID, string fileName);

		[OperationContract]
		void SKDSetIgnoreRegime(Guid deviceUID);

		[OperationContract]
		void SKDResetIgnoreRegime(Guid deviceUID);

		[OperationContract]
		void SKDOpenDevice(Guid deviceUID);

		[OperationContract]
		void SKDCloseDevice(Guid deviceUID);

		[OperationContract]
		void SKDExecuteDeviceCommand(Guid deviceUID, XStateBit stateBit);

		[OperationContract]
		void SKDAllowReader(Guid deviceUID);

		[OperationContract]
		void SKDDenyReader(Guid deviceUID);
		#endregion
        
    }
}
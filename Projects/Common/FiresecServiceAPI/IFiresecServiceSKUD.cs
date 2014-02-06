using System;
using System.Collections.Generic;
using System.ServiceModel;
using XFiresecAPI;

namespace FiresecAPI
{
	[ServiceContract(SessionMode = SessionMode.Required)]
	public interface IFiresecServiceSKUD
	{
		#region Get
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
		IEnumerable<SKDCard> GetCards(CardFilter filter);
		[OperationContract]
		IEnumerable<CardZoneLink> GetCardZoneLinks(CardZoneLinkFilter filter);
		#endregion

		#region Save
		[OperationContract]
		void SaveEmployees(IEnumerable<Employee> Employees);
		[OperationContract]
		void SaveDepartments(IEnumerable<Department> Departments);
		[OperationContract]
		void SavePositions(IEnumerable<Position> Positions);
		[OperationContract]
		void SaveSKDJournalItems(IEnumerable<SKDJournalItem> journalItems);
		[OperationContract]
		void SaveFrames(IEnumerable<Frame> frames);
		[OperationContract]
		void SaveCards(IEnumerable<SKDCard> items);
		[OperationContract]
		void SaveCardZoneLinks(IEnumerable<CardZoneLink> items);
		#endregion

		#region MarkDeleted
		[OperationContract]
		void MarkDeletedEmployees(IEnumerable<Employee> Employees);
		[OperationContract]
		void MarkDeletedDepartments(IEnumerable<Department> Departments);
		[OperationContract]
		void MarkDeletedPositions(IEnumerable<Position> Positions);
		[OperationContract]
		void MarkDeletedSKDJournalItems(IEnumerable<SKDJournalItem> journalItems);
		[OperationContract]
		void MarkDeletedFrames(IEnumerable<Frame> frames);
		[OperationContract]
		void MarkDeletedCards(IEnumerable<SKDCard> items);
		[OperationContract]
		void MarkDeletedCardZoneLinks(IEnumerable<CardZoneLink> items);
		#endregion

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
		void SKDSetRegimeOpen(Guid deviceUID);

		[OperationContract]
		void SKDSetRegimeClose(Guid deviceUID);

		[OperationContract]
		void SKDSetRegimeControl(Guid deviceUID);

		[OperationContract]
		void SKDSetRegimeConversation(Guid deviceUID);

		[OperationContract]
		void SKDOpenDevice(Guid deviceUID);

		[OperationContract]
		void SKDCloseDevice(Guid deviceUID);

		[OperationContract]
		void SKDAllowReader(Guid deviceUID);

		[OperationContract]
		void SKDDenyReader(Guid deviceUID);
		#endregion
		
	}
}
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
		void SaveSKDJournalItems(IEnumerable<SKDJournalItem> journalItems);
		[OperationContract]
		IEnumerable<Frame> GetFrames(FrameFilter filter);
		[OperationContract]
		void SaveFrames(IEnumerable<Frame> frames);

		#region DeviceCommands
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
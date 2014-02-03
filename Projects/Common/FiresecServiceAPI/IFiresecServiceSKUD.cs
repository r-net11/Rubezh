using System.Collections.Generic;
using System.ServiceModel;
using FiresecAPI.Models.SKDDatabase;

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
        
    }
}
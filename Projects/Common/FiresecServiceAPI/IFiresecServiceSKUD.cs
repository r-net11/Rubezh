using System.Collections.Generic;
using System.ServiceModel;
using FiresecAPI.Models.Skud;

namespace FiresecAPI
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface IFiresecServiceSKUD
    {
        [OperationContract]
        IEnumerable<EmployeeCard> GetAllEmployees(EmployeeCardIndexFilter filter);

        [OperationContract]
        bool DeleteEmployee(int id);

        [OperationContract]
        EmployeeCardDetails GetEmployeeCard(int id);

        [OperationContract]
        int SaveEmployeeCard(EmployeeCardDetails employeeCard);

        [OperationContract]
        IEnumerable<EmployeeDepartment> GetEmployeeDepartments();
        [OperationContract]
        IEnumerable<EmployeeGroup> GetEmployeeGroups();
        [OperationContract]
        IEnumerable<EmployeePosition> GetEmployeePositions();

        [OperationContract]
        IEnumerable<Employee> GetEmployees(EmployeeFilter filter);
        [OperationContract]
        IEnumerable<Department> GetDepartments(DepartmentFilter filter);
        [OperationContract]
        IEnumerable<Position> GetPositions(PositionFilter filter);
        
    }
}
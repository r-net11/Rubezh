using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models.Skud;

namespace FiresecService.SKUD
{
	public class FiresecServiceSKUD : IFiresecServiceSKUD
	{
        DataAccess.SKUDDataContext Context;
        
        public FiresecServiceSKUD()
        {
            Context = new DataAccess.SKUDDataContext();
        }


		#region IFiresecServiceSKUD Members

		public IEnumerable<EmployeeCard> GetAllEmployees(EmployeeCardIndexFilter filter)
		{
			return new List<EmployeeCard>(); 
		}

		public bool DeleteEmployee(int id)
		{
			return false; 
		}

		public EmployeeCardDetails GetEmployeeCard(int id)
		{
			return new EmployeeCardDetails(); 
		}

		public int SaveEmployeeCard(EmployeeCardDetails employeeCard)
		{
			return 0;
		}

		public IEnumerable<EmployeeDepartment> GetEmployeeDepartments()
		{
			return new List<EmployeeDepartment>();
		}

		public IEnumerable<EmployeeGroup> GetEmployeeGroups()
		{
			return new List<EmployeeGroup>();
		}

		public IEnumerable<EmployeePosition> GetEmployeePositions()
		{
			return new List<EmployeePosition>();
		}

        public IEnumerable<Employee> GetEmployees(EmployeeFilter filter)
        {
            try
            {
                var employees = new List<Employee>();
                var databaseEmployees = Context.Employee.ToList().Where(x => IsInFilter(x, filter)).ToList();
                databaseEmployees.ForEach(x => employees.Add(Translator.Translate(x)));
                return employees;
            }
            catch { return new List<Employee>(); }
        }

        public IEnumerable<Department> GetDepartments(DepartmentFilter filter)
        {
            try
            {
                var departments = new List<Department>();
                var databaseDepartments = Context.Department.ToList().Where(x => IsInFilter(x, filter)).ToList();
                databaseDepartments.ForEach(x => departments.Add(Translator.Translate(x)));
                return departments;
            }
            catch { return new List<Department>(); }
        }
        
        public IEnumerable<Position> GetPositions(PositionFilter filter)
        {
            try
            {
                var positions = new List<Position>();
                var databasePositions = Context.Position.ToList().Where(x => IsInFilter(x, filter)).ToList();
                databasePositions.ForEach(x => positions.Add(Translator.Translate(x)));
                return positions;
            }
            catch { return new List<Position>(); }
        }
        #endregion

        bool IsInFilter(FiresecService.SKUD.DataAccess.Employee employee, EmployeeFilter filter)
        {
            if (filter == null)
                return true;
            
            bool isInUids = !filter.HasUids || filter.Uids.Any(x => employee.Uid == x);
            bool isInDepartments = !filter.HasDepartments || filter.DepartmentUids.Any(x => employee.DepartmentUid == x);
            bool isInPositions = !filter.HasPositions || filter.PositionUids.Any(x => employee.PositionUid == x);
            bool isInAppointed = filter.Appointed == null ||
                (employee.Appointed >= filter.Appointed.StartDate && employee.Appointed <= filter.Appointed.EndDate);
            bool isInDismissed = filter.Dismissed == null ||
                (employee.Dismissed >= filter.Dismissed.StartDate && employee.Dismissed <= filter.Dismissed.EndDate);

            return isInUids && isInDepartments && isInPositions && isInAppointed && isInDepartments;
        }

        bool IsInFilter(FiresecService.SKUD.DataAccess.Department item, DepartmentFilter filter)
        {
            if (filter == null)
                return true;

            bool isInUids = !filter.HasUids || filter.Uids.Any(x => item.Uid == x);

            return isInUids;
        }

        bool IsInFilter(FiresecService.SKUD.DataAccess.Position item, PositionFilter filter)
        {
            if (filter == null)
                return true;

            bool isInUids = !filter.HasUids || filter.Uids.Any(x => item.Uid == x);

            return isInUids;
        }
	}
}
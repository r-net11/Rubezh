using System.Collections.Generic;
using FiresecAPI.Models.Skud;
using System;
using System.Linq;

namespace FiresecClient
{
	public partial class FiresecManager
	{
		public static IEnumerable<EmployeeCard> GetAllEmployees(EmployeeCardIndexFilter filter)
		{
			return FiresecService.GetAllEmployees(filter);
		}
		public static bool DeleteEmployeeCard(EmployeeCard card)
		{
			return FiresecService.DeleteEmployee(card.Id);
		}
		public static EmployeeCardDetails GetEmployeeCard(EmployeeCard card)
		{
			return FiresecService.GetEmployeeCard(card.Id);
		}
		public static bool SaveEmployeeCard(EmployeeCardDetails card)
		{
			int id = FiresecService.SaveEmployeeCard(card);
			if (id != -1)
				card.Id = id;
			return id != -1;
		}

		public static IEnumerable<EmployeeDepartment> GetEmployeeDepartments()
		{
			return FiresecService.GetEmployeeDepartments();
		}
		public static IEnumerable<EmployeeGroup> GetEmployeeGroups()
		{
			return FiresecService.GetEmployeeGroups();
		}
		public static IEnumerable<EmployeePosition> GetEmployeePositions()
		{
			return FiresecService.GetEmployeePositions();
		}
        public static IEnumerable<Employee> GetEmployees(EmployeeFilter filter)
        {
            return FiresecService.GetEmployees(filter);
        }
        public static IEnumerable<Department> GetDepartments(DepartmentFilter filter)
        {
            return FiresecService.GetDepartments(filter);
        }
        public static Department GetDepartment(Guid? uid)
        {
            if (uid == null)
                return new Department(); ;
            var filter = new DepartmentFilter();
            filter.Uids.Add((Guid)uid);
            return FiresecService.GetDepartments(filter).ToList().FirstOrDefault();
        }
        public static IEnumerable<Position> GetPositions(PositionFilter filter)
        {
            return FiresecService.GetPositions(filter);
        }
        public static Position GetPosition(Guid? uid)
        {
            if (uid == null)
                return new Position();
            var filter = new PositionFilter();
            filter.Uids.Add((Guid)uid);
            return FiresecService.GetPositions(filter).ToList().FirstOrDefault();
        }
	}
}
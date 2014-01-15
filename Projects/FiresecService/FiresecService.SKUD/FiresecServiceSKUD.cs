using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models.Skud;

namespace FiresecService.SKUD
{
	public class FiresecServiceSKUD : IFiresecServiceSKUD
	{
        DataAccess.SKUDDataContext Context;
        Translator Translator;

        public FiresecServiceSKUD()
        {
            Context = new DataAccess.SKUDDataContext();
            Translator = new Translator(Context);
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

        public IEnumerable<Employee> GetEmployees()
        {
            try
            {
                var employees = new List<Employee>();
                Context.Employee.ToList().ForEach(x => employees.Add(Translator.Translate(x)));
                return employees;
            }
            catch { return new List<Employee>(); }
        }

		#endregion
	}
}
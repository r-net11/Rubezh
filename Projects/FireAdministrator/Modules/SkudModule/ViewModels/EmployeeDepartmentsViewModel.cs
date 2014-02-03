using System.Collections.Generic;
using FiresecAPI.Models.SKDDatabase;
using FiresecClient;

namespace SKDModule.ViewModels
{
	public class EmployeeDepartmentsViewModel : EmployeeDictionaryViewModel<EmployeeDepartment>
	{
		protected override IEnumerable<EmployeeDepartment> GetDictionary()
		{
			return new List<EmployeeDepartment>();
			//return FiresecManager.GetEmployeeDepartments();
		}
	}
}
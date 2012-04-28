using System.Collections.Generic;
using FiresecAPI.Models.Skud;
using FiresecClient;

namespace SkudModule.ViewModels
{
	public class EmployeeDepartmentsViewModel : EmployeeDictionaryViewModel<EmployeeDepartment>
	{
		protected override IEnumerable<EmployeeDepartment> GetDictionary()
		{
			return FiresecManager.GetEmployeeDepartments();
		}
	}
}
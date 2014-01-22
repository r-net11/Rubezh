using System.Collections.Generic;
using FiresecAPI.Models.Skud;
using FiresecClient;

namespace SKDModule.ViewModels
{
	public class EmployeeGroupsViewModel : EmployeeDictionaryViewModel<EmployeeGroup>
	{
		protected override IEnumerable<EmployeeGroup> GetDictionary()
		{
			return FiresecManager.GetEmployeeGroups();
		}
	}
}
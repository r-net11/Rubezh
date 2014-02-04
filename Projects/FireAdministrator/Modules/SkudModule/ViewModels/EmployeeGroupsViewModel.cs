using System.Collections.Generic;
using FiresecClient;
using FiresecAPI.Models.SKDDatabase;

namespace SKDModule.ViewModels
{
	public class EmployeeGroupsViewModel : EmployeeDictionaryViewModel<EmployeeGroup>
	{
		protected override IEnumerable<EmployeeGroup> GetDictionary()
		{
			return new List<EmployeeGroup>();
			//return FiresecManager.GetEmployeeGroups();
		}
	}
}
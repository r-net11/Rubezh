using System.Collections.Generic;
using FiresecAPI.Models.SKDDatabase;
using FiresecClient;

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
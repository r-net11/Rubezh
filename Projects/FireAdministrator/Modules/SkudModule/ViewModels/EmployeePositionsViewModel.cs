using System.Collections.Generic;
using FiresecAPI.Models.SKDDatabase;
using FiresecClient;

namespace SKDModule.ViewModels
{
	public class EmployeePositionsViewModel : EmployeeDictionaryViewModel<EmployeePosition>
	{
		protected override IEnumerable<EmployeePosition> GetDictionary()
		{
			return new List<EmployeePosition>();
			//return FiresecManager.GetEmployeePositions();
		}
	}
}
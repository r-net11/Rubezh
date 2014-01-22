using System.Collections.Generic;
using FiresecAPI.Models.Skud;
using FiresecClient;

namespace SKDModule.ViewModels
{
	public class EmployeePositionsViewModel : EmployeeDictionaryViewModel<EmployeePosition>
	{
		protected override IEnumerable<EmployeePosition> GetDictionary()
		{
			return FiresecManager.GetEmployeePositions();
		}
	}
}
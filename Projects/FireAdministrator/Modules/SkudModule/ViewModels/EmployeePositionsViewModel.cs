using System.Collections.Generic;
using FiresecAPI.Models.Skud;
using FiresecClient;

namespace SkudModule.ViewModels
{
	public class EmployeePositionsViewModel : EmployeeDictionaryViewModel<EmployeePosition>
	{
		protected override IEnumerable<EmployeePosition> GetDictionary()
		{
			return FiresecManager.GetEmployeePositions();
		}
	}
}
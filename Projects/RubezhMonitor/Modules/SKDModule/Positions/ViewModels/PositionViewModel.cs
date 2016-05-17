using RubezhAPI.SKD;

namespace SKDModule.ViewModels
{
	public class PositionViewModel : OrganisationElementViewModel<PositionViewModel, ShortPosition>, IEmployeeListParent 
	{
		public void InitializeEmployeeList()
		{
			if (EmployeeListViewModel == null)
			{
				EmployeeListViewModel = new PositionEmployeeListViewModel(this);
			}
		}
		public PositionEmployeeListViewModel EmployeeListViewModel { get; private set; }
	}
}
using FiresecAPI.SKD;
using FiresecClient;

namespace SKDModule.ViewModels
{
	public class PositionViewModel : OrganisationElementViewModel<PositionViewModel, ShortPosition> 
	{
		public PositionEmployeeListViewModel EmployeeListViewModel { get; private set; }

		public override void InitializeModel(Organisation organisation, ShortPosition model, Infrastructure.Common.Windows.ViewModels.ViewPartViewModel parentViewModel)
		{
			base.InitializeModel(organisation, model, parentViewModel);
			InitializeEmployeeList();
		}

		public void InitializeEmployeeList()
		{
			EmployeeListViewModel = new PositionEmployeeListViewModel(this, (ParentViewModel as PositionsViewModel).IsWithDeleted);
		}

		public bool IsShowEmployeeList
		{
			get { return !IsOrganisation && FiresecManager.CheckPermission(FiresecAPI.Models.PermissionType.Oper_SKD_Employees_View); }
		}

	}
}
using FiresecAPI.SKD;
using FiresecClient;

namespace SKDModule.ViewModels
{
	public class DepartmentViewModel : OrganisationElementViewModel<DepartmentViewModel, ShortDepartment>
	{
		public DepartmentEmployeeListViewModel EmployeeListViewModel { get; private set; } 

		public override void InitializeModel(Organisation organisation, ShortDepartment model, Infrastructure.Common.Windows.ViewModels.ViewPartViewModel parentViewModel)
		{
			base.InitializeModel(organisation, model, parentViewModel);
			InitializeEmployeeList();
		}

		public void InitializeEmployeeList()
		{
			EmployeeListViewModel = new DepartmentEmployeeListViewModel(this, (ParentViewModel as DepartmentsViewModel).IsWithDeleted);
		}

		public string Phone
		{
			get
			{
				if (IsOrganisation)
					return Organisation.Phone;
				else
					return Model.Phone;
			}
		}

		public override void Update()
		{
			base.Update();
			OnPropertyChanged(() => Phone);
		}

		public bool IsShowEmployeeList
		{
			get { return !IsOrganisation && FiresecManager.CheckPermission(FiresecAPI.Models.PermissionType.Oper_SKD_Employees_View); }
		}
	}
}
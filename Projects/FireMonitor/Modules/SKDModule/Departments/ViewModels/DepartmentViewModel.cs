using RubezhAPI.SKD;

namespace SKDModule.ViewModels
{
	public class DepartmentViewModel : OrganisationElementViewModel<DepartmentViewModel, ShortDepartment>, IEmployeeListParent
	{
		public override void InitializeModel(Organisation organisation, ShortDepartment model, Infrastructure.Common.Windows.Windows.ViewModels.ViewPartViewModel parentViewModel)
		{
			base.InitializeModel(organisation, model, parentViewModel);
		}

		public string Phone
		{
			get
			{
				if (IsOrganisation)
					return "";
				else
					return Model.Phone;
			}
		}

		public void InitializeEmployeeList()
		{
			if (EmployeeListViewModel == null)
			{
				EmployeeListViewModel = new DepartmentEmployeeListViewModel(this);
			}
		}

		public DepartmentEmployeeListViewModel EmployeeListViewModel { get; private set; }

		public override void Update()
		{
			base.Update();
			OnPropertyChanged(() => Phone);
		}
	}
}
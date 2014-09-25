using FiresecAPI.SKD;

namespace SKDModule.ViewModels
{
	public class DepartmentViewModel : CartothequeTabItemElementBase<DepartmentViewModel, ShortDepartment>
	{
		public DepartmentEmployeeListViewModel EmployeeListViewModel { get; private set; } 

		public override void InitializeModel(Organisation organisation, ShortDepartment model, Infrastructure.Common.Windows.ViewModels.ViewPartViewModel parentViewModel)
		{
			base.InitializeModel(organisation, model, parentViewModel);
			EmployeeListViewModel = new DepartmentEmployeeListViewModel(Model, organisation.UID);
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
	}
}
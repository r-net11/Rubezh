using FiresecAPI.SKD;

namespace SKDModule.ViewModels
{
	public class DepartmentViewModel : CartothequeTabItemElementBase<DepartmentViewModel, ShortDepartment>
	{
		public DepartmentEmployeeListViewModel EmployeeListViewModel { get; private set; } 

		public override void InitializeModel(Organisation organisation, ShortDepartment model, Infrastructure.Common.Windows.ViewModels.ViewPartViewModel parentViewModel)
		{
			base.InitializeModel(organisation, model, parentViewModel);
			var departmentsViewModel = parentViewModel as DepartmentsViewModel;
			EmployeeListViewModel = new DepartmentEmployeeListViewModel(Model.UID, organisation.UID, null);
		}
	}
}
using System;
using FiresecAPI.SKD;

namespace SKDModule.ViewModels
{
	public class DepartmentViewModel : CartothequeTabItemElementBase<DepartmentViewModel, ShortDepartment>
	{
		public DepartmentEmployeeListViewModel DepartmentEmployeeListViewModel { get; private set; } 

		public override void InitializeModel(Organisation organisation, ShortDepartment model, Infrastructure.Common.Windows.ViewModels.ViewPartViewModel parentViewModel)
		{
			base.InitializeModel(organisation, model, parentViewModel);
			DepartmentEmployeeListViewModel = new DepartmentEmployeeListViewModel(Model.UID);
		}
	}
}
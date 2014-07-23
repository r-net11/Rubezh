using FiresecAPI.SKD;
using Infrastructure.Common.TreeList;

namespace SKDModule.ViewModels
{
	public class DepartmentViewModel : TreeNodeViewModel<DepartmentViewModel>
	{
		public Organisation Organisation { get; private set; }
		public bool IsOrganisation { get; private set; }
		public string Name { get; private set; }
		public string Description { get; private set; }
		public ShortDepartment Department { get; private set; }

		public DepartmentViewModel(Organisation organisation)
		{
			Organisation = organisation;
			IsOrganisation = true;
			Name = organisation.Name;
			IsExpanded = true;
		}

		public DepartmentViewModel(Organisation organisation, ShortDepartment department)
		{
			Organisation = organisation;
			Department = department;
			IsOrganisation = false;
			Name = department.Name;
			Description = department.Description;
		}

		public void Update(ShortDepartment department)
		{
			Name = department.Name;
			Description = department.Description;
			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => Description);
		}

		public void Update()
		{
			OnPropertyChanged(() => Department);
		}
	}
}
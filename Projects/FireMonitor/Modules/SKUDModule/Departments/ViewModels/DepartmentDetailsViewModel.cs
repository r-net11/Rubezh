using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class DepartmentDetailsViewModel : SaveCancelDialogViewModel
	{
		public Department Department { get; private set; }
		Department ParentDepartment;
		OrganisationDepartmentsViewModel OrganisationDepartmentsViewModel;

		public DepartmentDetailsViewModel(OrganisationDepartmentsViewModel organisationDepartmentsViewModel, Department department = null, Department parentDepartment = null)
		{
			OrganisationDepartmentsViewModel = organisationDepartmentsViewModel;
			if (department == null)
			{
				Title = "Создание отдела";
				department = new Department()
				{
					Name = "Новый отдел",
				};
			}
			else
			{
				Title = string.Format("Свойства отдела: {0}", department.Name);
			}
			Department = department;
			ParentDepartment = parentDepartment;
			CopyProperties();
		}

		public void CopyProperties()
		{
			Name = Department.Name;
			Description = Department.Description;
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				if (_name != value)
				{
					_name = value;
					OnPropertyChanged("Name");
				}
			}
		}

		string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				if (_description != value)
				{
					_description = value;
					OnPropertyChanged("Description");
				}
			}
		}

		protected override bool CanSave()
		{
			return true;
		}

		protected override bool Save()
		{
			Department.Name = Name;
			Department.Description = Description;
			Department.OrganizationUID = OrganisationDepartmentsViewModel.Organization.UID;
			if(ParentDepartment != null)
				Department.ParentDepartmentUID = ParentDepartment.UID;
			return true;
		}
	}
}
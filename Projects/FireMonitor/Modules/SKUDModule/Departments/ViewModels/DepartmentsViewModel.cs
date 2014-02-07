using System.Collections.ObjectModel;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class DepartmentsViewModel : ViewPartViewModel
	{
		public DepartmentsViewModel()
		{
		}

		ObservableCollection<Department> _departments;
		public ObservableCollection<Department> Departments
		{
			get { return _departments; }
			set
			{
				_departments = value;
				OnPropertyChanged("Departments");
			}
		}

		Department _selectedDepartment;
		public Department SelectedDepartment
		{
			get { return _selectedDepartment; }
			set
			{
				_selectedDepartment = value;
				OnPropertyChanged("SelectedDepartment");
			}
		}
	}
}
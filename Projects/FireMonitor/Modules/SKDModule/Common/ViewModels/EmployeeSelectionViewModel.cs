using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.SKD;
using RubezhClient.SKDHelpers;
using System.Collections.ObjectModel;
using System.Linq;

namespace SKDModule.ViewModels
{
	public class EmployeeSelectionDialogViewModel : SaveCancelDialogViewModel
	{
		EmployeeFilter _filter;

		public EmployeeSelectionDialogViewModel(ShortEmployee selectedEmployee, EmployeeFilter filter)
		{
			Initialize(filter);
			if (selectedEmployee != null)
			{
				SelectedEmployee = Employees.FirstOrDefault(x => x.UID == selectedEmployee.UID);
				if (SelectedEmployee == null)
				{
					Employees.Insert(0, selectedEmployee);
					SelectedEmployee = selectedEmployee;
				}
			}
		}

		public EmployeeSelectionDialogViewModel(EmployeeFilter filter)
		{
			Initialize(filter);
		}

		void Initialize(EmployeeFilter filter)
		{
			Title = "Выбор сотрудника";
			_filter = filter;
			var employeeModels = EmployeeHelper.Get(_filter);
			if (employeeModels == null)
				return;
			Employees = new ObservableCollection<ShortEmployee>(employeeModels);
		}

		public ObservableCollection<ShortEmployee> Employees { get; set; }

		protected override bool CanSave()
		{
			return SelectedEmployee != null;
		}

		ShortEmployee _selectedEmployee;
		public ShortEmployee SelectedEmployee
		{
			get { return _selectedEmployee; }
			set
			{
				_selectedEmployee = value;
				OnPropertyChanged(() => SelectedEmployee);
			}
		}
	}
}

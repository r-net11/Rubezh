using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels 
{
	public class EmployeeSelectionViewModel : SaveCancelDialogViewModel
	{
		EmployeeFilter _filter;

		public EmployeeSelectionViewModel(Guid selectedEmployeeUID, EmployeeFilter filter)
		{
			Initialize(filter);
			if (selectedEmployeeUID != Guid.Empty)
			{
				SelectedEmployee = Employees.FirstOrDefault(x => x.UID == selectedEmployeeUID);
			}
		}

		public EmployeeSelectionViewModel(EmployeeFilter filter)
		{
			Initialize(filter);
		}

		void Initialize(EmployeeFilter filter)
		{
			Title = "Выбор руководителя";
			_filter = filter;
			var employeeModels = EmployeeHelper.Get(_filter);
			if (employeeModels == null)
				return;
			Employees = new ObservableCollection<ShortEmployee>(employeeModels);
		}

		public ObservableCollection<ShortEmployee> Employees { get; set; }

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

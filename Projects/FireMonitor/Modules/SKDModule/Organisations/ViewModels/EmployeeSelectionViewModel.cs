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
			Title = "Выбор руководителя";
			_filter = filter;
			var employeeModels = EmployeeHelper.Get(_filter);
			if (employeeModels == null)
				return;	
			Employees = new ObservableCollection<ShortEmployee>(employeeModels);
			if (selectedEmployeeUID != Guid.Empty)
			{
				SelectedEmployee = Employees.FirstOrDefault(x => x.UID == selectedEmployeeUID);
			}
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

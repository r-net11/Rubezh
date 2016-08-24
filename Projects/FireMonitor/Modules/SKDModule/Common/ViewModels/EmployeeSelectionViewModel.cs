using System;
using System.Collections.ObjectModel;
using System.Linq;
using Localization.SKD.ViewModels;
using StrazhAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels 
{
	public class EmployeeSelectionDialogViewModel : SaveCancelDialogViewModel
	{
		EmployeeFilter _filter;

		public EmployeeSelectionDialogViewModel(Guid selectedEmployeeUID, EmployeeFilter filter)
		{
			Initialize(filter);
			if (selectedEmployeeUID != Guid.Empty)
			{
				SelectedEmployee = Employees.FirstOrDefault(x => x.UID == selectedEmployeeUID);
			}
		}

		public EmployeeSelectionDialogViewModel(EmployeeFilter filter)
		{
			Initialize(filter);
		}

		void Initialize(EmployeeFilter filter)
		{
			Title = CommonViewModels.SelectEmployee;
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

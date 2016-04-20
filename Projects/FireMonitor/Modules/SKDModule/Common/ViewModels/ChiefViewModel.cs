using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.SKD;
using RubezhClient.SKDHelpers;
using System;

namespace SKDModule.ViewModels
{
	public class EmployeeSelectationViewModel : BaseViewModel
	{
		EmployeeFilter _filter;

		public EmployeeSelectationViewModel(Guid chiefUID, EmployeeFilter filter)
		{
			SelectCommand = new RelayCommand(OnSelect);
			SelectedEmployee = EmployeeHelper.GetSingleShort(chiefUID);
			_filter = filter;
		}

		ShortEmployee _selectedEmployee;
		public ShortEmployee SelectedEmployee
		{
			get { return _selectedEmployee; }
			set
			{
				_selectedEmployee = value;
				OnPropertyChanged(() => SelectedEmployee);
				OnPropertyChanged(() => HasSelected);
			}
		}
		public bool HasSelected
		{
			get { return SelectedEmployee != null && !SelectedEmployee.IsDeleted; }
		}
		public Guid SelectedEmployeeUID
		{
			get { return SelectedEmployee != null ? SelectedEmployee.UID : Guid.Empty; }
		}

		public RelayCommand SelectCommand { get; private set; }
		void OnSelect()
		{
			var employeeSelectionViewModel = new EmployeeSelectionDialogViewModel(HasSelected ? SelectedEmployee : null, _filter);
			if (DialogService.ShowModalWindow(employeeSelectionViewModel))
			{
				SelectedEmployee = employeeSelectionViewModel.SelectedEmployee;
			}
		}
	}
}

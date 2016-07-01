using System;
using StrazhAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class EmployeeSelectationViewModel : BaseViewModel
	{
		private readonly EmployeeFilter _filter;
		private ShortEmployee _selectedEmployee;

		public EmployeeSelectationViewModel(Guid chiefUID, EmployeeFilter filter)
		{
			SelectCommand = new RelayCommand(OnSelect);
			SelectedEmployee = EmployeeHelper.GetSingleShort(chiefUID);
			_filter = filter;
		}

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

		void OnSelect()
		{
			var employeeSelectionViewModel = new EmployeeSelectionDialogViewModel(HasSelected ? SelectedEmployee.UID : Guid.Empty, _filter);
			if (DialogService.ShowModalWindow(employeeSelectionViewModel))
			{
				SelectedEmployee = employeeSelectionViewModel.SelectedEmployee;
			}
		}

		public RelayCommand SelectCommand { get; private set; }
	}
}

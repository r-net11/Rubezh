using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StrazhAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class EmployeeSelectionViewModel : SaveCancelDialogViewModel
	{
		private EmployeeViewModel _selectedEmployee;

		public EmployeeViewModel SelectedEmployee
		{
			get { return _selectedEmployee; }
			set
			{
				if (_selectedEmployee == value)
					return;
				_selectedEmployee = value;
				OnPropertyChanged(() => SelectedEmployee);
			}
		}

		public List<EmployeeViewModel> Employees { get; private set; }

		public EmployeeSelectionViewModel(ShortEmployee employee)
		{
			Title = "Выбор сотрудника";

			BuildTreeAndSelect(employee);
		}

		private void BuildTreeAndSelect(ShortEmployee selectedEmployee)
		{
			Employees = new List<EmployeeViewModel>();
			foreach (var organisation in OrganisationHelper.Get(new OrganisationFilter { LogicalDeletationType = LogicalDeletationType.Active }).OrderBy(x => x.Name))
			{
				var rootNode = new EmployeeViewModel(organisation);
				foreach(var employee in EmployeeHelper.Get(new EmployeeFilter { OrganisationUIDs = new List<Guid> { organisation.UID }, LogicalDeletationType = LogicalDeletationType.Active }))
				{
					var childNode = new EmployeeViewModel(employee);
					if (selectedEmployee != null && employee.UID == selectedEmployee.UID)
					{
						SelectedEmployee = childNode;
						rootNode.IsExpanded = true;
					}
					rootNode.AddChild(childNode);
				}
				Employees.Add(rootNode);
			}
		}

		protected override bool CanSave()
		{
			return SelectedEmployee != null && SelectedEmployee.IsEmployee;
		}
	}
}

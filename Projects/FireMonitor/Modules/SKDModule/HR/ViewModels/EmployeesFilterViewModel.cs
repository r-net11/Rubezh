using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;

namespace SKDModule.ViewModels
{
	public class EmployeesFilterViewModel : OrganisationBaseViewModel<ShortEmployee, EmployeeFilter, EmployeesFilterItemViewModel, EmployeeDetailsViewModel>
	{
		public EmployeesFilterViewModel():base()
		{
			SelectAllCommand = new RelayCommand(OnSelectAll);
			SelectNoneCommand = new RelayCommand(OnSelectNone);
		}

		public RelayCommand SelectAllCommand { get; private set; }
		void OnSelectAll()
		{
			var employees = Organisations.SelectMany(x => x.Children);
			foreach (var employee in employees)
				employee.IsChecked = true;
		}

		public RelayCommand SelectNoneCommand { get; private set; }
		void OnSelectNone()
		{
			var employees = Organisations.SelectMany(x => x.Children);
			foreach (var employee in employees)
				employee.IsChecked = false;
		}
		
		protected override IEnumerable<ShortEmployee> GetModels(EmployeeFilter filter)
		{
			return EmployeeHelper.Get(filter);
		}

		protected override IEnumerable<ShortEmployee> GetModelsByOrganisation(Guid organisationUID)
		{
			return EmployeeHelper.GetShortByOrganisation(organisationUID);
		}

		protected override bool MarkDeleted(ShortEmployee model)
		{
			return EmployeeHelper.MarkDeleted(model);
		}

		protected override bool Restore(ShortEmployee model)
		{
			return EmployeeHelper.Restore(model);
		}

		protected override bool Add(ShortEmployee item)
		{
			throw new NotImplementedException();
		}

		public void SetEmployees(List<Guid> employeeUIDs)
		{
			var employees = Organisations.SelectMany(x => x.Children).Where(x => employeeUIDs.Any(y => y == x.Model.UID));
			foreach (var employee in employees)
			{
				employee.IsChecked = true;
			}
		}

		public List<Guid> EmployeeUIDs
		{
			get { return Organisations.SelectMany(x => x.Children).Where(x => x.IsChecked).Select(x => x.Model.UID).ToList(); }
		}
	}
}

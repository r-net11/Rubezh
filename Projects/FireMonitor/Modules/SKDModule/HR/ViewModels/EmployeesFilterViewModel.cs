using System;
using System.Collections.Generic;
using System.Linq;
using RubezhAPI.SKD;
using RubezhClient.SKDHelpers;
using Infrastructure.Common;

namespace SKDModule.ViewModels
{
	public class EmployeesFilterViewModel : OrganisationBaseViewModel<ShortEmployee, EmployeeFilter, EmployeesFilterItemViewModel, EmployeeDetailsViewModel>
	{
		public EmployeesFilterViewModel()
			: base()
		{
			SelectAllCommand = new RelayCommand(OnSelectAll);
			SelectNoneCommand = new RelayCommand(OnSelectNone);
		}

		EmployeeFilter _Filter;

		public override void Initialize(EmployeeFilter filter)
		{
			Initialize(filter, filter.LogicalDeletationType, filter.PersonType);
		}

		public void Initialize(EmployeeFilter filter, LogicalDeletationType logicalDeletationType, PersonType personType)
		{
			_Filter = filter;
			var emptyFilter = new EmployeeFilter { LogicalDeletationType = logicalDeletationType, PersonType = personType };
			base.Initialize(emptyFilter);
			FirstName = filter.FirstName;
			LastName = filter.LastName;
			SecondName = filter.SecondName;
			IsSearch = (FirstName != null && FirstName.Length != 0) ||
				(LastName != null && LastName.Length != 0) ||
				(SecondName != null && SecondName.Length != 0);
			if (filter.UIDs == null)
				return;
			var employees = Organisations.SelectMany(x => x.Children).Where(x => filter.UIDs.Any(y => y == x.Model.UID));
			foreach (var employee in employees)
				employee.IsChecked = true;
		}

		public void Initialize(List<Guid> uids, LogicalDeletationType logicalDeletationType = LogicalDeletationType.Active, PersonType personType = PersonType.Employee)
		{
			var filter = new EmployeeFilter { LogicalDeletationType = logicalDeletationType, UIDs = uids, PersonType = personType };
			Initialize(filter);
		}

		public void Initialize()
		{
			Initialize(_Filter);
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

		public EmployeeFilter Filter
		{
			get
			{
				var filter = new EmployeeFilter();
				if (IsSelection)
				{
					filter.UIDs = Organisations.SelectMany(x => x.Children).Where(x => x.IsChecked).Select(x => x.Model.UID).ToList();
				}
				else
				{
					filter.FirstName = FirstName;
					filter.LastName = LastName;
					filter.SecondName = SecondName;
				}
				return filter;
			}
		}

		public bool IsSelection { get { return !IsSearch; } }
		bool _isSearch;
		public bool IsSearch
		{
			get { return _isSearch; }
			set
			{
				_isSearch = value;
				OnPropertyChanged(() => IsSearch);
				OnPropertyChanged(() => IsSelection);
			}
		}

		string _firstName;
		public string FirstName
		{
			get { return _firstName; }
			set
			{
				_firstName = value;
				OnPropertyChanged(() => FirstName);
			}
		}

		string _lastName;
		public string LastName
		{
			get { return _lastName; }
			set
			{
				_lastName = value;
				OnPropertyChanged(() => LastName);
			}
		}

		string _secondName;
		public string SecondName
		{
			get { return _secondName; }
			set
			{
				_secondName = value;
				OnPropertyChanged(() => SecondName);
			}
		}


		protected override RubezhAPI.Models.PermissionType Permission
		{
			get { return RubezhAPI.Models.PermissionType.Oper_SKD_Employees_Edit; }
		}

		public List<Guid> UIDs { get { return Organisations.SelectMany(x => x.Children).Where(x => x.IsChecked).Select(x => x.Model.UID).ToList(); } }
	}
}
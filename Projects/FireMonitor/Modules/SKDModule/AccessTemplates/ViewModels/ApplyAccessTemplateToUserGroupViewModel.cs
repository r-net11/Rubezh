using Common;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.SKDReports;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Events;
using StrazhAPI.Models;
using StrazhAPI.SKD;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SKDModule.ViewModels
{
	public class ApplyAccessTemplateToUserGroupViewModel : SaveCancelDialogViewModel
	{
		private Guid _organisationID;
		private HRFilter _filter;

		#region <Верхняя инструментальная панель>

		#region <Выбор режима работы "Сотрудник / Посетитель>

		public ObservableCollection<PersonType> PersonTypes { get; private set; }

		private PersonType _selectedPersonType;
		public PersonType SelectedPersonType
		{
			get { return _selectedPersonType; }
			set
			{
				_selectedPersonType = value;
				OnPropertyChanged(() => SelectedPersonType);
				_filter = CreateDefaultFilter(_organisationID, value);
				Employees.Update(_filter);
			}
		}

		#endregion </Выбор режима работы "Сотрудник / Посетитель>

		#region <Фильтр>

		public RelayCommand EditFilterCommand { get; private set; }
		private void OnEditFilter()
		{
			var filterViewModel = new ApplyAccessTemplateToUserGroupFilterViewModel(CreateDefaultFilter(_organisationID, SelectedPersonType), SelectedPersonType == PersonType.Employee);
			if (DialogService.ShowModalWindow(filterViewModel))
			{
				_filter = filterViewModel.Filter;
				var cardFilter = new CardFilter
				{
					EmployeeFilter = _filter.EmployeeFilter,
					AccessTemplateFilter = new AccessTemplateFilter
					{
						LogicalDeletationType = _filter.EmployeeFilter.LogicalDeletationType,
						OrganisationUIDs = _filter.EmployeeFilter.OrganisationUIDs,
						UIDs = filterViewModel.AccessTemplatesFilterViewModel.UIDs,
						UserUID = _filter.EmployeeFilter.UserUID
					},
					WithEmptyAccessTemplate = filterViewModel.AccessTemplatesFilterViewModel.ShowCardsWithEmptyAccessTemplate
				};
				Employees.Update(_filter, cardFilter);
			}
		}
		
		#endregion </Фильтр>

		#endregion </Верхняя инструментальная панель>

		public EmployeeCardsForApplyAccessTemplateToUserGroupViewModel Employees { get; private set; }

		#region <Список шаблонов доступа>

		public ObservableCollection<AccessTemplate> AccessTemplates { get; private set; }

		private AccessTemplate _selectedAccessTemplate;
		public AccessTemplate SelectedAccessTemplate
		{
			get { return _selectedAccessTemplate; }
			set
			{
				_selectedAccessTemplate = value;
				OnPropertyChanged(() => SelectedAccessTemplate);
			}
		}

		#endregion </Список шаблонов доступа>

		#region <Нижняя инструментальная панель>

		public RelayCommand SelectAllCommand { get; private set; }
		private void OnSelectAll()
		{
			Employees.SelectAll();
		}

		public RelayCommand SelectNoneCommand { get; private set; }
		private void OnSelectNone()
		{
			Employees.SelectNone();
		}

		#endregion </Нижняя инструментальная панель>

		public ApplyAccessTemplateToUserGroupViewModel(Guid organisationID, Guid accessTemplateID)
		{
			Title = "Назначение шаблона доступа группе сотрудников (посетителей)";
			_organisationID = organisationID;
			
			EditFilterCommand = new RelayCommand(OnEditFilter);
			SelectAllCommand = new RelayCommand(OnSelectAll);
			SelectNoneCommand = new RelayCommand(OnSelectNone);

			PersonTypes = new ObservableCollection<PersonType>();
			if (FiresecManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_Employees_View))
				PersonTypes.Add(PersonType.Employee);
			if (FiresecManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_Guests_View))
				PersonTypes.Add(PersonType.Guest);

			AccessTemplates = new ObservableCollection<AccessTemplate>(AccessTemplateHelper.GetByOrganisation(_organisationID));
			SelectedAccessTemplate = AccessTemplates.FirstOrDefault(x => x.UID == accessTemplateID);

			_filter = CreateDefaultFilter(_organisationID, PersonType.Employee);

			Employees = new EmployeeCardsForApplyAccessTemplateToUserGroupViewModel();
			Employees.Update(_filter);
		}

		#region <Методы>

		private static HRFilter CreateDefaultFilter(Guid organisationID, PersonType personType)
		{
			var userUID = FiresecManager.CurrentUser.UID;
			var organisationUIDs = new List<Guid> { organisationID };
			return new HRFilter
			{
				OrganisationUIDs = organisationUIDs,
				UserUID = userUID,
				EmployeeFilter = new EmployeeFilter
				{
					OrganisationUIDs = organisationUIDs,
					UserUID = userUID,
					PersonType = personType
				}
			};
		}

		protected override bool CanSave()
		{
			return SelectedAccessTemplate != null && Employees.HasCheckedCards;
		}

		protected override bool Save()
		{
			Employees.Items.ForEach(empl =>
			{
				var cards = empl.Children.Where(card => card.IsChecked);
				cards.ForEach(card =>
				{
					card.Card.AccessTemplateUID = SelectedAccessTemplate.UID;
					CardHelper.Edit(card.Card, empl.Name);
					ServiceFactoryBase.Events.GetEvent<CardAccessTemplateChangedEvent>().Publish(card.Card);
				});
			});
			return base.Save();
		}

		#endregion </Методы>
	}

	public class EmployeeCardsForApplyAccessTemplateToUserGroupViewModel : BaseViewModel
	{
		#region <Поля и свойства>

		public string PositionOrDescriptionHeaderTitle { get; private set; }

		public ObservableCollection<EmployeeCardForApplyAccessTemplateToUserGroupViewModel> Items { get; private set; }

		public bool HasCheckedCards
		{
			get
			{
				return Items.Any(item => item.Children.Any(card => card.IsChecked));
			}
		}

		#endregion </Поля и свойства>

		#region <Методы>

		public void Update(HRFilter filter, CardFilter cardFilter = null)
		{
			BuildTree(filter.EmployeeFilter, cardFilter);
			OnPropertyChanged(() => Items);
			SetPositionOrDescriptionHeaderTitle(filter.EmployeeFilter.PersonType);
		}

		private void SetPositionOrDescriptionHeaderTitle(PersonType personType)
		{
			PositionOrDescriptionHeaderTitle = personType == PersonType.Employee
				? "Должность"
				: "Примечание";
			OnPropertyChanged(() => PositionOrDescriptionHeaderTitle);
		}

		public void SelectAll()
		{
			SetChecked(true);
		}

		public void SelectNone()
		{
			SetChecked(false);
		}

		private void SetChecked(bool isChecked)
		{
			Items.ForEach(x => x.IsChecked = isChecked);
		}

		private void BuildTree(EmployeeFilter filter, CardFilter cardFilter)
		{
			var filteredEmployees = EmployeeHelper.Get(filter).ToList();
			Items = new ObservableCollection<EmployeeCardForApplyAccessTemplateToUserGroupViewModel>();
			filteredEmployees.ForEach(empl =>
			{
				//var employee = new EmployeeCardForApplyAccessTemplateToUserGroupViewModel(empl);
				//var cards = CardHelper.GetByEmployee(empl.UID);
				//cards.ForEach(card => employee.AddChild(new EmployeeCardForApplyAccessTemplateToUserGroupViewModel(card)));
				//Items.Add(employee);
				IEnumerable<SKDCard> cards;
				if (cardFilter == null)
				{
					cards = CardHelper.GetByEmployee(empl.UID).ToList();
				}
				else
				{
					cardFilter.EmployeeFilter.UIDs = new List<Guid> { empl.UID };
					cards = CardHelper.Get(cardFilter).ToList();
				}
				if (cards.Any())
				{
					var employee = new EmployeeCardForApplyAccessTemplateToUserGroupViewModel(empl);
					cards.ForEach(card => employee.AddChild(new EmployeeCardForApplyAccessTemplateToUserGroupViewModel(card)));
					Items.Add(employee);
				}
			});
		}

		#endregion </Методы>
	}

	public class EmployeeCardForApplyAccessTemplateToUserGroupViewModel : TreeNodeViewModel<EmployeeCardForApplyAccessTemplateToUserGroupViewModel>
	{
		#region <Поля и свойства>

		private bool _isChecked;
		private string _name;
		private string _department;
		private string _positionOrDescription;

		/// <summary>
		/// Признак выделения
		/// </summary>
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				SetIsChecked(value);
				PropogateDown(value);
				PropogateUp(value);
			}
		}

		/// <summary>
		/// Название (ФИО сотрудника или номер пропуска сотрудника)
		/// </summary>
		public string Name
		{
			get { return _name; }
			private set
			{
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}

		/// <summary>
		/// Подразделение
		/// </summary>
		public string Department
		{
			get { return _department; }
			private set
			{
				_department = value;
				OnPropertyChanged(() => Department);
			}
		}

		/// <summary>
		/// Должность для сотрудника или Примечание для посетителя
		/// </summary>
		public string PositionOrDescription
		{
			get { return _positionOrDescription; }
			private set
			{
				_positionOrDescription = value;
				OnPropertyChanged(() => PositionOrDescription);
			}
		}

		public bool IsEmployee { get; private set; }

		public SKDCard Card { get; private set; }

		#endregion </Поля и свойства>

		#region <Конструктор>

		public EmployeeCardForApplyAccessTemplateToUserGroupViewModel(ShortEmployee employee)
		{
			Name = employee.FIO;
			Department = employee.DepartmentName;
			PositionOrDescription = employee.Type == PersonType.Employee
				? employee.PositionName
				: employee.Description;
			IsEmployee = true;
		}

		public EmployeeCardForApplyAccessTemplateToUserGroupViewModel(SKDCard card)
		{
			Name = String.Format("Пропуск {0}", card.Number);
			Card = card;
		}

		#endregion </Конструктор>

		#region <Методы>

		private void PropogateDown(bool value)
		{
			foreach (var child in Children)
			{
				child.SetIsChecked(value);
				child.PropogateDown(value);
			}
		}

		private void PropogateUp(bool value)
		{
			if (Parent == null)
				return;

			Parent.SetIsChecked(Parent.Children.All(x => x.IsChecked));
			Parent.PropogateUp(value);
		}

		private void SetIsChecked(bool value)
		{
			_isChecked = value;
			OnPropertyChanged(() => IsChecked);
		}

		#endregion </Методы>
	}
}

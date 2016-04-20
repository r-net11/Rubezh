using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using RubezhAPI.SKD;
using RubezhClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common.Windows.Windows;
using SKDModule.Events;
using RubezhClient;
using RubezhAPI;

namespace SKDModule.ViewModels
{
	public class EmployeesViewModel : OrganisationBaseViewModel<ShortEmployee, EmployeeFilter, EmployeeViewModel, EmployeeDetailsViewModel>
	{
		public List<AdditionalColumnType> AdditionalColumnTypes { get; private set; }
       

		public EmployeesViewModel():base()
		{
			ServiceFactory.Events.GetEvent<EditEmployeeEvent>().Unsubscribe(OnEditEmployee);
			ServiceFactory.Events.GetEvent<EditEmployeeEvent>().Subscribe(OnEditEmployee);
			ServiceFactory.Events.GetEvent<EditAdditionalColumnEvent>().Unsubscribe(OnUpdateIsInGrid);
			ServiceFactory.Events.GetEvent<EditAdditionalColumnEvent>().Subscribe(OnUpdateIsInGrid);
		}

		public override void Initialize(EmployeeFilter filter)
		{
			base.Initialize(filter);
			PersonType = filter.PersonType;
			InitializeAdditionalColumns();
			ServiceFactory.Events.GetEvent<ChangeEmployeeGuestEvent>().Publish(null);
		}

		protected override void OnOrganisationUsersChanged(Organisation newOrganisation)
		{
			base.OnOrganisationUsersChanged(newOrganisation);
			InitializeAdditionalColumns();
		}

		public void InitializeAdditionalColumns()
		{
			AdditionalColumnNames = new ObservableCollection<string>();
			var columnFilter = new AdditionalColumnTypeFilter
			{
				Type = AdditionalColumnDataType.Text,
				OrganisationUIDs = Organisations.Select(x => x.UID).ToList(),
				IsInGrid = true
			};
			var columnTypes = AdditionalColumnTypeHelper.Get(columnFilter);
			if (columnTypes == null)
				return;
			AdditionalColumnTypes = columnTypes.ToList();
			foreach (var columnType in AdditionalColumnTypes)
			{
				AdditionalColumnNames.Add(columnType.Name);
			}
			ServiceFactory.Events.GetEvent<UpdateAdditionalColumns>().Publish(null);
		}

		protected override void Remove()
		{
			if (SelectedItem.Cards.Count() == 0 || MessageBoxService.ShowQuestion("Привязанные к сотруднику пропуска будут деактивированы. Продолжить?"))
			{
				var cardUIDs = SelectedItem.Cards.Select(x => x.UID);
				base.Remove();
				foreach (var uid in cardUIDs)
				{
					ServiceFactory.Events.GetEvent<BlockCardEvent>().Publish(uid);
				}
			}
		}

		protected override void OnRemoveOrganisation(Guid organisationUID)
		{
			var cards = CardHelper.GetOrganisationCards(organisationUID);
			if (cards != null)
			{
				foreach (var uid in cards.Select(x => x.UID))
				{
					ServiceFactory.Events.GetEvent<BlockCardEvent>().Publish(uid);
				}
			}
			base.OnRemoveOrganisation(organisationUID);
			SelectedItem = Organisations.FirstOrDefault();
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

		protected override void AfterRemove(ShortEmployee model)
		{
			base.AfterRemove(model);
			ServiceFactory.Events.GetEvent<EditEmployee2Event>().Publish(model.UID);
		}

		protected override void AfterRestore(ShortEmployee model)
		{
			base.AfterRestore(model);
			ServiceFactory.Events.GetEvent<EditEmployee2Event>().Publish(model.UID);
		}

		public bool IsEmployeeSelected 
		{
			get { return SelectedItem != null && !SelectedItem.IsOrganisation; }
		}

		protected override void UpdateSelected() 
		{
			OnPropertyChanged(() => IsEmployeeSelected);

			foreach (var item in Organisations.SelectMany(x => x.GetAllChildren()))
			{
				item.EmployeeCardsViewModel = null;
			}	
			if (SelectedItem != null)
			{
				SelectedItem.UpdatePhoto();
				SelectedItem.InitializeCards();
			}
		}

		public ObservableCollection<string> AdditionalColumnNames { get; private set; }

		PersonType _personType;
		public PersonType PersonType
		{
			get { return _personType; }
			private set
			{
				_personType = value;
				OnPropertyChanged(() => PersonType);
				OnPropertyChanged(() => ItemRemovingName);
				OnPropertyChanged(() => AddCommandToolTip);
				OnPropertyChanged(() => RemoveCommandToolTip);
				OnPropertyChanged(() => EditCommandToolTip);
				OnPropertyChanged(() => TabItemHeader);
			}
		}
		
		protected override string ItemRemovingName
		{
			get 
			{ 
				if(PersonType == RubezhAPI.SKD.PersonType.Employee)
					return "сотрудника"; 
				else
					return "посетителя"; 
			}
		}
		
		public string AddCommandToolTip
		{
			get { return "Добавить " + ItemRemovingName; }
		}

		public string RemoveCommandToolTip
		{
			get { return "Удалить " + ItemRemovingName; }
		}

		public string EditCommandToolTip
		{
			get { return "Редактировать " + ItemRemovingName; }
		}

		public string TabItemHeader
		{
			get { return PersonType == RubezhAPI.SKD.PersonType.Employee ? "Сотрудники" : "Посетители"; }
		}

		protected override bool Add(ShortEmployee item)
		{
			throw new NotImplementedException();
		}

		void OnEditEmployee(Guid employeeUID)
		{
			var viewModel = Organisations.SelectMany(x => x.Children).FirstOrDefault(x => x.Model.UID == employeeUID);
			if (viewModel != null)
			{
				var model = EmployeeHelper.GetSingleShort(employeeUID);
				if(model != null)
					viewModel.Update(model);
			}
		}

		protected override void OnEdit()
		{
			base.OnEdit();
			ServiceFactory.Events.GetEvent<EditEmployee2Event>().Publish(SelectedItem.Model.UID);
		}
	
		void OnUpdateIsInGrid(object obj)
		{
			InitializeAdditionalColumns();
		}

		protected override RubezhAPI.Models.PermissionType Permission
		{
			get 
			{
				if (PersonType == PersonType.Employee)
					return RubezhAPI.Models.PermissionType.Oper_SKD_Employees_Edit;
				return RubezhAPI.Models.PermissionType.Oper_SKD_Guests_Edit;
			}
		}
	}
}
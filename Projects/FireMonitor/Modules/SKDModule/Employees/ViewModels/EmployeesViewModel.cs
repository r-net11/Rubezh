using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public class EmployeesViewModel : OrganisationBaseViewModel<ShortEmployee, EmployeeFilter, EmployeeViewModel, EmployeeDetailsViewModel>
	{
		public List<ShortAdditionalColumnType> AdditionalColumnTypes { get; private set; }
		
		public EmployeesViewModel():base()
		{
			ServiceFactory.Events.GetEvent<EditEmployeeEvent>().Unsubscribe(OnEditEmployee);
			ServiceFactory.Events.GetEvent<EditEmployeeEvent>().Subscribe(OnEditEmployee);
		}

		public override void Initialize(EmployeeFilter filter)
		{
			base.Initialize(filter);
			PersonType = filter.PersonType;
			InitializeAdditionalColumns();
		}	

		public void InitializeAdditionalColumns()
		{
			AdditionalColumnNames = new ObservableCollection<string>();
			var columnTypes = AdditionalColumnTypeHelper.GetByCurrentUser();
			if (columnTypes == null)
				return;
			AdditionalColumnTypes = columnTypes.Where(x => x.DataType == AdditionalColumnDataType.Text && x.IsInGrid).ToList();
			foreach (var columnType in AdditionalColumnTypes)
			{
				AdditionalColumnNames.Add(columnType.Name);
			}
			ServiceFactory.Events.GetEvent<UpdateAdditionalColumns>().Publish(null);
		}

		protected override void Remove()
		{
			var cardUIDs = SelectedItem.Cards.Select(x => x.Card.UID);
			base.Remove();
			foreach (var uid in cardUIDs)
			{
				ServiceFactory.Events.GetEvent<BlockCardEvent>().Publish(uid);
			}
		}

		protected override IEnumerable<ShortEmployee> GetModels(EmployeeFilter filter)
		{
			return EmployeeHelper.Get(filter);
		}

		protected override IEnumerable<ShortEmployee> GetModelsByOrganisation(Guid organisationUID)
		{
			return EmployeeHelper.GetShortByOrganisation(organisationUID);
		}

		protected override bool MarkDeleted(Guid uid)
		{
			return EmployeeHelper.MarkDeleted(uid);
		}

		protected override bool Restore(Guid uid)
		{
			return EmployeeHelper.Restore(uid);
		}

		public bool IsEmployeeSelected 
		{
			get { return SelectedItem != null && !SelectedItem.IsOrganisation; }
		}

		protected override void UpdateSelected() 
		{
			OnPropertyChanged(() => IsEmployeeSelected);
			if (SelectedItem != null)
				SelectedItem.UpdatePhoto();
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
			}
		}
		
		protected override string ItemRemovingName
		{
			get 
			{ 
				if(PersonType == FiresecAPI.SKD.PersonType.Employee)
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

		protected override bool Save(ShortEmployee item)
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
	}
}
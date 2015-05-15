using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public class EmployeeViewModel : OrganisationElementViewModel<EmployeeViewModel, ShortEmployee>
	{
		public string CredentialsStartDateString
		{
			get { return IsOrganisation ? "" : Model.CredentialsStartDate; }
		}
		public string DepartmentName
		{
			get 
			{
				var isWithDeleted = (ParentViewModel as EmployeesViewModel).IsWithDeleted;
				if (!IsOrganisation && (isWithDeleted || !IsDepartmentDeleted))
					return IsOrganisation ? "" : Model.DepartmentName;
				else
					return "";
			}
		}
		public string PositionName
		{
			get 
			{
				var isWithDeleted = (ParentViewModel as EmployeesViewModel).IsWithDeleted;
				if (!IsOrganisation && (isWithDeleted || !IsPositionDeleted))
					return Model.PositionName;
				else
					return "";
			}
		}
		public string FirstName
		{
			get { return IsOrganisation ? "" : Model.FirstName; }
		}
		public string SecondName
		{
			get { return IsOrganisation ? "" : Model.SecondName; }
		}
		public string LastName
		{
			get { return IsOrganisation ? "" : Model.LastName; }
		}
		public string Phone
		{
			get { return IsOrganisation ? "" : Model.Phone; }
		}
		public string OrganisationName
		{
			get { return IsOrganisation ? "" : Model.OrganisationName; }
		}
		public bool IsDepartmentDeleted
		{
			get { return IsOrganisation ? false : Model.IsDepartmentDeleted || IsOrganisationDeleted; }
		}
		public bool IsPositionDeleted
		{
			get { return IsOrganisation ? false : Model.IsPositionDeleted || IsOrganisationDeleted; }
		}
		
		public override void InitializeOrganisation(Organisation organisation, ViewPartViewModel parentViewModel)
		{
			base.InitializeOrganisation(organisation, parentViewModel);
		}

		public string[] AdditionalColumnValues 
		{ 
			get
			{
				var additionalColumnTypes = (ParentViewModel as EmployeesViewModel).AdditionalColumnTypes;
				var result = new string[additionalColumnTypes.Count];
				if (this.IsOrganisation)
					return result;
				var i = 0;
				foreach (var additionalColumnType in additionalColumnTypes)
				{
					var textColumn = Model.TextColumns.FirstOrDefault(x => x.ColumnTypeUID == additionalColumnType.UID);
					var columnValue = textColumn != null ? textColumn.Text : "";
					result[i] = columnValue;
					i++;
				}
				return result;
			}
		}

		public override void InitializeModel(Organisation organisation, ShortEmployee model, ViewPartViewModel parentViewModel)
		{
			base.InitializeModel(organisation, model, parentViewModel);
			
			Update();
		}

		public void InitializeCards()
		{
			EmployeeCardsViewModel = new EmployeeCardsViewModel(this);
		}

		public EmployeeCardsViewModel EmployeeCardsViewModel { get; private set; }
		
		
		public PhotoColumnViewModel Photo { get; private set; }

		public void UpdatePhoto()
		{
			Photo photo = null;
			if (IsOrganisation)
			{
				var details = OrganisationHelper.GetDetails(Organisation.UID);
				if (details != null)
					photo = details.Photo;
			}
			else
			{
				var details = EmployeeHelper.GetDetails(Model.UID);
				if (details != null)
					photo = details.Photo;
			}
			Photo = new PhotoColumnViewModel(photo);
			OnPropertyChanged(() => Photo);
		}

		public override void Update()
		{
			base.Update();
			OnPropertyChanged(() => FirstName);
			OnPropertyChanged(() => SecondName);
			OnPropertyChanged(() => LastName);
			OnPropertyChanged(() => OrganisationName);
			OnPropertyChanged(() => Phone);
			OnPropertyChanged(() => Description);
			OnPropertyChanged(() => DepartmentName);
			OnPropertyChanged(() => PositionName);
			OnPropertyChanged(() => CredentialsStartDateString);
			OnPropertyChanged(() => AdditionalColumnValues);
			OnPropertyChanged(() => IsDepartmentDeleted);
			OnPropertyChanged(() => IsPositionDeleted);
			if (IsOrganisation)
			{
				foreach (var child in Children)
				{
					child.Update();
				}
			}
		}

		public bool IsGuest { get { return !IsOrganisation && Model.Type == PersonType.Guest; } }

		#region Cards
		public PersonType PersonType
		{
			get { return (ParentViewModel as EmployeesViewModel).PersonType; }
		}

		bool _isEmployeeSelected = true;
		public bool IsEmployeeSelected
		{
			get { return _isEmployeeSelected; }
			set
			{
				_isEmployeeSelected = value;
				OnPropertyChanged(() => IsEmployeeSelected);
			}
		}

		bool _isCardSelected;
		public bool IsCardSelected
		{
			get { return _isCardSelected; }
			set
			{
				_isCardSelected = value;
				OnPropertyChanged(() => IsCardSelected);
			}
		}

		public IEnumerable<SKDCard> Cards { get { return EmployeeCardsViewModel != null ? EmployeeCardsViewModel.Cards.Select(x => x.Card) : new List<SKDCard>(); } }
		#endregion
	}

	public class EmployeeCardsViewModel : BaseViewModel, ICardDoorsParentList<EmployeeCardViewModel>
	{
		EmployeeViewModel _employeeViewModel;

		public ShortEmployee Employee { get { return _employeeViewModel.Model; } }
		
		public EmployeeCardsViewModel(EmployeeViewModel employeeViewModel)
		{
			_employeeViewModel = employeeViewModel;
			AddCardCommand = new RelayCommand(OnAddCard, CanAddCard);
			SelectEmployeeCommand = new RelayCommand(OnSelectEmployee);
			ServiceFactory.Events.GetEvent<UpdateAccessTemplateEvent>().Unsubscribe(OnUpdateAccessTemplate);
			ServiceFactory.Events.GetEvent<UpdateAccessTemplateEvent>().Subscribe(OnUpdateAccessTemplate);
			Cards = new ObservableCollection<EmployeeCardViewModel>();
			if (!_employeeViewModel.IsOrganisation)
			{
				var cards = CardHelper.GetByEmployee(_employeeViewModel.Model.UID);
				foreach (var item in cards)
					Cards.Add(new EmployeeCardViewModel(_employeeViewModel.Organisation, this, item));
				SelectedCard = Cards.FirstOrDefault();
			}
			_updateOrganisationDoorsEventSubscriber = new UpdateOrganisationDoorsEventSubscriber<EmployeeCardViewModel>(this);
		}

		UpdateOrganisationDoorsEventSubscriber<EmployeeCardViewModel> _updateOrganisationDoorsEventSubscriber;

		public ObservableCollection<EmployeeCardViewModel> Cards { get; private set; }

		EmployeeCardViewModel _selectedCard;
		public EmployeeCardViewModel SelectedCard
		{
			get { return _selectedCard; }
			set
			{
				_selectedCard = value;
				OnPropertyChanged(() => SelectedCard);
			}
		}

		public RelayCommand AddCardCommand { get; private set; }
		void OnAddCard()
		{
			if (Cards.Count > 100)
			{
				MessageBoxService.ShowWarning("У сотрудника не может быть более 100 пропусков");
				return;
			}
			var cardDetailsViewModel = new EmployeeCardDetailsViewModel(_employeeViewModel.Organisation, _employeeViewModel.Model);
			if (DialogService.ShowModalWindow(cardDetailsViewModel))
			{
				var card = cardDetailsViewModel.Card;
				var cardViewModel = new EmployeeCardViewModel(_employeeViewModel.Organisation, this, card);
				Cards.Add(cardViewModel);
				SelectedCard = cardViewModel;
				SelectCard(cardViewModel);
			}
		}
		bool CanAddCard()
		{
			return FiresecManager.CheckPermission(FiresecAPI.Models.PermissionType.Oper_SKD_Cards_Etit) && !_employeeViewModel.IsDeleted;
		}

		public RelayCommand SelectEmployeeCommand { get; private set; }
		public void OnSelectEmployee()
		{
			_employeeViewModel.IsEmployeeSelected = !_employeeViewModel.IsOrganisation;
			_employeeViewModel.IsCardSelected = false;
			foreach (var card in Cards)
			{
				card.IsCardSelected = false;
			}
		}

		public void SelectCard(EmployeeCardViewModel employeeCardViewModel)
		{
			_employeeViewModel.IsEmployeeSelected = false;
			_employeeViewModel.IsCardSelected = true;
			foreach (var card in Cards)
			{
				card.IsCardSelected = false;
			}
			employeeCardViewModel.IsCardSelected = true;
			SelectedCard = employeeCardViewModel;
		}

		void OnUpdateAccessTemplate(Guid accessTemplateUID)
		{
			var cards = Cards.Where(x => x.Card.AccessTemplateUID != null && x.Card.AccessTemplateUID.Value == accessTemplateUID);
			foreach (var card in cards)
				card.UpdateCardDoors();
		}

		public List<EmployeeCardViewModel> CardDoorsParents
		{
			get { return Cards.ToList(); }
		}
	}
}
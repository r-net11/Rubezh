using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.SKD;
using RubezhClient;
using RubezhClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
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
				if(cards != null)
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
			return ClientManager.CheckPermission(RubezhAPI.Models.PermissionType.Oper_SKD_Cards_Etit) && !_employeeViewModel.IsOrganisation && !_employeeViewModel.IsDeleted && IsConnected;
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

		public List<EmployeeCardViewModel> DoorsParents
		{
			get { return Cards.ToList(); }
		}

		public bool IsConnected
		{
			get { return ((SafeFiresecService)ClientManager.FiresecService).IsConnected; }
		}
	}
}

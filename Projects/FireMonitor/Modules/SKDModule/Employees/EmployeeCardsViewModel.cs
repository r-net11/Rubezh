using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using Localization.SKD.ViewModels;
using StrazhAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public class EmployeeCardsViewModel : BaseViewModel, ICardDoorsParentList<EmployeeCardViewModel>
	{
		private readonly EmployeeViewModel _employeeViewModel;

		public ShortEmployee Employee { get { return _employeeViewModel.Model; } }

		public EmployeeCardsViewModel(EmployeeViewModel employeeViewModel)
		{
			_employeeViewModel = employeeViewModel;
			AddCardCommand = new RelayCommand(OnAddCard, CanAddCard);
			SelectEmployeeCommand = new RelayCommand(OnSelectEmployee);
			
			// Отслеживаем события изменения шаблона доступа
			ServiceFactoryBase.Events.GetEvent<UpdateAccessTemplateEvent>().Unsubscribe(OnUpdateAccessTemplate);
			ServiceFactoryBase.Events.GetEvent<UpdateAccessTemplateEvent>().Subscribe(OnUpdateAccessTemplate);
			
			// Отслеживаем события прохода по "Гостевой" карте
			ServiceFactoryBase.Events.GetEvent<GuestCardPassedEvent>().Unsubscribe(OnGuestCardPassed);
			ServiceFactoryBase.Events.GetEvent<GuestCardPassedEvent>().Subscribe(OnGuestCardPassed);

			// Отслеживаем события деактивации карты
			ServiceFactoryBase.Events.GetEvent<CardDeactivatedEvent>().Unsubscribe(OnCardDeactivated);
			ServiceFactoryBase.Events.GetEvent<CardDeactivatedEvent>().Subscribe(OnCardDeactivated);

			CanShowResetRepeatEnterButton = FiresecManager.CheckPermission(StrazhAPI.Models.PermissionType.Oper_SKD_Cards_ResetRepeatEnter);
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

		private void OnCardDeactivated(SKDCard card)
		{
			var cards = Cards.Where(x => x.Card.UID == card.UID).ToList();
			foreach (var currentCard in cards)
			{
				currentCard.Remove();
			}
		}

		private void OnGuestCardPassed(SKDCard card)
		{
			var cards = Cards.Where(x => x.Card.UID == card.UID).ToList();
			foreach (var currentCard in cards)
			{
				currentCard.Update(card);
			}
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

		private bool _canShowResetRepeatEnterButton;

		public bool CanShowResetRepeatEnterButton
		{
			get { return _canShowResetRepeatEnterButton; }
			set
			{
				if (_canShowResetRepeatEnterButton == value) return;
				_canShowResetRepeatEnterButton = value;
				OnPropertyChanged(() => CanShowResetRepeatEnterButton);
			}
		}

		public RelayCommand AddCardCommand { get; private set; }
		void OnAddCard()
		{
			if (Cards.Count > 100)
			{
				MessageBoxService.ShowWarning(CommonViewModels.PasscardLimitForEmpl);
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
			return FiresecManager.CheckPermission(StrazhAPI.Models.PermissionType.Oper_SKD_Cards_Etit) && !_employeeViewModel.IsDeleted && !_employeeViewModel.IsOrganisation;
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
	}
}

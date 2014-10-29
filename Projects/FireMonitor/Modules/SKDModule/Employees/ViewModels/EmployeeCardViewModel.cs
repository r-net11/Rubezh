using System.Collections.Generic;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Events;
using SKDModule.PassCard.ViewModels;

namespace SKDModule.ViewModels
{
	public class EmployeeCardViewModel : BaseViewModel
	{
		public Organisation Organisation { get; private set; }
		public SKDCard Card { get; private set; }
		public EmployeeViewModel EmployeeViewModel { get; private set; }
		public CardDoorsViewModel CardDoorsViewModel { get; private set; }

		public EmployeeCardViewModel(Organisation organisation, EmployeeViewModel employeeViewModel, SKDCard card)
		{
			RemoveCommand = new RelayCommand(OnRemove);
			EditCommand = new RelayCommand(OnEdit);
			PrintCommand = new RelayCommand(OnPrint);
			SelectCardCommand = new RelayCommand(OnSelectCard);

			Organisation = organisation;
			EmployeeViewModel = employeeViewModel;
			Card = card;

			var cardDoors = GetCardDoors(Card);
			CardDoorsViewModel = new CardDoorsViewModel(cardDoors);
		}

		public string Name
		{
			get { return "Пропуск " + Card.Number; }
		}

		List<CardDoor> GetCardDoors(SKDCard card)
		{
			var cardDoors = new List<CardDoor>();
			cardDoors.AddRange(card.CardDoors);
			if (card.AccessTemplateUID != null)
			{
				var accessTemplates = AccessTemplateHelper.Get(new AccessTemplateFilter());
				if (accessTemplates != null)
				{
					var accessTemplate = accessTemplates.FirstOrDefault(x => x.UID == card.AccessTemplateUID);
					if (accessTemplate != null)
					{
						foreach (var cardZone in accessTemplate.CardDoors)
						{
							if (!cardDoors.Any(x => x.DoorUID == cardZone.DoorUID))
								cardDoors.Add(cardZone);
						}
					}
				}
			}
			return cardDoors;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			var cardRemovalReasonViewModel = new CardRemovalReasonViewModel();
			if (DialogService.ShowModalWindow(cardRemovalReasonViewModel))
			{
				var cardRemovalReason = cardRemovalReasonViewModel.RemovalReason;
				var toStopListResult = CardHelper.DeleteFromEmployee(Card, cardRemovalReason);
				if (!toStopListResult)
					return;
				EmployeeViewModel.Cards.Remove(this);
				ServiceFactory.Events.GetEvent<BlockCardEvent>().Publish(Card.UID);
				EmployeeViewModel.OnSelectEmployee();
			}
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var employeeCardDetailsViewModel = new EmployeeCardDetailsViewModel(Organisation, EmployeeViewModel.PersonType, Card);
			if (DialogService.ShowModalWindow(employeeCardDetailsViewModel))
			{
				var card = employeeCardDetailsViewModel.Card;
				var saveResult = CardHelper.Edit(card);
				if (!saveResult)
					return;
				Card = card;
				OnPropertyChanged(() => Card);
				OnPropertyChanged(() => Name);

				var cardDoors = GetCardDoors(Card);
				CardDoorsViewModel.Update(cardDoors);
			}
		}

		public RelayCommand PrintCommand { get; private set; }
		void OnPrint()
		{
			var passCardViewModel = new PassCardViewModel(EmployeeViewModel.Model.UID, Card);
			DialogService.ShowModalWindow(passCardViewModel);
		}

		public RelayCommand SelectCardCommand { get; private set; }
		void OnSelectCard()
		{
			EmployeeViewModel.SelectCard(this);
			IsCardSelected = true;
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
	}
}
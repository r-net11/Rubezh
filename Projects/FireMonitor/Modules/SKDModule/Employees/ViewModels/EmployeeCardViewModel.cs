using System;
using System.Collections.Generic;
using System.Linq;
using RubezhAPI.SKD;
using RubezhClient;
using RubezhClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Events;
using SKDModule.PassCard.ViewModels;

namespace SKDModule.ViewModels
{
	public class EmployeeCardViewModel : BaseViewModel, IDoorsParent
	{
		public Organisation Organisation { get; private set; }
		public SKDCard Card { get; private set; }
		public EmployeeCardsViewModel EmployeeCardsViewModel { get; private set; }
		public CardDoorsViewModel CardDoorsViewModel { get; private set; }

		public EmployeeCardViewModel(Organisation organisation, EmployeeCardsViewModel employeeCardsViewModel, SKDCard card)
		{
			RemoveCommand = new RelayCommand(OnRemove, CanEditDelete);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
			PrintCommand = new RelayCommand(OnPrint, CanPrint);
			SelectCardCommand = new RelayCommand(OnSelectCard, CanSelectCard);

			Organisation = organisation;
			EmployeeCardsViewModel = employeeCardsViewModel;
			Card = card;

			SetCardDoors();
		}

		public string Name
		{
			get { return "Пропуск " + Card.Number; }
		}

		List<CardDoor> GetCardDoors()
		{
			var cardDoors = new List<CardDoor>();
			cardDoors.AddRange(Card.CardDoors);
			if (Card.AccessTemplateUID != null)
			{
				var accessTemplates = AccessTemplateHelper.Get(new AccessTemplateFilter());
				if (accessTemplates != null)
				{
					var accessTemplate = accessTemplates.FirstOrDefault(x => x.UID == Card.AccessTemplateUID);
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

		public void SetCardDoors()
		{
			var cardDoors = GetCardDoors();
			CardDoorsViewModel = new CardDoorsViewModel(cardDoors, this);
			OnPropertyChanged(() => CardDoorsViewModel);
		}

		public void UpdateCardDoors()
		{
			var cardDoors = GetCardDoors();
			CardDoorsViewModel.Update(cardDoors);
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			var cardRemovalReasonViewModel = new CardRemovalReasonViewModel();
			if (DialogService.ShowModalWindow(cardRemovalReasonViewModel))
			{
				var cardRemovalReason = cardRemovalReasonViewModel.RemovalReason;
				if (cardRemovalReasonViewModel.RemoveIsChecked)
				{
					if (MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить карту?"))
					{
						ServiceFactory.Events.GetEvent<DeleteCardEvent>().Publish(Card.UID);
						CardHelper.Delete(Card);
					}
				}
				if (cardRemovalReasonViewModel.DeactivatedIsChecked)
				{
					var toStopListResult = CardHelper.DeleteFromEmployee(Card, EmployeeCardsViewModel.Employee.Name, cardRemovalReason);
					if (!toStopListResult)
						return;
					ServiceFactory.Events.GetEvent<BlockCardEvent>().Publish(Card.UID);
				}
				EmployeeCardsViewModel.Cards.Remove(this);
				EmployeeCardsViewModel.OnSelectEmployee();
			}
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var employeeCardDetailsViewModel = new EmployeeCardDetailsViewModel(Organisation, EmployeeCardsViewModel.Employee, Card);
			if (DialogService.ShowModalWindow(employeeCardDetailsViewModel))
			{
				var card = employeeCardDetailsViewModel.Card;
				Card = card;
				OnPropertyChanged(() => Card);
				OnPropertyChanged(() => Name);
				SetCardDoors();
			}
		}

		bool CanEditDelete()
		{
			return ClientManager.CheckPermission(RubezhAPI.Models.PermissionType.Oper_SKD_Cards_Etit) && IsConnected;
		}

		public RelayCommand PrintCommand { get; private set; }
		void OnPrint()
		{
			var passCardViewModel = new PassCardViewModel(EmployeeCardsViewModel.Employee.UID, Card);
			DialogService.ShowModalWindow(passCardViewModel);
		}
		bool CanPrint()
		{
			return IsConnected;
		}

		public RelayCommand SelectCardCommand { get; private set; }
		void OnSelectCard()
		{
			EmployeeCardsViewModel.SelectCard(this);
			IsCardSelected = true;
		}
		bool CanSelectCard()
		{
			return IsConnected;
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

		public void UpdateCardDoors(IEnumerable<Guid> doorUIDs)
		{
			Card.CardDoors.RemoveAll(x => !doorUIDs.Any(y => y == x.DoorUID));
			CardDoorsViewModel.UpdateDoors(doorUIDs);
		}

		public bool IsConnected
		{
			get { return ((SafeFiresecService)ClientManager.FiresecService).IsConnected; }
		}
	}
}
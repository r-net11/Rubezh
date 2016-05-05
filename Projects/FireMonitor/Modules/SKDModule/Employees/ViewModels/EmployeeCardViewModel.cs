using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
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
			PrintCommand = new RelayCommand(OnPrint);
			SelectCardCommand = new RelayCommand(OnSelectCard);
			ResetRepeatEnterCommand = new RelayCommand(OnResetRepeatEnter);

			Organisation = organisation;
			EmployeeCardsViewModel = employeeCardsViewModel;
			Card = card;

			SetCardDoors();
		}

		public string Name
		{
			get { return "Пропуск " + Card.Number; }
		}

		private List<CardDoor> GetCardDoors()
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
		private void OnRemove()
		{
			var cardRemovalReasonViewModel = new CardRemovalReasonViewModel();
			if (DialogService.ShowModalWindow(cardRemovalReasonViewModel))
			{
				var cardRemovalReason = cardRemovalReasonViewModel.RemovalReason;
				var toStopListResult = CardHelper.DeleteFromEmployee(Card, EmployeeCardsViewModel.Employee.Name, cardRemovalReason);
				if (!toStopListResult)
					return;
				Remove();
			}
		}

		public void Remove()
		{
			EmployeeCardsViewModel.Cards.Remove(this);
			ServiceFactoryBase.Events.GetEvent<BlockCardEvent>().Publish(Card.UID);
			EmployeeCardsViewModel.OnSelectEmployee();
		}

		public RelayCommand EditCommand { get; private set; }
		private void OnEdit()
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

		public RelayCommand ResetRepeatEnterCommand { get; set; }

		private void OnResetRepeatEnter()
		{
			var dialog = new ResetRepeatEnterViewModel();
			var doorGuids = new List<Guid>();
			var cardNo = Card.Number;
			string doorName = null;

			if (!DialogService.ShowModalWindow(dialog)) return;

			if (dialog.IsForAllAccessPoints)
				doorGuids.AddRange(CardDoorsViewModel.Doors.Select(x => x.CardDoor.DoorUID));
			else
			{
				if (CardDoorsViewModel.SelectedDoor == null || CardDoorsViewModel.SelectedDoor.CardDoor == null)
				{
					MessageBoxService.ShowWarning("Точки доступа отсутствуют.");
					return;
				}

				doorGuids.Add(CardDoorsViewModel.SelectedDoor.CardDoor.DoorUID);
				doorName = CardDoorsViewModel.SelectedDoor.Name;
			}

			var result = CardHelper.ResetRepeatEnter(new Dictionary<SKDCard, List<Guid>>{{Card, doorGuids}}, (int)cardNo, doorName);
			if (result && !dialog.IsForAllAccessPoints)
			{
				MessageBoxService.Show("Ограничение на повторный проход по пропуску сброшено для точки доступа");
			}
			else if (result && dialog.IsForAllAccessPoints)
			{
				MessageBoxService.Show("Ограничение на повторный проход по пропуску сброшено для всех точек доступа");
			}
		}

		private bool CanEditDelete()
		{
			return FiresecManager.CheckPermission(FiresecAPI.Models.PermissionType.Oper_SKD_Cards_Etit);
		}

		public RelayCommand PrintCommand { get; private set; }
		private void OnPrint()
		{
			var passCardViewModel = new PassCardViewModel(EmployeeCardsViewModel.Employee.UID, Card);
			DialogService.ShowModalWindow(passCardViewModel);
		}

		public RelayCommand SelectCardCommand { get; private set; }
		private void OnSelectCard()
		{
			EmployeeCardsViewModel.SelectCard(this);
			IsCardSelected = true;
		}

		private bool _isCardSelected;
		public bool IsCardSelected
		{
			get { return _isCardSelected; }
			set
			{
				_isCardSelected = value;
				OnPropertyChanged(() => IsCardSelected);
			}
		}

		public void UpdateCardDoors(IEnumerable<Guid> doorUIDs, Guid organisationUID) //TODO: Adding Guid organisationUID to fix SKDDEV-625. Check the necessity
		{
			Card.CardDoors.RemoveAll(x => doorUIDs.All(y => y != x.DoorUID));
			CardDoorsViewModel.UpdateDoors(doorUIDs);
		}

		public void Update(SKDCard card)
		{
			Card.IsInStopList = card.IsInStopList;
			Card.StopReason = card.StopReason;
			Card.AllowedPassCount = card.AllowedPassCount;
		}
	}
}
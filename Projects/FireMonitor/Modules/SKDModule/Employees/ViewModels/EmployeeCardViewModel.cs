using System.Threading.Tasks;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Employees.ViewModels.DialogWindows;
using SKDModule.Events;
using StrazhAPI.SKD;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SKDModule.ViewModels
{
	public class EmployeeCardViewModel : BaseViewModel, IDoorsParent
	{
		private bool _isCardSelected;
		public Organisation Organisation { get; private set; }
		public SKDCard Card { get; private set; }
		public EmployeeCardsViewModel EmployeeCardsViewModel { get; private set; }
		public CardDoorsViewModel CardDoorsViewModel { get; private set; }
		public string Name
		{
			get { return "Пропуск " + Card.Number; }
		}
		public bool IsCardSelected
		{
			get { return _isCardSelected; }
			set
			{
				_isCardSelected = value;
				OnPropertyChanged(() => IsCardSelected);
			}
		}

		public EmployeeCardViewModel(Organisation organisation, EmployeeCardsViewModel employeeCardsViewModel, SKDCard card)
		{
			RemoveCommand = new RelayCommand(OnRemove, CanEditDelete);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
			SelectCardCommand = new RelayCommand(OnSelectCard);
			ResetRepeatEnterCommand = new RelayCommand(OnResetRepeatEnter);
			OpenPrintPreviewWindowCommand = new RelayCommand(OnOpenPrintPreviewWindow);

			Organisation = organisation;
			EmployeeCardsViewModel = employeeCardsViewModel;
			Card = card;

			SetCardDoors();
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

		private void OnOpenPrintPreviewWindow()
		{
			var vm = new PreviewReportDialogViewModel(Organisation, Card.PassCardTemplateUID);
			if (DialogService.ShowModalWindow(vm))
			{
				Card.PassCardTemplateUID = vm.SelectedTemplate.UID;
				Task.Factory.StartNew(() => CardHelper.Edit(Card, EmployeeCardsViewModel.Employee.Name));
			}
		}

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
			return FiresecManager.CheckPermission(StrazhAPI.Models.PermissionType.Oper_SKD_Cards_Etit);
		}
		
		private void OnSelectCard()
		{
			EmployeeCardsViewModel.SelectCard(this);
			IsCardSelected = true;
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

		public RelayCommand RemoveCommand { get; private set; }
		public RelayCommand SelectCardCommand { get; private set; }
		public RelayCommand EditCommand { get; private set; }
		public RelayCommand ResetRepeatEnterCommand { get; set; }
		public RelayCommand OpenPrintPreviewWindowCommand { get; private set; }
	}
}
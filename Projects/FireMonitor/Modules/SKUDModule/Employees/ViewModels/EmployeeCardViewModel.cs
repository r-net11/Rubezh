using System;
using System.Collections.Generic;
using FiresecAPI;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient.SKDHelpers;
using System.Diagnostics;

namespace SKDModule.ViewModels
{
	public class EmployeeCardViewModel : BaseViewModel
	{
		public SKDCard Card { get; private set; }
		EmployeeViewModel EmployeeViewModel;
		List<Guid> ZoneUIDs;
		public CardZonesViewModel CardZonesViewModel { get; private set; }

		public EmployeeCardViewModel(EmployeeViewModel employeeViewModel, SKDCard card)
		{
			RemoveCommand = new RelayCommand(OnRemove);
			EditCommand = new RelayCommand(OnEdit);
			SelectCardCommand = new RelayCommand(OnSelectCard);

			EmployeeViewModel = employeeViewModel;
			Card = card;
			ZoneUIDs = new List<Guid>();
			CardZonesViewModel = new CardZonesViewModel(Card.CardZones);
		}

		bool _isBlocked;
		public bool IsBlocked
		{
			get { return _isBlocked; }
			set
			{
				_isBlocked = value;
				OnPropertyChanged("IsBlocked");
			}
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			var cardRemovalReasonViewModel = new CardRemovalReasonViewModel();
			if (DialogService.ShowModalWindow(cardRemovalReasonViewModel))
			{
				var cardRemovalReason = cardRemovalReasonViewModel.RemovalReason;
				var toStopListResult = CardHelper.ToStopList(Card, cardRemovalReason);
				if (!toStopListResult)
					return;
				EmployeeViewModel.Cards.Remove(this);
				EmployeeViewModel.OrganisationEmployeesViewModel.SelectedEmployee = EmployeeViewModel.OrganisationEmployeesViewModel.SelectedEmployee;
			}
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var employeeCardDetailsViewModel = new EmployeeCardDetailsViewModel(Card);
			if (DialogService.ShowModalWindow(employeeCardDetailsViewModel))
			{
				var card = employeeCardDetailsViewModel.Card;
				var saveResult = CardHelper.Save(card);
				if (!saveResult)
					return;
				Card = card;
				OnPropertyChanged("Card");
				CardZonesViewModel.Update();
			}
		}

		public RelayCommand SelectCardCommand { get; private set; }
		void OnSelectCard()
		{
			EmployeeViewModel.OrganisationEmployeesViewModel.SelectedCard = this;
		}

		bool _isBold;
		public bool IsBold
		{
			get { return _isBold; }
			set
			{
				_isBold = value;
				OnPropertyChanged("IsBold");
			}
		}
	}
}
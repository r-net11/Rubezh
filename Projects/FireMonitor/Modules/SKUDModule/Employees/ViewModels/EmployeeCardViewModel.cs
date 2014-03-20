using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class EmployeeCardViewModel : BaseViewModel
	{
		Organization Organization;
		public SKDCard Card { get; private set; }
		EmployeeViewModel EmployeeViewModel;
		List<Guid> ZoneUIDs;
		public CardZonesViewModel CardZonesViewModel { get; private set; }

		public EmployeeCardViewModel(Organization organization, EmployeeViewModel employeeViewModel, SKDCard card)
		{
			RemoveCommand = new RelayCommand(OnRemove);
			EditCommand = new RelayCommand(OnEdit);
			PrintCommand = new RelayCommand(OnPrint);
			SelectCardCommand = new RelayCommand(OnSelectCard);
			SelectPassCommand = new RelayCommand(OnSelectPass);

			Organization = organization;
			EmployeeViewModel = employeeViewModel;
			Card = card;
			ZoneUIDs = new List<Guid>();

			var cardZones = GetCardZones(Card);
			CardZonesViewModel = new CardZonesViewModel(cardZones);
		}

		List<CardZone> GetCardZones(SKDCard card)
		{
			var cardZones = new List<CardZone>();
			cardZones.AddRange(card.CardZones);
			if (card.AccessTemplateUID != null)
			{
				var accessTemplates = AccessTemplateHelper.Get(new AccessTemplateFilter());
				if (accessTemplates != null)
				{
					var accessTemplate = accessTemplates.FirstOrDefault(x => x.UID == card.AccessTemplateUID);
					if (accessTemplate != null)
					{
						foreach (var cardZone in accessTemplate.CardZones)
						{
							if (!cardZones.Any(x => x.ZoneUID == cardZone.ZoneUID))
								cardZones.Add(cardZone);
						}
					}
				}
			}
			return cardZones;
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
				EmployeeViewModel.EmployeesViewModel.SelectedEmployee = EmployeeViewModel.EmployeesViewModel.SelectedEmployee;
			}
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var employeeCardDetailsViewModel = new EmployeeCardDetailsViewModel(Organization, Card);
			if (DialogService.ShowModalWindow(employeeCardDetailsViewModel))
			{
				var card = employeeCardDetailsViewModel.Card;
				var saveResult = CardHelper.Save(card);
				if (!saveResult)
					return;
				Card = card;
				OnPropertyChanged("Card");
				CardZonesViewModel.Update(Card.CardZones);
			}
		}

		public RelayCommand PrintCommand { get; private set; }
		void OnPrint()
		{
		}

		public RelayCommand SelectCardCommand { get; private set; }
		void OnSelectCard()
		{
			IsCard = true;
			EmployeeViewModel.EmployeesViewModel.SelectedCard = this;
			IsCardBold = true;
			IsPassBold = false;
		}

		public RelayCommand SelectPassCommand { get; private set; }
		void OnSelectPass()
		{
			IsCard = false;
			EmployeeViewModel.EmployeesViewModel.SelectedCard = this;
			IsCardBold = false;
			IsPassBold = true;
		}

		public bool IsCard = true;

		bool _isCardBold;
		public bool IsCardBold
		{
			get { return _isCardBold; }
			set
			{
				_isCardBold = value;
				OnPropertyChanged("IsCardBold");
			}
		}

		bool _isPassBold;
		public bool IsPassBold
		{
			get { return _isPassBold; }
			set
			{
				_isPassBold = value;
				OnPropertyChanged("IsPassBold");
			}
		}

		bool SaveCardTemplate(Employee employee, Guid? cardTemplateUID)
		{
			employee.CardTemplateUID = cardTemplateUID;
			return EmployeeHelper.SaveCardTemplate(employee);		
		}
	}
}
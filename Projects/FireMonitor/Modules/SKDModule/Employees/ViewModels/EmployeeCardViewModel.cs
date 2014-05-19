using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.PassCard.ViewModels;

namespace SKDModule.ViewModels
{
	public class EmployeeCardViewModel : BaseViewModel
	{
		public Organisation Organisation { get; private set; }
		public SKDCard Card { get; private set; }
		public EmployeeViewModel EmployeeViewModel { get; private set; }
		public CardZonesViewModel CardZonesViewModel { get; private set; }

		public EmployeeCardViewModel(Organisation organisation, EmployeeViewModel employeeViewModel, SKDCard card)
		{
			RemoveCommand = new RelayCommand(OnRemove);
			EditCommand = new RelayCommand(OnEdit);
			PrintCommand = new RelayCommand(OnPrint);
			SelectCardCommand = new RelayCommand(OnSelectCard);

			Organisation = organisation;
			EmployeeViewModel = employeeViewModel;
			Card = card;

			var cardZones = GetCardZones(Card);
			CardZonesViewModel = new CardZonesViewModel(cardZones);
		}

		public string Name
		{
			get { return "Карат " + Card.PresentationName; }
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
				EmployeeViewModel.OnSelectEmployee();
			}
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var employeeCardDetailsViewModel = new EmployeeCardDetailsViewModel(Organisation, Card);
			if (DialogService.ShowModalWindow(employeeCardDetailsViewModel))
			{
				var card = employeeCardDetailsViewModel.Card;
				var saveResult = CardHelper.Save(card);
				if (!saveResult)
					return;
				Card = card;
				OnPropertyChanged("Card");
				OnPropertyChanged("Name");
				CardZonesViewModel.Update(Card.CardZones);
			}
		}

		public RelayCommand PrintCommand { get; private set; }
		void OnPrint()
		{
			var passCardViewModel = new PassCardViewModel(EmployeeViewModel.ShortEmployee.UID, Card);
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
				OnPropertyChanged("IsCardSelected");
			}
		}
	}
}
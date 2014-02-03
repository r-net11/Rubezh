using System;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using XFiresecAPI;
using Controls.Converters;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows;
using System.Collections.Generic;
using FiresecAPI;
using FiresecAPI.Models.Skud;
using System.Collections.ObjectModel;

namespace SKDModule.ViewModels
{
	public class UserAccessViewModel : TreeNodeViewModel<ZoneViewModel>
	{
		public Employee Employee { get; private set; }

		public UserAccessViewModel(Employee employee)
		{
			Employee = employee;
			AddCardCommand = new RelayCommand(OnAddCard, CanAddCard);
			RemoveCardCommand = new RelayCommand(OnRemoveCard, CanRemoveCard);
			RemoveCardsCommand = new RelayCommand(OnRemoveCards, CanRemoveCards);
			CopyRightsCommand = new RelayCommand(OnCopyRights, CanCopyRights);
			ChangeZonesCommand = new RelayCommand(OnChangeZones, CanChangeZones);
			ChangeTemplateCommand = new RelayCommand(OnChangeTemplate, CanChangeTemplate);
			DeleteZoneCommand = new RelayCommand(OnDeleteZone, CanDeleteZone);
			ChangeGroupRightsCommand = new RelayCommand(OnChangeGroupRights, CanChangeGroupRights);

			Cards = new ObservableCollection<CardViewModel>();
		}

		public ObservableCollection<CardViewModel> Cards { get; private set; }

		CardViewModel _selectedCard;
		public CardViewModel SelectedCard
		{
			get { return _selectedCard; }
			set
			{
				_selectedCard = value;
				OnPropertyChanged("SelectedCard");
			}
		}

		public RelayCommand AddCardCommand { get; private set; }
		void OnAddCard()
		{
			var cardDetailsViewModel = new CardDetailsViewModel();
			if (DialogService.ShowModalWindow(cardDetailsViewModel))
			{
				var card = cardDetailsViewModel.Card;
				var cardViewModel = new CardViewModel(card);
				Cards.Add(cardViewModel);
			}

		}
		public bool CanAddCard()
		{
			return true;
		}

		public RelayCommand RemoveCardCommand { get; private set; }
		void OnRemoveCard()
		{

		}
		public bool CanRemoveCard()
		{
			return true;
		}

		public RelayCommand RemoveCardsCommand { get; private set; }
		void OnRemoveCards()
		{

		}
		public bool CanRemoveCards()
		{
			return true;
		}

		public RelayCommand CopyRightsCommand { get; private set; }
		void OnCopyRights()
		{
		}
		public bool CanCopyRights()
		{
			return true;
		}

		public RelayCommand ChangeZonesCommand { get; private set; }
		void OnChangeZones()
		{
		}
		public bool CanChangeZones()
		{
			return true;
		}

		public RelayCommand ChangeTemplateCommand { get; private set; }
		void OnChangeTemplate()
		{
		}
		public bool CanChangeTemplate()
		{
			return true;
		}

		public RelayCommand DeleteZoneCommand { get; private set; }
		void OnDeleteZone()
		{
		}
		public bool CanDeleteZone()
		{
			return true;
		}

		public RelayCommand ChangeGroupRightsCommand { get; private set; }
		void OnChangeGroupRights()
		{
		}
		public bool CanChangeGroupRights()
		{
			return true;
		}
	}
}
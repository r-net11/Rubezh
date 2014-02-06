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
				var cardViewModel = new CardViewModel(this, card);
				Cards.Add(cardViewModel);
				SelectedCard = cardViewModel;
			}

		}
		public bool CanAddCard()
		{
			return Cards.Count < 10;
		}

		public RelayCommand RemoveCardCommand { get; private set; }
		void OnRemoveCard()
		{
			var cardRemovalReasonViewModel = new CardRemovalReasonViewModel();
			if (DialogService.ShowModalWindow(cardRemovalReasonViewModel))
			{
				var cardRemovalReason = cardRemovalReasonViewModel.CardRemovalReason;
				Cards.Remove(SelectedCard);
			}
		}
		public bool CanRemoveCard()
		{
			return SelectedCard != null;
		}
	}
}
using System.Collections.ObjectModel;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.Generic;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using System;

namespace SKDModule.ViewModels
{
	public class CardsViewModel : ViewPartViewModel
	{
		public CardsViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			RefreshCommand = new RelayCommand(OnRefresh);
			Initialize();
		}

		public void Initialize()
		{
			var cards = new List<SKDCard>();
			Cards = new ObservableCollection<CardViewModel>();
			foreach (var card in cards)
			{
				var cardViewModel = new CardViewModel(card);
				Cards.Add(cardViewModel);
			}
		}

		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
			Initialize();
		}

		ObservableCollection<CardViewModel> _cards;
		public ObservableCollection<CardViewModel> Cards
		{
			get { return _cards; }
			set
			{
				_cards = value;
				OnPropertyChanged("Cards");
			}
		}

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

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var cardDetailsViewModel = new CardDetailsViewModel(this);
			if (DialogService.ShowModalWindow(cardDetailsViewModel))
			{
				var card = cardDetailsViewModel.Card;
				var cardViewModel = new CardViewModel(card);
				Cards.Add(cardViewModel);
				SelectedCard = cardViewModel;
			}
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			var index = Cards.IndexOf(SelectedCard);
			Cards.Remove(SelectedCard);
			index = Math.Min(index, Cards.Count - 1);
			if (index > -1)
				SelectedCard = Cards[index];
		}
		bool CanRemove()
		{
			return SelectedCard != null;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var cardDetailsViewModel = new CardDetailsViewModel(this, SelectedCard.Card);
			if (DialogService.ShowModalWindow(cardDetailsViewModel))
			{
				SelectedCard.Update(cardDetailsViewModel.Card);
			}
		}
		bool CanEdit()
		{
			return SelectedCard != null;
		}
	}
}
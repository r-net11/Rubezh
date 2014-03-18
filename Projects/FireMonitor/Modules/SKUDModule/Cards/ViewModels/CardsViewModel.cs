using System.Collections.ObjectModel;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.Generic;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using System;
using FiresecClient.SKDHelpers;

namespace SKDModule.ViewModels
{
	public class CardsViewModel : ViewPartViewModel
	{
		CardFilter Filter;

		public CardsViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			RefreshCommand = new RelayCommand(OnRefresh);
			Filter = new CardFilter();
			Initialize();
		}

		public void Initialize()
		{
			Cards = new ObservableCollection<CardViewModel>();
			var cards = CardHelper.Get(Filter);
			if (cards == null)
				return;
			foreach (var card in cards)
			{
				var cardViewModel = new CardViewModel(card);
				Cards.Add(cardViewModel);
			}
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
				var saveResult = CardHelper.Save(card);
				if (saveResult == false)
					return;
				var cardViewModel = new CardViewModel(card);
				Cards.Add(cardViewModel);
				SelectedCard = cardViewModel;
			}
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			var card = SelectedCard.Card;
			var removeResult = CardHelper.MarkDeleted(card);
			if (removeResult == false)
				return;
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
				var card = cardDetailsViewModel.Card;
				var saveResult = CardHelper.Save(card);
				if (!saveResult)
					return;
				SelectedCard.Update(cardDetailsViewModel.Card);
			}
		}
		bool CanEdit()
		{
			return SelectedCard != null;
		}

		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
			Initialize();
		}
	}
}
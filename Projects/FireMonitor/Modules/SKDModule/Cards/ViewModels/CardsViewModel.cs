using System;
using System.Collections.ObjectModel;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class CardsViewModel : ViewPartViewModel
	{
		CardFilter Filter;

		public CardsViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			Filter = new CardFilter();
		}

		public void Initialize(CardFilter filter)
		{
			Cards = new ObservableCollection<CardViewModel>();
			var cards = CardHelper.Get(filter);
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
			var cardDetailsViewModel = new CardDetailsViewModel();
			if (DialogService.ShowModalWindow(cardDetailsViewModel))
			{
				foreach (var card in cardDetailsViewModel.Cards)
				{
					var cardViewModel = new CardViewModel(card);
					Cards.Add(cardViewModel);
				}
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
			return SelectedCard != null && SelectedCard.Card.IsInStopList;
		}
	}
}
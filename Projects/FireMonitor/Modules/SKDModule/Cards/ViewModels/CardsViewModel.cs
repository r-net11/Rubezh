using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class CardsViewModel : ViewPartViewModel
	{
		CardFilter Filter;

		public CardsViewModel()
		{
			Filter = new CardFilter();
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
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
				OnPropertyChanged(() => Cards);
			}
		}

		CardViewModel _selectedCard;
		public CardViewModel SelectedCard
		{
			get { return _selectedCard; }
			set
			{
				_selectedCard = value;
				OnPropertyChanged(() => SelectedCard);
			}
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			CardHelper.Delete(SelectedCard.Card.UID);
			Cards.Remove(SelectedCard);
			SelectedCard = Cards.FirstOrDefault();
		}
		bool CanRemove()
		{
			return SelectedCard != null && SelectedCard.Card.IsInStopList;
		}
	}
}
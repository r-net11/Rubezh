using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public class CardsViewModel : ViewPartViewModel
	{
		CardFilter Filter;

		public CardsViewModel()
		{
			Filter = new CardFilter();
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			ServiceFactory.Events.GetEvent<NewCardEvent>().Unsubscribe(OnNewCard);
			ServiceFactory.Events.GetEvent<NewCardEvent>().Subscribe(OnNewCard);
		}

		void OnNewCard(SKDCard newCard)
		{
			var condition = newCard.IsInStopList ? (Func<CardViewModel, bool>)(x => x.IsDeactivatedRootItem) : x => x.IsOrganisation && x.Organisation.UID == newCard.OrganisationUID;
			var rootItem = RootItems.FirstOrDefault(condition);
			var cards = rootItem.Children;
			var cardViewModel = cards.FirstOrDefault(x => x.Card.UID == newCard.UID);
			if (cardViewModel != null)
				cardViewModel.Update(newCard);
			else
				rootItem.AddChild(new CardViewModel(newCard));
			OnPropertyChanged(() => RootItems);
			OnPropertyChanged(() => RootItemsArray);
		}

		public void Initialize(CardFilter filter)
		{
			var organisations = OrganisationHelper.GetByCurrentUser();
			if (organisations == null)
				return;
			RootItems = new ObservableCollection<CardViewModel>();
			foreach (var organisation in organisations)
			{
				RootItems.Add(new CardViewModel(organisation));
			}
			RootItems.Add(CardViewModel.DeactivatedRootItem);
			var cards = CardHelper.Get(filter);
			if (cards == null)
				return;
			foreach (var card in cards)
			{
				var condition = card.IsInStopList ? (Func<CardViewModel, bool>)(x => x.IsDeactivatedRootItem) : x => x.IsOrganisation && x.Organisation.UID == card.OrganisationUID;
				var rootItem = RootItems.FirstOrDefault(condition);
				if(rootItem != null)
					rootItem.AddChild(new CardViewModel(card));
			}
			foreach (var rootItem in RootItems)
			{
				if (rootItem.HasChildren)
					rootItem.ExpandChildren();
			}
			OnPropertyChanged(() => RootItems);
			OnPropertyChanged(() => RootItemsArray);
		}
		
		ObservableCollection<CardViewModel> rootItems;
		public ObservableCollection<CardViewModel> RootItems
		{
			get { return rootItems; }
			set
			{
				rootItems = value;
				OnPropertyChanged(() => RootItems);
				OnPropertyChanged(() => RootItemsArray);
			}
		}

		public CardViewModel[] RootItemsArray
		{
			get { return RootItems.ToArray(); }
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
			var parent = SelectedCard.Card.IsInStopList ? RootItems.FirstOrDefault(x => x.IsDeactivatedRootItem) : RootItems.FirstOrDefault(x => x.Organisation.UID == SelectedCard.Card.OrganisationUID);
			parent.RemoveChild(SelectedCard);
			SelectedCard = parent.HasChildren ? parent : parent.Children.FirstOrDefault();
		}
		bool CanRemove()
		{
			return SelectedCard != null && SelectedCard.IsCard && SelectedCard.Card.IsInStopList;
		}
	}
}
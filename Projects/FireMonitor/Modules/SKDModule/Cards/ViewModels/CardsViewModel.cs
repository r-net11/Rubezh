using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
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
			ServiceFactory.Events.GetEvent<BlockCardEvent>().Unsubscribe(OnBlockCard);
			ServiceFactory.Events.GetEvent<BlockCardEvent>().Subscribe(OnBlockCard);
			ServiceFactory.Events.GetEvent<EditOrganisationEvent>().Unsubscribe(OnEditOrganisation);
			ServiceFactory.Events.GetEvent<EditOrganisationEvent>().Subscribe(OnEditOrganisation);
			ServiceFactory.Events.GetEvent<RemoveOrganisationEvent>().Unsubscribe(OnRemoveOrganisation);
			ServiceFactory.Events.GetEvent<RemoveOrganisationEvent>().Subscribe(OnRemoveOrganisation);
			ServiceFactory.Events.GetEvent<OrganisationUsersChangedEvent>().Unsubscribe(OnOrganisationUsersChanged);
			ServiceFactory.Events.GetEvent<OrganisationUsersChangedEvent>().Subscribe(OnOrganisationUsersChanged);
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
			if (!newCard.IsInStopList)
			{
				var deactivatedRoot = RootItems.FirstOrDefault(x => x.IsDeactivatedRootItem);
				var blockedCard = deactivatedRoot.Children.FirstOrDefault(x => x.Card.UID == newCard.UID);
				if (blockedCard != null)
					deactivatedRoot.RemoveChild(blockedCard);
			}
			rootItem.IsExpanded = true;
			OnPropertyChanged(() => RootItems);
			OnPropertyChanged(() => RootItemsArray);
		}

		void OnBlockCard(Guid uid)
		{
			var card = RootItems.SelectMany(x => x.Children).FirstOrDefault(x => x.Card.UID == uid);
			card.Parent.RemoveChild(card);
			var newCard = CardHelper.GetSingle(uid);
			if (newCard == null)
				return;
			RootItems.FirstOrDefault(x => x.IsDeactivatedRootItem).AddChild(new CardViewModel(newCard));
			OnPropertyChanged(() => RootItems);
			OnPropertyChanged(() => RootItemsArray);
		} 

		void OnEditOrganisation(Organisation newOrganisation)
		{
			var organisation = RootItems.FirstOrDefault(x => x.Organisation != null && x.Organisation.UID == newOrganisation.UID);
			if (organisation != null)
			{
				organisation.Update(newOrganisation);
			}
			OnPropertyChanged(() => RootItems);
		}

		void OnOrganisationUsersChanged(Organisation newOrganisation)
		{
            if (newOrganisation.UserUIDs.Any(x => x == FiresecManager.CurrentUser.UID))
            {
                if (!RootItems.Any(x => x.IsOrganisation && x.Organisation.UID == newOrganisation.UID))
                {
                    var organisationViewModel = new CardViewModel(newOrganisation);
                    RootItems.Add(organisationViewModel);
                    var cards = CardHelper.Get(new CardFilter());
                    if (cards == null)
                        return;
                    foreach (var card in cards.Where(x => x.OrganisationUID == newOrganisation.UID))
                    {
                        organisationViewModel.AddChild(new CardViewModel(card));
                    }
                    OnPropertyChanged(() => RootItems);
                    OnPropertyChanged(() => RootItemsArray);
                }
            }
            else
            {
                var organisationViewModel = RootItems.FirstOrDefault(x => x.IsOrganisation && x.Organisation.UID == newOrganisation.UID);
                if (organisationViewModel != null)
                {
                    RootItems.Remove(organisationViewModel);
                    OnPropertyChanged(() => RootItems);
                    OnPropertyChanged(() => RootItemsArray);
                }
            }
        }

		protected virtual void OnRemoveOrganisation(Guid organisationUID)
		{
			var organisationViewModel = RootItems.FirstOrDefault(x => x.IsOrganisation && x.Organisation.UID == organisationUID);
			if (organisationViewModel != null)
			{
				RootItems.Remove(organisationViewModel);
				OnPropertyChanged(() => RootItems);
				OnPropertyChanged(() => RootItemsArray);
			}
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
			if (MessageBoxService.ShowQuestion2("Вы уверены, что хотите удалить карту?"))
			{
				CardHelper.Delete(SelectedCard.Card.UID);
				var parent = SelectedCard.Card.IsInStopList ? RootItems.FirstOrDefault(x => x.IsDeactivatedRootItem) : RootItems.FirstOrDefault(x => x.Organisation.UID == SelectedCard.Card.OrganisationUID);
				parent.RemoveChild(SelectedCard);
				SelectedCard = parent.HasChildren ? parent : parent.Children.FirstOrDefault();
			}
		}
		bool CanRemove()
		{
			return SelectedCard != null && SelectedCard.IsCard && SelectedCard.Card.IsInStopList;
		}
	}
}
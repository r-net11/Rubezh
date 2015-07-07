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
using FiresecAPI;
using System.Collections.Generic;

namespace SKDModule.ViewModels
{
	public class CardsViewModel : ViewPartViewModel
	{
		CardFilter _filter;
        public Guid DbCallbackResultUID;
		
		public CardsViewModel()
		{
			_filter = new CardFilter();
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
			ServiceFactory.Events.GetEvent<EditEmployee2Event>().Unsubscribe(OnEditEmployee);
			ServiceFactory.Events.GetEvent<EditEmployee2Event>().Subscribe(OnEditEmployee);
            SafeFiresecService.DbCallbackResultEvent -= new Action<DbCallbackResult>(OnDbCallbackResultEvent);
            SafeFiresecService.DbCallbackResultEvent += new Action<DbCallbackResult>(OnDbCallbackResultEvent);
			DbCallbackResultUID = Guid.NewGuid();
		}

        void OnDbCallbackResultEvent(DbCallbackResult dbCallbackResult)
        {
            if (dbCallbackResult.ClientUID == DbCallbackResultUID)
            {
                int portionSize = 5000;
                int i = 0;
                foreach (var card in dbCallbackResult.Cards)
                {
                    var rootItem = card.IsInStopList ? RootItems.FirstOrDefault(x => x.IsDeactivatedRootItem) : 
                        RootItems.FirstOrDefault(x => x.IsOrganisation && x.Organisation.UID == card.OrganisationUID);
                    ApplicationService.Invoke(() => { 
                        rootItem.AddChild(new CardViewModel(card));
                        ItemsCount = RootItems.Select(x => x.Children.Count()).Sum();
                    });
                    if(i++ % portionSize == 0)
                        ApplicationService.DoEvents();
                }
                ApplicationService.DoEvents();
                //foreach (var rootItem in RootItems.Where(x => x.HasChildren))
                //    ApplicationService.Invoke(() => rootItem.ExpandChildren());
                //foreach (var rootItem in RootItems.Where(x => x.GetAllChildren(false).Count == 0))
                //    ApplicationService.Invoke(() => RootItems.Remove(rootItem));
                IsLoading = !dbCallbackResult.IsLastPortion;
                OnPropertyChanged(() => RootItems);
                OnPropertyChanged(() => RootItemsArray);
            }
        }

        void OnNewCard(SKDCard newCard)
		{
			var condition = newCard.IsInStopList ? (Func<CardViewModel, bool>)(x => x.IsDeactivatedRootItem) : x => x.IsOrganisation && x.Organisation.UID == newCard.OrganisationUID;
			var rootItem = RootItems.FirstOrDefault(condition);
			if (rootItem == null)
			{
				if (newCard.IsInStopList)
					RootItems.Add(CardViewModel.DeactivatedRootItem);
				else
				{
					var organisation = OrganisationHelper.GetSingle(newCard.OrganisationUID);
					if (organisation == null)
						return;
					rootItem = new CardViewModel(organisation);
					RootItems.Add(rootItem);
				}
			}
			var cards = rootItem.Children;
			var cardViewModel = cards.FirstOrDefault(x => x.Card.UID == newCard.UID);
			if (cardViewModel != null)
				cardViewModel.Update(newCard);
			else
				rootItem.AddChild(new CardViewModel(newCard));
			if (!newCard.IsInStopList)
			{
				var deactivatedRoot = RootItems.FirstOrDefault(x => x.IsDeactivatedRootItem);
				if (deactivatedRoot != null)
				{
					var blockedCard = deactivatedRoot.Children.FirstOrDefault(x => x.Card.UID == newCard.UID);
					if (blockedCard != null)
						deactivatedRoot.RemoveChild(blockedCard);
				}
			}
			rootItem.IsExpanded = true;
			RootItems = new ObservableCollection<CardViewModel>(RootItems.OrderBy(x => x.IsDeactivatedRootItem));
			OnPropertyChanged(() => RootItems);
			OnPropertyChanged(() => RootItemsArray);
		}

		void OnBlockCard(Guid uid)
		{
			var card = RootItems.SelectMany(x => x.Children).FirstOrDefault(x => x.Card.UID == uid);
			if (card != null)
			{
				var parent = card.Parent;
				parent.RemoveChild(card);
				if (parent.Children.Count() == 0)
					RootItems.Remove(parent);
			}
			var newCard = CardHelper.GetSingle(uid);
			if (newCard == null)
				return;
			var deactivatedRoot = RootItems.FirstOrDefault(x => x.IsDeactivatedRootItem);
			if(deactivatedRoot == null)
			{
				deactivatedRoot = CardViewModel.DeactivatedRootItem;
				RootItems.Add(deactivatedRoot);
			}
			deactivatedRoot.AddChild(new CardViewModel(newCard));
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

		void OnEditEmployee(Guid employeeUID)
		{
			var card = RootItems.SelectMany(x => x.Children).FirstOrDefault(x => x.Card.EmployeeUID == employeeUID);
			if (card != null)
			{
				var employee = EmployeeHelper.GetSingleShort(employeeUID);
				if (employee != null)
				{
					card.Card.EmployeeName = employee.Name;
					card.Update(card.Card);
				}
			}
		}

		void OnOrganisationUsersChanged(Organisation newOrganisation)
		{
			if (newOrganisation.UserUIDs.Any(x => x == FiresecManager.CurrentUser.UID))
			{
				if (!RootItems.Any(x => x.IsOrganisation && x.Organisation.UID == newOrganisation.UID))
				{
					var organisationViewModel = new CardViewModel(newOrganisation);
					var cardFilter = new CardFilter();
					cardFilter.EmployeeFilter = new EmployeeFilter { OrganisationUIDs = new System.Collections.Generic.List<Guid> { newOrganisation.UID } };
					var cards = CardHelper.Get(cardFilter);
					if (cards == null || cards.Count() == 0)
						return;
					RootItems.Add(organisationViewModel);
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

		public void Initialize()
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
            //DbCallbackResultUID = Guid.NewGuid();
            //FiresecManager.FiresecService.BeginGetCards(filter, DbCallbackResultUID);
            IsLoading = true;
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
            get { return RootItems != null ? RootItems.ToArray() : new CardViewModel[]{new CardViewModel()}; }
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

        int _itemsCount;
        public int ItemsCount
        {
            get { return _itemsCount; }
            set
            {
                _itemsCount = value;
                OnPropertyChanged(() => ItemsCount);
            }
        }

        bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                OnPropertyChanged(() => IsLoading);
            }
        }

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить карту?"))
			{
				CardHelper.Delete(SelectedCard.Card);
				var parent = SelectedCard.Card.IsInStopList ? RootItems.FirstOrDefault(x => x.IsDeactivatedRootItem) : RootItems.FirstOrDefault(x => x.Organisation.UID == SelectedCard.Card.OrganisationUID);
				parent.RemoveChild(SelectedCard);
				SelectedCard = parent.HasChildren ? parent : parent.Children.FirstOrDefault();
			}
		}
		bool CanRemove()
		{
			return SelectedCard != null && SelectedCard.IsCard && SelectedCard.Card.IsInStopList && FiresecManager.CheckPermission(FiresecAPI.Models.PermissionType.Oper_SKD_Cards_Etit) && !IsLoading;
		}
	}
}
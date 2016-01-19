using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using GKWebService.DataProviders.SKD;
using RubezhAPI.SKD;

namespace GKWebService.Models.SKD.Cards
{
    public class CardsViewModel
    {
        public List<CardViewModel> RootItems { get; set; }

        public void Initialize(CardFilter filter)
        {
            var organisations = OrganisationHelper.GetByCurrentUser();
            if (organisations == null)
                return;
            RootItems = new List<CardViewModel>();
            foreach (var organisation in organisations)
            {
                var cardViewModel = new CardViewModel(organisation);
                RootItems.Add(cardViewModel);
            }
            var deactivatedRootItem = new CardViewModel(true);
            deactivatedRootItem.UID = Guid.NewGuid();
            RootItems.Add(deactivatedRootItem);
            var cards = CardHelper.Get(filter);
            if (cards == null)
                return;
            foreach (var card in cards)
            {
                var condition = card.IsInStopList ? (Func<CardViewModel, bool>)(x => x.IsDeactivatedRootItem) : x => x.IsOrganisation && x.UID == card.OrganisationUID;
                var rootItem = RootItems.FirstOrDefault(condition);
                if (rootItem != null)
                {
                    rootItem.IsLeaf = false;
                    rootItem.IsExpanded = true;
                    var cardViewModel = new CardViewModel(card);
                    cardViewModel.ParentUID = rootItem.UID;
                    RootItems.Insert(RootItems.IndexOf(rootItem) + 1, cardViewModel);
                }
            }
            var rootItemsToRemove = RootItems.Where(x => x.Level == 0 && x.IsLeaf).ToList();
            foreach (var rootItem in rootItemsToRemove)
            {
                RootItems.Remove(rootItem);
            }
        }
    }
}
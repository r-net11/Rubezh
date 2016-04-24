using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RubezhAPI;
using RubezhAPI.SKD;

namespace GKWebService.Models.SKD.Cards
{
    public class CardViewModel
    {
        private SKDCard Card { get; set; }
        private Organisation Organisation { get; set; }
        public Guid UID { get; set; }
        public bool IsCard { get; set; }
        public bool IsOrganisation { get; set; }
        public bool IsDeactivatedRootItem { get; set; }
        public string Name { get; set; }
        public string CardType { get; set; }
        public bool IsInStopList { get; set; }
        public string EmployeeName { get; set; }
        public string StopReason { get; set; }
        public bool IsExpanded { get; set; }
        public int Level { get; set; }
        public Guid ParentUID { get; set; }
        public bool IsLeaf { get; set; }

        public CardViewModel() { }

        public CardViewModel(SKDCard card)
        {
            Card = card;
            IsCard = true;
            Level = 1;
            IsLeaf = true;
            IsExpanded = false;
            UID = card.UID;
            Init();
        }

        public CardViewModel(Organisation organisation)
        {
            Organisation = organisation;
            IsOrganisation = true;
            Level = 0;
            IsLeaf = true;
            IsExpanded = false;
            UID = organisation.UID;
            Init();
        }

        public CardViewModel(bool isDeactivatedRootItem)
        {
            IsDeactivatedRootItem = isDeactivatedRootItem;
            Level = 0;
            IsLeaf = true;
            IsExpanded = false;
            Init();
        }

        private void Init()
        {
            if (IsDeactivatedRootItem)
            {
                Name = "Деактивированные";
            }
            else if (IsOrganisation)
            {
                Name = Organisation.Name;
            }
            else if (Card != null)
            {
                Name = Card.Number.ToString();
            }
            else
            {
                Name = "";
            }

            CardType = IsCard ? Card.GKCardType.ToDescription() : "";
            IsInStopList = IsCard && Card.IsInStopList;
            EmployeeName = IsCard ? Card.EmployeeName : "";
            StopReason = IsCard ? Card.StopReason : "";
        }
    }
}
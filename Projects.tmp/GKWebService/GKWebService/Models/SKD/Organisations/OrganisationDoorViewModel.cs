using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GKWebService.DataProviders.SKD;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhAPI.SKD;
using RubezhClient;

namespace GKWebService.Models.SKD.Organisations
{
    public class OrganisationDoorViewModel
    {
        public Guid DoorUID { get; set; }
        public string PresentationName { get; set; }
        public int No { get; set; }

        public bool IsChecked { get; set; }

        public OrganisationDoorViewModel()
        {
            
        }

        public OrganisationDoorViewModel(Organisation organisation, GKDoor door)
        {
            DoorUID = door.UID;
            PresentationName = door.PresentationName;
            No = door.No;
            IsChecked = organisation != null && organisation.DoorUIDs.Contains(door.UID);
        }

        public Organisation SetDoorChecked(Organisation organisation)
        {
            if (IsChecked)
            {
                if (!organisation.DoorUIDs.Contains(DoorUID))
                    organisation.DoorUIDs.Add(DoorUID);
                OrganisationHelper.AddDoor(organisation, DoorUID);
            }
            else
            {
                var linkedCards = GetLinkedCards(organisation.UID);

                var linkedAccessTemplates = GetLinkedAccessTemplates(organisation.UID);

                foreach (var card in linkedCards)
                {
                    card.CardDoors.RemoveAll(x => x.DoorUID == DoorUID);
                    CardHelper.Edit(card, organisation.Name);
                }

                foreach (var accessTemplate in linkedAccessTemplates)
                {
                    accessTemplate.CardDoors.RemoveAll(x => x.DoorUID == DoorUID);
                    AccessTemplateHelper.Save(accessTemplate, false);
                }

                if (organisation.DoorUIDs.Contains(DoorUID))
                {
                    organisation.DoorUIDs.Remove(DoorUID);
                }
                OrganisationHelper.RemoveDoor(organisation, DoorUID);
            }

            return organisation;
        }

        public bool IsDoorLinked(Guid organisationId)
        {
            var linkedCards = GetLinkedCards(organisationId);

            var linkedAccessTemplates = GetLinkedAccessTemplates(organisationId);

            return linkedCards.Any() || linkedAccessTemplates.Any() || HasLinkedSchedules(organisationId);
        }

        private List<AccessTemplate> GetLinkedAccessTemplates(Guid organisationId)
        {
            var accessTemplatesResult = AccessTemplateHelper.Get(new AccessTemplateFilter());
            var linkedAccessTemplates = accessTemplatesResult.Where(x => !x.IsDeleted && x.OrganisationUID == organisationId && x.CardDoors.Any(y => y.DoorUID == DoorUID)).ToList();
            return linkedAccessTemplates;
        }

        private List<SKDCard> GetLinkedCards(Guid organisationId)
        {
            List<SKDCard> linkedCards;
            var cardsResult = CardHelper.Get(new CardFilter());
            linkedCards = cardsResult.Where(x => x.OrganisationUID == organisationId && x.CardDoors.Any(y => y.DoorUID == DoorUID)).ToList();
            return linkedCards;
        }

        private bool HasLinkedSchedules(Guid organisationId)
        {
            var operationResult = ScheduleHelper.Get(new ScheduleFilter());
            return operationResult.Any(x => x.OrganisationUID == organisationId && x.Zones.Any(y => y.DoorUID == DoorUID));
        }
    }
}
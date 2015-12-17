using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhAPI.SKD;
using RubezhClient;
using RubezhClient.SKDHelpers;

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
                var saveResult = ClientManager.FiresecService.AddOrganisationDoor(organisation, DoorUID);
                if (saveResult.HasError)
                {
                    throw new InvalidOperationException(saveResult.Error);
                }
            }
            else
            {
                var linkedCards = GetLinkedCards(organisation.UID);

                var linkedAccessTemplates = GetLinkedAccessTemplates(organisation.UID);

                foreach (var card in linkedCards)
                {
                    card.CardDoors.RemoveAll(x => x.DoorUID == DoorUID);
                    var editCardResult = ClientManager.FiresecService.EditCard(card, organisation.Name);
                    if (editCardResult.HasError)
                    {
                        throw new InvalidOperationException(editCardResult.Error);
                    }
                }

                foreach (var accessTemplate in linkedAccessTemplates)
                {
                    accessTemplate.CardDoors.RemoveAll(x => x.DoorUID == DoorUID);
                    var saveAccessTemplateResult = ClientManager.FiresecService.SaveAccessTemplate(accessTemplate, false);
                    if (saveAccessTemplateResult.HasError)
                    {
                        throw new InvalidOperationException(saveAccessTemplateResult.Error);
                    }
                }

                if (organisation.DoorUIDs.Contains(DoorUID))
                {
                    organisation.DoorUIDs.Remove(DoorUID);
                }
                var removeResult = ClientManager.FiresecService.RemoveOrganisationDoor(organisation, DoorUID);
                if (removeResult.HasError)
                {
                    throw new InvalidOperationException(removeResult.Error);
                }
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
            var accessTemplatesResult = ClientManager.FiresecService.GetAccessTemplates(new AccessTemplateFilter());
            if (accessTemplatesResult.HasError)
            {
                throw new InvalidOperationException(accessTemplatesResult.Error);
            }
            var linkedAccessTemplates = accessTemplatesResult.Result.Where(x => !x.IsDeleted && x.OrganisationUID == organisationId && x.CardDoors.Any(y => y.DoorUID == DoorUID)).ToList();
            return linkedAccessTemplates;
        }

        private List<SKDCard> GetLinkedCards(Guid organisationId)
        {
            List<SKDCard> linkedCards;
            var cardsResult = ClientManager.FiresecService.GetCards(new CardFilter());
            if (cardsResult.HasError)
            {
                throw new InvalidOperationException(cardsResult.Error);
            }
            linkedCards = cardsResult.Result.Where(x => x.OrganisationUID == organisationId && x.CardDoors.Any(y => y.DoorUID == DoorUID)).ToList();
            return linkedCards;
        }

        private bool HasLinkedSchedules(Guid organisationId)
        {
            var operationResult = ClientManager.FiresecService.GetSchedules(new ScheduleFilter());
            if (operationResult.HasError)
            {
                throw new InvalidOperationException(operationResult.Error);
            }
            return operationResult.Result.Any(x => x.OrganisationUID == organisationId && x.Zones.Any(y => y.DoorUID == DoorUID));
        }
    }
}
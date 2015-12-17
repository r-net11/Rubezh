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
            }
            else
            {
                if (organisation.DoorUIDs.Contains(DoorUID))
                    organisation.DoorUIDs.Remove(DoorUID);
            }

            //var result = ClientManager.FiresecService.SaveOrganisationUsers(organisation);
            //if (result.HasError)
            //{
            //    throw new InvalidOperationException(result.Error);
            //}

            return organisation;
        }
    }
}
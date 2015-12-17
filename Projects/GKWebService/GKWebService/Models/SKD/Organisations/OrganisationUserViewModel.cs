﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RubezhAPI.Models;
using RubezhAPI.SKD;
using RubezhClient;
using RubezhClient.SKDHelpers;

namespace GKWebService.Models.SKD.Organisations
{
    public class OrganisationUserViewModel
    {
        public User User { get; set; }

        public bool IsChecked { get; set; }

        public OrganisationUserViewModel()
        {
            
        }

        public OrganisationUserViewModel(Organisation organisation, User user)
        {
            User = user;
            IsChecked = organisation != null && organisation.UserUIDs != null && organisation.UserUIDs.Contains(user.UID);
        }

        public Organisation SetUserChecked(Organisation organisation)
        {
            if (IsChecked)
            {
                if (!organisation.UserUIDs.Contains(User.UID))
                    organisation.UserUIDs.Add(User.UID);
            }
            else
            {
                if (organisation.UserUIDs.Contains(User.UID))
                    organisation.UserUIDs.Remove(User.UID);
            }

            var result = ClientManager.FiresecService.SaveOrganisationUsers(organisation);
            if (result.HasError)
            {
                throw new InvalidOperationException(result.Error);
            }

            return organisation;
        }
    }
}
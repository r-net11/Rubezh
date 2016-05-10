using System;
using System.Collections.Generic;
using System.Linq;
using RubezhAPI;
using RubezhAPI.SKD;
using RubezhClient;

namespace GKWebService.DataProviders.SKD
{
    public static class OrganisationHelper
    {
        public static IEnumerable<Organisation> Get(OrganisationFilter filter)
        {
            var result = ClientManager.FiresecService.GetOrganisations(filter);
            return Common.ThrowErrorIfExists(result);
        }

        public static IEnumerable<Organisation> GetByCurrentUser(LogicalDeletationType logicalDeletationType = LogicalDeletationType.Active)
        {
            return Get(new OrganisationFilter() { User = ClientManager.CurrentUser, LogicalDeletationType = logicalDeletationType });
        }

        public static bool Save(OrganisationDetails organisation, bool isNew)
        {
            var result = ClientManager.FiresecService.SaveOrganisation(organisation, isNew);
            return Common.ThrowErrorIfExists(result);
        }

        public static Organisation GetSingle(Guid? uid)
        {
            if (uid == null)
                return null;
            var filter = new OrganisationFilter();
            filter.UIDs.Add((Guid)uid);
            var operationResult = ClientManager.FiresecService.GetOrganisations(filter);
            var result = Common.ThrowErrorIfExists(operationResult);
            if (result != null)
                return result.FirstOrDefault();
            return null;
        }

        public static OrganisationDetails GetDetails(Guid uid)
        {
            var result = ClientManager.FiresecService.GetOrganisationDetails(uid);
            return Common.ThrowErrorIfExists(result);
        }

        public static bool AddDoor(Organisation organisation, Guid doorUID)
        {
            var result = ClientManager.FiresecService.AddOrganisationDoor(organisation, doorUID);
            return Common.ThrowErrorIfExists(result);
        }

        public static bool RemoveDoor(Organisation organisation, Guid doorUID)
        {
            var result = ClientManager.FiresecService.RemoveOrganisationDoor(organisation, doorUID);
            return Common.ThrowErrorIfExists(result);
        }

        public static bool SaveUsers(Organisation organisation)
        {
            var result = ClientManager.FiresecService.SaveOrganisationUsers(organisation);
            return Common.ThrowErrorIfExists(result);
        }

        public static bool MarkDeleted(Organisation organisation)
        {
            return MarkDeleted(organisation.UID, organisation.Name);
        }

        public static bool Restore(Organisation organisation)
        {
            return Restore(organisation.UID, organisation.Name);
        }

        public static bool MarkDeleted(Guid uid, string name)
        {
            var result = ClientManager.FiresecService.MarkDeletedOrganisation(uid, name);
            return Common.ThrowErrorIfExists(result);
        }

        public static bool Restore(Guid uid, string name)
        {
            var operationResult = ClientManager.FiresecService.RestoreOrganisation(uid, name);
            return Common.ThrowErrorIfExists(operationResult);
        }

        public static bool SaveChief(Guid uid, Guid? chiefUID, string name)
        {
            var result = ClientManager.FiresecService.SaveOrganisationChief(uid, chiefUID, name);
            return Common.ThrowErrorIfExists(result);
        }

        public static bool SaveHRChief(Guid uid, Guid? chiefUID, string name)
        {
            var result = ClientManager.FiresecService.SaveOrganisationHRChief(uid, chiefUID, name);
            return Common.ThrowErrorIfExists(result);
        }

        public static bool IsAnyItems(Guid uid)
        {
            var operationResult = ClientManager.FiresecService.IsAnyOrganisationItems(uid);
            return Common.ThrowErrorIfExists(operationResult);
        }
    }
}
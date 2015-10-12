using System;
using System.Collections.Generic;
using FiresecAPI.SKD;

namespace RubezhClient.SKDHelpers
{
	public static class AccessTemplateHelper
	{
		public static IEnumerable<AccessTemplate> Get(AccessTemplateFilter filter)
		{
			var result = ClientManager.FiresecService.GetAccessTemplates(filter);
			return Common.ShowErrorIfExists(result);
		}

		public static IEnumerable<AccessTemplate> GetByCurrentUser()
		{
			return Get(new AccessTemplateFilter() { UserUID = ClientManager.CurrentUser.UID });
		}

		public static bool Save(AccessTemplate accessTemplate, bool isNew)
		{
			var result = ClientManager.FiresecService.SaveAccessTemplate(accessTemplate, isNew);
			Common.ShowErrorIfExists(result);
			return result.Result;
		}

		public static bool MarkDeleted(AccessTemplate accessTemplate)
		{
			return MarkDeleted(accessTemplate.UID, accessTemplate.Name);
		}

		public static bool Restore(AccessTemplate accessTemplate)
		{
			return Restore(accessTemplate.UID, accessTemplate.Name);
		}

		public static bool MarkDeleted(Guid uid, string name)
		{
			var result = ClientManager.FiresecService.MarkDeletedAccessTemplate(uid, name);
			return Common.ShowErrorIfExists(result);
		}

		public static bool Restore(Guid uid, string name)
		{
			var operationResult = ClientManager.FiresecService.RestoreAccessTemplate(uid, name);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static IEnumerable<AccessTemplate> GetByOrganisation(Guid organisationUID)
		{
			var operationResult = ClientManager.FiresecService.GetAccessTemplates(new AccessTemplateFilter { OrganisationUIDs = new List<Guid> { organisationUID } });
			return Common.ShowErrorIfExists(operationResult);
		}
	}
}
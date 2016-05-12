using RubezhAPI.SKD;
using System;
using System.Collections.Generic;

namespace RubezhClient.SKDHelpers
{
	public static class AccessTemplateHelper
	{
		public static IEnumerable<AccessTemplate> Get(AccessTemplateFilter filter, bool isShowError = true)
		{
			var result = ClientManager.FiresecService.GetAccessTemplates(filter);
			return Common.ShowErrorIfExists(result, isShowError);
		}

		public static IEnumerable<AccessTemplate> GetByCurrentUser()
		{
			return Get(new AccessTemplateFilter() { User = ClientManager.CurrentUser });
		}

		public static bool Save(AccessTemplate accessTemplate, bool isNew)
		{
			var result = ClientManager.FiresecService.SaveAccessTemplate(accessTemplate, isNew);
			Common.ShowErrorIfExists(result);
			return result.Result;
		}

		public static bool MarkDeleted(AccessTemplate accessTemplate)
		{
			var result = ClientManager.FiresecService.MarkDeletedAccessTemplate(accessTemplate);
			return Common.ShowErrorIfExists(result) != null;
		}

		public static bool Restore(AccessTemplate accessTemplate)
		{
			var operationResult = ClientManager.FiresecService.RestoreAccessTemplate(accessTemplate);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static IEnumerable<AccessTemplate> GetByOrganisation(Guid organisationUID)
		{
			var operationResult = ClientManager.FiresecService.GetAccessTemplates(new AccessTemplateFilter { OrganisationUIDs = new List<Guid> { organisationUID } });
			return Common.ShowErrorIfExists(operationResult);
		}
	}
}
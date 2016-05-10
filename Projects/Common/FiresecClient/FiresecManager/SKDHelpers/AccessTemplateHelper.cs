using System;
using System.Collections.Generic;
using StrazhAPI.SKD;

namespace FiresecClient.SKDHelpers
{
	public static class AccessTemplateHelper
	{
		public static IEnumerable<AccessTemplate> Get(AccessTemplateFilter filter)
		{
			var result = FiresecManager.FiresecService.GetAccessTemplates(filter);
			return Common.ShowErrorIfExists(result);
		}

		public static IEnumerable<AccessTemplate> GetByCurrentUser()
		{
			return Get(new AccessTemplateFilter() { UserUID = FiresecManager.CurrentUser.UID });
		}

		public static bool Save(AccessTemplate accessTemplate, bool isNew)
		{
			var result = FiresecManager.FiresecService.SaveAccessTemplate(accessTemplate, isNew);
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
			var result = FiresecManager.FiresecService.MarkDeletedAccessTemplate(uid, name);
			return Common.ShowErrorIfExists(result);
		}

		public static bool Restore(Guid uid, string name)
		{
			var operationResult = FiresecManager.FiresecService.RestoreAccessTemplate(uid, name);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static IEnumerable<AccessTemplate> GetByOrganisation(Guid organisationUID)
		{
			var operationResult = FiresecManager.FiresecService.GetAccessTemplates(new AccessTemplateFilter { OrganisationUIDs = new List<Guid> { organisationUID } });
			return Common.ShowErrorIfExists(operationResult);
		}
	}
}
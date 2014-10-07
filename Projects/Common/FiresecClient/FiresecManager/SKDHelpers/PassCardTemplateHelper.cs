using System;
using System.Collections.Generic;
using FiresecAPI.SKD;

namespace FiresecClient.SKDHelpers
{
	public static class PassCardTemplateHelper
	{
		public static bool Save(PassCardTemplate passCardTemplate)
		{
			var operationResult = FiresecManager.FiresecService.SavePassCardTemplate(passCardTemplate);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool MarkDeleted(Guid uid)
		{
			var operationResult = FiresecManager.FiresecService.MarkDeletedPassCardTemplate(uid);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool Restore(Guid uid)
		{
			var operationResult = FiresecManager.FiresecService.RestorePassCardTemplate(uid);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static PassCardTemplate GetDetails(Guid? uid)
		{
			if (uid == null)
				return null;
			var operationResult = FiresecManager.FiresecService.GetPassCardTemplateDetails(uid.Value);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static IEnumerable<ShortPassCardTemplate> GetByOrganisation(Guid organisationUID)
		{
			var result = FiresecManager.FiresecService.GetPassCardTemplateList(new PassCardTemplateFilter
			{
				OrganisationUIDs = new List<System.Guid> { organisationUID }
			});
			return Common.ShowErrorIfExists(result);
		}
		public static IEnumerable<ShortPassCardTemplate> Get(PassCardTemplateFilter filter)
		{
			var operationResult = FiresecManager.FiresecService.GetPassCardTemplateList(filter);
			return Common.ShowErrorIfExists(operationResult);
		}
	}
}
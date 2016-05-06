using System;
using System.Collections.Generic;
using StrazhAPI.SKD;

namespace FiresecClient.SKDHelpers
{
	public static class PassCardTemplateHelper
	{
		public static bool Save(PassCardTemplate passCardTemplate, bool isNew)
		{
			var operationResult = FiresecManager.FiresecService.SavePassCardTemplate(passCardTemplate, isNew);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool MarkDeleted(ShortPassCardTemplate item)
		{
			return MarkDeleted(item.UID, item.Name);
		}

		public static bool Restore(ShortPassCardTemplate item)
		{
			return Restore(item.UID, item.Name);
		}

		public static bool MarkDeleted(Guid uid, string name)
		{
			var operationResult = FiresecManager.FiresecService.MarkDeletedPassCardTemplate(uid, name);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool Restore(Guid uid, string name)
		{
			var operationResult = FiresecManager.FiresecService.RestorePassCardTemplate(uid, name);
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
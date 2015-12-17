﻿using RubezhAPI.SKD;
using System;
using System.Collections.Generic;

namespace RubezhClient.SKDHelpers
{
	public static class PassCardTemplateHelper
	{
		public static bool Save(PassCardTemplate passCardTemplate, bool isNew)
		{
			var operationResult = ClientManager.FiresecService.SavePassCardTemplate(passCardTemplate, isNew);
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
			var operationResult = ClientManager.FiresecService.MarkDeletedPassCardTemplate(uid, name);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool Restore(Guid uid, string name)
		{
			var operationResult = ClientManager.FiresecService.RestorePassCardTemplate(uid, name);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static PassCardTemplate GetDetails(Guid? uid)
		{
			if (uid == null)
				return null;
			var operationResult = ClientManager.FiresecService.GetPassCardTemplateDetails(uid.Value);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static IEnumerable<ShortPassCardTemplate> GetByOrganisation(Guid organisationUID)
		{
			var result = ClientManager.FiresecService.GetPassCardTemplateList(new PassCardTemplateFilter
			{
				OrganisationUIDs = new List<System.Guid> { organisationUID }
			});
			return Common.ShowErrorIfExists(result);
		}
		public static IEnumerable<ShortPassCardTemplate> Get(PassCardTemplateFilter filter)
		{
			var operationResult = ClientManager.FiresecService.GetPassCardTemplateList(filter);
			return Common.ShowErrorIfExists(operationResult);
		}
	}
}
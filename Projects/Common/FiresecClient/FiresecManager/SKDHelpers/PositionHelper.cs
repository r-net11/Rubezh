using System;
using System.Collections.Generic;
using FiresecAPI;

namespace FiresecClient.SKDHelpers
{
	public static class PositionHelper
	{
		public static bool Save(Position position)
		{
			var operationResult = FiresecManager.FiresecService.SavePosition(position);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool MarkDeleted(Guid uid)
		{
			var operationResult = FiresecManager.FiresecService.MarkDeletedPosition(uid);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static Position GetDetails(Guid? uid)
		{
			if (uid == null)
				return null;
			var operationResult = FiresecManager.FiresecService.GetPositionDetails(uid.Value);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static IEnumerable<ShortPosition> GetByOrganisation(Guid? organisationUID)
		{
			if (organisationUID == null)
				return null;
			var filter = new PositionFilter();
			filter.OrganisationUIDs.Add(organisationUID.Value);
			var operationResult = FiresecManager.FiresecService.GetPositionList(filter);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static IEnumerable<ShortPosition> Get(PositionFilter filter)
		{
			var operationResult = FiresecManager.FiresecService.GetPositionList(filter);
			return Common.ShowErrorIfExists(operationResult);
		}
	}
}
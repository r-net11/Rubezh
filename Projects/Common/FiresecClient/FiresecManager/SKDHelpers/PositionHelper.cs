using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;

namespace FiresecClient.SKDHelpers
{
	public static class PositionHelper
	{
		public static bool Save(Position position)
		{
			var operationResult = FiresecManager.FiresecService.SavePositions(new List<Position> { position });
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool MarkDeleted(Position position)
		{
			var operationResult = FiresecManager.FiresecService.MarkDeletedPositions(new List<Position> { position });
			return Common.ShowErrorIfExists(operationResult);
		}

		public static Position GetSingle(Guid? uid)
		{
			if (uid == null)
				return null;
			var filter = new PositionFilter();
			filter.Uids.Add(uid.Value);
			var operationResult = FiresecManager.FiresecService.GetPositions(filter);
			return Common.ShowErrorIfExists(operationResult).FirstOrDefault();
		}

		public static IEnumerable<Position> GetByOrganization(Guid? organizationUID)
		{
			if (organizationUID == null)
				return null;
			var filter = new PositionFilter();
			filter.OrganizationUIDs.Add(organizationUID.Value);
			var operationResult = FiresecManager.FiresecService.GetPositions(filter);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static IEnumerable<Position> Get(PositionFilter filter)
		{
			var operationResult = FiresecManager.FiresecService.GetPositions(filter);
			return Common.ShowErrorIfExists(operationResult);
		}
	}
}

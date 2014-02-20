using System.Collections.Generic;
using FiresecAPI;
using System;
using System.Linq;
using Infrastructure.Common.Windows;

namespace FiresecClient.SKDHelpers
{
	public static class PositionHelper
	{
		public static bool Save(Position position)
		{
			var operatonResult = FiresecManager.FiresecService.SavePositions(new List<Position> { position });
			if (operatonResult.HasError)
			{
				MessageBoxService.ShowWarning(operatonResult.Error);
				return false;
			}
			return true;
		}

		public static bool MarkDeleted(Position position)
		{
			var operatonResult = FiresecManager.FiresecService.MarkDeletedPositions(new List<Position> { position });
			if (operatonResult.HasError)
			{
				MessageBoxService.ShowWarning(operatonResult.Error);
				return false;
			}
			return true;
		}

		public static Position GetPosition(Guid? uid)
		{
			if (uid == null)
				return null;
			var filter = new PositionFilter();
			filter.Uids.Add((Guid)uid);
			var operationResult = FiresecManager.FiresecService.GetPositions(filter);
			if (operationResult.HasError)
			{
				MessageBoxService.ShowWarning(operationResult.Error);
				return null;
			}
			else
				return operationResult.Result.FirstOrDefault();
		}

		public static IEnumerable<Position> GetPositions(PositionFilter filter)
		{
			var operatonResult = FiresecManager.FiresecService.GetPositions(filter);
			if (operatonResult.HasError)
			{
				MessageBoxService.ShowWarning(operatonResult.Error);
				return null;
			}
			return operatonResult.Result;
		}
	}
}

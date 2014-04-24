using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.EmployeeTimeIntervals;

namespace FiresecClient.SKDHelpers
{
	public static class NamedIntervalHelper
	{
		public static bool Save(NamedInterval namedInterval)
		{
			var operationResult = FiresecManager.FiresecService.SaveNamedIntervals(new List<NamedInterval> { namedInterval });
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool MarkDeleted(NamedInterval namedInterval)
		{
			var operationResult = FiresecManager.FiresecService.MarkDeletedNamedIntervals(new List<NamedInterval> { namedInterval });
			return Common.ShowErrorIfExists(operationResult);
		}

		public static NamedInterval GetSingle(Guid? uid)
		{
			if (uid == null)
				return null;
			var filter = new NamedIntervalFilter();
			filter.UIDs.Add(uid.Value);
			var operationResult = FiresecManager.FiresecService.GetNamedIntervals(filter);
			return Common.ShowErrorIfExists(operationResult).FirstOrDefault();
		}

		public static IEnumerable<NamedInterval> GetByOrganisation(Guid? organisationUID)
		{
			if (organisationUID == null)
				return null;
			var filter = new NamedIntervalFilter();
			filter.OrganisationUIDs.Add(organisationUID.Value);
			var operationResult = FiresecManager.FiresecService.GetNamedIntervals(filter);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static IEnumerable<NamedInterval> Get(NamedIntervalFilter filter)
		{
			var operationResult = FiresecManager.FiresecService.GetNamedIntervals(filter);
			return Common.ShowErrorIfExists(operationResult);
		}
	}
}

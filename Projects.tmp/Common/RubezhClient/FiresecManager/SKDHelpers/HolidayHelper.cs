﻿using RubezhAPI.SKD;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RubezhClient.SKDHelpers
{
	public static class HolidayHelper
	{
		public static bool Save(Holiday holiday, bool isNew)
		{
			var operationResult = ClientManager.FiresecService.SaveHoliday(holiday, isNew);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool MarkDeleted(Holiday item)
		{
			return MarkDeleted(item.UID, item.Name);
		}

		public static bool Restore(Holiday item)
		{
			return Restore(item.UID, item.Name);
		}

		public static bool MarkDeleted(Guid uid, string name)
		{
			var operationResult = ClientManager.FiresecService.MarkDeletedHoliday(uid, name);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static bool Restore(Guid uid, string name)
		{
			var operationResult = ClientManager.FiresecService.RestoreHoliday(uid, name);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static Holiday GetSingle(Guid? uid)
		{
			if (uid == null)
				return null;
			var filter = new HolidayFilter();
			filter.UIDs.Add(uid.Value);
			var operationResult = ClientManager.FiresecService.GetHolidays(filter);
			return Common.ShowErrorIfExists(operationResult).FirstOrDefault();
		}

		public static IEnumerable<Holiday> Get(HolidayFilter filter)
		{
			var operationResult = ClientManager.FiresecService.GetHolidays(filter);
			return Common.ShowErrorIfExists(operationResult);
		}

		public static IEnumerable<Holiday> GetByOrganisation(Guid organisationUID)
		{
			var result = ClientManager.FiresecService.GetHolidays(new HolidayFilter
			{
				OrganisationUIDs = new List<System.Guid> { organisationUID }
			});
			return Common.ShowErrorIfExists(result);
		}
	}
}
using Infrastructure.Common.Windows;
using ResursAPI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ResursDAL
{
	public static partial class DbCache
	{
		public static int? GetJournalCount(Filter filter)
		{
			try
			{
				using (var context = DatabaseContext.Initialize())
				{
					return GetFiltered(filter, context).Count();
				}
			}
			catch (Exception e)
			{
				MessageBoxService.ShowException(e);
				return null;
			}
		}

		public static List<Journal> GetJournalPage(Filter filter, int page)
		{
			try
			{
				using (var context = DatabaseContext.Initialize())
				{
					var journalItems = GetFiltered(filter, context);
					return journalItems.Skip((page - 1) * filter.PageSize).Take(filter.PageSize).ToList();
				}
			}

			catch (Exception e)
			{
				MessageBoxService.ShowException(e);
				return null;
			}
		}

		static IQueryable<Journal> GetFiltered(Filter filter, DatabaseContext context)
		{
			IQueryable<Journal> result = context.Journal;
			if (filter.JournalTypes.Any())
				result = result.Where(x => filter.JournalTypes.Contains(x.JournalType));

			if (filter.ConsumerUIDs.Any())
				result = result.Where(x => filter.ConsumerUIDs.Contains(x.ObjectUID));

			if (filter.DeviceUIDs.Any())
				result = result.Where(x => filter.DeviceUIDs.Contains(x.ObjectUID));

			if (filter.UserUIDs.Any())
				result = result.Where(x => filter.UserUIDs.Contains(x.UserUID));

			if (filter.TariffUIDs.Any())
				result = result.Where(x => filter.TariffUIDs.Contains(x.ObjectUID));

			result = result.Where(x => x.DateTime > filter.StartDate && x.DateTime < filter.EndDate);

			if (filter.IsSortAsc)
				result = result.OrderBy(x => x.DateTime);
			else
				result = result.OrderByDescending(x => x.DateTime);

			return result;
		}

		public static void AddJournal(JournalType journalType, string description = null)
		{
			var journalEvent = new Journal()
			{
				JournalType = journalType,
				Description = description
			};

			try
			{
				using (var context = DatabaseContext.Initialize())
				{
					context.Journal.Add(journalEvent);
					context.SaveChanges();
				}
			}

			catch (Exception e)
			{
				MessageBoxService.ShowException(e);
			}
		}

		public static void AddJournalForUser(JournalType journalType, ModelBase modelBase = null, string Description = null)
		{
			var journalEvent = new Journal();
			if (modelBase != null)
			{
				if (modelBase is User)
					journalEvent.ClassType = ClassType.IsUser;
				if (modelBase is Consumer)
					journalEvent.ClassType = ClassType.IsConsumer;
				if (modelBase is Tariff)
					journalEvent.ClassType = ClassType.IsTariff;
				if (modelBase is Device)
					journalEvent.ClassType = ClassType.IsDevice;

				journalEvent.ObjectUID = modelBase.UID;
				journalEvent.ObjectName = modelBase.Name;
			}
			else
			{
				journalEvent.ClassType = ClassType.IsSystem;
			}
			journalEvent.UserName = CurrentUser.Name;
			journalEvent.UserUID = CurrentUser.UID;
			journalEvent.JournalType = journalType;
			journalEvent.Description = Description;

			try
			{
				using (var context = DatabaseContext.Initialize())
				{
					context.Journal.Add(journalEvent);
					context.SaveChanges();
				}
			}

			catch (Exception e)
			{
				MessageBoxService.ShowException(e);
			}
		}
	}
}